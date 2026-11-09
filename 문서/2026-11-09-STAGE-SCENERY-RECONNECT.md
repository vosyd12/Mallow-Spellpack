# StageScenery 타입별 필드 재연결

작성일: 2026-11-09.

## 문제와 원인

사용자가 StageScenery를 `_groups` 배열에서 타입별 설정·수량 필드로 변경했다. Stage Prefab에는 기존 배열 데이터만 남아 있어 새 필드에 Prefab과 배치 지점이 자동으로 연결되지 않는 상태였다.

## 변경과 선택 이유

사용자의 연결 요청에 따라 `Assets/Prefab/Stage.prefab`의 해당 컴포넌트 데이터만 이전했다. 기존 연결을 재사용해 배경 종류·배치 후보·크기를 유지했으며 C# 코드는 수정하지 않았다.

| 기존 그룹 | 새 설정 / 수량 필드 | Prefab 수 | 배치 지점 수 | 크기 배율 |
| --- | --- | --- | --- | --- |
| Tree | `_trees` / `_treeCountRange` | 2 | 14 | 0.65~0.8 |
| GrassClump | `_bushes` / `_bushCountRange` | 2 | 12 | 0.9~1.1 |
| FlowerBush | `_flowerBushes` / `_flowerBushCountRange` | 2 | 12 | 0.9~1.1 |
| Flowers | `_flowers` / `_flowerCountRange` | 2 | 12 | 0.9~1.1 |

네 타입의 수량 범위는 기존과 동일한 `(3, 4)`다. 기존 `_groups` 데이터는 제거하고 `_sceneryRoot`, 위치 변화 0.14, 최소 간격 0.65 및 다른 컴포넌트는 유지했다. SampleScene은 이 Stage Prefab을 참조하므로 Prefab 연결을 상속한다.

## 확인 결과와 남은 확인

- 새 필드명과 현재 C# 선언을 대조했다.
- Prefab 참조 8개의 GUID와 대상 GameObject fileID, 배치 지점 50개의 Transform fileID가 실제 에셋에 존재함을 확인했다.
- 이전 대상 블록 외 Prefab 내용이 동일하고 C# 파일 해시가 변경되지 않았음을 확인했다.
- 예상 효과는 새 타입별 필드에서 기존 배경 설정을 읽어 배치하는 것이다. Unity 임포트·컴파일·실제 배치·재진입은 이번 작업에서 실행 검증하지 않았다. 후보와 간격에 따라 요청 수량보다 적게 배치될 수 있다.

11월 8일 검토 기록에서 미구현으로 남겼던 구조 정리는 이후 사용자가 코드에 반영했다. 이번 작업은 그 코드에 맞춘 Prefab 연결이며, 당시 기록의 실행 결과를 이번 변경의 실행 검증으로 간주하지 않는다.
