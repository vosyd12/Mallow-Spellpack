# 워터볼 자동 발동·스킬 슬롯·명중 연출

저장된 전투 씬에서 자동 발사가 멈춘 문제를 수정했다. `PlayerSkill`의 워터볼 참조를 복구한 뒤 현재 씬에서 자동 발사·첫 슬롯·명중 효과를 다시 확인했다. [씬 참조 복구와 최신 재생 결과](2026-11-18-WATERBALL-SCENE-REFERENCE.md)

## 현재 연결

| 위치 | 적용된 동작 |
| --- | --- |
| `IngameScene > Player` | `PlayerSkill`이 0.1초 간격으로 발동 가능 여부를 검사한다. `EnemySpawner`에서 가장 가까운 활성 상태의 살아 있는 적을 찾고, `WaterBall`이 플레이어 기준 사거리 6을 확인한 뒤 기존 `FirePos`에서 발사한다. 1레벨 기본 피해는 20이다. |
| `SkillSlot01` | 발사에 성공한 시점부터 3초 쿨다운을 표시한다. 워터볼 아이콘 위에 원형 덮개와 남은 초를 표시하며, 자동 모드에서는 버튼 입력이 비활성화된다. `SkillSlot02`~`SkillSlot05`는 빈 슬롯이다. |
| `Waterball.prefab` | `BulletMesh`는 구체 Sprite, `WaterCast`는 발사 물방울, `WaterTrail`과 `WaterDroplets`는 비행 효과를 맡는다. 워터볼은 기존 기본 공격과 별개로 발사한다. |
| `Bullet.OnTriggerEnter2D` | 지정된 적과의 유효한 충돌에서 사용자가 추가한 `_hitEffectPrefab`으로 명중 위치에 효과를 생성한다. 이 필드는 워터볼에서 `WaterballImpact`를 참조한다. 루트 Shuriken의 Stop Action이 효과를 자동 정리한다. |

## 비행 효과 보정

초기 화면에서는 Trail이 큰 위치 오프셋을 가진 부모 아래에 있었고, 발사·비행 Shuriken은 Local로 동작했다. `WaterTrail`을 투사체 바로 아래로 옮겨 원점을 맞췄다. 두 Shuriken을 World로 바꿔 물방울이 발사·비행 위치에 잠시 남게 했다. Trail은 지속 시간 0.22초, 폭 0.28을 사용한다. 구체는 렌더링 자식의 크기만 2.8배 조정했고 투사체 물리 값은 보존했다. [원본 텍스처·재질 제작과 메타 보정](2026-11-18-WATERBALL-MATERIALS.md)

## 명중 효과 설정

초기 물보라와 물결은 잔디 위에서 작게 보여 명중을 놓치기 쉬웠다. 큰 물방울이 먼저 퍼지고, 작은 물방울이 더 빠르게 그 바깥으로 튀도록 두 Shuriken을 나눴다. 물결 고리는 타격 위치에서 넓어지며 사라진다.

| `WaterballImpact.prefab` 위치 | 개수 | 시작 크기 | 시작 속도 |
| --- | ---: | ---: | ---: |
| 루트 Shuriken | 40 | 0.23–0.39 | 2.2–4.3 |
| `FineDroplets` | 32 | 0.065–0.145 | 3.2–5.15 |
| `WaterRipple` | 1 | 1.55 | 0 |

두 물방울 Renderer는 `Assets/Materials/Effects/Waterball/WaterballImpactDrops.mat`을 사용한다. 기존 URP Particles/Unlit 셰이더에서 HDR Emission을 켰으며 저장된 RGB 값은 `(2.25, 4.35, 5.8)`이다. `IngameScene > Main Camera`는 HDR과 Post Processing을 사용한다. `Post Processing` Global Volume의 `Assets/Settings/IngameBloom.asset`에서 Threshold 1.12, Intensity 0.78, Scatter 0.54를 적용했다. High Quality Filtering은 꺼져 있다.

현재 수치는 사용자가 요청한 강한 조정 버전이다. 물방울 수·크기·속도는 각 Shuriken에서, 명중 밝기는 전용 머티리얼에서, 화면의 빛 번짐은 `IngameBloom`에서 조절한다.

## 최초 연출 작업 확인 결과

- 파일 대조에서 씬의 발사·슬롯 참조, 워터볼의 명중 Prefab, 두 물방울 Renderer의 머티리얼, Global Volume의 Profile을 확인했다. 기본 공격의 발사 참조와 워터볼 Collider·Rigidbody 값도 이전 사본과 대조했다.
- Unity MCP 최종 Play Mode에서는 실제 적중 7회 모두 명중 효과가 생성되고 자동으로 정리됐다. 큰 물방울 40개와 작은 물방울 32개, 물결 고리, 기본 공격, 첫 슬롯 쿨다운을 확인했다. 같은 명중 프레임에서 Bloom 켜기·끄기를 비교했다. 최종 재생의 오류는 0건이었다.

## 남은 작업

- 사망 시 워터볼 쿨다운 초기화와 슬롯 표시 복구를 구현한다. `Player.CanAct`의 사망·재시작 중 발동 차단 경로는 코드에서 확인했으며, 해당 상황을 포함한 재생 검증은 남았다. [사망 후 쿨다운과 검증 기준](2026-11-18-WATERBALL-FOLLOW-UP.md)
- 기기 Simulator에서 물보라 가시성을, 실제 모바일 기기에서 Bloom 성능을 확인한다.
- 현재 씬의 워터볼은 자동 모드다. 수동 모드 전환 UI와 화염장판의 씬 연결은 후속 범위다.
