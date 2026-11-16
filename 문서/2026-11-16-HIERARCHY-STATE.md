# 현재 Hierarchy 구조와 화면 비율 작업 상태

작성일: 2026-11-16. 저장된 `Assets/Scenes/SampleScene.unity`를 읽어 현재 상태를 기록했다. 이번 문서화에서는 C#·씬·Prefab·이미지·설정을 수정하지 않았고 MCP·새 Unity 실행 검증을 하지 않았다.

## 현재 주요 계층

```text
SampleScene                     실제 씬 파일
└ IngameScene                   최상위 GameObject / Canvas
  ├ Stage                       Stage·StageScenery·EnemySpawner
  ├ SceneSetup
  │ ├ Main Camera
  │ └ Global Light 2D
  ├ Characters                  플레이어·생성 적 부모·시작 위치
  ├ UI
  │ ├ EventSystem
  │ ├ HUDCanvas
  │ │ └ TopHUD
  │ │   ├ PlayerInfoPanel
  │ │   └ StageInfoPanel
  │ └ ScreenTransition          암전 UI Prefab 인스턴스
  └ Effects
    └ LandingDustPool
```

`IngameScene`은 새 씬 파일 이름이 아니라 기존 SampleScene 안의 최상위 오브젝트 이름이다. Stage·카메라·캐릭터·UI·효과를 한 부모에 모은 현재 구조를 보존했다. 플레이어 외형은 기존 PlayerVisual Prefab을 사용한다.

## 저장된 설정

| 대상 | 현재 설정 |
| --- | --- |
| IngameScene | RectTransform·Screen Space Overlay Canvas·Canvas Scaler·GraphicRaycaster |
| 최상위 Canvas Scaler | Scale With Screen Size, 기준 **1980×1080**, Match 0.5 |
| 내부 HUDCanvas Scaler | 기준 1080×1920, Match Width(0) 설정이 남아 있음 |
| TopHUD | 현재 Y 앵커 약 0.047~0.954, Size Delta Y 73. 이전 전체 Stretch 설정과 달라짐 |
| PlayerInfoPanel | 좌상단 비율 앵커, 위치 (-4,-6), Local Scale **(1, 0.43747, 1)** |
| Main Camera | Orthographic Size 7.5, Viewport 전체 화면, SceneSetup 아래에서 최상위 Canvas를 부모로 가짐 |

1980×1080은 요청한 점검 기준이며 최상위 Canvas에도 저장돼 있다. **Game 뷰의 실제 출력 해상도를 1980×1080으로 고정했는지는 확인하지 못했다.** Canvas 기준 해상도와 Game 뷰 출력 해상도를 같은 것으로 보고하지 않는다. [파일에서 읽은 설정](검증자료/2026-11-16-하이어라키현황/HierarchySnapshot.json)

## 패널·Image 구성

씬에서 이름이 Panel인 주요 배경은 PlayerInfoPanel과 StageInfoPanel 두 개다. 그 외 경험치/보스 게이지·레벨업 버튼·보스 아이콘의 머리/얼굴/눈/뿔 등의 개별 Image 오브젝트가 있다. HP바와 암전에도 별도 UI 계층이 존재한다.

사용자는 Scene에서 과도하고 복잡하게 보이는 구성을 지적했다. 이번에는 수와 역할을 기록했으며 Panel이라는 이름만으로 불필요하다고 판단하거나 일괄 삭제하지 않았다. 개별 Image의 활성·참조·실제 사용을 대조한 뒤 정리 범위를 정할 필요가 있다.

## 문제 경위와 현재 판단

사용자의 목표는 **Stage·캐릭터·UI가 화면 변화에 함께 반응하고 상대적인 크기·배치를 유지하는 것**이었다. AI가 이를 UI만의 배율 문제로 좁혀 고정 폭 HUD, 이어 화면 폭 기준 UI 확대를 적용하면서 전투 화면과 UI의 크기 균형을 맞추지 못했다. 사용자가 제공한 `화면 캡쳐.png`에는 과하게 큰 HUD와 파란 영역에 걸친 정보 패널이 보였다.

그 후 사용자가 현재의 IngameScene 부모 계층과 1980×1080 기준을 구성했다. 이것은 **현재 저장된 구조**이며 모든 좌표계와 표시 배율이 통합 검증됐다는 뜻은 아니다. 카메라·SpriteRenderer·물리 객체·UI가 함께 부모의 영향을 받는 점, 최상위/중첩 Canvas의 관계, 정보 패널의 비균등 Scale을 실제 화면에서 확인해야 한다.

이 문서화에서는 구조를 다시 바꾸지 않고 현재 상태와 미확인 사항을 분리했다. 이전 두 UI 수정 문서의 배치 계산·과거 한 해상도 Play 결과를 현재 구조의 정상 동작 증거로 사용하지 않는다. 성능이나 비율 개선 수치도 측정하지 않았다.

## 확인 결과와 남은 작업

씬의 단일 최상위 오브젝트·부모/자식 참조·Canvas 설정·패널 이름을 파일로 확인했다. 현재 문서화 전후 게임 에셋의 내용은 유지했다. 화면 캡처 도구의 실패와 프로젝트 기능 오류는 별개이며, 이번 확인은 실제 Hierarchy 창을 캡처해 검사한 결과가 아니다.

현재 미완료 범위는 출력 해상도 확인, Stage·캐릭터·HUD의 상대 비율, 패널/글자/아이콘의 왜곡·가독성, 불필요한 UI 계층 정리 여부다. 중지 요청 이후 이번 문서화로 추가 수정 권한을 확대 해석하지 않는다. 다음 편집은 현재 파일을 다시 대조하고 사용자 지시 범위에서 수행한다.

[이전 고정 폭 UI 시도](2026-11-16-SCREEN-UI-LAYOUT.md) · [이전 UI 폭 비례 수정](2026-11-16-FLUID-UI-CORRECTION.md) · [기능 구현 상태](../MILESTONES.md)
