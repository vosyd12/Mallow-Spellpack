# Mallow Spellpack

Unity 6000.5.10f1로 작업 중인 2D 전투 프로토타입이다. 사용자가 코드를 주도하고 AI는 검토·조언과 요청받은 Hierarchy·외형 작업을 보조한다.

## 열기

1. 이 저장소를 내려받고 Unity Hub에서 프로젝트 폴더를 추가한다.
2. `ProjectSettings/ProjectVersion.txt`의 Unity 버전을 사용한다. Git LFS를 사용하는 환경에서는 이미지 원본도 내려받는다.
3. Package Manager 복원이 끝나면 `Assets/Scenes/SampleScene.unity`를 연다.

`Assets`와 `.meta`, `Packages/manifest.json`·`packages-lock.json`, `ProjectSettings`를 함께 유지한다. Library·Temp·Logs·IDE 캐시는 다시 생성되는 로컬 파일이다.

## 현재 상태

타일 Stage·경계 벽·맵 가장자리 랜덤 생성·플레이어 타깃 탐색/자동 공격·적 처치 경험치 지급을 연결했다. 슬라임 도약/공격 Animator·착지 먼지 풀링·머리 위 공통 빨간 HP바 UI를 구성했다. HP바는 표시만 준비됐으며 실제 체력 연동은 남아 있다. 생성 수/재시도 시간, 플레이어 초기 HP와 적 타격 호출 등 남은 문제는 [최신 현황](문서/README.md)을 따른다.

- [작업 규칙](AGENTS.md)
- [확정 규칙과 미정 사항](GAME_DESIGN.md)
- [기능 구현 현황](MILESTONES.md)
- [작업 이력](CHANGELOG.md)
- [문서와 남은 문제](문서/README.md)
