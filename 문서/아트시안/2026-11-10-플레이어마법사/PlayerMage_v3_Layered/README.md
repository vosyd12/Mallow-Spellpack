# 플레이어 v3 레이어 작업본

확정 원화의 보이는 픽셀을 나누어 보관하는 편집용 작업본이다. 게임 적용·리깅·애니메이션 완료본은 아니다.

## 파일
- `PlayerMage_v3_Layered.psd`: RGB 8비트 PSD. 파츠 12개와 숨김 비교 원화 1개, 총 13개 레이어.
- `PNG/`: 파츠별 투명 PNG 12개. 모두 1145×1374이며 조립 위치를 유지한다.
- `PlayerMage_v3_Assembled.png`: 확정 원화와 같은 기본 조립 모습.
- `PartsManifest.json`: 레이어 순서, 원점, 각 파츠 범위. 좌우는 화면에서 보는 방향이다.
- `Validation.json`: PSD 구조와 채널 데이터 검사 결과.
- `Sources/ApprovedMaster.png`: 변경하지 않은 확정 원화.
- `Sources/GeneratedPartsAtlas.png`: imagegen으로 새로 그린 연결부 보완 참고 시트. 확정 원화와 형태가 다르므로 최종 파츠로 취급하지 않는다.
- `Sources/ReconstructedPreview.png`: 참고 시트로 조립한 비교용 이미지. 확정 외형을 대체하지 않는다.
- `Sources/GenerationPrompt.txt`: 참고 시트 생성 프롬프트.

## 파츠
`hat`, `head`, `hair_view_left`, `hair_view_right`, `torso`,
`arm_view_left`, `arm_view_right`, `leg_view_left`, `leg_view_right`,
`staff`, `eyes_open`, `mouth_happy`.
팔에는 손과 소매가 함께 들어 있다.

## 사용
PSD를 열면 파츠 조립 모습이 표시된다. 맨 위 `REFERENCE_Approved_v3_HIDDEN`은 비교용 원화이며 기본적으로 숨겨져 있다. 이 레이어를 켜면 아래 파츠를 가리므로 파츠 편집 시에는 끈다.
PNG를 개별로 잘라 저장하면 위치 정보가 달라질 수 있으므로, 원본 크기와 레이어 위치를 보존한다.

## 확인과 한계
보이는 픽셀을 중복 없이 분리했으며 재결합한 RGBA가 확정 원화와 일치하는 것을 검사했다. PSD에는 실제 개별 레이어 채널이 들어 있다. Photoshop·Krita에서 직접 열어보는 호환성 검증은 하지 않았다.
가려진 모자 안쪽·어깨·손 뒤 지팡이·얼굴의 눈과 입 아래에는 원래 이미지에 정보가 없다. 파츠를 이동하면 빈 곳이 드러난다. 경계 마스크의 다른 파츠 색 혼입·잘린 외곽선도 확대 편집으로 정리해야 한다. 참고 시트를 이용해 해당 부분을 덧그린 후 움직임을 붙인다. 현 상태를 바로 애니메이션 가능한 완성 파츠로 간주하지 않는다.
