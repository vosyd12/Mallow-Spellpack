# 속성 슬라임의 분리된 얼굴 흔들림

후속 상태: [18종의 크기·속성 지정과 Spawner 등록](2026-11-07-ELEMENTAL-SLIME-SPAWN.md)을 완료했다. 아래 생성 미연결 설명은 얼굴 연출 작업 당시의 범위를 보존한 것이다.

작업일: 2026-11-07. 사용자가 미리 장착한 속성 슬라임 18종에 얼굴만 따로 흔들리는 연출도 마무리하도록 요청했다.

## 문제와 원인

[사전 장착](2026-11-07-ELEMENTAL-SLIME-PREFABS.md)에서는 얼굴까지 그려진 PNG를 Body에 사용했다. 기존 FaceMotion을 켜기만 하면 새 그림 속 얼굴 위에 원본 눈·입이 중복 표시된다. FaceMotion을 꺼 두어 중복은 막았지만 얼굴만 따로 움직이는 연출은 보이지 않았다. 이동·상태 관리 코드의 문제가 아니라 이미지 레이어와 렌더러 연결의 문제였다.

## 선택한 방법과 변경 결과

빌트인 `image_gen` 편집으로 속성·크기마다 몸과 얼굴이 나뉜 2칸 아틀라스를 만들었다. 왼쪽은 얼굴을 지운 몸과 속성 장식, 오른쪽은 투명 배경 위의 얼굴이다. Small의 홍조, Medium의 눈썹, Large의 콧수염은 얼굴과 함께 움직인다. 속성 장식은 몸에 남긴다.

- 이미지: `Assets/Sprites/Characters/Slimes/{속성}/Slime{속성}{크기}Layers.png` 18개.
- 적용 대상: `Assets/Prefab/Slimes/{속성}/Slime{속성}{크기}.prefab` 18개.
- 예: [불 Small 레이어](../Assets/Sprites/Characters/Slimes/Fire/SlimeFireSmallLayers.png), [물 Medium 레이어](../Assets/Sprites/Characters/Slimes/Water/SlimeWaterMediumLayers.png), [무속성 Large 레이어](../Assets/Sprites/Characters/Slimes/Pure/SlimePureLargeLayers.png).

```text
VisualRoot/Visual
├─ Body                       얼굴 없는 몸 + 속성 장식
└─ FaceMotion                 기존 지연 흔들림 Animation
   └─ Face
      ├─ FaceFeatures         눈·입·홍조/눈썹/콧수염 Sprite
      └─ Smile/Moustache      기존 부품 비활성
```

기존 Eyes 자식을 FaceFeatures로 바꿔 얼굴 Sprite 전체를 표시하고, 이전 입·콧수염은 숨겼다. FaceMotion 경로와 준비·도약·착지·공격의 기존 클립은 그대로 상속한다. 새 상태 머신이나 C#을 추가하면 같은 연출을 이중으로 관리하게 되므로 Prefab·Sprite 설정만 변경했다.

두 Sprite는 같은 좌표계·피벗·PPU·배치를 사용한다. 몸은 가만히 두고 FaceMotion만 이동해도 얼굴이 따로 움직인다. 투명 여백에 따라 크기가 바뀌지 않게 몸의 보이는 가로폭을 기준으로 정규화했다. Small 가로폭 20% 확대, Medium·Large의 기존 가로폭, 그림자·먼지·HP바 연결을 유지한다. 높이는 새 이미지의 비율을 따른다.

원본 시안과 기존 게임용 통이미지는 보존한다. 이미지 편집 생성이므로 원본과 픽셀 단위로 동일하다는 의미는 아니다. 분리 결과의 표정·장식·윤곽을 확인했으며, 경계를 넘거나 배경에 큰 잡티가 남은 결과는 다시 생성해 교체했다. 앞으로 외형 교체 시 얼굴이 없는 Body와 투명한 Face의 한 쌍을 사용해야 한다.

## 조절 위치

얼굴의 흔들림 크기·지연 시점은 `Assets/Animations/Slime/`에서 `VisualRoot/Visual/FaceMotion`의 위치 곡선을 조정한다. 모든 속성 Variant가 크기별 원본의 클립을 상속하므로 공통 조정은 원본 클립에 적용한다. 얼굴의 기본 위치·크기는 해당 Variant의 FaceFeatures에서 조정한다. 이번에는 클립 자체를 변경하지 않았다.

## 확인 결과와 남은 확인

파일 검사로 18종의 몸·얼굴 Sprite 참조, 아틀라스 2칸의 좌표·피벗·배치 일치, FaceMotion 활성화와 중복 표정 비활성화를 확인했다. 기존 클립에 준비·도약·착지·공격의 FaceMotion 위치 변화가 있는지도 대조했다. 기존 파일 중 바뀐 것은 대상 Variant 18개이며, C#·원본 3종·씬·Animation·기존 PNG는 작업 전 해시와 동일하다.

[레이어 겹침 미리보기](검증자료/2026-11-07-속성슬라임얼굴/FACE-PREVIEW.html)에서 얼굴을 숨기거나 분리 확인용 웹 흔들림을 켤 수 있다. 이 데모는 Unity 재생 검증을 대신하지 않는다. [정적 확인 결과](검증자료/2026-11-07-속성슬라임얼굴/STATIC-CHECKS.json)와 [전체 연결 값·최종 프롬프트](검증자료/2026-11-07-속성슬라임얼굴/LAYER-SETUP.json)를 보존한다.

Unity 임포트·Prefab 열기·Play Mode·MCP·실기기 검증은 수행하지 않았다. 실제 도약 중 얼굴 이탈·압축 후 외곽 품질·작은 화면의 가독성을 확인해야 한다. Renderer는 몸·얼굴·그림자 3개가 활성화되는 구조이며, 실제 드로 콜·성능 개선은 측정하지 않았다. 생성 목록 등록과 속성 전투 규칙은 이번에도 변경하지 않았다.
