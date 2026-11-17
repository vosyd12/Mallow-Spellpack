# 계층 정리와 Stage·HUD 화면 배율

**같은 날 후속:** 사용자 확인에서 HUD 축소·왜곡이 남아 Area와 Panel을 분리하고 고정 종횡비·균일 배율로 교체했다. 아래 Match Height와 패널 설정 보존은 이전 작업의 기록이다. [현재 설정·실제 Simulator 확인](2026-11-17-AREA-RESPONSIVE-HUD.md)을 따른다.

2026-11-17. 사용자가 저장한 최신 SampleScene을 Unity MCP로 확인하고 수정·저장했다. C# 파일은 수정하지 않았다.

## 변경과 원인

이전 부모에서 분리된 Stage·SceneSetup·Characters·Effects에는 공통 위치 `(-133.39929, 146.65808)`와 배율 `0.651863`이 남아 있었다. HUD는 화면 폭, Orthographic Camera는 화면 높이 기준이어서 가로·세로 화면 전환 시 월드 대비 UI 크기가 달라졌다. 파일에 저장된 Canvas Transform 값은 Unity가 구동 중 다시 계산하는 값과 달랐으므로, 저장된 Scale 0만으로 화면이 보이지 않는다고 판단하지 않았다. 실제 수정 전 Game 화면에서도 HUD는 표시됐다.

이미지 요청에 맞춰 현재 계층을 다음과 같이 구성했다. UI와 Ingame은 빈 구분선 오브젝트이며 Canvas 부모가 아니다. 사용자가 삭제한 IngameScene은 다시 만들지 않았다.

```text
SampleScene
├ UI ====================
├ EventSystem
├ ScreenTransition
├ HUDCanvas
│ ├ ScreenBlock
│ ├ TopHUD
│ │ ├ PlayerInfoPanel
│ │ └ StageInfoPanel
│ ├ SkillBar
│ └ SkillScene                 비활성
├ Ingame ====================
└ World
  ├ Main Camera
  ├ Global Light 2D
  ├ Stage
  ├ Characters
  └ Effects
```

ScreenBlock·SkillBar·SkillScene은 요청한 배치를 위한 빈 RectTransform 컨테이너다. 입력 차단은 기존 ScreenTransition의 InputBlocker가 담당하며 새 스킬 동작·설명창·입력 차단 기능을 구현한 것은 아니다.

World·Characters·Effects를 원점·Scale 1로 정리하고 Stage는 기존 상대 크기인 `(2,2,1)`로 복원했다. Stage에 남은 X축 1도 회전을 제거했다. 카메라는 `(0,0,-10)`, Orthographic Size 7.5로 유지한다. 이전 공통 축소 배율 제거로 월드가 화면에서 더 크게 보이며, Stage 대비 캐릭터의 배율 관계는 유지한다.

HUDCanvas와 ScreenTransition은 독립 Overlay Canvas로 두고 둘 다 `1080×1920 / Match Height(1)`로 맞췄다. TopHUD는 전체 화면 Stretch·위치/크기 오프셋 0으로 정리했다. 카메라의 월드 표시 높이 15와 HUD 배율이 모두 화면 높이를 기준으로 변하므로 상대적인 세로 크기를 유지한다. 화면이 넓어지면 카메라는 더 넓은 월드 영역을 표시하고 패널 폭은 사용자가 설정한 앵커를 따른다.

사용자가 수정한 두 정보 패널의 앵커·Anchored Position·Size Delta·Pivot·Scale과 이미지의 Sliced·Pixels Per Unit Multiplier·스프라이트 참조는 보존했다. 기존 비균등 Scale과 내부 글자 배치는 새 디자인으로 교체하지 않았다.

1980×1080 재생에서 잔디 바닥의 양쪽 끝 바깥으로 파란 카메라 배경이 드러났다. 카메라나 충돌 경계를 늘리는 대신 Stage Prefab의 장식용 Grass 타일을 가로 14칸에서 24칸으로 확장했다. 기존 224칸의 타일·색·행렬을 보존하고 바깥쪽에 160칸을 추가했다. Prefab의 타일맵 외 오브젝트와 로컬 충돌 경계·생성 설정은 동일하다. Stage 자체의 배율 복원과 장식 바닥 확장은 구분한다.

## 실제 확인 결과

- **파일·참조:** 보호 대상 379개 파일 중 Stage Prefab만 변경됐다. C#·플레이어 Prefab·모션·아틀라스·메타는 유지했다. 씬 로컬 참조 누락 0, PlayerVisual 인스턴스 Override 동일, PlayerMotion·FirePos·Sprite Skin·별도 피격 효과 참조와 원본 Prefab GUID를 보존했다.
- **Unity 임포트·재생:** 원본 프로젝트에서 Stage 재임포트와 Play를 수행하고 1080×1920·1980×1080·1080×1080의 실제 Game 화면을 촬영했다. 두 패널의 경계가 화면 안에 있고 화면 높이 대비 패널 높이는 각각 약 5.013%·3.955%로 유지됨을 실측했다. 가로 화면의 파란 여백이 사라진 것을 이미지로 확인했다.
- **복귀:** Play 종료, 원래 Game View 크기 복원, 임시 해상도 항목 제거, 씬 저장 상태와 Missing Script 0개를 확인했다.
- **검증 중 문제:** 일시정지 중 해상도를 바꾼 최초 캡처는 이전 UI 렌더 상태가 남아 폐기하고 재생을 재개한 뒤 최종 캡처를 다시 얻었다. 임시 해상도 정리 중 인덱스를 잘못 전달한 MCP 작업 오류 7건은 전체 인덱스로 수정해 정리했다. 이 로그를 게임 오류 또는 오류 0건으로 보고하지 않는다.
- **남은 경고:** Play 종료 시 기존 `PlayerMotion.OnDisable → StopAttackMotion`에서 `Game object with animator is inactive` 경고가 발생했다. C# 수정 범위가 아니므로 유지했다. 카메라 추적·Safe Area·모든 기기 비율·최종 글자 가독성 및 장시간 전투 검증은 이번 확인에 포함하지 않았다.

[파일·실측 검사](검증자료/2026-11-17-계층과화면배율/Checks.json) · [실제 계층](검증자료/2026-11-17-계층과화면배율/Hierarchy.json)

[세로 1080×1920](검증자료/2026-11-17-계층과화면배율/GameView-1080x1920.png) · [가로 1980×1080](검증자료/2026-11-17-계층과화면배율/GameView-1980x1080.png) · [정사각형 1080×1080](검증자료/2026-11-17-계층과화면배율/GameView-1080x1080.png)
