# 사용자 리깅을 정식 플레이어 에셋으로 통합

작성일: 2026-11-11. 기존 Sprite·Prefab을 사용자 리깅 버전으로 교체하고 중복을 정리하라는 요청으로 진행했다. MCP·Notion은 사용하지 않았고 게임 C# 코드는 수정하지 않았다.

## 문제·원인·선택 이유

새 PlayerPartsAtlas_RigTest의 메타파일에 몸통·양팔·양다리·머리의 뼈대와 가중치가 저장돼 있었다. 하지만 PlayerVisual_RigTest Prefab도 기존 아틀라스를 참조했고 Sprite Skin과 실제 뼈 Transform은 없었다. 이름만 교체하면 리깅이 연결되지 않는 상태였다.

정식 경로 PlayerPartsAtlas.png·PlayerVisual.prefab에 새 데이터를 통합하고 기존 GUID와 Sprite 내부 ID를 유지했다. 씬 override·HitReaction 몸통·Staff의 FirePos·기존 애니메이션 참조를 보존하고, RigTest 사본은 Assets에서 제거해 편집 대상을 하나로 줄였다.

## 달라진 점

- `Assets/Sprites/Characters/Player/V3/PlayerPartsAtlas.png`: RigTest PNG와 기존 PNG의 SHA-256은 같아 그림 자체는 동일하다. 정식 .meta를 사용자 리깅 데이터로 교체하고 기존 아틀라스 GUID를 보존했다. 사용자가 만든 뼈 이름·계층·위치·회전·가중치와 메시를 유지했다.
- `Assets/Prefab/Player/PlayerVisual.prefab`: Torso 3개, ArmLeft 3개, ArmRight 3개, LegLeft 1개, LegRight 1개, Head 2개의 뼈를 연결했다. 해당 6개 SpriteRenderer 오브젝트에 Sprite Skin·Root Bone·Bone Transforms를 추가했다.
- 다리는 현재 각 발목 뼈 1개다. 무릎 관절을 새로 만들거나 사용자 리깅을 재설계하지 않았다.
- 설치된 2D Animation 15.1의 SpritePostProcess와 CreateBoneHierarchy 처리에 맞춰 루트는 피벗을 보정하고 모든 뼈 위치를 PPU 804로 변환했다. 뼈 배열은 Sprite 데이터 순서를 유지하며 부모를 지정했다.
- PlayerVisual_RigTest.prefab·PlayerPartsAtlas_RigTest.png와 각각의 .meta를 Assets에서 제거했다. 이 사본들의 GUID를 참조하는 다른 게임 에셋이 없음을 먼저 확인했다. 복구용 원본은 Unity 임포트 대상이 아닌 `Temp/PlayerRigPromotion/Before`와 `Retired`에 보관했다.
- 기존 파츠 계층·Controller·클립과 씬은 유지했다. Hat은 기존 Head 오브젝트 자식이고 Staff는 IdleMotion 자식이다. 머리·손 뼈에 부착하는 작업과 뼈 기반 클립 수정은 별도 후속 범위다.

## 확인 결과·남은 문제

[검증 기록](검증자료/2026-11-11-PLAYER-RIG-PROMOTION/Verification.json)에 부위별 뼈 이름·정점 수·보존 GUID·변경 파일 해시를 남겼다.

- 6개 부위의 정점과 가중치 수 일치·가중치 합·유효한 뼈 인덱스·부모 순환 부재·회전값을 확인했다.
- Sprite Skin 6개의 뼈 참조 13개, 로컬 ID 중복 부재, Transform 부모·자식의 상호 연결을 확인했다.
- 원래 Sprite ID·피벗·크기·PPU와 기존 Prefab 객체 ID를 보존했다. 씬이 참조하는 Prefab 내부 객체가 모두 존재하고 기존 플레이어 클립 경로 614개가 유지되는 것을 확인했다.
- 게임 C#·씬·기존 애니메이션 파일은 작업 전후 해시가 일치했다. Assets에는 정식 PlayerVisual Prefab과 PlayerPartsAtlas만 남았다. 이전 단일 이미지 Idle 등 이번 리깅 사본과 무관한 에셋은 정리 대상에서 제외했다.

예상 결과는 현재 게임의 플레이어가 정식 리깅 아틀라스를 사용하고 뼈를 조작하면 해당 메시가 변형되는 것이다. Unity 네이티브 임포트·Sprite Skin Ready 상태·실제 화면 변형은 이번에 확인하지 않았다. 기존 클립은 파츠 Transform을 움직이는 방식이므로 새 뼈에 키를 작성하는 애니메이션 수정은 아직 하지 않았다. 연결부 변형 품질과 Hat·표정 파츠·Staff의 뼈 추종은 후속 확인·작업이 필요하다.
