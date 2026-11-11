# 플레이어·슬라임 전체 파츠의 공통 중심 림 틴트

작성일: 2026-11-11. 사용자가 작성한 HitReaction·HitEffectVisual의 인터페이스에 맞춰 Shader Graph·Material·Prefab·씬·효과용 Animator·클립을 연결했다. C# 소스를 수정하지 않았고 MCP·Notion을 사용하지 않았다.

## 문제·원인·선택 이유

기존 피격 효과는 몸통 Renderer 하나에 일정한 붉은색을 전달했다. 사용자 코드가 효과용 Animator 재생과 전체 파츠의 MaterialPropertyBlock 전달로 분리됐지만 새 셰이더 프로퍼티·공통 재질·필수 참조는 연결되지 않은 상태였다.

캐릭터마다 하나의 HitArea를 두고 모든 파츠가 같은 좌표를 사용하도록 했다. 중심은 약하고 외곽은 강한 그라데이션이며, 실제 합성 실루엣 경계까지의 거리를 계산하는 방식과는 다르다. 파츠별 UV 경계는 강조하지 않는다.

## 적용한 에셋

- `Assets/Shaders/Characters/SlimeHitFlash.shadergraph`: 기존 Sprite Lit 경로를 유지하고 Absolute World Position을 `_HitWorldToArea` 행렬로 변환한다. 위치의 W를 1로 만들어 이동을 포함하고, 영역 좌표 XY를 2배 정규화해 중심으로부터의 거리를 구한다.
- `_HitRimWidth`로 외곽에서 안쪽으로 퍼지는 폭을 조절하고 `_HitCenterStrength`로 중심의 최소 색상 강도를 정한다. 혼합 강도는 `HitAmount × HitColor.a × lerp(CenterStrength, 1, smoothstep(1 - RimWidth, 1, distance))`이며 0~1로 제한한다.
- 최종 Alpha는 기존 텍스처 Alpha × Vertex Color Alpha 경로를 그대로 사용한다. 피격색은 RGB 혼합에만 사용해 투명 여백과 사망 페이드를 유지한다.
- `Assets/Materials/Characters/SlimeHitFlash.mat`: 기존 GUID를 보존한 공통 재질이다. 파일명은 기존 이름을 유지하지만 플레이어와 슬라임 모두 사용한다. 평상시 HitAmount는 0, 림 폭은 0.6, 중심 강도는 0.12다.
- `Assets/Animations/Effects/HitEffect.controller`: Base Layer의 Idle·Hit 상태와 Hit→Idle 자동 복귀를 구성했다. Exit Time 1, 전환 시간 0이며 기존 캐릭터 모션 Animator와 독립적이다.
- `HitEffectIdle.anim`: HitEffectVisual._hitAmount를 0으로 유지하는 0.1초 반복 클립이다.
- `HitEffectHit.anim`: 0초 강도 0 → 0.03초 강도 1 → 0.20초 강도 0의 비반복 클립이다. 색상·림 설정은 Prefab 기본값을 사용하며 아티스트가 추가 키를 작성할 수 있다.

## 캐릭터 연결

- `PlayerVisual.prefab` 아래 HitEffect와 HitArea를 추가하고 Animator·HitEffectVisual을 부착했다. Visual Root는 플레이어 외형 루트, Hit Area는 해당 영역 Transform이다. 얼굴·눈 뒤 보완 Sprite·모자·머리카락·팔다리·지팡이 등 보이는 14개 Renderer에 공통 재질을 연결했다.
- SampleScene의 기존 Player/HitReaction은 Prefab 내부의 효과용 Animator·HitEffectVisual을 참조한다. 기존 모션 Animator·PlayerMotion·HitReaction 컴포넌트 ID를 유지했다.
- SlimeSmall·Medium·Large의 VisualRoot 아래 같은 구성을 추가하고 기존 HitReaction의 참조를 교체했다. 현재 보이는 Body·Eyes·Smile 또는 Moustache 3개를 포함하며, 외형 루트 아래의 비활성 Sprite도 공통 재질을 사용하도록 준비했다.
- 속성 Variant 18종은 기본 3종의 새 연결을 상속한다. 그림자와 체력바는 Visual Root 범위 밖이므로 피격색 대상에서 제외된다.
- HitArea는 기본 자세의 활성 Sprite 사각형을 합친 영역에 8% 여유를 둔 초기값이다. 자동 추적하는 Bounds 함수가 아니라 저장된 Transform이므로 아티스트가 위치·크기를 조절한다.

## 아티스트 조절 방법

1. 플레이어는 `Player/Visual/HitEffect`, 슬라임은 `VisualRoot/HitEffect`를 선택한다.
2. HitEffectVisual에서 Hit Color·Rim Width·Center Strength를 조절한다. Hit Amount는 0이 평상시, 1이 최대 피격색이다.
3. HitEffect를 선택하면 HitArea의 사각형 Gizmo가 표시된다. HitArea의 Position으로 중심을 옮기고 Scale X·Y로 전체 너비·높이를 조절한다. Scale을 0으로 만들지 않는다.
4. Animation 창에서 HitEffectHit를 선택해 Hit Amount의 상승·감쇠 곡선을 조절한다. 마지막 키는 0으로 유지한다. 필요하면 색상·림 폭·중심 강도에도 키를 추가한다.
5. 공통 클립 편집은 모든 캐릭터에 반영된다. 캐릭터별 차이는 우선 각 Prefab의 HitArea·색상·림 설정에서 조절한다.

## 확인 결과·남은 문제

[검증 기록](검증자료/2026-11-11-CHARACTER-RIM-TINT/Verification.json)을 남겼다.

- Shader Graph JSON 객체 133개·노드 33개·연결 31개의 ID·슬롯·입력 중복을 검사했다. 행렬 프로퍼티는 설치된 Shader Graph의 Matrix4ShaderProperty 형식과 UnityPerMaterial 선언 규칙을 따랐다.
- 그래프를 수치 평가한 90개 사례에서 중심·외곽 혼합, 이동·크기 변환, 피격 강도·색상 알파와 원본 투명도를 확인했다. 텍스처 알파 0과 사망 페이드의 Vertex Color 알파도 유지된다. 이는 오프라인 계산이며 실제 GPU 렌더 결과는 아니다.
- 효과용 Animator·클립·C# 필드·플레이어 씬 참조, 기본 3종·Variant 18종 상속과 로컬 객체 ID를 확인했다. 기존 캐릭터 Animator와 Sprite의 재질 외 설정은 유지된다.
- 기존 C#·기존 애니메이션·Controller의 SHA-256은 작업 전후 일치했다. 피격색 전달과 타이밍을 중복 제어하는 새 C#은 추가하지 않았다.
- Unity 네이티브 임포트·컴파일·Play·Animation 창 미리보기·실제 피격·성능 측정은 이번 작업에서 수행하지 않았다. 실제 화면에서 마젠타·누락 파츠·연속 피격·비활성화 복원·공격 중 피격색·사망 페이드를 확인해야 한다.

예상 결과는 플레이어와 슬라임 전체가 같은 규칙으로 붉게 반응하되 중심 디테일은 남고 외곽이 강하게 보이는 것이다. 실제 시각적 개선과 성능은 아직 확인한 결과가 아니다. 피격 모션의 공격 중 생략 규칙, 실제 실루엣 기반 마스크, Bloom, 사망 표정은 이번 변경에 포함하지 않았다.
