# 양갈래 마법사 플레이어 시안 v1

Walk 실제 클립: [Unity 파츠 프리뷰 GIF](../../플레이어WalkV3/PlayerMageV3_Walk_Unity.gif). 단독 Animation Clip을 제작했으며 현재 게임의 자동 전환은 미연결이다. [제작·검증 기록](../../2026-11-10-PLAYER-WALK-CLIP.md)

Walk 검토: [반복 GIF](WalkPreview/PlayerMage_Walk_Concept.gif). v3를 참조해 생성한 동작 시안이며 게임에는 미적용이다. [프레임 구성·프롬프트·한계](../../2026-11-10-PLAYER-WALK-PREVIEW.md)

확정된 플레이어 표준 원화: [단순화한 v3 PNG](PlayerMageConcept_v3.png). 사용자 승인 완료이며 [v3 파츠·Idle 게임 적용과 MCP 검증](../../2026-11-10-PLAYER-V3-IDLE.md)을 진행했다. [원화 제작 기록](../../2026-11-10-PLAYER-ART-V3.md)

후속 작업: [레이어 PSD·파츠 PNG 작업본](PlayerMage_v3_Layered/README.md)을 추가했다. 원본 픽셀을 분리해 조립 외형을 보존했으며 경계 정리·가려진 연결부 보완은 남아 있다. [제작·검사 기록](../../2026-11-10-PLAYER-LAYERED-ASSETS.md)

후속 요청의 모자 한 치수 축소 v2와 게임 화면 배치 예상도는 [v2 검토 기록](V2-PREVIEW.md)을 따른다. 실제 게임 적용 결과와 구분한다.

2026-11-10. [시안 PNG](PlayerMageConcept_v1.png). 기본 제공 imagegen으로 생성한 검토용 이미지다. 실제 플레이어 이미지·Prefab·애니메이션·C#은 교체하지 않았다. MCP는 사용하지 않았다.

- 요청 반영: 10대 초반 마법사, 머리보다 두 치수 큰 모자, 보라색 바탕의 로브, 양갈래 머리.
- 제안 표현: 갈색 머리·장화, 절제된 금색 테두리, 민트색 수정이 있는 작은 지팡이. 이 세부 요소는 확정 기획이 아니다.
- 확인: 모자부터 장화까지 전신과 양쪽 머리, 보라색 의상을 확인했다. 배경은 투명하다. 게임 크기에서의 가독성·다른 방향·동작·기존 에셋과의 최종 스타일 통일은 후속 검토 대상이다.

## 생성 프롬프트

```text
Create ONE original full-body character concept for Mallow Spellpack, a cute cozy 2D idle-survival magic game set on a bright grassy meadow with chunky outlined colorful slimes. This is a new playable character concept, not a UI mockup. Subject: one visibly early-teen apprentice witch/mage, approximately 12–13 years old, wholesome youthful proportions and an expressive friendly round face. Chibi proportions roughly three heads tall excluding the hat, modest comfortable practical outfit. Most important design feature: a floppy pointed witch hat TWO SIZES TOO BIG for the character's head, with a wide soft brim and pleasantly oversized drooping crown, making the character especially cute; still show the eyes and face clearly below the brim. Hat in violet/plum purple with one simple muted gold band. Hair MUST be clearly recognizable as TWO PIGTAILS / twin tails, warm chestnut brown, one on each side, emerging visibly beneath the giant hat and tied with small purple ties; do not hide the twin tails behind the robe. Robe: predominantly PURPLE, a simple rounded silhouette with oversized sleeves, restrained lavender shading and only minimal muted gold edging; purple must remain the dominant costume color. Small brown boots. A short simple wooden magic staff with a small softly luminous mint crystal is held close beside the body, restrained and easy to animate, not covering the robe or face. Pose: relaxed idle standing, front-facing with a subtle three-quarter turn and lightly elevated game-camera view. Both hands and feet clearly readable, entire hat, hair, robe, and boots in frame with generous transparent margin. Art direction must fit chunky pixel-inspired 2D meadow/slime assets: bold consistent dark plum/charcoal outlines, clean broad shapes, a few stepped shadow bands with gentle highlights, warm cute readable silhouette, polished game art rather than elaborate anime illustration or 3D render. Keep facial details and cloth details simple enough to read at small sprite sizes. No huge eyes filling the entire face, no adult anatomy, no heels, no armor, no backpack yet, no floating symbols, no attack effects, no UI, no text, no extra characters, no scenic background. Genuinely transparent alpha background. Compose a high-quality single character portrait canvas about 1200x1400 or similar portrait aspect ratio. This is a proposal image only; prioritize the exact requested oversized-hat, purple-robe, early-teen, twin-tail design.
```
