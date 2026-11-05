# Stage 경계 벽

- 요청: Player와 Enemy가 Stage 밖으로 나가지 않도록 오브젝트 설정.
- 변경: `Assets/Prefab/Stage.prefab`의 `Bounds` 아래 Left·Right·Top·Bottom을 추가했다. 각 벽은 Trigger가 꺼진 BoxCollider2D만 사용하며 SpriteRenderer·Rigidbody2D·스크립트는 없다.
- 범위: 내부 경계는 로컬 X=-3~3, Y=-3.75~3.75. 벽 두께 0.2, 좌우 벽 중심 X=±3.1, 상하 벽 중심 Y=±3.85로 모서리까지 연결했다. 벽의 크기는 좌우 0.2×7.9, 상하 6.4×0.2다.
- 연결: SampleScene은 기존 Stage Prefab을 참조하므로 변경을 이어받는다. 기존 GUID·오브젝트 ID·타일 구성·C# 코드는 보존했다.
- 확인: 파일 구조와 참조, 벽 4개의 설정을 정적으로 확인했다. 저장된 씬의 Player·Enemy는 Dynamic Rigidbody2D와 비Trigger Collider를 사용하고, Default 레이어 간 충돌도 활성화돼 있다.
- 미확인: MCP 연결·Unity 실행 검증은 하지 않았다. Unity가 Prefab을 다시 가져온 뒤 Play에서 네 가장자리·모서리를 직접 확인해야 한다. 벽에서 방향을 바꾸는 AI나 강제 Transform 이동/순간이동 제한은 이번 범위에 포함하지 않았다.
