# 사망 시 체력바 UI 숨김 참조 연결

작성일: 2026-11-11. 사용자가 HealthBar.Hide와 Player.Die·Enemy.Die의 호출을 수정한 뒤, AI는 요청받은 씬·Prefab 참조만 연결했다. MCP·Notion은 사용하지 않았고 C# 소스를 수정하지 않았다.

## 문제·원인·선택 이유

HealthBar 컴포넌트가 캐릭터 본체에 부착되어 기존의 `_healthBar.gameObject.SetActive(false)`는 체력바와 함께 본체도 비활성화했다. 슬라임은 OnDisable에서 Died 구독을 해제해 Death 재생 전에 제거될 수 있었다. 사용자가 UI만 숨기는 Hide 메서드로 수정했으나 새 `_visualRoot` 참조가 비어 있어 사망 시 예외가 예상됐다.

본체·Animator·이벤트 연결을 유지하도록 UI 전용 하위 오브젝트를 지정했다. 본체 참조나 UI 에셋 자체 대신 각 캐릭터에 배치된 UI 인스턴스를 사용한다.

## 달라진 점

- SampleScene의 플레이어 HealthBar._visualRoot를 기존 자식 HealthBar UI GameObject에 연결했다. 중첩 Prefab의 GameObject를 참조하는 stripped 항목만 추가했으며 새 UI나 컴포넌트를 생성하지 않았다.
- SlimeSmall·SlimeMedium·SlimeLarge의 HealthBar._visualRoot를 HealthBarMotion에 연결했다. 이 오브젝트는 체력바 UI만 포함하며 본체 VisualRoot·Animator·그림자는 포함하지 않는다.
- 속성 Variant 18종은 기본 3종의 참조를 상속한다. 개별 override는 추가하지 않았다.
- 기존 체력·잔상 Image 참조, 도약에 따른 체력바 위치 추종, 사망 클립·Controller는 변경하지 않았다.

## 확인 결과·남은 문제

파일 기준으로 플레이어 UI의 원본 GameObject·PrefabInstance, 기본 3종의 UI 전용 부모·자식 관계, 6속성×3크기의 상속 경로와 override 부재, 씬·기본 Prefab의 로컬 ID 중복 여부를 확인했다. Player·Enemy의 Hide 호출과 UI 숨김 함수도 대조했다. C# 소스의 작업 전후 SHA-256 일치를 확인했다.

예상 동작은 HP 0에서 체력바 배경·Fill·DamageFill만 숨기고 본체를 유지해 사망 이벤트와 Death 재생을 진행하는 것이다. 이번 작업에서는 MCP 접속·Unity 임포트·컴파일·Play 검증을 하지 않았으므로 실제 정상 재생으로 기록하지 않는다. 이후 슬라임 Death 재생 후 제거와 플레이어 Death·그림자 숨김을 확인해야 한다. 게임오버·재시작과 화면 비율별 이탈 판정은 기존 미완료 범위다.
