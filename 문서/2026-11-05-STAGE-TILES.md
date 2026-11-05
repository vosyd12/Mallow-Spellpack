# Stage 타일 바닥

- 요청: 이전과 같은 연한 민트색 타일 스테이지 외형만 구성.
- 변경: `Assets/Scenes/SampleScene.unity`에 `Stage` 추가. `Backdrop`·`Border`와 `Row01~Row10` 아래 각각 8개 타일을 배치했다.
- 크기: 타일 한 칸 0.75×0.75, 바닥 전체 6×7.5 월드 단위. 이번 외형 배치값이며 전투 영역 규칙을 확정한 것은 아니다.
- 방식: Unity 기본 Square 스프라이트와 URP Sprite-Lit 재질을 사용하는 SpriteRenderer. 바닥 정렬 순서를 -100으로 설정해 기존 플레이어·적 뒤에 표시한다.
- 직접 수정: Stage Transform으로 전체 이동·크기를 조정하고, 각 Tile의 SpriteRenderer Color로 색상을 바꾼다.
- 범위: C# 코드·전투·충돌 기능·기존 UI는 수정하지 않았다. MCP의 Hierarchy/컴포넌트 조작으로 배치 후 씬을 저장했다.
- 확인: Main Camera 캡처로 체크무늬·테두리·기존 캐릭터 표시 확인. Play 모드 실행 및 게임 동작 검증은 하지 않았다. 카메라 직접 캡처에는 Overlay UI가 포함되지 않는다.
