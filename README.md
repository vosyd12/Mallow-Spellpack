# Mallow Spellpack

Unity 6000.5.10f1로 작업 중인 2D 전투 프로토타입이다. 사용자가 코드를 주도하고 AI는 검토·조언과 요청받은 Hierarchy·외형 작업을 보조한다.

## 열기

1. 이 저장소를 내려받고 Unity Hub에서 프로젝트 폴더를 추가한다.
2. `ProjectSettings/ProjectVersion.txt`의 Unity 버전을 사용한다. Git LFS를 사용하는 환경에서는 이미지 원본도 내려받는다.
3. Package Manager 복원이 끝나면 `Assets/Scenes/SampleScene.unity`를 연다.

`Assets`와 `.meta`, `Packages/manifest.json`·`packages-lock.json`, `ProjectSettings`를 함께 유지한다. Library·Temp·Logs·IDE 캐시는 다시 생성되는 로컬 파일이다.

## 현재 상태

초원 Stage·경계·랜덤 적 생성·타깃 탐색/자동 공격·처치 경험치 지급을 연결했다. 속성 슬라임 18종, 도약/공격·착지 효과 풀링·피격·실제 체력과 연결된 HP바를 구성했다. 상단 HUD는 이름·레벨·EXP·Stage·보스 진행도를 표시하며 레벨업 버튼은 조건 충족 시 한 번에 한 레벨을 올린다. 골드·하단 메뉴·보스 자동 진행·사망 후속 처리는 미구현이다. 레벨업의 스탯 보상도 없다. 적용 작업·시안·개선 제안과 검증 한계는 [최신 작업 요약](문서/2026-11-09-WORK-SUMMARY.md)을 따른다.

- [작업 규칙](AGENTS.md)
- [확정 규칙과 미정 사항](GAME_DESIGN.md)
- [기능 구현 현황](MILESTONES.md)
- [작업 이력](CHANGELOG.md)
- [문서와 남은 문제](문서/README.md)
