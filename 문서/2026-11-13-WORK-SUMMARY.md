# 플레이어 에셋 통합·걷기와 상체 공격 작업 현황

갱신일: 2026-11-13. 사용자 작성 PlayerAttack·PlayerMotion, 현재 Controller·Mask·클립·Prefab과 이전 작업 기록을 대조했다. 이번 문서화에서는 C#·게임 에셋을 수정하거나 MCP·Notion을 사용하지 않았다. 사용자가 작업 중인 상태를 기록하며, 코드·에셋 연결과 실제 게임 출력 성공을 구분한다.

## 확정한 방향과 관리 기준

- 외형 Prefab은 `Assets/Prefab/Player/PlayerVisual.prefab` 하나로 관리한다.
- 아틀라스는 사용자가 Skinning 작업한 `Assets/Sprites/Characters/Player/V3/PlayerPartsAtlas.png`를 기준으로 한다. PNG와 .meta의 메시·뼈·가중치·Sprite ID를 보존한다.
- 모션은 Player Controller와 PlayerIdle·PlayerWalk·PlayerAttack·PlayerHit·PlayerDeath 5개를 기준으로 관리한다. 실험·복구 사본은 Assets 밖에 둔다.
- 걷는 중에는 Base의 이동 모션을 유지하고 Attack 레이어로 팔·지팡이 공격을 합성한다. 공격 종료는 상체만 해제하고, 사망·부활 때 상체 공격도 초기화한다.
- Prefab의 모션 루트 이름은 사용자 선택에 따라 **Motion으로 통일한다.** 기존 IdleMotion 기준을 대체하는 결정이며, 현재 클립 경로 이전은 아직 남았다.
- 현재 2D Sprite Skin에서 과장된 움직임은 뼈·가중치와 Animation 키로 만든다. 별도 Avatar 생성은 필수가 아니다. Avatar Mask에는 Humanoid 체크가 아닌 실제 파츠·뼈의 상대 Transform 경로를 등록해야 한다.

## 문제·원인·선택한 해결 방법

| 문제 | 원인 → 방법 | 확인한 결과 |
| --- | --- | --- |
| HitReaction 비활성화 | 씬이 피격 효과 없는 Prefab을 사용하고 Effect Visual이 비어 있었음 → 외형 Prefab 통합·효과 전용 참조 연결 | 파일 연결 복구. 실제 출력은 미확인 |
| 공격 모습만 있고 탄환 발사 없음 | Controller가 이벤트 없는 버전별 Attack을 사용 → 이벤트가 있는 기준 클립으로 참조 통일 | 발사 이벤트 보존. 이후 종료 함수 분리로 종료 이벤트 재연결은 다시 필요 |
| 파츠가 보이지 않음 | 삭제된 아틀라스 GUID 참조 → 기준 PlayerPartsAtlas의 동일 Sprite ID로 이전 | 14개 Renderer 참조 복구, 리깅 데이터 보존 |
| 원치 않는 사망·피격 진입 | 조건 없는 Death·Hit 전환 → 해당 전환 제거 | 현재 Controller에서도 제거된 상태 확인 |
| 걷기·공격 중 하나만 재생 | 단일 상태로 전신 모션 선택 → Base 이동·Attack 상체 분리 | 사용자 코드·레이어 부분 구현. Mask·클립 연결 미완료 |
| 공격 종료 역할 혼동 | 데이터 종료 함수 삭제·피격 종료 호출 혼용 → PlayerAttack.FinishAttack과 PlayerMotion.StopAttackMotion 역할 분리 | 현재 두 파일에는 수정 반영됨 |

## 현재 구현 상태

### 에셋·Animator

- PlayerVisual 하나로 외형 참조를 통일하고 HitReaction에 피격 전용 Animator·HitEffectVisual을 연결했다. 모션 Animator와 효과 Animator는 별개다.
- Player Controller의 Base 기본 상태는 `Locomotion`이며 Idle·Walk Blend Tree를 사용한다. `Blend`는 0이 Idle, 0.5가 Walk다. Hit·Death는 별도 Base 상태다.
- Attack 레이어는 Override이며 PlayerUpperBody 마스크가 연결돼 있다. 기본 상태는 Motion 없는 Empty, 공격 상태는 PlayerAttack이다. 기본 Weight 0은 코드에서 공격 중 1로 올리는 구성과 맞다.
- **PlayerUpperBody의 m_Elements는 아직 빈 목록이다.** Humanoid 영역 값이 바뀌어 있어도 2D 파츠·뼈의 선택 목록 등록이 끝난 것은 아니다.
- PlayerAttack은 비반복이고 FireProjectile 이벤트가 있다. **Attack·Hit 마지막 이벤트는 여전히 FinishAction**이다. 현재 코드의 종료 함수 이름과 맞지 않는다.
- Prefab은 **Motion**, 클립의 파츠 경로는 **IdleMotion/...**이다. 문자열뿐 아니라 내부 경로 바인딩까지 일치시켜야 한다.

### 사용자 작성 C#

- `PlayerMotion.cs`: Base.Locomotion/Hit/Death, Attack.Attack/Empty를 검사하고 준비 완료 후 이벤트를 구독한다. 공격 중에도 Base 이동 Blend를 갱신하고, Attack 레이어만 재생한다. 사망·부활·비활성화 시 상체를 정리한다. 기존 MotionState.Idle 참조 오류와 준비 검사 위치 문제는 수정됐다.
- `FireProjectile()`은 실제 IsAttacking을 검사해 PlayerAttack.MagicAttack을 호출한다. 공격 종료는 FinishAttackMotion, 피격 종료는 FinishHitMotion으로 분리했다.
- `PlayerAttack.cs`: 대상·거리·쿨타임 검사 후 AttackStarted를 알리고, 발사 시점에 탄환을 생성한다. FinishAttack은 공격 플래그·발사 여부·대상을 비우고, ResetAttack은 여기에 쿨타임 초기화를 더한다. 두 함수의 역할은 다르다.
- 이전 검토 때 삭제됐던 FinishAttack은 현재 복구됐다. StopAttackMotion은 이제 피격 종료가 아니라 PlayerAttack.FinishAttack을 호출한다. 공격 불가 시 취소도 데이터 종료 함수를 직접 호출한다.
- 공격 중 피격 동작은 생략하는 기존 정책을 유지한다. 피격색·피해 처리 자체를 생략한다는 의미는 아니다.

## 다음 작업 순서

1. 모든 기준 클립의 IdleMotion 경로와 내부 바인딩을 Motion 기준으로 이전한다. 사용자 리깅과 원래 모션·사망 효과는 보존한다.
2. 실제 모션 Animator 루트를 기준으로 마스크 경로를 등록한다. 우선 양팔·하위 뼈·Staff를 포함하고 다리와 공통 부모 Motion의 공격 변형은 제외한다. 몸통·머리 포함은 합성을 확인한 뒤 조절한다.
3. PlayerAttack 0.70초의 FinishAction을 FinishAttackMotion으로, PlayerHit 0.42초의 FinishAction을 FinishHitMotion으로 변경한다. Attack 0.30초 FireProjectile은 유지한다.
4. 컴파일과 실제 게임에서 정지 공격·걷기 공격·한 발 발사·상체 종료 후 걷기 유지·피격 복귀·사망/부활과 반복 재개를 확인한다.

종료 이벤트 미변경 상태에서는 현재 코드에 없는 FinishAction을 호출하므로 공격·피격 상태가 끝나지 않을 수 있다. 빈 Transform 마스크와 경로 불일치 상태에서는 상체 합성이 정상이라고 판단할 수 없다.

## 확인 범위와 상세 기록

이번에는 파일 내용만 대조했다. 컴파일·Unity 임포트·Play·최종 합성·성능 검증은 하지 않았다. 자연스러운 상하체 분리와 과장된 모션은 기대 목표이며 실제 확인 결과가 아니다. 뼈에 키를 작성하는 과장된 공격과 Hat·얼굴·Staff의 뼈 추종 보완도 별도 후속이다.

- [외형 Prefab 통합·재발 방지](2026-11-13-PLAYER-PREFAB-UNIFICATION.md)
- [모션 5종·Controller 통합 당시 기록](2026-11-13-PLAYER-ANIMATION-UNIFICATION.md)
- [리깅 아틀라스 연결 복구](2026-11-13-PLAYER-ATLAS-RECONNECT.md)

이전 기록의 IdleMotion 기준·단일 Base Layer·종료 이벤트 설명은 당시 상태다. 현재 확정 이름과 부분 구현·남은 문제는 이 문서를 따른다. 복구 자료가 있는 Temp는 임시 폴더이며 영구 백업으로 간주하지 않는다.
