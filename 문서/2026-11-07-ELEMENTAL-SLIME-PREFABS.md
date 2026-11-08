# 속성 슬라임 외형의 Prefab 사전 장착

후속 상태: [18종의 몸·얼굴 레이어 분리와 FaceMotion 연결](2026-11-07-ELEMENTAL-SLIME-FACE-MOTION.md)을 완료했다. 아래 통이미지·얼굴 흔들림 제한은 사전 장착 당시의 기록이며, 현재 Variant는 각 `Layers.png`를 사용한다.

작업일: 2026-11-07. 사용자가 18종 이미지를 승인하고, 전체 기능 연결 전에 해당 외형을 미리 장착하도록 요청했다.

## 완료한 범위와 위치

- `Assets/Sprites/Characters/Slimes/{속성}/Slime{속성}{크기}.png`: 승인한 PNG의 게임용 사본 18개와 Sprite 임포트 설정.
- `Assets/Prefab/Slimes/{속성}/Slime{속성}{크기}.prefab`: 크기별 원본을 상속하는 Prefab Variant 18개.
- 속성 폴더: `Pure`, `Fire`, `Water`, `Earth`, `Wind`, `Lightning`. 크기: `Small`, `Medium`, `Large`.
- 예: [불 Small](../Assets/Prefab/Slimes/Fire/SlimeFireSmall.prefab), [물 Medium](../Assets/Prefab/Slimes/Water/SlimeWaterMedium.prefab), [무속성 Large](../Assets/Prefab/Slimes/Pure/SlimePureLarge.prefab).

Unity Project 창에서 해당 Variant를 열면 장착된 외형을 확인할 수 있다. 기존 씬에는 자동 배치하지 않았다. 향후 생성 목록에 등록할 때는 원본 PNG가 아닌 이 Variant를 사용한다. `Enemy` 컴포넌트는 원본으로부터 상속한다.

## 예상 문제와 선택한 방법

18개 Prefab을 완전히 복사하면 이동·도약·먼지·HP바의 공통 수정이 서로 달라질 수 있다. 따라서 `SlimeSmall`, `SlimeMedium`, `SlimeLarge`를 원본으로 두고 이미지·색상·외형 배치만 Variant에서 재정의했다. 전투 수치나 컴포넌트는 재정의하지 않았다. 공통 변경은 원본에서, 속성 이미지 변경은 Variant에서 관리한다.

원본 이미지의 투명 여백과 장식 높이가 달라 같은 임포트 값만 쓰면 표시 크기·바닥 위치가 흔들린다. 알파 32 이상 영역의 가로폭을 기준으로 각 이미지의 Pixels Per Unit을 계산하고, 중심·바닥 피벗을 맞췄다. PNG 픽셀은 편집하지 않았다. Point 필터·투명 알파·Mip Map 비활성·최대 2048·기본 압축으로 설정했다. 기존 몸 색상이 새 PNG에 곱해지지 않도록 Renderer 색상은 흰색이다.

부모의 X/Y 스케일 차이를 Body 배치에서 보정해, 기본 자세에서 원본 이미지 비율을 유지한다. 도약 중 의도한 압축·늘어남은 상속한 Animation이 담당한다. Variant의 시작 위치는 원점으로 정리했다.

## 크기와 연출 차이

- Small: 새 Variant의 보이는 외형 가로폭을 기존 Small보다 20% 키웠다. 원본 Prefab·Rigidbody·충돌 크기·피격 범위는 유지했다.
- Medium·Large: 기존 외형 가로폭을 유지했다. 높이는 승인한 이미지의 비율과 장식에 따라 달라진다.
- 도약·공격 모션·그림자·먼지·HP바의 기존 연결은 그대로 상속한다. 실행 결과를 확인했다는 뜻은 아니다.
- 새 PNG에는 얼굴이 포함돼 있다. 기존 `VisualRoot/Visual/FaceMotion`을 Variant에서 비활성화해 얼굴 중첩을 막았다. 몸 전체는 움직이지만 얼굴만 따로 지연되는 흔들림은 현재 Variant에 표시되지 않는다. 원본 3종의 분리된 얼굴 연출은 유지한다.

이는 전체 연결 전 외형 준비 방식이다. 기존 얼굴 연출까지 동일하게 유지하려면 승인 이미지를 몸·얼굴·속성 장식 레이어로 나눠 연결해야 한다. 현재 완료로 표시한 것은 이미지 장착과 상속 구조이며, 레이어 분리까지 완료한 것은 아니다.

## 확인 결과와 남은 작업

파일 검사에서 18개 조합, 원본 PNG와 사본의 SHA256 일치, GUID 중복 여부, 원본 Prefab·재정의 대상의 존재, Sprite 설정과 크기 배율을 확인했다. 작업 전 존재하던 모든 Assets 파일의 해시가 동일하다. 씬·C#·Animation·생성 목록·기존 Prefab을 수정하지 않았다.

[전체 에셋·배치 값](검증자료/2026-11-07-속성슬라임프리팹/ASSET-SETUP.json)과 [정적 확인 결과](검증자료/2026-11-07-속성슬라임프리팹/STATIC-CHECKS.json)를 남겼다. Unity 임포트·Prefab 열기·Play Mode·MCP·모바일 성능 측정은 수행하지 않았다. 실제 작은 화면의 가독성·압축 품질·HP바 간격은 Unity에서 확인해야 한다. 성능 개선 수치는 측정하지 않았다.

속성별 전투 효과·상성·피해 계산, `Element` 값과 외형 선택의 연결, 생성 목록 등록은 이번 범위에 포함하지 않았다. `Enemy`에는 아직 속성 저장 필드가 없으므로 이름·폴더만으로 전투 속성이 적용된다고 해석하지 않는다. 기존 코드의 사망·생성 제한 등 미완료 사항도 그대로 남아 있다.
