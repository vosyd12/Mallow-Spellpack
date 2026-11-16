# 플레이어 모션 통합과 실제 재생 연결

후속: 이후 Base/Attack 레이어 분리와 사용자 코드 변경을 진행했다. 모션 루트는 Motion으로 통일하기로 했으나 클립 경로와 새 종료 이벤트 연결은 아직 남았다. 아래는 통합 당시 검증이며 [최신 상태](2026-11-13-WORK-SUMMARY.md)와 구분한다.

## 문제·원인·해결 방법

버전별 PlayerMageV3 클립과 이벤트·페이드·표정을 추가한 Player 클립이 동시에 존재했다. Player Controller는 버전별 클립을 재생해 수정한 내용이 게임에 반영되지 않았다. 특히 실제 Attack 클립에 발사·종료 이벤트가 없어 공격 모션과 탄환 생성이 분리됐다.

검사 중 현재 Prefab의 모션 루트 이름이 Motion인 반면 클립은 IdleMotion 경로를 사용하는 것도 확인했다. 곡선·바인딩 전체를 다시 쓰지 않고 Prefab의 해당 이름만 IdleMotion으로 맞춰 기존 키와 효과 데이터를 보존했다.

## 기준 파일과 달라진 점

`Assets/Animations/Player/Player.controller` 하나와 다음 5개 클립을 기준으로 통일했다.

| 상태 | 기준 클립 | 이벤트·효과 |
| --- | --- | --- |
| Idle | PlayerIdle.anim | 기존 파츠 Idle인 Player3Idle을 GUID 유지하여 이름 변경 |
| Walk | PlayerWalk.anim | 기존 Walk 곡선 유지 |
| Attack | PlayerAttack.anim | 0.30초 FireProjectile, 0.70초 FinishAction |
| Hit | PlayerHit.anim | 0.42초 FinishAction |
| Death | PlayerDeath.anim | 기존 이동·회전·페이드·사망 표정 유지 |

버전별 5개 클립·PlayerMageV3Idle Controller와 기존 단일 이미지 PlayerIdle은 Assets 밖 `Temp/PlayerAnimationUnify20261113/Retired`로 이동했다. 동일 이름으로 남은 PlayerIdle은 기존 Player3Idle의 데이터이며 이전 단일 이미지 클립과 다르다. Temp는 임시 복구 자료다.

Player Controller의 5상태 참조를 기준 클립 GUID로 교체했다. 실제 PlayerVisual과 SampleScene은 기존 Player Controller를 계속 사용한다. C#·공격 함수·Bullet Prefab·FirePos 참조·피격 효과 전용 Animator는 변경하지 않았다. [단일 외형 Prefab 관리](2026-11-13-PLAYER-PREFAB-UNIFICATION.md)와 함께 AGENTS에 클립 관리 기준·이벤트·경로 검증 규칙을 추가했다.

## 확인 결과·남은 확인

[검증 결과](검증자료/2026-11-13-PLAYER-ANIMATION-UNIFICATION/Verification.json)에 기준 GUID·이동 파일·상태 연결을 기록했다.

- Controller 1개·클립 5개, 5상태별 실제 참조와 폐기 GUID 참조 부재를 확인했다.
- Attack·Hit·Walk·Death 파일 해시는 그대로다. Idle은 파일·내부 이름만 변경하고 기존 GUID·곡선을 유지했다.
- 기준 Prefab의 파츠 경로와 애니메이션 경로 616개를 대조했다. Prefab 변경은 Motion → IdleMotion 이름 변경뿐이다.
- PlayerMotion의 FireProjectile → PlayerAttack.MagicAttack, FinishAction 수신 함수와 씬의 Bullet·FirePos·분리된 효과 Animator 참조를 파일로 확인했다. 보호 대상 C#·Sprite·재질·셰이더 216개는 통합 시 해시가 동일했다.

MCP·Unity 임포트·컴파일·Play는 수행하지 않았다. 실제 공격당 한 발 생성, 공격 종료 후 Idle/Walk 복귀, 피격 복귀와 사망·부활 외형을 게임에서 확인해야 한다. 파일 연결 완료를 실제 발사 성공으로 기록하지 않는다.

걷는 하체와 공격 상체의 동시 재생은 이번 범위가 아니다. 현재는 단일 Base Layer와 PlayerMotion의 상태 선택을 유지한다. 별도 레이어·마스크·상체 클립과 상태 관리 보완은 후속 작업이다.
