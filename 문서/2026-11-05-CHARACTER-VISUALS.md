# 플레이어·슬라임 외형

## 요청과 반영

2026-11-05 사용자 참고 화면의 보라색 플레이어와 원형 슬라임을 바탕으로 이미지 외형만 제작했다. 대형의 검은 막대는 끝이 위로 휘어진 콧수염으로 표현했다. 사용자는 이번 외형 적용·화면 확인을 위한 MCP 연결을 허용했다.

| 이미지 | 적용 대상 |
| --- | --- |
| `Assets/Sprites/Characters/PlayerMage.png` | 씬의 `Characters/Player/Visual` |
| `Assets/Sprites/Characters/SlimeSmall.png` | `Assets/Prefab/EnemySmall.prefab` |
| `Assets/Sprites/Characters/SlimeMedium.png` | 기존 `Assets/Prefab/Enemy.prefab` |
| `Assets/Sprites/Characters/SlimeLarge.png` | `Assets/Prefab/EnemyLarge.prefab` |

세 적 Prefab의 `Visual` 자식에서 SpriteRenderer를 수정할 수 있다. 기존 생성기는 기존 Enemy(중간 크기)를 계속 사용한다. 작은·큰 종류의 무작위 생성이나 능력치 차이는 추가하지 않았다.

## 제작 방식

기본 내장 imagegen으로 각각 투명 PNG를 생성했다. 프롬프트의 공통 조건은 단순한 저해상도 2D 게임 스프라이트, 정면에 가까운 시점, 투명 배경, 전체 실루엣 포함, 글자·UI·바닥·무기·효과 제외였다. 개별 조건은 다음과 같다.

- PlayerMage: 짧고 둥근 보라색 후드/로브, 밝은 노란 얼굴, 세로 점 형태의 눈 2개와 작은 손.
- SlimeSmall: 작은 연녹색 둥근 몸체, 점 눈과 작은 미소, 상단 하이라이트, 수염 없음.
- SlimeMedium: 중간 크기의 녹색 말랑한 몸체, 작은 타원 눈과 미소, 상단 하이라이트, 수염 없음.
- SlimeLarge: 큰 녹색 몸체, 눈 아래 중앙에서 갈라져 양끝이 올라가는 검은 콧수염. 직사각형 막대는 피하도록 지정.

생성 결과에는 색상 음영과 가장자리 반투명 표현이 포함된다. 참고 화면을 그대로 복제한 그림은 아니다. 원본 생성 PNG를 편집 없이 프로젝트에 복사했고, Unity의 Sprite Import 설정과 PPU로 표시 크기를 맞췄다. Point 필터·압축 없음·mipmap 없음으로 가져왔다.

## 연결과 확인

기존 루트 SpriteRenderer는 비활성화하고 `Visual` 자식에 새 SpriteRenderer를 연결했다. 플레이어·적 루트 Scale을 바꾸지 않고 자식 외형의 비율만 보정했다. 기존 Enemy GUID와 스크립트·물리 컴포넌트 참조는 유지했다. Small/Large는 기존 Enemy를 복사한 별도 Prefab이며 차이는 외형뿐이다.

MCP에서 Play 없이 별도 임시 카메라와 슬라임 표시용 오브젝트로 [외형 미리보기](CHARACTER-VISUALS-PREVIEW.png)를 확인했다. 이는 실제 랜덤 생성 화면이 아닌 편집기 외형 비교다. 확인 후 임시 오브젝트·카메라를 제거하고 씬을 저장했다. 실제 Main Camera 설정은 변경하지 않았다.

C# 파일 해시 대조에서 변경 없음, 기존 씬/Enemy의 Rigidbody2D·Collider2D·스크립트 컴포넌트 직렬화 내용 변경 없음을 확인했다. Play·공격·생성 기능 검증은 하지 않았다. 소·대형도 충돌 크기/스탯은 기존 적과 같으므로, 외형 크기에 따른 판정 변경은 별도 기능 작업이다.
