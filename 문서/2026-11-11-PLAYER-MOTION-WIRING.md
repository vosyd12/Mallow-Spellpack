# 플레이어 Controller 통합과 모션 연결

작성일: 2026-11-11. 사용자 요청으로 Controller·Prefab·씬·클립 이벤트를 연결했다. C# 수정과 MCP·Notion 사용은 없다.

## 문제·원인·선택 이유

기존 Player와 v3 전용 Controller가 함께 남아 편집 대상이 나뉘었다. 현재 Visual에는 v3 Controller가 연결되어 있었지만 PlayerMotion과 클립 이벤트가 없어 행동 전환과 자동 공격이 시작되지 않는 구조였다. 기존 Player Controller의 GUID를 보존하고 현재 5개 클립을 통합해 관리 대상을 하나로 줄였다.

## 달라진 점

- `Assets/Animations/Player/Player.controller`에 Idle·Walk·Attack·Hit·Death를 등록했다. PlayerMotion의 Animator.Play 호출에 맞춰 기존 isMoving 파라미터·전환을 제거했다.
- 현재 파일명 Player3Idle.anim·PlayerWalk.anim·PlayerAttack.anim·PlayerHit.anim·PlayerDeath.anim을 사용한다. 이전 단일 이미지용 PlayerIdle.anim은 이번 삭제 대상이 아니며 현재 Controller에서는 사용하지 않는다.
- PlayerMageV3Idle.controller와 해당 .meta를 삭제하고 PlayerMageV3Visual.prefab의 Animator를 기존 Player.controller로 연결했다. 실제 Idle 클립은 유지했다.
- SampleScene의 Player/Visual에 PlayerMotion을 씬 추가 컴포넌트로 부착하고 Player·PlayerAttack·같은 Visual의 Animator·GroundShadow를 지정했다. 순수 외형 Prefab에 씬 객체 참조를 저장하지 않도록 씬에서 연결했다.
- Attack 0.30초에 FireProjectile, 0.70초에 FinishAction, Hit 0.42초에 FinishAction 이벤트를 등록했다. 발사 0.30초는 이번 연결값이며 실제 화면에서 타격감 조정이 필요할 수 있다.

## 확인 결과·남은 문제

파일 기준으로 Controller 상태·클립 GUID, Visual의 Controller 참조, 씬 컴포넌트와 참조 ID, 이벤트 이름·시간을 대조했다. 삭제한 Controller GUID의 Assets 참조가 남지 않는지 확인했다. C# 소스는 수정하지 않았다.

예상 동작은 Idle/Walk 전환, Attack 재생당 한 발 발사와 종료 후 복귀, 피격 후 복귀, 사망 시 그림자 숨김과 Death 재생이다. Unity 임포트·컴파일·Play 실행은 검증하지 않았으므로 실제 동작 성공과 구분한다.

실제 공격당 한 발·대상 사망 중 발사 취소·반복 피격·HP 0 이후 공격/이동 중지와 화면 비율별 사망 이탈을 확인해야 한다. 기존 코드는 공격 중 Hit 모션을 생략한다. Walk 속도 연동, 사망 순간 자세 유지, 게임오버·재시작은 남아 있다.
