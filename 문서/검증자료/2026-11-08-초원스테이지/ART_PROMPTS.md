# 초원 이미지 생성 프롬프트

2026-11-08. imagegen 스킬의 내장 도구로 생성했다. 아래 출력은 프로젝트 에셋으로 복사하고 Unity에서 Sprite·Tile·Prefab으로 연결했다. CLI/API 우회나 별도 이미지 편집은 사용하지 않았다.

## meadowGrass

저장 위치: `Assets/Sprites/Environment/Meadow/MeadowGrass.png`. 투명 배경: 미사용.

```text
Use case: stylized-concept. Asset type: one SEAMLESS REPEATING 2D GRASS GROUND TILE for a cute top-down mobile pixel sprite game. A single square 1024x1024 texture filling the entire canvas, no margins, NO grid or sheet. Orthographic top down, no horizon, no perspective, no directional height. Gentle light-medium spring grass green, deliberately LOW contrast and QUIET to make small blue/orange/yellow slime characters and their colorful landing effects easy to read. Broad flat natural green base with subtle soft irregular green variations and only 8 to 12 sparse tiny stylized grass blade clusters across the whole texture. Restrained chunky 2D game pixel-art aesthetic, simple stepped edges, limited harmonious green palette, no realistic brush texture, no noisy single-pixel speckles, no detailed individual grass carpet. Opposite edges must seamlessly match: all four edges are the SAME even green base without isolated leaves touching them; texture details remain in the inner area. No square border, no checkerboard, no tiles drawn inside the image, no vignette, no lighting gradient, no drop shadow, no foliage clumps casting shadows, no flowers, no rocks, no tree, no path, no characters, no text, no watermark. Designed as an actual reusable ground texture, not a screenshot or concept painting. Calm friendly pastel green but not washed-out mint and not neon lime.
```

## meadowTrees

저장 위치: `Assets/Sprites/Environment/Meadow/MeadowTrees.png`. 투명 배경: 사용.

```text
Use case: stylized-concept. Asset type: production 2D TREE SPRITE ATLAS on a genuinely transparent background. Wide landscape canvas 1536x1024, EXACTLY two equal vertical cells side by side (2 columns, 1 row). Each cell contains one whole simple cute broadleaf tree, fully visible from leaf canopy top to trunk base, generous empty transparent margins around it, no cell border, no labels or text, no ground plane, no environment, no grass around base, no baked shadow. Tree 1 in left cell: a compact round oak tree with three broad overlapping round leaf masses and a short visible warm brown trunk. Tree 2 right cell: a slightly taller round broadleaf tree with four broad leaf lobes and a short warm brown trunk, a subtle different natural green hue. Same trunk base baseline at 88% down the cell; same front/top-down RPG 3/4 viewing angle. Clear friendly silhouette, SIMPLE CHUNKY 2D PIXEL SPRITE aesthetic matching small elastic slime sprites: discrete stepped contours, broad flat color masses, restrained deep green edge, 3 green leaf shades, 2 trunk shades, gentle lighter upper-left and darker underside. No individual leaves and no extreme texture. Maintain coherent quality between the two. Trees should read clearly when displayed only 70–100 pixels tall in game. Not 3D renders, not photorealistic, not smooth vector icons, not detailed painterly illustrations, not cinematic isometric assets, no faces, no fruit, no decorations, no crystals, no magical glow, no MMO ornamentation. Isolated sprites, true alpha transparency, no checkerboard baked into pixels.
```

