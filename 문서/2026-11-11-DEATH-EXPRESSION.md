# 사망 표정 연결

## 변경과 선택 이유

플레이어·슬라임의 사망 전용 표정이 없어 승인된 X 눈·점 입 시안을 Death 클립에 연결했다. 기존 사망 처리와 페이드를 유지하도록 C#이나 Prefab 대신 기존 얼굴 SpriteRenderer의 Sprite만 교체한다. 새 이미지 크기를 맞추기 위한 얼굴 파츠의 고정 배율도 클립에 반영했다.

- 이미지: `Assets/Sprites/Characters/Expressions/DeathFaceParts.png`. image_gen으로 제작한 투명 PNG이며 눈·입 8개 Sprite로 분할했다. [생성 프롬프트](검증자료/2026-11-11-DEATH-EXPRESSION/ImagePrompt.txt)
- 대상: `PlayerDeath.anim`, `SlimeDeathSmall.anim`, `SlimeDeathMedium.anim`, `SlimeDeathLarge.anim`.
- Death 0초부터 X 눈·점 입으로 교체하고 0.9초까지 유지한다. 대형 슬라임은 콧수염을 유지한다.
- 플레이어 입에는 피부색 바탕을 넣어 기존 머리 이미지의 입 구멍이 드러나지 않도록 했다. 파일상 기존 입 영역 3,246픽셀이 모두 덮이는 것을 확인했다.

## 확인 결과와 남은 확인

변경 전 클립과 비교해 얼굴 Sprite·배율 이외의 애니메이션 데이터가 동일함을 확인했다. 4개 클립의 알파 곡선 해시도 모두 일치한다. 비행·회전·0.9초 길이·기존 이벤트와 0.18초 이후 페이드아웃은 유지된다.

기존 Prefab·일반 모션·재질·셰이더는 변경하지 않았다. 작업 시작 이후 `Assets/Scripts/Stage/Stage.cs`의 외부 변경을 감지했으며 해당 파일을 수정하거나 되돌리지 않았다. 이번 작업에서 C#은 수정하지 않았다. [파일 검증 결과](검증자료/2026-11-11-DEATH-EXPRESSION/Verification.json)

MCP와 Unity 재생 검증은 하지 않았다. 실제 화면에서 표정 크기·위치와 사망 중 페이드의 최종 확인은 남았다. 현재 사망 흐름을 기준으로 연결했으며, 향후 부활·풀링을 추가하면 일반 표정 복원도 확인해야 한다.
