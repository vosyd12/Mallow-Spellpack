# 플레이어 외형 Prefab 통합과 재발 방지

## 문제·원인

PlayerVisual과 PlayerMageV3Visual이 함께 존재했고 SampleScene은 피격 효과 구성이 없는 PlayerMageV3Visual을 참조했다. 플레이어 HitReaction은 모션 Animator를 효과용으로 참조했으며 Effect Visual은 비어 있었다. Awake의 필수 참조 검사에 실패하면 컴포넌트가 비활성화되는 구조다. 컴포넌트가 삭제되는 코드와는 구분한다.

두 Prefab이 다시 나뉜 생성 경위는 이번 작업에서 확정하지 않았다. 이전의 통합 완료 보고와 현재 참조가 달랐으므로 현재 파일을 기준으로 재통합했다.

## 선택한 방법과 달라진 점

- 기준은 `Assets/Prefab/Player/PlayerVisual.prefab`, GUID `360b288df2cc6974b92dca49d2c5b4da`다. Sprite Skin 6개, HitEffect의 전용 Animator·HitEffectVisual·HitArea가 있는 이 Prefab 자체는 변경하지 않았다.
- 기존 Prefab 객체 ID가 기준 Prefab에 모두 존재하고 기존 파츠 Transform의 위치·회전·배율이 같음을 확인했다. SampleScene의 외형 Prefab GUID를 기준으로 이전하면서 모션·FirePos·씬 위치 보정·GroundShadow·PlayerMotion의 기존 객체 ID와 참조를 유지했다.
- 플레이어 루트의 HitReaction은 유지하되 Effect Animator를 자식 HitEffect의 Animator에, Effect Visual을 같은 자식의 HitEffectVisual에 연결했다. PlayerMotion은 기존 모션 Animator를 계속 사용한다.
- `PlayerMageV3Visual.prefab`과 .meta는 Assets에서 제거했다. 복구용 파일은 `Temp/PlayerPrefabUnify20261113/Retired`, 변경 전 씬은 같은 폴더의 Before에 보관했다. Temp는 임시 복구 자료이며 영구 백업이 아니다.
- Sprite 아틀라스·모션 클립·Controller·C#·재질·셰이더는 이번에 변경하지 않았다. Sprite 파일의 별도 정리 작업으로 범위를 확대하지 않았다.

## 재발 방지 기준

[AGENTS.md](../AGENTS.md)에 실제 편집 기준을 추가했다. 모션은 하나의 외형 Prefab·기존 Controller에서 작업하고, 모션별 런타임 Prefab을 복제하지 않는다. 시험 사본은 Assets 밖에 둔다. 외형 교체·병합 뒤에는 파일명뿐 아니라 실제 씬의 GUID와 모션/피격 Animator 분리, Sprite Skin·FirePos·HitArea·필수 참조를 확인한다. 중복 삭제는 참조 이전과 폐기 GUID 참조 부재 확인 후 진행한다.

이는 작업 규칙이며 중복 생성을 자동 차단하는 Editor 기능을 추가한 것은 아니다.

## 확인 결과와 남은 확인

[파일 검증 결과](검증자료/2026-11-13-PLAYER-PREFAB-UNIFICATION/Verification.json)를 남겼다. 기존 Prefab 객체 ID 이전 가능 여부, 씬의 의도한 참조 변경 외 데이터 동일, 기준 Prefab 미변경, Sprite Skin 6개, 관련 보호 파일 290개의 해시 유지와 폐기 GUID의 게임 에셋 참조 부재를 확인했다.

C# 수정·MCP·Unity 임포트/Play 검증은 하지 않았다. Unity에서 실제 컴포넌트 활성 상태, 피격 시 외형 전체 림 틴트, 이동·공격·FirePos·Death·부활 후 외형과 효과 복귀를 확인해야 한다. 파일 검증을 실제 출력 성공으로 기록하지 않는다.
