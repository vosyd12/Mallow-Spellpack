# 단순화한 플레이어 원화 v3
작성일: 2026-11-10

## 표준 확정
사용자가 이 v3 원화를 플레이어 표준으로 확정했다. v2는 이전 시안으로 보존한다. 후속 요청에 따라 [레이어 PSD·파츠 PNG 작업본](2026-11-10-PLAYER-LAYERED-ASSETS.md)을 만들고 [v3 파츠 외형·Idle을 게임에 연결해 검증](2026-11-10-PLAYER-V3-IDLE.md)했다. 큰 자세 변형용 경계 정리와 가려진 연결부 보완은 남았다. 아래 원화 제작 기록의 미적용 설명은 당시 기준이다.

## 파츠 보관 권장안
확정 원화와 레이어가 유지되는 편집 원본(PSD·KRA 등)을 보존하고, 모자·머리·양갈래·몸통·팔·다리·지팡이·눈·입을 필요한 단위로 나눈다. 가려진 연결부는 원화의 선·색을 기준으로 보완한다. 각 파츠는 같은 캔버스 크기와 조립 위치를 유지한 투명 PNG로 내보내면 재조립하기 쉽다. Unity에서 여백을 잘라 사용할 경우에는 동일한 위치가 유지되도록 피벗과 좌표를 별도로 맞춘다. 개별 PNG만으로 편집 원본을 대체하지 않는다. 보관 절차는 권장안이다. 이후 레이어 PSD·파츠 PNG 작업본을 추가했으며 경계 정리·연결부 보완과 게임 적용은 남아 있다.

## 요청·제작 범위
최근 인게임 표현 시안의 단순한 외곽선·명암을 기준으로 플레이어 한 명의 투명 배경 원화를 제작했다. [PNG](아트시안/2026-11-10-플레이어마법사/PlayerMageConcept_v3.png). 기본 제공 imagegen을 사용했다. 기존 게임 이미지·씬·C#·Idle은 변경하지 않았다.

## 문제와 선택한 방법
이전 플레이어는 배경·슬라임에 비해 세부 표현이 많아 따로 보인다는 사용자 의견이 있었다. 큰 보라색 모자·갈색 양갈래·보라색 로브와 금색 테두리·민트색 지팡이를 중심으로, 눈·의복 주름·머리카락을 단순화했다. 실제 게임에서 더 자연스러워지는지는 아직 검증하지 않았다.

## 파츠 제작 방법 제안
현재 이미지는 단일 PNG이며 실제 레이어 파일이 아니다. 외형이 확정되면 이 원화를 편집 도구에서 레이어로 분리하고 가려진 어깨·머리·손목 등의 연결 부분만 보완하는 방식을 권장한다. 각 파츠를 별도로 생성하면 비율·선·명암이 달라질 위험이 있으므로 재조립 전후를 대조해야 한다. 사용자가 직접 분리·보정하는 방법도 적절하며, 전체를 처음부터 다시 그릴 필요는 없다. 위 내용은 최초 제안이다. 이후 원본 픽셀을 분리한 레이어 작업본을 추가했으며 자세한 상태는 후속 제작 기록을 따른다.

## 확인과 남은 작업
전신·모자·양갈래·지팡이·장화의 표시와 투명 배경을 확인했다. 파일 크기와 모서리 알파도 확인했다. 기준 시안과 완전히 동일한 이미지를 추출한 결과가 아니라 참조를 바탕으로 새로 생성한 원화다. 외형은 사용자 승인 완료이며, 실제 표시 크기·파츠 연결부·피격·Idle은 후속 확인 대상이다. 플레이어 전체 구현 완료로 처리하지 않는다.

## 생성 프롬프트
참조: 대화 중 생성한 인게임 표현 시안의 오른쪽 마법사 확대 모습.

```text
Use case: stylized-concept. Deliver ONE clean standalone full body 2D game PLAYER BASE ART, genuinely TRANSPARENT background. Reference is the supplied in-game art direction board. Extract and faithfully redraw the SIMPLIFIED CHIBI MAGE shown in the RIGHT HAND enlarged panel of this reference; NOT an elaborate anime mage, not a character sheet. Preserve the approved simple style, face and outfit. This image is to become the single master reference for subsequent manual layered cutout preparation.
Subject: early-teen girl chibi mage, compact roughly 2.5-head body height excluding floppy hat tip, large friendly simple dark oval eyes with a single tiny highlight each, tiny cheerful open mouth, chestnut brown hair in TWO short clearly separated pigtails with small purple ties, very large floppy purple witch hat with simple gold band. Purple short A-line robe, gold-trim simple shoulder capelet, single gold hem and sleeve cuffs, small brown boots. Small wooden staff held on viewer LEFT with ONE mint-green diamond crystal. Same relaxed near-front-facing pose as reference. Other arm relaxed slightly away from torso so silhouette reads. Hat/pigtails/arm/staff visibly distinct without disconnected floating parts.
Style: dark plum moderately bold continuous crisp outlines, rounded cute mobile game sprite design, smooth 2D edges, two or at most three broad cel shade tones per material. Exact uncomplicated style of mage in supplied board. Only one or two large robe fold shadow shapes. Hair rendered in a few large masses, no strands. Eyes deliberately simple, no complex iris details, no decorative lashes. No photorealism, no glossy 3D render, no soft painted texture, no elaborate fabric, no extra ornament, no extra ribbons/buckles/embroidery, no fingernails or detailed fingers. Keep visual density low even at large output. Color palette: muted rich purple, restrained gold, warm chestnut, brown boots, peach skin, mint staff crystal.
Composition: one complete character centered, head to toe fully visible, roomy transparent margins (at least 8% beyond hat tip, brim, staff, boots). Occupies roughly 80% image height. No crop. No environment, no ground oval shadow baked into character, no HUD, no text, no label, no panels, no palette chips, no tiny duplicates, no comparison board. Actual alpha transparency, no checkerboard painted background. This is ONE flattened master drawing, not a falsely claimed PSD. Faithfulness to reference and readability at 80-120px tall matter more than adding detail.
```
