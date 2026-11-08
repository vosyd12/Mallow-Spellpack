# 슬라임 그림자·착지 파동·얼굴의 지연 흔들림

이미지와 Face 하위 구성은 이후 [스타일 통일 작업](2026-11-07-SLIME-STYLE-UNIFICATION.md)에서 공통 아틀라스·Eyes·Smile/Moustache 구조로 변경됐다. 아래는 최초 분리 작업의 경위이며 그림자·파동·FaceMotion 경로는 유지된다.

작업일: 2026-11-07. 사용자가 코드 작성 없이 추가할 연출 세 가지를 요청했다. 기존 이동·전투·HP바 코드와 도약 높이·재생 시간은 유지하고 이미지·프리팹·Animation으로 구성했다.

## 문제와 선택 이유

- 도약은 높아졌지만 바닥 위치와 높이를 읽을 기준이 부족했다. 몸과 별개로 바닥에 남는 그림자를 두고, 공중에서는 작고 연하게, 착지에서는 넓고 진하게 보이도록 했다.
- 먼지만으로 착지 순간의 바닥 충격이 약했다. 기존 LandingDust 아래에 파동 Particle System을 붙여 기존 재생·초기화·풀 재사용 흐름을 공유했다. 별도 생성 코드나 풀을 추가하지 않았다.
- 눈·입·대형 슬라임의 콧수염이 원본 PNG에 합쳐져 있어 얼굴만 늦게 흔들 수 없었다. 이미지 생성 스킬로 몸/얼굴 아틀라스를 새로 준비하고 별도 자식으로 분리했다. 원본 PNG는 보존했다. 새 이미지는 기존 색·특징을 참고한 분리 버전이며 원본 픽셀의 단순 추출본은 아니다.

## 프리팹 구조

슬라임 3종에서 같은 경로를 사용한다.

```text
SlimeSmall / SlimeMedium / SlimeLarge
├─ VisualRoot                     기존 몸 압축·도약 Animation
│  └─ Visual                      기존 크기 설정 유지
│     ├─ Body                     얼굴 없는 몸 SpriteRenderer
│     └─ FaceMotion               지연 흔들림 Animation
│        └─ Face                  눈·입·콧수염 SpriteRenderer
├─ GroundShadow                   바닥 위치·크기 기준
│  └─ ShadowPulse                 그림자 크기·알파 Animation
└─ HealthBarMotion                기존 HP바 높이 추종

LandingDust                       기존 공용 풀의 재생 대상
└─ LandingRipple                  착지 파동 Particle System
```

기존 Visual의 스케일은 유지하고 새 Body·Face 자식의 스케일·위치로 원본 몸과 얼굴의 크기·배치를 맞췄다. 그림자는 VisualRoot 밖에 있어 함께 뛰거나 몸처럼 찌그러지지 않는다. 바닥 타일(-100) 위에서 그림자(-3), 파동(-2), 먼지(-1), 몸(0), 얼굴(1) 순서로 그린다.

## 조절 위치

| 연출 | 수정할 자산·항목 |
| --- | --- |
| 얼굴의 지연 흔들림 | `Assets/Animations/Slime/*.anim`의 `VisualRoot/Visual/FaceMotion` Position Y |
| 그림자 반응 | 같은 클립의 `GroundShadow/ShadowPulse` Scale, SpriteRenderer Color Alpha |
| 개체별 그림자 크기·바닥 위치 | 슬라임 프리팹의 `GroundShadow` Transform |
| 파동 크기·납작함 | `Assets/Prefab/Effects/LandingRipple.prefab`의 3D Start Size X/Y |
| 파동 확산·소멸 | 같은 프리팹의 Size over Lifetime, Color over Lifetime, Start Lifetime |
| 몸·얼굴 이미지 | `Assets/Sprites/Characters/SlimeSmallLayers.png`, `SlimeMediumLayers.png`, `SlimeLargeLayers.png` |
| 그림자·파동 마스크 | `Assets/Sprites/Effects/GroundFeedback.png` |
| 파동 재질 | `Assets/Materials/Effects/LandingRipple.mat` |

얼굴은 기존 상태에 맞춰 위아래로 늦게 움직이는 키프레임 연출이다. 실시간 물리 스프링은 아니다. 일반 도약·공격의 시작/끝과 착지 경계에서 얼굴 위치를 맞추고 Idle에도 복원 키를 넣었다. 기존 몸과 HP바의 곡선은 유지했다.

그림자 알파는 일반 도약 정점에서 0.14, 공격 정점에서 0.10으로 낮아지고, 일반/공격 착지에서 0.38/0.42로 높아진다. 기본 크기에 대한 가로 배율은 일반 정점 0.55, 공격 정점 0.46, 첫 착지 1.25/1.40이다.

## 파동 재생과 풀 반환

파동은 반복 없이 1개를 방출한다. Start Size X/Y는 0.9/0.315로 납작하게 만들고 회전은 0으로 고정했다. 크기 곡선은 0.3 → 1 → 1.8, 수명은 0.22초다. 별도 이동·중력·Shape 방출은 끈 상태다. 파동용 스크립트나 Animator는 없다.

기존 `LandingDust.Play()`의 `ParticleSystem.Play()`가 자식까지 재생하며 `Clear(true)`가 함께 초기화한다. Unity의 [Play 공식 참조 소스](https://github.com/Unity-Technologies/UnityCsReference/blob/master/Modules/ParticleSystem/ScriptBindings/ParticleSystem.bindings.cs)와 [Clear 문서](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/ParticleSystem.Clear.html)를 확인했다. 자식에는 풀 반환 스크립트를 붙이지 않아 반환 주체는 기존 먼지 하나로 유지한다. 파동 수명 0.22초는 기존 먼지의 최소 입자 수명 0.26초보다 짧게 설정했다.

슬라임당 SpriteRenderer는 기존 1개에서 몸·얼굴·그림자 3개가 된다. 먼지 풀의 각 인스턴스에는 파동 Particle System 1개가 추가된다. 새 아틀라스는 일반 텍스처 압축을 설정했으며 GPU 메모리·실기기 프레임·GC 개선 수치는 측정하지 않았다.

## 확인 결과와 남은 확인

이미지 생성 결과의 몸/얼굴 분리와 콧수염을 확인했고, 파일 기준으로 3종의 참조·부모 자식 관계, 아틀라스 Sprite ID, 9개 클립의 실행/편집 곡선과 바인딩, 기존 몸/HP바 곡선 유지, 파동의 수명·비반복 설정을 확인했다. C#·씬·Controller·원본 PNG는 작업 전후 파일 해시가 동일하다.

MCP·Notion·Play Mode·새 성능 측정은 사용하지 않았다. 실제 Unity 임포트·재생 화면에서 그림자 위치, 얼굴 겹침/흔들림 강도, 파동의 동시 재생과 풀 반복 재사용은 사용자가 확인해야 한다. 위 효과는 자산에 구현한 예상 동작이며 실행 성공이나 성능 개선의 실측 결과로 해석하지 않는다.
