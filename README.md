# Mallow Spellpack

Unity 6000.5.10f1로 작업 중인 2D 전투 프로토타입이다. 사용자가 코드를 주도하고 AI는 검토·조언과 요청받은 Hierarchy·외형 작업을 보조한다.

## 열기

1. 이 저장소를 내려받고 Unity Hub에서 프로젝트 폴더를 추가한다.
2. `ProjectSettings/ProjectVersion.txt`의 Unity 버전을 사용한다. Git LFS를 사용하는 환경에서는 이미지 원본도 내려받는다.
3. Package Manager 복원이 끝나면 `Assets/Scenes/IngameScene.unity`를 연다. 과거 문서의 SampleScene은 현재 씬 경로와 구분한다.

`Assets`와 `.meta`, `Packages/manifest.json`·`packages-lock.json`, `ProjectSettings`를 함께 유지한다. Library·Temp·Logs·IDE 캐시는 다시 생성되는 로컬 파일이다.

## 현재 상태

플레이어는 하단 왼쪽 조이스틱이나 WASD·방향키로 이동하고 기본 공격은 자동으로 발동한다. 보스 자동 진행, 사망 후 같은 스테이지 재시작, 처치 골드 HUD, 별도 스탯창과 워터볼·화염 장판 카드 보상도 연결돼 있다. 조작 검증과 1인 던전·전투력 검토는 [최신 인수인계](문서/2026-11-24-MANUAL-COMBAT-AND-DUNGEON-HANDOFF.md)에 정리했다.

워터볼은 기본 공격과 별개로 자동 발사한다. 첫 슬롯에서 3초 쿨다운을 표시하며, 실제 명중 시 물보라와 물결 고리를 재생한다. 씬 구성·Shuriken·Bloom 설정과 Unity 재생 결과는 [워터볼 작업 기록](문서/2026-11-18-WATERBALL-INTEGRATION.md)을 따른다.

이전 UI 구성은 [11월 17일 작업 요약](문서/2026-11-17-WORK-SUMMARY.md), 당시 이월 과제는 [11월 18일 수정·개선 목록](문서/2026-11-18-IMPROVEMENT-BACKLOG.md)에 기록했다. 아래는 이전 기반 기능의 연결 기록이다.

플레이어·슬라임 전체의 [공통 중심 림 틴트](문서/2026-11-11-CHARACTER-RIM-TINT.md)를 연결했다. HitEffect의 전용 Animator·Animation과 HitArea에서 피격 강도·색상·범위를 조절하며 기존 캐릭터 모션과 사망 페이드를 유지한다. 파일·수치 검증 완료이며 실제 화면 검증은 남았다.

플레이어 외형은 [사용자 리깅을 통합한 정식 에셋](문서/2026-11-11-PLAYER-RIG-PROMOTION.md)인 `PlayerPartsAtlas.png`·`PlayerVisual.prefab`을 사용한다. 몸통·양팔·양다리·머리에 Sprite Skin 6개와 뼈 13개를 연결하고 중복 RigTest 에셋을 Assets에서 제거했다. 파일 검증 완료이며 실제 변형 검증과 뼈 기반 클립 수정은 남아 있다.

11월 9일에는 초원 Stage·경계·랜덤 적 생성·타깃 탐색/자동 공격·처치 경험치 지급을 연결했다. 속성 슬라임 18종과 도약/공격, 착지 효과 풀링, 피격, HP바, 경험치 HUD의 초기 연결은 [당시 작업 요약](문서/2026-11-09-WORK-SUMMARY.md)에 기록했다.

플레이어는 [11월 11일 Controller 통합·모션 연결](문서/2026-11-11-PLAYER-MOTION-WIRING.md)에서 기존 Player Controller에 Idle·Walk·Attack·Hit·Death와 PlayerMotion·클립 이벤트를 연결했다. Bullet 생성 위치는 Staff/FirePos다. 이번 연결의 실제 재생 검증과 Walk 속도 연동·사망 후속 처리는 남아 있다. 사망은 현재 외형 전체가 회전하며 포물선으로 화면 밖에 날아가는 방향으로 정했다. [11월 10일 요약](문서/2026-11-10-PLAYER-WORK-SUMMARY.md)은 이전 상태를 기록한다.

- [작업 규칙](AGENTS.md)
- [확정 규칙과 미정 사항](GAME_DESIGN.md)
- [기능 구현 현황](MILESTONES.md)
- [작업 이력](CHANGELOG.md)
- [문서와 남은 문제](문서/README.md)
