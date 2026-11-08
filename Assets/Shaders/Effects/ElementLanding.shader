Shader "Mallow/Effects/ElementLanding"
{
    Properties
    {
        [MainTexture] _BaseMap ("Particle Texture", 2D) = "white" {}
        [MainColor] _BaseColor ("Tint", Color) = (1, 1, 1, 1)
        [HDR] _GlowColor ("Sweep Color", Color) = (2, 1.8, 1.2, 1)
        _ParticleRect ("Sprite UV Rect", Vector) = (0, 0, 1, 1)
        _GlowStrength ("Sweep Strength", Range(0, 4)) = 1
        _SweepWidth ("Sweep Width", Range(0.02, 0.3)) = 0.12
        _LineWidth ("Ground Line Width", Range(0.005, 0.05)) = 0.014
        [Toggle(_GROUND_LIGHTNING)] _GroundLightning ("Ground Lightning", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
        }

        Pass
        {
            Name "ElementLanding"
            Tags { "LightMode" = "SRPDefaultUnlit" }

            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma shader_feature_local _GROUND_LIGHTNING

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                float4 _ParticleRect;
                half4 _BaseColor;
                half4 _GlowColor;
                float _GlowStrength;
                float _SweepWidth;
                float _LineWidth;
                float _GroundLightning;
            CBUFFER_END

            // Renderer streams: Position, Color, UV, AgePercent.
            // AgePercent is packed after UV.xy, into TEXCOORD0.z.
            struct Attributes
            {
                float4 positionOS : POSITION;
                half4 color : COLOR;
                float3 uvAge : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                half4 color : COLOR;
                float3 uvAge : TEXCOORD0;
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.color = input.color;
                output.uvAge = input.uvAge;
                return output;
            }

            // Track the nearest segment and its progress from the impact center.
            void SampleSegment(
                float2 samplePosition,
                float2 segmentStart,
                float2 segmentEnd,
                float progressStart,
                float progressEnd,
                inout float nearestDistance,
                inout float nearestProgress)
            {
                float2 edge = segmentEnd - segmentStart;
                float along = saturate(dot(samplePosition - segmentStart, edge) / max(dot(edge, edge), 0.0001));
                float distanceToEdge = length(samplePosition - segmentStart - edge * along);

                if (distanceToEdge < nearestDistance)
                {
                    nearestDistance = distanceToEdge;
                    nearestProgress = lerp(progressStart, progressEnd, along);
                }
            }

            half4 DrawGroundLightning(float2 uv, float age, half4 particleColor)
            {
                float distanceToLine = 2;
                float progress = 0;

                SampleSegment(uv, float2(0.50, 0.54), float2(0.38, 0.47), 0.00, 0.18, distanceToLine, progress);
                SampleSegment(uv, float2(0.38, 0.47), float2(0.54, 0.41), 0.18, 0.35, distanceToLine, progress);
                SampleSegment(uv, float2(0.54, 0.41), float2(0.42, 0.27), 0.35, 0.65, distanceToLine, progress);
                SampleSegment(uv, float2(0.42, 0.27), float2(0.62, 0.12), 0.65, 1.00, distanceToLine, progress);

                SampleSegment(uv, float2(0.38, 0.47), float2(0.22, 0.42), 0.18, 0.45, distanceToLine, progress);
                SampleSegment(uv, float2(0.22, 0.42), float2(0.29, 0.31), 0.45, 0.65, distanceToLine, progress);
                SampleSegment(uv, float2(0.29, 0.31), float2(0.10, 0.23), 0.65, 1.00, distanceToLine, progress);

                SampleSegment(uv, float2(0.54, 0.41), float2(0.72, 0.53), 0.35, 0.55, distanceToLine, progress);
                SampleSegment(uv, float2(0.72, 0.53), float2(0.65, 0.64), 0.55, 0.74, distanceToLine, progress);
                SampleSegment(uv, float2(0.65, 0.64), float2(0.89, 0.74), 0.74, 1.00, distanceToLine, progress);

                SampleSegment(uv, float2(0.50, 0.54), float2(0.43, 0.71), 0.00, 0.35, distanceToLine, progress);
                SampleSegment(uv, float2(0.43, 0.71), float2(0.59, 0.80), 0.35, 0.65, distanceToLine, progress);
                SampleSegment(uv, float2(0.59, 0.80), float2(0.55, 0.93), 0.65, 1.00, distanceToLine, progress);

                float aa = max(fwidth(distanceToLine), 0.001);
                float core = 1 - smoothstep(_LineWidth, _LineWidth + aa, distanceToLine);
                float halo = 1 - smoothstep(_LineWidth, _LineWidth * 4 + aa, distanceToLine);
                float sweepHead = lerp(-0.16, 1.16, age);
                float sweep = 1 - smoothstep(_SweepWidth * 0.3, _SweepWidth, abs(progress - sweepHead));
                float appearance = smoothstep(0.0, 0.06, age);
                float fade = 1 - smoothstep(0.68, 1.0, age);

                half3 color = lerp(_BaseColor.rgb, _GlowColor.rgb, saturate(sweep * _GlowStrength));
                color += _GlowColor.rgb * sweep * halo * 0.2;
                half alpha = saturate(core * 0.85 + halo * 0.18) * appearance * fade;
                return half4(color * particleColor.rgb, alpha * particleColor.a * _BaseColor.a);
            }

            half4 Frag(Varyings input) : SV_Target
            {
                float age = saturate(input.uvAge.z);
                float2 localUV = saturate((input.uvAge.xy - _ParticleRect.xy) / max(_ParticleRect.zw, float2(0.0001, 0.0001)));

                #if defined(_GROUND_LIGHTNING)
                    return DrawGroundLightning(localUV, age, input.color);
                #else
                    // Sprite-mode UVs already select the exact atlas region.
                    half4 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uvAge.xy);
                    color *= input.color * _BaseColor;
                    float sweepHead = lerp(-0.2, 1.2, age);
                    float acrossSprite = (localUV.x + localUV.y) * 0.5;
                    float sweep = 1 - smoothstep(_SweepWidth * 0.3, _SweepWidth, abs(acrossSprite - sweepHead));
                    color.rgb += _GlowColor.rgb * sweep * _GlowStrength;
                    return color;
                #endif
            }
            ENDHLSL
        }
    }
    FallBack Off
}
