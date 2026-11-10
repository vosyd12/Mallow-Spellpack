# 모자 축소 v2와 게임 화면 배치 예상도

후속: 사용자가 v2를 외형 기준으로 확정했으며, 이를 참고한 새 시트와 Idle을 SampleScene에 연결했다. 아래 PNG 설명은 시안 제작 당시의 기록이다. [Idle 적용·실제 확인 범위](../../2026-11-10-PLAYER-IDLE.md)

2026-11-10. 기본 제공 imagegen으로 기존 시안을 편집하고 기존 게임 캡처에 배치한 검토용 PNG다.

- [모자 한 치수 축소 시안](PlayerMageConcept_v2.png): 기존 얼굴·양갈래·보라색 로브 방향을 유지하고 모자를 줄였다. 요청의 한 치수는 약 18~20% 축소 지시로 해석했으며, 실제 게임 크기를 수치로 확정한 것은 아니다.
- [게임 화면 배치 예상도](PlayerMage_IngamePreview_v2.png): 기존 캡처와 v2를 참조해 작은 플레이어 크기로 표현했다. 실제 Unity 실행 캡처나 픽셀 단위의 정확한 합성이 아니며, 슬라임·배경·UI의 현재 저장 상태를 완전히 재현한 증거가 아니다.
- Unity가 PNG의 그림체를 자동으로 바꾸지는 않는다. 표시 크기·카메라·Pixels Per Unit·필터 설정에 따라 세부가 덜 보일 수 있다. 이번 예상도는 그 인상을 살피기 위한 이미지이며 실제 임포트·설정·가독성 검증은 하지 않았다.
- 기존 v1은 보존했다. Assets·씬·Prefab·C#은 변경하지 않았고 MCP도 사용하지 않았다. 캐릭터 스타일·최종 배율과 게임 적용은 미확정이다.

## 모자 수정 프롬프트

```text
Edit target: the supplied transparent full-body young witch concept. Make ONE precise design change: reduce only the purple witch hat by ONE SIZE, approximately 18–20 percent smaller in both brim width and crown height relative to the unchanged head, so it is still adorably slightly oversized, now about one size too big rather than two. Keep the hat seated naturally on the same head and preserve its floppy drooping silhouette, purple hues, simple gold band and shading style. Keep the exact same character identity, youthful early-teen face, expression, chestnut twin pigtails, purple robe with gold edging, proportions, hands, staff with mint crystal, boots, pose and outline style. Do not enlarge the head or shrink the body to fake a hat reduction. Preserve the whole character and transparent alpha background with full hat-to-boots framing and comfortable margins. No text, scene, UI, extra props or alternate characters. High-quality clean concept art, suitable for later 2D game-sprite adaptation. Keep the original canvas proportions.
```

## 배치 예상도 프롬프트

```text
Make a carefully restrained IN-GAME PLACEMENT MOCKUP using the two supplied references. Reference 1 is the EDIT TARGET: an existing Mallow Spellpack gameplay screenshot, portrait 747 by 827 pixels, grassy meadow, cream/olive top HUD, chunky outlined cute slimes and trees. Preserve its entire composition, camera framing, grass texture, trees, flowers, HUD shape and Korean text and stage/EXP information as closely as possible. Reference 2 is the INSERT CHARACTER: the approved-direction early-teen chestnut twin-pigtail mage with the reduced-size purple floppy hat, purple robe/gold edging, brown boots and small mint-crystal wooden staff. Replace the old tiny faceless purple hooded player at approximately x282 y466 with this character. Use a genuinely small plausible gameplay size: total mage height including hat about 60–65 pixels in the 747x827 screenshot, roughly the previous player height, NOT a giant illustration pasted on the game. Position the new player slightly to the left of the nearby large beige slime so that its twin pigtails, hat, robe and staff silhouette can be distinguished, with feet planted on the grass and a restrained matching small ground shadow. Keep the new hat smaller exactly as in reference 2; do not restore the huge version. Keep the same character design and colors; demonstrate how the illustration reads when scaled down. Do not replace it with a different faceless generic mage. At this size face details should naturally become very simple and some fine robe details should be lost; no oversized face inset or extra character portrait. No change to camera zoom or scale of the rest of the game. The latest saved art direction removes floating element emblems above slime heads and eyebrows: remove only those little flame/wave/stone/wind/bolt badges from the slimes in reference 1 and remove medium slime eyebrows, keeping the actual slime bodies, colors, eyes, mouths, large moustaches and health bars intact. Keep all other scene elements unchanged. This is one realistic composition preview image, not a new UI design, not an advertisement, not a character sheet. No new title, arrows, callouts, labels, frames, extra menus or invented visual effects. Output the full portrait gameplay composition at the same aspect ratio as the screenshot; opaque background. It must communicate the plausible small on-screen look without suggesting that this mockup has already been rendered or installed in Unity.
```
