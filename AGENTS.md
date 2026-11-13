절대 규칙
한국어로 대화
불필요한 과정을 제거하라
해당 작업은 최대한 단순하고 최적화 하라
사용자의 모든 요구사항에 의심을 하라
Coding Convention 준수하라
Best Practice를 생각하라
GAME_DESIGN, MILESTONES, CHANGELOG파일은 작성하기 전에 상단에 존재하는 규칙을 읽을것!
언제나 기능 추가, 변경, 삭제할때마다 CHANGELOG파일에 이력을 남긴다.
사용자가 코드 작업을 주도하고 AI는 보조한다. 추가적으로 기능을 구현하라고 할때는 코드적인 부분을 제외하고 기능을 구현해야한다.
MCP, notion은 요청 시에만 사용한다.
문서화 요청 시 관련 코드·문서 확인 → 구현 상태와 확정 여부 정리 → 관련 문서 최신화 → CHANGELOG 기록 → 코드·문서 간 일치 확인 순서로 진행한다.
상세 기록에는 기능 추가·삭제·변경과 문제 또는 예상 위험 → 원인 → 선택한 해결 방법과 이유 → 달라진 점 → 확인 결과·남은 문제를 간단히 남긴다. 예상 개선과 실제 확인 결과는 구분한다.

플레이어 외형 에셋 관리 규칙
- 게임에서 사용하는 플레이어 외형 Prefab은 Assets/Prefab/Player/PlayerVisual.prefab 하나로 관리한다. 현재 GUID는 360b288df2cc6974b92dca49d2c5b4da이며 임의로 재생성하지 않는다.
- 모션 제작·수정 때문에 별도 런타임 플레이어 Prefab을 만들지 않는다. 기존 Prefab과 Player Controller에 Animation 클립을 연결한다. 실험·복구 사본은 Assets 밖의 Temp에 보관하고 게임에서 참조하지 않는다.
- 외형 교체·파일 이동·병합 후에는 실제 씬의 Prefab GUID, PlayerMotion, FirePos, Sprite Skin, HitReaction의 효과 전용 Animator·HitEffectVisual·HitArea 참조를 확인한다. 기존 모션용 Animator를 피격 효과용 Animator에 연결하지 않는다.
- 중복 Prefab 정리는 참조를 이전하고 삭제 대상 GUID의 Assets 참조가 없는지 확인한 뒤 진행한다. 기준 Prefab이나 GUID가 이전 기록과 다르면 원인을 확인하고 현재 파일을 대조한다. 과거 완료 보고만으로 통합 상태를 단정하지 않는다.
- 완료 보고에는 파일 참조 확인과 실제 Unity 임포트·재생 확인을 구분해서 적는다.
- 플레이어 모션은 Assets/Animations/Player/Player.controller와 PlayerIdle·PlayerWalk·PlayerAttack·PlayerHit·PlayerDeath.anim 5개를 기준으로 관리한다. PlayerMageV3 등 버전별 클립·Controller를 런타임 폴더에 다시 만들지 않는다. 기준 클립의 GUID를 보존하고 기존 파일을 수정한다.
- 모션 변경 후에는 Controller 상태의 실제 클립 GUID와 수신 함수 이름을 함께 확인한다. 현재 목표 이벤트는 Attack의 FireProjectile·FinishAttackMotion, Hit의 FinishHitMotion이다. 이전 FinishAction 이벤트가 남았는지 검사한다.
- 모션 루트 이름은 사용자 결정에 따라 Motion으로 통일한다. 이전 IdleMotion 기준을 대체한다. 이름·계층 변경 시 모든 클립 경로와 내부 바인딩·Avatar Mask의 상대 Transform 경로를 함께 이전하고 검증한다. 2026-11-13 기록 시점에는 클립 경로 이전·Mask 경로 등록·종료 이벤트 연결이 미완료다. [현재 작업 상태](문서/2026-11-13-WORK-SUMMARY.md)를 확인한다.
- 플레이어 파츠 아틀라스는 Assets/Sprites/Characters/Player/V3/PlayerPartsAtlas.png를 기준으로 한다. GUID는 5412993dcc56fcb4098bdbfff14b744d다. 사용자가 작성한 .meta의 Sprite ID·메시·뼈·가중치를 보존한다. 스프라이트 교체·중복 제거 시 PNG뿐 아니라 모든 Renderer의 GUID/ID와 Sprite Skin의 뼈 순서·기본 자세까지 확인한다.
