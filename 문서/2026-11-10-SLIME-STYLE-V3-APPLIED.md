# 슬라임 스타일 교체와 Death 단독 클립

작성일: 2026-11-10. 사용자가 승인한 새 스타일로 **기본 Prefab 3종·속성 Variant 18종의 이미지 연결을 교체**하고 크기별 Death 클립을 제작했다. C#·씬·기존 Animator Controller·기존 Animation·피격 재질·물리 및 전투 설정은 변경하지 않았다. Death 상태와 사망 코드는 연결하지 않았으며 MCP·Notion은 사용하지 않았다.

후속: 사용자 코드 수정 확인 뒤 [Death 상태·크기별 Override·클립·그림자 연결](2026-11-10-ANIMATION-WIRING.md)을 완료했다. 아래 미연결·즉시 제거 설명은 최초 제작 시점의 기록이다. 실제 게임 Play 검증과 사망 순간 자세·화면 이탈 판정은 여전히 남아 있다.

## 요청·문제·선택한 방법

기존 픽셀 슬라임과 매끈한 플레이어 v3 사이의 그림체 차이를 줄이는 요청이다. [승인한 비교 시안](아트시안/2026-11-10-슬라임스타일/SlimeStyleStudy_v1.png)을 기준으로 같은 외곽선·명암의 투명 파츠 아틀라스를 새로 생성했다. 비교 시안 자체를 그대로 잘라 사용한 것은 아니다.

몸·눈·입을 분리하고 대형은 입과 콧수염을 한 파츠로 사용한다. 18개 몸과 크기별 눈/입 6개를 합쳐 24개 Sprite로 관리한다. 몸 색상이 이미 그려져 있으므로 이전 틴트를 제거해 흰색으로 맞추고 기존 공통 피격 재질은 보존했다. 작은 이미지에서 구분할 수 있게 단순한 눈·입, 홍조와 대형 콧수염을 유지하며 눈썹·머리 위 장식은 추가하지 않았다.

속성 Variant의 Eyes에는 기존 얼굴 전체 Sprite와 위치·크기 override가 있었다. 그대로 새 파츠를 끼우면 중복 표시·배치 불일치가 생기므로 해당 외형 override만 정리하고 Eyes·Smile/Moustache를 각각 연결했다. 스크립트·컴포넌트 번호·Prefab GUID·상속 원본·생성 목록은 유지했다.

## 적용 자산과 보존 범위

- 이미지: `Assets/Sprites/Characters/Slimes/StyleV3/SlimeStyleV3Parts.png`. 1774×887 RGBA이며 원본 생성 PNG의 픽셀은 편집하지 않았다. Unity Sprite rect로 파츠를 분리했다. 초안의 대형 몸통이 붙어 있던 문제는 내장 이미지 생성 도구로 간격을 보정한 뒤 분리했다.
- 연결: `Assets/Prefab/SlimeSmall.prefab`, `SlimeMedium.prefab`, `SlimeLarge.prefab`와 `Assets/Prefab/Slimes/`의 18개 Variant. 기존 파일을 유지하며 렌더러 참조·표시 여부·파츠 위치·크기만 조정했다.
- 각 조합의 기존 몸 너비·발 기준을 계산해 보존하고 새 몸의 비율에 맞춰 높이와 얼굴 위치를 정했다. 몸의 새 실루엣과 실제 게임의 충돌 영역이 완전히 일치한다고 주장하지 않는다. Collider·공격 범위·HP·이동·보상은 유지했다.
- 기존 BodyMotion·FaceMotion·VisualRoot와 도약/얼굴 지연 애니메이션 경로를 유지했다. HP바·그림자·속성 착지 효과·피격 참조를 새 기능으로 다시 만들지 않았다. 새 이미지가 기존 강한 압축·늘어남을 그대로 따르는 모습은 아래 미리보기로 검토한다.
- 기존 PNG는 삭제하지 않았다. 새 아틀라스는 bilinear 필터와 압축하지 않은 텍스처 설정을 사용한다. 실제 기기 메모리·FPS 개선은 측정하지 않았다.

## 소·중·대 Death 클립

| 파일 | 제작값 | 연결 상태 |
| --- | --- | --- |
| [SlimeDeathSmall.anim](../Assets/Animations/Slime/SlimeDeathSmall.anim) | 0.90초·1080도(3회전)·포물선 이탈 | 단독 클립, 사망 미연결 |
| [SlimeDeathMedium.anim](../Assets/Animations/Slime/SlimeDeathMedium.anim) | 같은 방향, 중형 회전 중심 보정 | 단독 클립, 사망 미연결 |
| [SlimeDeathLarge.anim](../Assets/Animations/Slime/SlimeDeathLarge.anim) | 같은 방향, 대형 회전 중심 보정 | 단독 클립, 사망 미연결 |

[플레이어 Death](2026-11-10-PLAYER-ACTION-CLIPS.md#후속-death-수정과-attack-검토)와 같은 한 덩어리 회전·포물선 방향이다. 쿠키런 크럼블의 공개 적 사망 영상에서 참고한 방향과 프로젝트 조정값은 플레이어 기록에서 구분한다. 새 사망 표정·흩어지는 파츠는 추가하지 않았다.

루트의 가로·세로 배율이 달라 단순히 회전만 넣으면 몸이 회전 중 찌그러질 수 있다. Death 안에서 VisualRoot의 배율을 보정하고 ReadabilityScale에서 회전하도록 구성했다. 시작 외형은 유지하고 그림자·HP바는 비행 그룹에서 제외했다. 기본 자세를 고정한 클립이며 실제 사망 순간 자세 포착·그림자/HP바 제거·화면 밖 판정·전투 정리는 미연결이다.

## 확인 결과와 남은 범위

- 21개 Prefab의 기존 내부 번호와 기본 컴포넌트 구성을 대조하고, 행동·물리·Animator 데이터와 속성/크기 분류 override가 그대로임을 확인했다. 외부 생성 참조가 사용하던 GUID와 컴포넌트 번호를 새 번호로 교체하지 않았다.
- 기존 C#·씬·기존 Animation·Shader·Material 파일 해시가 일치했다. 새 Death 클립은 추가 파일이다. Controller의 Death 상태나 재생 호출은 추가하지 않았다.
- 새 참조의 Sprite ID·눈/입 활성·몸 색상·크기별 구성을 파일로 확인하고, 실제 저장된 Prefab과 곡선으로 18종 외형·기존 Jump·Death를 오프라인 합성했다. [미리보기](슬라임StyleV3/Preview.html) · [18종](슬라임StyleV3/AppliedLineup.png) · [Jump](슬라임StyleV3/Bounce.gif) · [Death](슬라임StyleV3/Death.gif)
- Death 시작 변환 오차는 약 0.0000000000000002 이하다. 18조합·조합당 31시점에서 회전 중 선형 변환의 길이/각도 보존도 수치로 대조했다. [수치 기록](슬라임StyleV3/DeathVerification.json)
- Unity 6000.5.10f1 별도 임시 프로젝트에서 PNG TextureImporter와 Death 3종 NativeFormatImporter의 정상 임포트·종료 코드 0을 확인했다. 라이선스 토큰 갱신·종료 시 Curl 메시지는 [원본 로그](슬라임StyleV3/UnityImport.txt)에 남겼으며 자산 임포트 결과와 구분한다.

이번 GIF는 Unity 게임 녹화나 Unity 포즈 샘플링이 아니다. 원본 프로젝트의 Play Mode·21개 Prefab의 네이티브 재생·실제 조명/피격 재질·군집·모바일 검증은 수행하지 않았다. 사용자 확인 후 새 외형의 얼굴 배치·압축 정도·게임 화면 가독성을 조정할 수 있다. 사망 코드는 여전히 기존처럼 HP 0에서 즉시 제거하는 상태다.

[자산 대조 기록](슬라임StyleV3/AssetVerification.json). 이미지 교체 완료와 사망 기능 완료를 구분한다.
