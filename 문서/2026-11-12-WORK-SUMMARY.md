# 보스 게이지·플레이어 사망 재개 작업 현황

최신 변경: 이후 대화에서 기존 전투 유지 대신 성장 상태만 유지한 같은 스테이지 재시작으로 변경했다. 사용자가 전투 초기화 코드를 작성했고 Stage의 Spawner·시작점 연결도 완료했다. 아래 기존 적 HP/위치 보존·재배치 생략 설명은 당시 기록이며 [최신 규칙·구현·연결](2026-11-12-STAGE-RESTART-WIRING.md)으로 대체한다.

작성일: 2026-11-12. 사용자 작성 코드와 현재 Prefab·씬·기존 기록을 확인했다. 이번 작업은 문서화만 수행했으며 C#·게임 에셋을 수정하거나 MCP·Notion을 사용하지 않았다. 아래의 코드 반영은 사용자가 작성한 결과이며 실제 게임 실행 성공과 구분한다.

후속 작업으로 전환 UI·Clear/Cover/Reveal·Controller·종료 이벤트·입력 차단을 제작하고 Stage/Player·ScreenTransition의 씬 참조를 연결했다. [현재 에셋·연결·확인 범위](2026-11-12-DEATH-SCREEN-TRANSITION.md). 아래 최초 정리 당시의 전환 에셋 미연결 상태는 이 후속 기록으로 대체하며 C# 보완 과제는 유지한다.

## 확정한 방향

- 일반 적 사망마다 처치 수를 1 증가시켜 보스 게이지를 채운다. 초기 목표는 30마리다. 보스 처치 후 다음 스테이지로 진행할 때 목표를 1씩 높이되 100으로 제한한다. 실제 보스 생성·처치 호출은 아직 연결하지 않았다.
- 플레이어 사망 시 검은 화면이 위→아래로 덮이고, 완전 암전에서 0.5초 대기한 뒤 위→아래로 화면을 드러내 같은 스테이지를 재개한다. 별도 패배 문구·재도전 버튼·사망 불이익은 두지 않는다.
- 진행 수치 중 처치 수·보스 게이지만 초기화한다. 스테이지 번호·목표 처치 수·레벨·경험치·스탯·기존 적 HP와 위치는 유지한다. 플레이어 HP·사망 상태·물리·외형·체력바는 전투 가능한 상태로 복구한다.
- UI 움직임은 Animation·Animator로 작성한다. 코드에서 RectTransform을 매 프레임 계산하는 제안은 대체했다. 코루틴은 재생 완료와 암전 시간 대기에 사용한다.

## 문제·원인·선택한 해결 방법

| 문제 또는 위험 | 원인 → 선택한 방법 | 현재 결과 |
| --- | --- | --- |
| 처치해도 보스 게이지 전달 안 됨 | EnemySpawner의 Stage 참조 누락 → Stage Prefab 내부 참조 연결 | 초기 목표 30과 참조 저장, 파일 대조 완료 |
| 부활 함수가 바로 반환됨 | 사망·전환 중 false인 CanAct를 부활 조건으로 사용 → IsDead 검사로 변경 | Player.Revive에 반영됨 |
| 사망 표정·투명도·체력바가 남음 | 기존 사망 흐름은 복귀가 없었음 → Revived 이벤트, 모션 재바인딩·원본 Sprite/Color 복구, HealthBar.Show 추가 | 코드 반영, 실제 외형 복원은 미검증 |
| 전환 완료를 무한 대기 | Animation Event만 기다림 → Animator·상태 검사, 비반복 클립 완료 확인과 최대 대기 추가 | ScreenTransition에 반영됨 |
| 전환 실패 후 잘못된 전투 재개 | 완료 여부 확인 없이 다음 단계 진행 위험 → Stage에서 Succeeded 확인 | 실패하면 IsRestarting을 유지하고 중단. 자동 복구·재시도는 없음 |
| 과잉 생성·생성 대기 무효 | Count 비교와 과거 시간 지정 → >= 비교, Time.time + 0.5f | EnemySpawner에 반영됨 |

## 현재 코드·에셋 상태

- `Stage.cs`: 중복 전환 방지, Cover 완료 → 게이지 초기화·Revive → 실시간 0.5초 대기 → Reveal 완료 → 재개 순서. 사망 재개에서는 배경을 재배치하는 EnterStage를 호출하지 않는다. CompleteStage에는 목표 +1·최대 100·다음 스테이지 진입 코드가 있다.
- `ScreenTransition.cs`: Cover·Reveal 재생과 Succeeded 전달. FinishTransition 이벤트 또는 비반복 클립 normalizedTime으로 완료 판단. 최대 대기 기본 10초와 Min(1f)이 있으며 이 값은 정상 연출 시간이 아닌 오류 대기 상한이다.
- `Player.cs`: Stage 기반 CanAct로 이동·피해 차단, Revive에서 HP·물리·회피·타깃 상태 복구, 레벨·경험치 유지.
- `PlayerMotion.cs`: Revived 구독을 조기 반환 앞으로 이동했고, RestoreMotion에서 공격 종료·Animator.Rebind·Idle 평가·원본 Sprite/Color·그림자 복구를 작성했다.
- `HealthBar.cs`: Show에서 표시와 컴포넌트를 켜고 잔상 초기화 후 SetHealth로 동기화한다. Hide·OnDisable은 유지한다.
- `HitReaction.cs`: ResetEffect에서 효과용 Animator를 Idle로 평가하고 피격 프로퍼티를 초기화한다.
- `Enemy.cs`: CanAct로 기본 이동·피해를 차단한다. 적 HP·위치 초기화와 재생성은 추가하지 않았다.
- `EnemySpawner.cs`: 전환 중 생성 차단·생성 상한·0.5초 재시도 간격 반영.
- `PlayerAttack.cs`: Update·MagicAttack에 CanAct 검사가 추가됐고 중복 타이머 검사는 제거됐다. IsDead도 함께 검사하지만 CanAct에 포함된 조건이므로 해당 부분은 정리 가능하다.
- `Assets/Prefab/Stage.prefab`: EnemySpawner → Stage 참조와 초기 목표 30 저장. SampleScene의 기존 StageInfo 연결은 유지한다. [게이지 연결 상세](2026-11-12-BOSS-GAUGE-WIRING.md)

## 아직 남은 작업·위험

1. PlayerMotion의 일반 행동 진입은 여전히 IsDead 위주다. CanAct 적용을 마무리하되 PlayDeath·부활 복구는 전환 중에도 실행되도록 유지한다.
2. SlimeMotion은 아직 IsDead만 검사한다. 이동·공격 타이머와 Animator를 함께 정지·재개하고, 기존 Animator 속도를 복원해야 한다. 이미 사망 중인 슬라임의 Death까지 정지하지 않도록 구분한다.
3. 화면 전환 Canvas·검은 Image·Clear/Cover/Reveal·Controller·ScreenTransition 부착과 씬 참조는 후속 에셋 작업에서 연결했다. Animator는 Unscaled Time, 클립은 비반복·자동 전환 없음이다. 실제 게임에서 반복 사망·화면 비율·진행 보존 확인은 남았다.
4. PlayerMotion.Awake의 참조 오류 처리에는 enabled = false 이후 return이 아직 없다. 잘못된 참조에서 후속 초기화가 계속되는 부분은 정리 대상이다.
5. 전환 실패 후 정지 상태를 유지하는 것은 안전 중단이며 정상 재시작 성공이 아니다. 이벤트는 마지막에 배치하고 전환 완료 전 호출하지 않는다. 자동 재시도 정책은 정하지 않았다.
6. 기존 탄환·적 사망 코루틴 등 모든 게임 객체의 시간을 멈추는 구현은 아니다. 필요 시 남은 탄환의 이동·소멸과 사망 중 적 제거 동작을 별도로 확인한다.

## 확인 결과와 다음 확인

코드의 호출·조건·이벤트 위치와 저장된 Prefab 참조를 파일로 확인했다. 컴파일·Unity 임포트·Play·화면 전환 출력·성능 측정은 이번 문서화에서 수행하지 않았다. 기대 결과인 자연스러운 반복 재개와 실제 확인 결과는 구분한다.

연결 후 연속 사망, 암전 0.5초, HP·표정·알파·체력바 복원, 보스 게이지 0, 기존 레벨·경험치·적 HP·위치 유지, 화면 복구 전 공격 차단과 실패 시 안전 중단을 확인해야 한다. 현재 코드는 Death 종료를 별도로 기다리지 않고 다음 Stage.Update에서 Cover를 시작하므로 두 연출의 겹침과 템포도 확인 대상이다.

[이전 캐릭터 연출 요약](2026-11-11-WORK-SUMMARY.md)의 재시작 미정·미구현 설명은 이번 확정 방향과 부분 구현 상태로 갱신한다. 사망 화면 시안은 이해 확인용 이미지이며 실제 게임 적용 결과가 아니다.
