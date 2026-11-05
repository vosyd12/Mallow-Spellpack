# Mallow Spellpack

Unity 6000.5.10f1로 작업 중인 2D 전투 프로토타입이다. 사용자가 코드를 주도하고 AI는 검토·조언과 요청받은 Hierarchy·외형 작업을 보조한다.

## 열기

1. 이 저장소를 내려받고 Unity Hub에서 프로젝트 폴더를 추가한다.
2. `ProjectSettings/ProjectVersion.txt`의 Unity 버전을 사용한다. Git LFS를 사용하는 환경에서는 이미지 원본도 내려받는다.
3. Package Manager 복원이 끝나면 `Assets/Scenes/SampleScene.unity`를 연다.

`Assets`와 `.meta`, `Packages/manifest.json`·`packages-lock.json`, `ProjectSettings`를 함께 유지한다. Library·Temp·Logs·IDE 캐시는 다시 생성되는 로컬 파일이다.

## 현재 상태

타일 Stage·경계 벽·랜덤 슬라임 생성·크기별 외형/도약·착지 먼지 풀링을 구성했다. 플레이어의 적 선택·자동 공격에는 연결 결함이 남아 있다. 전체 게임의 정상 작동을 보장하는 완료 버전은 아니다.

- [작업 규칙](AGENTS.md)
- [기획 기록 기준](GAME_DESIGN.md): 아직 확정 게임 규칙은 등록하지 않았다.
- [기능 구현 현황](MILESTONES.md)
- [작업 이력](CHANGELOG.md)
- [문서와 남은 문제](문서/README.md)
