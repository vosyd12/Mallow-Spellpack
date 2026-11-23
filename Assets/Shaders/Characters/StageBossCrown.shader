Shader "Mallow/Characters/StageBossCrown"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite", 2D) = "white" {}
        _CrownMask ("Crown highlight mask", 2D) = "black" {}
        [HDR] _CrownGlint ("Gold reflection", Color) = (4.6, 2.9, 1.25, 1)
        _GlintPeriod ("Seconds between glints", Range(1.5, 8)) = 3.6
        _GlintWidth ("Glint width", Range(0.01, 0.2)) = 0.042
        [HideInInspector] _Color ("Tint", Color) = (1,1,1,1)
        [HideInInspector] _HitColor ("Hit color", Color) = (1,.4,.4,1)
        [HideInInspector] _HitAmount ("Hit amount", Float) = 0
        [HideInInspector] _HitRimWidth ("Hit rim", Float) = .6
        [HideInInspector] _HitCenterStrength ("Hit center", Float) = .12
        [HideInInspector] _PreviewPhase ("Preview phase override", Float) = -1
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Core2D.hlsl"

            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile_instancing

            struct Attributes
            {
                COMMON_2D_INPUTS
                half4 color : COLOR;
            };

            struct Varyings
            {
                COMMON_2D_OUTPUTS
                half4 color : COLOR;
                float2 hitPosition : TEXCOORD2;
            };

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/2DCommon.hlsl"

            TEXTURE2D(_CrownMask);
            SAMPLER(sampler_CrownMask);

            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
                half4 _CrownGlint;
                half4 _HitColor;
                float4x4 _HitWorldToArea;
                float _GlintPeriod;
                float _GlintWidth;
                float _PreviewPhase;
                float _HitAmount;
                float _HitRimWidth;
                float _HitCenterStrength;
            CBUFFER_END

            Varyings Vert(Attributes input)
            {
                SetUpSpriteInstanceProperties();
                input.positionOS = UnityFlipSprite(input.positionOS, unity_SpriteProps.xy);
                Varyings output = CommonUnlitVertex(input);
                output.color = input.color * _Color * unity_SpriteColor;
                float3 worldPosition = TransformObjectToWorld(input.positionOS);
                output.hitPosition = mul(_HitWorldToArea, float4(worldPosition, 1)).xy;
                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                half4 sprite = CommonUnlitFragment(input, input.color);

                // Keep the shared HitEffectVisual property block and the sprite's fade alpha.
                float rim = smoothstep(1.0 - _HitRimWidth, 1.0, length(input.hitPosition) * 2.0);
                float hit = saturate(_HitAmount * lerp(_HitCenterStrength, 1.0, rim));
                sprite.rgb = lerp(sprite.rgb, _HitColor.rgb, hit);

                // A narrow diagonal reflection crosses only the gold mask, then rests.
                float phase = _PreviewPhase >= 0.0 ? _PreviewPhase : frac(_Time.y / _GlintPeriod);
                float sweep = lerp(.15, .85, saturate(phase / .24));
                float distanceToGlint = abs(input.uv.x + (input.uv.y - .8) * .4 - sweep);
                float band = 1.0 - smoothstep(.0, _GlintWidth, distanceToGlint);
                float window = smoothstep(.0, .035, phase) * (1.0 - smoothstep(.19, .24, phase));
                half mask = SAMPLE_TEXTURE2D(_CrownMask, sampler_CrownMask, input.uv).r;
                sprite.rgb += _CrownGlint.rgb * (mask * band * window);
                return sprite;
            }
            ENDHLSL
        }
    }
}
