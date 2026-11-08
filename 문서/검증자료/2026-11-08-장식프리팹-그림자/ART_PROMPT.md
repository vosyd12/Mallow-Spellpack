# 승인 시안의 게임 에셋 변환

2026-11-08. imagegen 스킬의 내장 편집 도구로 승인한 4타입 시안을 참고해 제작했다. 투명 배경을 사용하며 CLI/API 우회는 사용하지 않았다.

- 입력: 승인 시안 `exec-c2d1fc1b-8cc5-493b-9b51-2344cb573480.png`.
- 최종 이미지: `Assets/Sprites/Environment/Meadow/Props/MeadowPropsAtlas.png`.
- 실제 출력: 1774×887. Unity Sprite 분리 단계에서 불투명 픽셀의 범위를 확인하고 여백을 줄였다. 행 경계의 다른 오브젝트 픽셀은 분리 영역에서 제외했다.
- 그림자는 이미지에 구워 넣지 않고 기존 `GroundShadowMask`와 별도 SpriteRenderer로 표시한다.

## 생성 프롬프트

```text
Edit the supplied approved concept sheet into a PRODUCTION SPRITE ATLAS. Preserve the eight object designs, chunky crisp pixel-art edge style, dark forest-green outlines, simple 3-green leaf shading, warm brown trunks, and upper-left highlight direction. Remove ALL background, panels, Korean labels and ALL baked contact shadows. True transparent alpha background everywhere outside the object silhouettes; no green halo, no glow, no checkerboard, no ground patches.
Output landscape 2048x1024, an EXACT REGULAR GRID of FOUR columns and TWO rows, eight equal square cells. Keep each complete object inside its own cell with generous transparent padding, never overlap neighboring cells. Each object base is on the same 88%-down baseline within its cell.
Row 1 left to right: round oak tree; taller clustered oak tree; plain grass clump A; plain grass clump B.
Row 2 left to right: pink/cream flowering grass bush A; yellow/white flowering grass bush B; small white/yellow wildflower group A; small pink/white wildflower group B.
Trees take roughly 85% cell height, grass clumps 35%, flowering bushes 40%, small flower groups 28%. Preserve these relative proportions and the same pixel stroke/cluster sizes. Keep trees faithful to approved silhouettes, grass with broad tapered stepped blades, flowering bushes with 3 to 5 simple flowers, loose flower groups with two short stems and sparse leaves. No extra objects, text, faces, decorations, characters, borders, scene or sprite shadows. This is to be imported as eight game Sprites, with separately rendered consistent shadows.
```
