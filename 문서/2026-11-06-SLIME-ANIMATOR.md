# 슬라임 Animator 전환

후속 작업으로 코드가 동작 순서를 관리하는 구조에 맞춰 자동 전환을 제거하고, 높은 공격 도약·강한 착지 먼지·일반 착지 반동을 추가했다. 현재 공격 설정과 검증 범위는 [공격 연출 기록](2026-11-06-SLIME-ATTACK-ANIMATION.md)을 따른다.

## 변경과 이유

- 기존 `SlimeMotion`이 이동과 외형 변형을 함께 계산해 준비·도약·착지의 곡선을 편집하기 어려웠다. 외형 연출을 Animation 클립으로 옮기고, 행동 상태·물리 이동·착지 먼지는 코드에 유지했다.
- 루트에 Animator를 연결하고 `SlimeRoot → VisualRoot → Visual`로 구성했다. 클립은 자식 `VisualRoot`의 위치·크기만 변경한다. 기존 `Visual`의 종류별 크기와 루트 Collider·Rigidbody2D 설정은 보존했다. Root Motion은 끄고 화면 밖에서도 상태가 진행되도록 Always Animate를 사용한다.
- 공통 `Slime.controller`의 Idle·Prepare·Jump·Land를 코드의 상태 진입에서 직접 재생한다. 자동 Transition은 두지 않아 행동 상태를 이중으로 관리하지 않는다. 타깃 소멸 시 Idle로 복귀한다.
- 준비 압축·공중 늘어남·착지 반동에 완만한 곡선을 적용했다. 자연스러움 개선은 의도이며 실제 화면 평가는 남아 있다.

## 에셋과 조정 방법

- 경로: `Assets/Animations/Slime/`.
- 공통 클립: `SlimeIdle.anim`, `SlimePrepare.anim`, `SlimeLand.anim`.
- 도약 클립: `SlimeJumpSmall.anim`, `SlimeJumpMedium.anim`, `SlimeJumpLarge.anim`. 로컬 최고 높이는 각각 1 / 0.75 / 0.5다.
- 소형은 기본 Controller를 사용한다. 중·대형은 `SlimeMedium.overrideController`, `SlimeLarge.overrideController`에서 Jump 클립만 교체한다.
- 모든 클립은 1초·반복 꺼짐을 유지한다. 코드가 Inspector의 Prepare / Jump / Land Duration에 맞춰 `Animator.speed`를 설정한다. 기존 주기 값은 보존했다.
- 높이·압축·반동은 Animation 창에서 `VisualRoot` 곡선을 편집한다. `_hopHeight`는 이제 먼지 강도 계산용이므로 Jump 클립의 최고 높이와 함께 맞춘다. 먼지 강도는 클립의 실제 착지 순간 속도를 측정한 값이 아니다.
- 코드의 `_visual`, `_squashAmount`, `SetVisual()`과 프리팹의 사용하지 않는 `_landingDustPrefab` 직렬화 항목을 제거했다. 기존 소스의 CP949 인코딩은 UTF-8로 통일했다.

## 확인 결과와 남은 확인

- `dotnet build Assembly-CSharp.csproj --no-restore --verbosity quiet` 성공. 오류 0개, 기존 `Stage._stageLevel` 미사용 경고 1개.
- 프리팹·Controller·Override·클립 참조, 오브젝트 계층, 기존 크기·물리·주기 보존, 클립 대상 경로, 지면 아래로 내려가지 않는 높이 곡선, 클립 경계 자세와 종류별 도약 높이의 정적 검사를 통과했다. Unity 임포트·재생 검증을 대체하는 결과는 아니다.
- MCP는 사용하지 않았다. Unity의 에셋 임포트·Play Mode 재생·프레임별 이동/착지 동기화·시각적 자연스러움은 실행 검증하지 않았다. Animator는 Normal, 물리는 FixedUpdate이므로 경계에서의 프레임 차이를 실제 화면에서 확인해야 한다.
- 플레이어 Spawner 연결·타깃 검색 타이머·공격 대상 전달 문제는 이번 작업 범위에 포함하지 않았다.
