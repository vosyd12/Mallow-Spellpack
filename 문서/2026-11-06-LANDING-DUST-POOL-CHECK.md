# 공용 먼지 풀 연결·실행 확인

후속: [기본·5배·10배 부하의 성능 비교](2026-11-06-DUST-POOL-PERFORMANCE.md). 아래 최초 확인 당시 미측정 사항과 구분한다.

날짜: 2026-11-06. Unity 6000.5.10f1 WindowsEditor, SampleScene, MCP 8099.

## 작업 범위

사용자는 공용 풀의 씬 연결과 전체 실행 확인을 요청했다. 에이전트는 `Effects/LandingDustPool`을 만들고 LandingDustPool 컴포넌트의 `_prefab`에 `Assets/Prefab/Effects/LandingDust.prefab`을 연결해 씬을 저장했다. Scale은 (1, 1, 1)이다. C# 코드는 사용자가 작성한 상태를 검토·실행했으며 수정하지 않았다.

시작할 때 LandingDust Prefab 편집 화면이 열려 있었고 변경 사항은 없었다. 이를 닫고 SampleScene에서 연결·검증한 뒤 원래 Prefab 편집 화면을 다시 열었다. 검증 중에만 runInBackground를 켰고 종료 후 false, timeScale=1, Play 종료 상태로 복원했다. 연결은 Play 전에 저장했다. Play 종료 후 씬은 dirty=true였으며, 미저장 내용을 임의로 버리거나 추가 저장하지 않았다.

## 확인한 흐름

실제 슬라임 40마리의 도약·착지에서 먼지가 풀을 통해 재생됐다. 사용자가 보완한 `_isInUse=true`와 Clear 호출을 확인했다. 약 5.04초의 게임 시간 동안 100회 관찰한 결과다.

| 관찰 항목 | 결과 |
| --- | --- |
| 총 먼지 오브젝트 | 시작·종료·최대 모두 40개 |
| 관찰한 고유 먼지 오브젝트 | 40개 |
| 활성 개수 범위 | 0~40개 |
| 비활성→활성 전환 관측 | 264회 |
| 활성→비활성 전환 관측 | 288회 |
| activeSelf와 `_isInUse` 불일치 | 0회 |
| Play 중·종료 후 Console 오류 | 각각 0건 |

관찰을 시작할 때 이미 29개가 사용 중이었으므로 대여/반환 전환 수는 일치하지 않는다. 0.05초 간격의 샘플링 값이며 전체 이벤트를 계측한 정확한 호출 횟수는 아니다. 관측 구간에서 총 개수가 늘어나지 않고 반복 반환된 사실을 확인했다.

한 시점의 슬라임 상태는 Prepare 5·Jump 35·Land 0이었다. 바닥 범위 밖의 슬라임 중심은 0개였다. 전체 경계 충돌 회귀를 완료했다는 의미는 아니다. [실행 캡처](DUST-POOL-LIVE.png), [계측 자료·코드 기준](2026-11-06-DUST-POOL-EVIDENCE.json)을 함께 보존했다.

## 전체 정상으로 볼 수 없는 부분

- Player의 `_enemySpawner`가 실행 중에도 null이었다. Player.Target도 null로 남았다.
- 현재 Player.cs는 `_targetSearchTimer = Time.deltaTime`으로 대입한다. `-= Time.deltaTime`으로 수정해야 타이머가 감소한다.
- PlayerAttack은 Player.Target을 받지 않고 별도의 빈 `_target`을 사용한다. 따라서 자동 공격·적 처치·다음 타깃 전환 흐름은 통과로 기록하지 않는다.
- 현재 세로 비율의 Game View에서 Stage 좌우가 잘려 보인다. 화면 비율 대응은 이번에 수정하지 않았다.

씬 연결 외의 코드/기획 변경은 하지 않았다. 플레이어 문제를 검증하려고 대상을 임의로 주입하거나 코드를 우회하지 않았다.

## 검사 한계

별도 Profiler의 GC Alloc·CPU 시간·프레임 스파이크 측정은 하지 않았다. 총 개수 유지가 GC 0이나 성능 개선량을 증명하지는 않는다. 모바일 실기기·장시간 검증·씬 전환/강제 종료·풀 최대치 초과는 확인하지 않았다.

진단 정보 출력 도중 Vector3를 직접 JSON 직렬화해 진단 명령이 한 번 실패했다. 좌표 성분을 숫자로 분리해 다시 조회했다. 이는 제품 코드의 런타임 오류가 아니며 최종 Play/종료 Console 오류는 0건이었다. 검사 전 보였던 Unity AI 서비스 NoSubscription 로그는 기존 외부 서비스 로그이며 이번 풀링 결과와 구분했다.
