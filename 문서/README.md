# 프로젝트 현황과 문서 안내

갱신일: 2026-11-06. 저장된 코드·씬·Prefab·Animation을 기준으로 정리했다. 구현/설정과 실행 확인을 구분한다.

## 현재 상태

타깃 검색과 자동 공격 연결, 가장자리 적 생성, 처치 경험치 지급이 작성됐다. 슬라임은 한 파일의 작은 상태 관리로 일반/공격 도약과 먼지를 재생한다. 공통 빨간 HP바는 머리 위 표시와 높이 추종만 준비됐으며 실제 체력 연결은 남았다.

## 어디부터 볼 것인가

| 목적 | 진입점 |
| --- | --- |
| 실행·작업 역할 | [루트 README](../README.md), [AGENTS](../AGENTS.md) |
| 현재 구현과 남은 문제 | [최신 정리](2026-11-06-CURRENT-IMPLEMENTATION.md) |
| 확정 규칙 / 구현 체크 | [GAME_DESIGN](../GAME_DESIGN.md), [MILESTONES](../MILESTONES.md) |
| 작업 이력 | [CHANGELOG](../CHANGELOG.md) |
| HP바 UI와 코드 연결 지점 | [HP바](2026-11-06-HEAD-HEALTH-BARS.md) |
| 회피 조건 / 공격 연출 | [회피](2026-11-06-PLAYER-AVOID-DISTANCE.md), [공격](2026-11-06-SLIME-ATTACK-ANIMATION.md) |
| Animator 전환 경위 | [Animator](2026-11-06-SLIME-ANIMATOR.md) |
| 이전 전투 연결 문제의 경위 | [초기 구현 기록](2026-11-06-COMBAT-AND-SLIME-IMPLEMENTATION.md) |
| 캐릭터·먼지 자원 | [외형](2026-11-05-CHARACTER-VISUALS.md), [먼지](2026-11-06-LANDING-DUST-ASSETS.md) |
| 과거 먼지 실행 / 성능 실측 | [풀링 확인](2026-11-06-LANDING-DUST-POOL-CHECK.md), [성능 비교](2026-11-06-DUST-POOL-PERFORMANCE.md) |
| 다른 환경에서 재개 / 파일 지문 | [공유 기록](2026-11-06-PROJECT-HANDOFF.md), [CURRENT-SOURCE](2026-11-06-CURRENT-SOURCE.json) |

## 우선 확인할 미완료 항목

1. HP바는 100% 고정이다. 현재 HP/최대 HP 연결이 없다.
2. 씬의 플레이어 최대 HP/MP는 0이며, HP 로그는 정수 나눗셈이다. 실제 플레이어 피격/사망 흐름을 확인해야 한다.
3. TryHitPlayer는 작성됐지만 공격 착지에서 호출하지 않는다.
4. 생성 설정은 4, 조건은 Count > 제한값이라 5마리까지 생성 가능하다. 우선 1마리 유지 규칙과 차이가 있다. 재시도 시간도 현재 시각에서 0.5초를 빼고 있어 대기가 적용되지 않는다.
5. 회피 여유 거리 코드와 기존 안전거리 반환이 겹친다. 게임오버·재시작·화면 비율 자동 대응은 없다.

타깃 검색 타이머·Spawner 슬롯·Player.Target과 Bullet의 Enemy 대상 연결은 현재 해결된 상태다. 과거 문서의 같은 미완료 항목보다 이 문서와 최신 정리를 우선한다. 코드 수정은 사용자가 주도한다.

## 검증 범위

이번에는 정적 대조와 문서 링크·파일 지문을 확인했다. .NET 빌드는 로컬 project.assets.json 누락으로 완료하지 못했다. 과거 빌드 성공·MCP 관찰·먼지 성능 수치는 각 기록의 시점과 조건에 한정한다. 이번에는 MCP·Notion·Play Mode·성능 측정을 사용하지 않았다.
