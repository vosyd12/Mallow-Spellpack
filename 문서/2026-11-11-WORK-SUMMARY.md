# 플레이어·슬라임 연출 작업 현황

갱신일: 2026-11-11. 이번 대화에서 진행한 모션 연결·리깅 통합·전체 피격색·사망 페이드·표정을 정리했다. 현재 C#·Controller·클립·Prefab·씬과 상세 기록을 대조했다. 이번 문서화에서는 코드·게임 에셋을 수정하거나 MCP·Notion을 사용하지 않았다.

## 구현한 내용과 선택 이유

| 작업 | 문제와 원인 → 선택한 방법 | 현재 상태·확인 범위 |
| --- | --- | --- |
| 플레이어 모션 통합 | Controller가 둘로 나뉘고 행동 참조·이벤트가 빠져 있었다. 기존 Player Controller에 Idle·Walk·Attack·Hit·Death를 모으고 PlayerMotion을 연결했다. | 상태·씬 참조·Attack 발사/종료·Hit 종료 이벤트 연결 완료. 실제 연속 행동 재생 확인은 남았다. |
| 체력바 숨김 | 본체 비활성화가 사망 연출을 끊지 않도록 HealthBar의 UI 전용 _visualRoot를 연결했다. | 플레이어·슬라임 기본 3종과 속성 Variant 18종 상속을 파일로 확인했다. |
| 리깅 에셋 통합 | RigTest 사본에 리깅 데이터가 있었지만 실제 Sprite Skin 연결이 부족했다. 정식 PlayerPartsAtlas·PlayerVisual에 통합하고 중복 사본을 Assets에서 제거했다. | 몸통·양팔·양다리·머리의 Sprite Skin 6개·뼈 13개 연결. 기존 GUID·파츠 경로 유지. 실제 변형 확인과 뼈 기반 클립 제작은 남았다. |
| 전체 피격 림 틴트 | 몸통 하나에만 전달되던 피격색을 공통 HitArea·재질과 독립 HitEffect Animator로 분리했다. | 플레이어·슬라임의 중심은 옅고 외곽은 강한 색상으로 연결. 파일·그래프·수치 확인 완료, 실제 GPU 출력은 미확인이다. |
| 사망 비행 페이드 | 비행 끝까지 불투명하던 외형에 Death 클립의 Color.a 곡선을 추가했다. | 0~0.18초 원래 알파 유지, 0.90초 알파 0. 플레이어 14개·슬라임별 3개 파츠 대상. 사용자가 기존 Death 재생을 확인했으나 새 효과의 화면 검증과 구분한다. |
| 사망 전용 표정 | 승인된 X 눈·점 입을 새 투명 시트로 제작하고 기존 얼굴 Renderer의 Sprite를 Death에서 교체했다. | 0초부터 표정 변경, 대형 콧수염 유지. 얼굴 배율 이외의 기존 곡선·페이드·길이 동일 확인. 새 표정의 실제 화면 확인은 남았다. |

## 현재 재생 구조

- `PlayerMotion.cs`: 사망 시 Death를 재생하고 그림자를 숨긴다. Attack 중에는 Hit 동작을 생략한다. Attack·Hit가 끝나면 Animation Event로 이동/대기 상태에 복귀한다. Walk 재생 속도는 이동 속도에 연동되지 않는다.
- `SlimeMotion.cs`: 사망 이벤트에서 그림자를 숨기고 Animator 속도를 1로 복구해 Death를 재생한다. `_deathClip.length`만큼 대기한 뒤 제거하므로 해당 참조는 실제 크기별 재생 클립과 일치해야 한다.
- `HitReaction.cs`·`HitEffectVisual.cs`: 피격색은 별도 Animator로 재생하고 외형 아래 SpriteRenderer에 공통 영역 행렬과 색상 값을 전달한다. 모션 우선순위와 색상 재생은 별개다.
- 표정 교체·페이드는 기존 Death 클립에서 처리한다. 이 연결을 위해 새 런타임 C#은 추가하지 않았다. 현재 사망은 일반 모션으로 복귀하는 흐름이 아니며 향후 부활·풀링 도입 시 표정·알파 복원을 확인해야 한다.

## 수정할 위치

| 조절 대상 | 위치 |
| --- | --- |
| 플레이어 기본 모션 | `Assets/Animations/Player/Player.controller`와 같은 폴더의 행동 클립 |
| 사망 표정·비행·회전·페이드 | `PlayerDeath.anim`, `Assets/Animations/Slime/SlimeDeathSmall.anim`·`Medium`·`Large` |
| 사망 표정 원본 | `Assets/Sprites/Characters/Expressions/DeathFaceParts.png` |
| 피격 중심·범위 | 플레이어 Visual 또는 슬라임 VisualRoot 아래 `HitEffect`·`HitArea` |
| 피격 강도·타이밍 | `Assets/Animations/Effects/HitEffectHit.anim`의 Hit Amount 곡선 |
| 피격색·외곽 폭·중심 강도 | 각 HitEffectVisual의 Hit Color·Rim Width·Center Strength |
| 정식 리깅 | `Assets/Prefab/Player/PlayerVisual.prefab`·`Assets/Sprites/Characters/Player/V3/PlayerPartsAtlas.png.meta` |

## 확인 결과와 남은 작업

표정 추가 시 4개 Death 클립을 원본과 비교해 얼굴 Sprite·배율 외의 애니메이션 데이터가 동일함을 확인했다. 페이드 해시도 일치한다. 이는 파일 검증이며 Unity 임포트·컴파일·게임 실행 성공을 의미하지 않는다. 최신 림 틴트·리깅·표정을 함께 적용한 최종 화면은 아직 확인하지 않았다. 시각적 개선·성능 향상도 실제 측정 결과로 기록하지 않는다.

1. 실제 게임에서 Walk·Attack·연속 피격·Death를 확인하고, 사망 표정의 위치·크기와 전체 파츠의 동시 페이드를 확인한다.
2. Hat·얼굴·Staff의 머리/손 뼈 추종과 FirePos를 정리하고, 새 뼈에 키를 작성해 움직임을 보완한다. 현재 기존 클립은 주로 파츠 Transform 방식이다.
3. 좌우 사망 비행 선택, 사망 순간 자세 유지, 화면 비율별 화면 밖 판정은 별도 후속 작업이다. 이번 표정 연결에서 추가하지 않았다.
4. Walk 속도 연동·Attack 강약 보완·Bloom·슬라임 피격 표정·게임오버/재시작은 남아 있다.

## 상세 기록

- [모션 통합·이벤트](2026-11-11-PLAYER-MOTION-WIRING.md), [체력바 연결](2026-11-11-HEALTHBAR-HIDE-WIRING.md)
- [리깅 통합·중복 정리](2026-11-11-PLAYER-RIG-PROMOTION.md), [림 틴트·아티스트 조절](2026-11-11-CHARACTER-RIM-TINT.md)
- [사망 페이드](2026-11-11-DEATH-FADE.md), [사망 표정·검증 자료](2026-11-11-DEATH-EXPRESSION.md)

위 문서는 각 작업 당시의 범위를 기록한다. 이전 기록의 ‘미구현’ 설명은 이후 적용 사항에 대해서는 이 요약과 최신 마일스톤으로 대체한다.
