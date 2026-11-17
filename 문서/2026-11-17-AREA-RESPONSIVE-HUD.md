# Area 배치와 Panel 비율 분리

**현재 파일 대조:** 아래 검증 이후 PlayerInfoArea·StageInfoArea에 음수 위치·폭 오프셋 편집이 추가됐다. 아래 32%/68% 배치와 당시 통과 결과를 현재 저장 상태 전체의 보장으로 사용하지 않는다. [현재 값·후속 범위](2026-11-17-WORK-SUMMARY.md#현재-파일과-과거-검증의-차이)를 따른다.

2026-11-17. 사용자 요청에 따라 Unity MCP로 현재 SampleScene을 수정했다. 작업 전 미저장 편집 내용까지 Assets 밖 `.utmp/AreaHUD-20261117/SampleScene.live-before.unity`에 복사하고 진행했다. 이번 작업에서 변경한 게임 파일은 SampleScene이며 C#·Stage·플레이어 에셋은 수정하지 않았다.

## 문제·원인·선택한 방식

이전 HUD는 패널 폭의 Stretch와 비균등 Scale을 유지한 채 Canvas의 높이 기준만 바꿨다. 기기 비율에 따라 패널 폭과 글자 배율이 다르게 변해 눌리거나 축소돼 보였다. 작업 시작 시 현재 메모리의 두 패널에는 큰 음수 Size Delta와 추가 위치 편집도 남아 있었다. 이를 새 구조로 정리하되 기존 이미지·데이터 참조·버튼 연결은 유지했다.

반응형 공간 배치와 일정한 패널 형태를 분리했다.

```text
HUDCanvas
└ TopHUD
  └ TopHUDArea
    ├ PlayerInfoArea
    │ └ PlayerInfoPanel
    └ StageInfoArea
      └ StageInfoPanel
```

- TopHUDArea는 화면 좌우 4%, 상하 6% 안쪽을 사용한다. PlayerInfoArea는 가로 32%, StageInfoArea는 나머지 68% 공간을 맡는다. 두 Area는 상단 기준 높이 180이다.
- Panel은 각 Area의 상단 중앙에 놓고 Stretch와 비균등 Scale을 제거했다. PlayerInfoPanel의 기준 폭은 240·종횡비 1.5, StageInfoPanel은 폭 460·종횡비 약 3.285714다. 기본 AspectRatioFitter의 Width Controls Height로 높이를 결정한다. 저장 파일의 높이는 Driven 값이므로 0으로 직렬화될 수 있으며 실제 표시 높이는 각각 160·140이다.
- Panel과 내부 요소는 Canvas의 같은 배율로 확대·축소된다. StageNumberText의 X Scale 0.95도 1로 정리했다. 보스 오른쪽 뿔의 대칭 반전은 유지했다. 기존 내부 앵커·폰트 자동 크기 범위·Sliced 이미지·게이지·버튼 이벤트·사용자 작성 스크립트 참조를 유지했다.
- HUD Canvas는 `1080×1920 / Match Width Or Height 0.5`로 바꿨다. 이 설정에서는 같은 픽셀 면적의 세로·가로 해상도가 같은 전체 UI 배율을 사용한다. 실제 1080×2400과 2400×1080에서 Player 패널 약 268.33×178.89, Stage 패널 약 514.30×156.52로 일치했다. 이것은 동일한 물리적 cm 크기나 모든 해상도에서 같은 픽셀 수를 보장하는 설정은 아니다.

Area만 추가하고 기존 Stretch를 남기는 방식과, AspectRatioFitter로 배경만 늘리면서 내부 글자를 별도 축으로 찌그러뜨리는 방식을 피하기 위해 Panel 기준 폭을 고정했다. 시험한 범위에서 Panel이 각 Area보다 작도록 잡았으며 기기별 임의 오프셋을 추가하지 않았다.

## 확인 결과

- **파일·참조:** 기존 Assets 파일 506개는 동일 해시다(SampleScene 제외). 씬 사용자 C# 컴포넌트 11개의 직렬화 내용과 World·Stage 인스턴스·PlayerVisual 인스턴스는 작업 직전과 동일하다. 씬 로컬 참조 누락과 Missing Script는 0개다.
- **원본 Unity 재생:** Game View 320×720, 1080×1920, 1080×2400, 1536×2048, 2400×1080에서 실제 렌더와 패널 크기를 확인했다. 두 패널은 화면과 각 Area 안에 들어가며 종횡비가 유지된다. 현재 표시 문자열의 글자 넘침은 없고 텍스트의 X/Y 배율도 동일하다.
- **Device Simulator:** iPhone 12(1170×2532), Galaxy Z Fold2의 기본 커버 화면(960×2658), iPad 5세대(1536×2048)를 실제 선택했다. 세 모델에서 비율과 Screen.safeArea 내 패널 경계를 확인했다. Fold2의 내부 화면 전환을 검사한 것으로 확대 해석하지 않는다.
- **캡처 구분:** 기본 screenshot 도구는 Game View에 포커스를 옮겨 Simulator 표시와 다른 결과를 만들었으므로 해당 최초 캡처는 폐기했다. 최종 Simulator 자료는 DeviceView의 실제 PreviewTexture를 읽고 DX의 텍스처 Y 방향을 보정해 저장했다. 기기 외곽 프레임·노치 마스크를 합성한 이미지가 아니라 시뮬레이션 해상도의 게임 렌더다.
- **복귀·콘솔:** Play 종료·씬 저장·원래 Game View와 Simulator 기기 상태 복원·임시 해상도 제거를 확인했다. 최종 콘솔 조회에는 오류가 없고 기존 `PlayerMotion.OnDisable → StopAttackMotion`의 비활성 Animator 경고만 있다. C#은 수정하지 않았다.

## 남은 범위

현재 여백은 고정 비율의 보수적 배치이며 `Screen.safeArea`를 실시간으로 읽는 자동 Safe Area 구현이 아니다. 검사한 모델에서 안전 영역 안에 있다는 결과를 모든 기기의 보장으로 기록하지 않는다. 실물 기기 DPI에 따른 물리적 크기·터치 편의성, 매우 긴 캐릭터 이름·큰 숫자, 모든 폴드 상태·극단적인 분할 화면도 별도 확인이 필요하다.

아주 작은 영역의 배치 전환, 전체 배율의 최소·최대 제한, 노치·회전·폴드 변화에 따른 자동 Safe Area 적용은 현재 기본 컴포넌트 설정에 추가하지 않았다. 이 동작이 필요하면 사용자 주도 C# 작업으로 연결해야 한다. 이번에 실제 확인한 것은 Area 분리·고정 종횡비·균일 배율과 위 시험 범위다.

[설정·실측·참조 검사](검증자료/2026-11-17-Area반응형HUD/Checks.json)

[iPhone 12](검증자료/2026-11-17-Area반응형HUD/Simulator-iPhone12.png) · [Fold2 커버 화면](검증자료/2026-11-17-Area반응형HUD/Simulator-Fold2.png) · [iPad](검증자료/2026-11-17-Area반응형HUD/Simulator-iPad.png) · [가로 화면](검증자료/2026-11-17-Area반응형HUD/GameView-2400x1080.png) · [소형 화면](검증자료/2026-11-17-Area반응형HUD/GameView-320x720.png)
