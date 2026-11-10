# 플레이어 마법사 외형과 Idle

후속: 이 문서는 이전 v2 적용 기록이다. 현재 게임은 [확정 v3 파츠 외형·Idle](2026-11-10-PLAYER-V3-IDLE.md)로 교체했으며 MCP Play 검증을 진행했다. 아래 미검증 설명은 v2 작업 당시 기준이다.
작성일: 2026-11-10

## 요청과 범위
사용자가 모자를 한 치수 줄인 [v2 시안](아트시안/2026-11-10-플레이어마법사/V2-PREVIEW.md)을 외형 기준으로 확정했다. 이번에는 새 외형과 Idle만 연결했다. 이동·공격·피격·사망 전용 애니메이션은 후속 작업이다.

## 문제와 선택한 방법
기존 플레이어는 단일 이미지라 슬라임에 비해 움직임이 단조로웠다. C#을 추가하지 않고 투명 스프라이트 시트와 Unity Animator의 단일 Idle 상태를 사용했다. 파츠별 리깅은 이번 범위에 포함하지 않았다.

## 변경
- `Assets/Sprites/Characters/Player/MageIdleAtlas.png`: v2를 참고해 생성한 4열×2행, 1448×1086 투명 시트. 셀 크기는 362×543이다.
- `Assets/Animations/Player/PlayerIdle.anim`: 3.2초 반복. 호흡·모자·양갈래의 작은 변화와 반쯤 감기→감기→뜨기 프레임을 재생한다. 완전히 감긴 구간은 약 0.08초다.
- `Assets/Animations/Player/Player.controller`: 기본 상태 Idle 하나만 사용한다. 잘림 여유가 부족한 원본 2번 프레임은 클립에서 제외했다.
- `Assets/Scenes/SampleScene.unity`: Player/Visual의 기존 SpriteRenderer에 새 기본 Sprite와 Animator를 연결했다.
- 기존 불투명 영역 높이와 발 위치를 기준으로 PPU 약 336.253, 피벗 Y 약 0.565224를 설정했다. 루트·Visual Transform과 기존 전투·피격 재질·HP바 연결은 유지했다. 원본 PlayerMage.png도 보존했다.

## 확인 결과와 남은 문제
파일 대조로 씬 변경 범위, Sprite ID·GUID·클립 참조·반복 설정·C# 보존을 확인했다. Unity의 로컬 Editor.log에서 새 시트·클립·Controller의 임포트 완료를 확인했다. **MCP 연결과 실제 게임 재생·피격·화면 크기 검증은 미실시**다. MCP 사용 확인 응답을 기다리는 상태다.

현재 Animator는 Idle을 계속 재생한다. 실제 정지 여부에 따라 다른 동작으로 전환하는 처리는 후속 애니메이션 작업과 함께 검토한다. 프레임 생성 방식이므로 확대하면 프레임 간 선·형태 차이가 보일 수 있다. 인게임에서의 자연스러움은 확인 전이며 개선 효과로 확정하지 않는다.

## 미리보기
- [원본 PNG 품질의 반복 미리보기](플레이어Idle/Preview.html): 브라우저에서 연다. 확대·작은 크기 비교와 일시 정지를 제공한다.
- [반복 GIF](플레이어Idle/PlayerIdle.gif): 같은 타이밍으로 내보냈으며 GIF 특성상 색·투명도 품질이 낮을 수 있다.

두 미리보기는 제작 에셋 재생이며 실제 Unity 캡처가 아니다.

## 이미지 생성 프롬프트
참조: `PlayerMageConcept_v2.png`. 생성된 원본 PNG를 유지하고 프로젝트 시트로 복사했다. 이미지 파일 자체의 후처리는 하지 않았으며 셀 분할은 Unity 메타데이터로 지정했다.

```text
Use the supplied young witch concept as the exact CHARACTER DESIGN REFERENCE to create ONE production-oriented IDLE ANIMATION SPRITE SHEET for the existing 2D game Mallow Spellpack. Exactly EIGHT full-body frames on a perfectly regular FOUR COLUMNS by TWO ROWS grid, read left-to-right top row then left-to-right bottom row. All eight cells identical size. Target atlas 2048x1536 pixels or a similar 4:3 landscape aspect ratio, each cell portrait-ish 512x768. Genuinely transparent background, including all gutters; no checkerboard painted into pixels, no labels, no grid lines, no cast shadows. Every frame must fit entirely inside its own cell with generous clear gutters. Exactly the SAME cute early-teen apprentice mage throughout: chestnut TWO PIGTAILS, purple robe with modest gold trim and bow, brown boots, small wooden staff with mint crystal in the SAME hand, violet floppy witch hat of the REDUCED v2 size in the reference, about one size too large. Preserve character identity, face proportions, hair length, robe length, staff shape, outfit, colors, restrained clean dark contour and game-friendly broad shading. Both eyes visible under the brim. Idle standing only, same slightly three-quarter front view in every frame, both boots planted in exactly the same location within each cell, same scale and consistent head/body proportions; never walk, attack, jump, change facing, lift a foot, or change accessories. Make an understated gentle breathing and settling motion: 0 neutral eyes open; 1 small inhale with robe and shoulders rising slightly and hat tip/pigtails gently lagging; 2 inhale apex eyes open; 3 gentle exhale eyes open; 4 settle near-neutral eyes open; 5 same resting pose with eyelids HALF CLOSED; 6 same resting pose eyes FULLY CLOSED, clean curved closed-eye strokes clearly readable at small game size; 7 eyes open again, settle back toward frame 0. Blink frames 5–7 must keep the face and all other parts in exactly the same location as frame 4; change eyes primarily, no exaggerated face change. Frame 0 and frame 7 must be nearly identical to make a clean loop. Movement should be small and cute, not rubbery or vigorous. Clear readable closed-eye shape matters more than eyelashes or iris detail. Keep the reference character quality while simplifying only unnecessary tiny detail for sprite readability. Consistency and precise equal-cell layout are essential. No environment, UI, text, duplicate faces within one cell, extra characters beyond the eight animation frames, effects, mouth talking, or alternate designs.
```
