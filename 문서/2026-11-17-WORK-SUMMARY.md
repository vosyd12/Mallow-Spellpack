# 계층·반응형 HUD·스킬 슬롯 작업 정리

작성일: 2026-11-17. 이번 문서화는 저장된 코드·씬·Prefab과 기존 검증 자료, 대화의 확정 사항을 대조한 작업이다. 게임 코드·씬·에셋을 변경하지 않았고 새로운 MCP·Unity 실행 검증은 하지 않았다. 아래 실제 재생 결과는 같은 날 앞선 작업에서 얻은 자료다.

## 완료한 작업과 현재 기준

| 범위 | 변경·이유 | 확인 범위 |
| --- | --- | --- |
| 계층 | IngameScene을 재생성하지 않고 UI 구분선·EventSystem·ScreenTransition·HUDCanvas·Ingame 구분선·World로 분리했다. World 아래 카메라·조명·Stage·Characters·Effects를 둔다. | 현재 저장 파일과 이전 실제 Unity 계층 조회 |
| 월드 | 이전 부모의 위치·축소 배율을 정리하고 Stage의 상대 배율을 복원했다. 가로 화면에 파란 여백이 드러나 장식용 잔디 160칸을 추가했다. | 당시 3개 해상도 실제 재생. 장식 확장은 이동 경계 확장과 구분 |
| 상단 HUD | 폭 Stretch·비균등 Scale로 UI가 눌리던 문제를 Area의 공간 배치와 Panel의 고정 종횡비로 분리했다. HUD Canvas는 1080×1920, Match 0.5다. | 당시 Game View 5개·Simulator 3개. 이후 Area 오프셋 편집은 아래 별도 기록 |
| 스킬 칸 | 하단 SkillBar에 정사각형 Button 5개를 배치했다. 단일 SkillSlot Prefab을 재사용하고 빈 칸은 +로 표시한다. | 당시 3개 해상도 실제 재생·5개 중심점 Raycast. 현재 파일에서 Prefab 인스턴스 5개와 빈 OnClick 확인 |

현재 주요 UI 계층은 다음과 같다.

```text
HUDCanvas
├ ScreenBlock
├ TopHUD
│ └ TopHUDArea
│   ├ PlayerInfoArea → PlayerInfoPanel
│   └ StageInfoArea  → StageInfoPanel
├ SkillBar
│ └ Slots → SkillSlot01~05
└ SkillScene（비활성）
```

Panel 기준 폭은 Player 240·Stage 460이며 AspectRatioFitter로 각각 1.5·약 3.285714의 종횡비를 유지한다. 높이는 Driven 값이어서 파일의 Size Delta Y가 0이어도 그 값만으로 표시 오류라고 판단하지 않는다. Overlay Canvas의 저장된 Scale 0도 런타임 Canvas 구동값과 구분한다.

SkillSlot은 `Assets/Prefab/UI/SkillSlot.prefab` 하나이며 현재 GUID는 `067ee1143497e0647a7e3201553ae6d3`다. 기준 셀 132×132, 간격 16, 5열 행 너비 724다. Icon·CooldownOverlay·ModeLabel은 비활성 연결 자리이며 장착·쿨다운·모드 전환 기능은 없다. SkillBar는 하단 6% 여백을 사용한다. 초기 빈 상태 문구는 문자열 전달·미포함 한글 글리프 문제로 제거했고 +만 표시한다.

이번 대화에서 AI가 게임 C# 파일을 추가·수정한 것은 없다. 에디터 작업용 C# 명령은 MCP에서 실행했지만 게임 스크립트로 저장하지 않았다. 사용자 코드 작성과 AI의 에셋·Prefab·연결 보조 역할은 유지한다.

## 현재 파일과 과거 검증의 차이

현재 상단 Area에는 이전 Area 검증 시점 이후의 편집값이 남아 있다.

| 대상 | Anchored Position | Size Delta |
| --- | --- | --- |
| PlayerInfoArea | (-110.71954, 0) | (-221.4391, 180) |
| StageInfoArea | (-160.91382, 0) | (-321.8276, 180) |

두 Area의 가로 앵커는 각각 0~0.32·0.32~1이지만 음수 폭 오프셋이 추가돼 있다. 따라서 처음 구현한 단순 32%/68% 공간 분배와 현재 실제 영역 폭은 다르다. 하단 슬롯 작업 때 이 상단 편집 상태는 보존했으며, 슬롯 작업의 세로폰·소형 캡처에는 PlayerInfoPanel이 왼쪽 화면 밖으로 일부 나간 모습이 있다. 과거 Area 검증 통과를 현재 저장 상태 전체의 화면 내 배치 보장으로 사용하지 않는다. 이번에는 값이나 배치를 수정하지 않았다.

자동 Safe Area·배율 최소/최대 제한·극단적인 분할 화면 배치 전환은 미구현이다. Simulator에서 안전 영역 안쪽이었던 결과는 검사한 모델에 한정된다. 실물 기기의 물리적 크기·터치 편의성까지 확인한 것은 아니다. 기존 Play 종료의 비활성 Animator 경고도 UI 좌표계 문제와 구분하며 남아 있다.

## 스킬 실험의 확정 방향과 구현 상태

사용자는 UI 작업을 마무리하고 스킬 시스템의 기초 실험으로 **물속성 Ball·불속성 장판**을 선택했다. 간단한 Shuriken Particle System과 Trail 계열 잔상을 활용하는 방향이다. 최종 이펙트 모양·Trail Renderer와 Particle Trails 중 선택·수치·피해 판정은 아직 정하지 않았다. 이펙트나 두 스킬의 동작을 제작한 상태는 아니다.

| 코드·자산 | 파일에서 확인한 상태 |
| --- | --- |
| SkillData.cs | 실제 외부 클래스 이름은 SkillDatta다. 형태 enum과 중첩 SkillData에 ID·속성·레벨·피해·쿨다운 등의 초안이 있다. 슬롯 장착·발동 시스템은 없다. |
| SkillSlotUI·PlayerSkill·FireballSkill | 빈 Start/Update만 있다. 파일 존재를 스킬 구현 완료로 보지 않는다. |
| SkillSlot Prefab | Image·Button·표시용 자식만 있다. 사용자 스킬 스크립트를 붙이지 않았고 OnClick의 영구 호출은 0개다. |

아래는 **미정**이다. 이전 답변의 추천을 사용자 확정 규칙으로 올리지 않는다.

- 자동/수동 전환을 슬롯별 또는 전체로 둘지와 전환 조작 방법
- 물 Ball의 조준·관통·충돌 후 종료 방식, 불 장판의 생성 위치·범위·지속시간·피해 주기·중첩
- 쿨다운 시작 시점·기본 수치·적이 없을 때의 자동 발동 조건·자원 소모 여부
- 빈 슬롯 탭 동작·장착/교체 흐름·슬롯별 중복 스킬 허용 여부
- UI 터치와 캐릭터 이동/전투 입력의 분리

발동→생성→이동/유지→피해→종료로 흐름을 나누고 VFX와 피해 판정을 분리하는 것은 후속 설계 제안이다. 슬롯별 모드·자동 표적·스킬별 쿨다운도 추천 단계이며 구현하지 않았다.

## 기록과 다음 착수 지점

먼저 모드 전환 단위·두 스킬의 조준/생성 위치·기본 발동 조건을 정한 뒤 사용자 코드와 데이터 연결 범위를 나눈다. 상단 Area의 후속 오프셋 점검은 스킬 UI 완료와 별도 과제로 관리한다. 카메라 추적·AI 개선·실행 중 MaxHP 동기화·HealthBar 외형 개선 등 기존 요청도 완료 처리하지 않았다.

- [계층·월드와 초기 배율 기록](2026-11-17-HIERARCHY-AND-SCREEN-SCALE.md)
- [Area 분리·균일 배율·과거 실제 Simulator 확인](2026-11-17-AREA-RESPONSIVE-HUD.md)
- [스킬 버튼 구성·실제 해상도 및 Raycast 확인](2026-11-17-SKILL-SLOTS.md)
- [Area 검증 자료](검증자료/2026-11-17-Area반응형HUD/Checks.json) · [슬롯 검증 자료](검증자료/2026-11-17-스킬슬롯/Checks.json)
