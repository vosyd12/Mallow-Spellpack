# Hierarchy 정리

- 요청: Hierarchy 정리, 이전 뷰로 이동, Stage Prefab 관리 방식 조언.
- SceneSetup: Main Camera, Global Light 2D.
- Stage: 기존 타일 바닥 구성 유지.
- Characters: Player, Enemy.
- UI: Canvas(SafeArea 포함), EventSystem.
- 그룹 Transform은 위치/회전 0, 크기 1이다. 기존 오브젝트 ID와 위치·크기를 확인하고 SampleScene을 저장했다. C# 코드는 수정하지 않았다.
- Scene 뷰는 Stage 중심으로 프레이밍했다. 사용자가 말한 이전 뷰의 의미는 확인 중이며, Game 뷰 비율·게임 카메라·실제 Stage 배치는 변경하지 않았다.
- Stage 전체를 하나의 Prefab으로 관리하는 방식을 제안했다. 아직 Prefab으로 전환하지 않았다. 개별 타일까지 별도 Prefab으로 나누는 것은 현재 구성에 불필요하다.
- 확인: MCP Hierarchy에서 그룹과 기존 Transform 유지 확인. Play 모드 검증은 하지 않았다.
