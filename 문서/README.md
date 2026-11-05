# 프로젝트 현황과 문서 안내

갱신일: 2026-11-06. 현재 저장된 코드·씬·Prefab을 기준으로 정리했다. 구현값은 확정 게임 규칙과 구분한다. GAME_DESIGN.md는 사용자가 규칙을 확정할 때 채운다.

## 어디부터 볼 것인가

| 목적 | 현재 진입점 |
| --- | --- |
| 프로젝트 실행·작업 역할 | [루트 README](../README.md), [AGENTS](../AGENTS.md) |
| 구현 여부 | [MILESTONES](../MILESTONES.md) |
| 날짜별 작업 찾기 | [CHANGELOG](../CHANGELOG.md) |
| 생성·추적·공격·도약 구조와 문제 해결 | [구현 정리](2026-11-06-COMBAT-AND-SLIME-IMPLEMENTATION.md) |
| 외형 자원 | [플레이어·슬라임](2026-11-05-CHARACTER-VISUALS.md), [먼지](2026-11-06-LANDING-DUST-ASSETS.md) |
| 먼지 풀 연결·한정 실행 | [풀링 확인](2026-11-06-LANDING-DUST-POOL-CHECK.md) |
| 수치와 그래프 | [성능 비교](2026-11-06-DUST-POOL-PERFORMANCE.md) |
| 현재 스냅샷과 다른 환경에서의 재개 | [공유 기록](2026-11-06-PROJECT-HANDOFF.md) |

## 현재 남은 문제

1. `Player.cs/UpdateTarget`은 `_targetSearchTimer = Time.deltaTime`을 사용해 검색에 도달하지 못한다. 감소 연산으로 고쳐야 한다.
2. 저장된 SampleScene의 Player `_enemySpawner`는 null이다.
3. `PlayerAttack.cs`는 별도 `_target`을 사용하고, 씬에서도 비어 있다. Player.Target을 받는 연결이 남았다.
4. 현재 고정 Orthographic Size는 일부 세로 화면에서 Stage 좌우를 잘라 보이게 한다. 화면 비율 자동 대응은 미구현이다.
5. 생성 시 적끼리 겹치지 않는 검사는 없다. 목록의 빈 Prefab이나 위치 탐색 실패는 생성을 건너뛰므로 설정한 수가 항상 보장되지는 않는다.

위 문제를 문서에 적은 것이 수정 완료를 뜻하지 않는다. 이번 문서화·공유 작업에서 사용자 C# 코드는 고치지 않았다.

## 검증을 읽을 때

MCP에서 도약/착지와 먼지 반환을 관찰했고, 별도 임시 Play 테스트에서 풀링 성능을 비교했다. 결과는 당시 파일과 실행 조건에 한정한다. 모바일 빌드·실기기·전체 전투 회귀는 미실행이다. 날짜별 문서는 과거 경위를 보존하며 현재 상태는 이 문서와 마일스톤을 먼저 따른다.
