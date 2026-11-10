# 슬라임 Death 연결과 플레이어 상태 등록

작성일: 2026-11-10. 사용자가 작성한 사망 코드를 확인한 뒤 Animator·Prefab의 연결만 진행했다. **C#·씬·클립·이미지는 수정하지 않았다.** MCP·Notion을 사용하지 않았다.

## 코드 확인과 원인

이전에는 Enemy.TakeDamage가 즉시 Destroy를 호출하고, 구독자가 없는 예외 경로에 Die 재호출이 들어가 있었다. 사용자가 이를 수정해 `TakeDamage → Die → IsDead 설정 → Died 통지`로 이어지고, 구독자가 없으면 Destroy하도록 바꾼 것을 확인했다. Enemy·SlimeMotion의 사망 후 이동 차단, 이벤트 구독/해제, 공격·탄환·검색·회피에서 죽은 적 제외도 확인했다.

코드는 Death 재생과 대기 후 제거를 처리하지만 Controller에 Death 상태가 없고 `_deathClip`·`_groundShadow`가 저장된 Prefab에 연결되지 않아 실행 준비가 끝나지 않은 상태였다. 이 참조를 연결하는 것이 이번 작업이다. 소스 검토 결과이며 원본 게임의 컴파일·Play 검증 완료를 뜻하지 않는다.

## 연결한 내용

| 대상 | 변경 |
| --- | --- |
| Slime.controller | `Base Layer.Death` 상태와 소형 Death 연결. 기존 5개 상태·기본 Idle·전환은 보존한다. Death의 자동 복귀 전환은 추가하지 않는다. |
| SlimeMedium/Large.overrideController | 공통 소형 Death를 각 크기의 Death로 교체하는 매핑 추가. 기존 Jump·Attack 교체도 유지한다. |
| 기본 슬라임 3종 | SlimeMotion의 `_deathClip`을 해당 크기 클립, `_groundShadow`를 기존 GroundShadow에 연결한다. |
| 속성 Variant 18종 | 기본 Prefab의 새 참조를 상속한다. 원본 Prefab·컴포넌트 내부 번호를 바꾸지 않는다. |
| PlayerMageV3Idle.controller | 기존 Idle을 기본 상태로 유지하면서 Walk·Attack·Hit·Death 상태에 각각 준비한 클립을 등록한다. 자동 전환·이벤트·코드 호출은 추가하지 않는다. |

슬라임은 기존 코루틴 흐름대로 Animator 속도를 1로 맞추고 크기에 맞는 0.90초 클립 길이만큼 대기한 뒤 제거하도록 연결했다. Animation Event 방식으로 바꾸거나 사망 규칙을 새로 결정하지 않았다. 사망 중인 적은 현재 Spawner 목록에서 제거되기 전까지 개수에 포함되므로 보충 시점은 기존 코드에 따른다.

## 플레이어에 남은 부분

이 문서의 최초 연결 당시에는 플레이어 재생 코드가 없었다. 이후 사용자가 Player·PlayerAttack·PlayerMotion에 이동 상태·공격 시작/발사/종료·피격·HP 0 사망 코드를 작성했고 소스 검토를 마쳤다. 현재 남은 필수 작업은 PlayerMotion 부착·참조와 Attack/Hit 클립 이벤트 연결이다. 후속 화면과 Attack 동작의 가독성 보완도 남았다. [최신 코드 검토·남은 연결](2026-11-10-PLAYER-MOTION-CODE-REVIEW.md)

## 확인 결과·검증 한계

- 기존 슬라임 상태와 클립 매핑, 플레이어 기본 Idle이 보존되는지 파일로 대조했다. Death 상태명은 코드의 `Base Layer.Death`와 일치한다.
- 3크기 클립 GUID·그림자 내부 참조와 18 Variant의 상속을 확인했다. 각 크기의 `_deathClip`과 실제 상태/Override 클립이 일치한다.
- 연결 스크립트의 작업 전후 비교에서 C#·씬 해시가 일치했다. 사용자가 앞서 수정한 C#은 그대로 유지했다.
- 별도 Unity 6000.5.10f1 임시 프로젝트에서 Controller·Override·참조 클립의 임포트를 확인했다. 원본 프로젝트의 Play Mode·실제 처치·콘솔 확인·모바일 재생은 수행하지 않았다. [참조 대조 결과](슬라임StyleV3/DeathWiringVerification.json) · [임포트 로그](슬라임StyleV3/UnityWiringImport.txt)
- 예상 흐름은 적 HP 0 → 전투/물리 중지·HP바 숨김 → 그림자 숨김·Death → 대기 후 제거다. 실제 화면에서 이 흐름을 확인한 결과와 구분한다.
- 고정 궤적 Death는 실제 사망 순간 자세 포착이나 모든 화면 비율에서의 화면 밖 판정을 구현한 것이 아니다. 해당 세부 처리와 플레이어의 실제 에셋 연결은 미완료다. 이후 작성된 재생 코드는 위 최신 검토 기록을 따른다.
