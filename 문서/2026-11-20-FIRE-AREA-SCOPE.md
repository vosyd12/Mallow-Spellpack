# 화염 장판 개별 구현과 후속 우선순위

## 결정

- **확정** — 기본 스킬은 개별 구현한다. 화염 장판을 기존 클래스와 슬롯 구조로 연결하고 워터볼의 기존 자동 발동과 첫 슬롯 표시를 유지한다.
- **확정** — 화염 장판 작업을 마무리하고 다음 작업으로 레벨업을 진행한다.
- **보류 · 최후순위** — 공통 스킬 등록·발동, 상태 통합, Inspector 기반 효과 조합과 스킬 트리 연동을 함께 재검토한다. 앞선 공통화 검토는 [11월 19일 기록](2026-11-19-SKILL-AUTOMATION.md)에 보관한다.
- **검토 중** — 장판 자동 표적의 최종 규칙, 중첩, 범위 이탈 후 연소, 사망 시 장판 쿨다운 초기화와 최종 수치는 미정이다.

스킬을 개별 제작한다는 결정을 적용한다. 화염 장판의 게임 내 자동 발동은 11월 19일에 정한 작업 목표로 유지한다.

## 현재 씬 기준

| 대상 | `Assets/Scenes/IngameScene.unity` 저장 상태 |
| --- | --- |
| 워터볼 | 첫 번째 슬롯. 자동 발동, 쿨다운 3초. |
| 화염 장판 | 두 번째 슬롯. 자동 발동, 현재 표적 위치에 생성. 사거리 6, 피해 반지름 1.5, 1레벨 주기 피해 5, 간격 0.5초, 수명 2초, 쿨다운 5초. |
| 장판 애니메이션 | `FireAreaVortex.anim` 2초. 여덟 갈래는 0.65초에 비활성화되고 중앙 불꽃·바닥·테두리는 약 1.47초부터 함께 옅어진다. |
| 슬롯 표시 | 편집 모드에서도 화염 전용 아이콘을 표시한다. Play Mode의 발동 후에는 워터볼과 같은 원형 덮개와 중앙 숫자를 표시한다. |

`FireAreaSkill.cs`의 수명 기본값은 3초이며, 현재 전투에서는 씬에 저장된 2초 설정이 적용된다.

## 코드 책임과 연결

| 클래스 | 역할 |
| --- | --- |
| `PlayerSkill` | `Update()`에서 워터볼과 화염 장판의 자동 설정을 각각 확인한다. 자동 검사와 슬롯 입력은 기존 장판 발동 경로를 사용하며, 장판 생성 성공 시 쿨다운을 시작한다. |
| `FireAreaSkill` | 현재 `Player.Target`과 사거리를 확인한다. 유효한 표적 위치에 프리팹을 생성하고 피해량·반지름·지속 시간·피해 간격을 넘긴다. |
| `FireArea` | 장판 하나의 범위 판정과 주기 피해를 처리한다. 한 피해 주기에서 같은 적의 여러 Collider를 중복 처리하지 않으며 만료 또는 소유자 행동 불가 시 제거한다. |
| `SkillData` | 기존 스킬 ID·레벨·피해·쿨다운 설정을 사용한다. 다음 발동 시각은 현재 위치인 `PlayerSkill`에서 관리한다. |
| `SkillSlotUI` | 기존 장판 슬롯의 호출 경로와 쿨다운 표시를 사용한다. |

## 장판 연출

- **확정 · 장판 본체** — `Assets/Prefab/FireArea.prefab`의 반투명 바닥과 지름 3유닛, 두께 약 0.08유닛의 주황색 테두리를 사용한다. 가장자리 점화, 여덟 갈래 수렴, 중앙 불꽃과 잔불을 순서대로 보여주고 테두리와 중앙 불꽃이 함께 옅어진다.
- **확정 · 에셋 재사용** — 작은 불꽃과 잔불은 기존 `LandingDust`와 `ElementLandingParticles` 머티리얼, 착지 아틀라스의 `Particle_1`을 사용한다. 큰 중앙 불꽃은 전용 8프레임 플립북을 사용한다.
- **확정 · 씬과 아이콘** — `SkillSlot02`는 링 임시 이미지 대신 `Assets/Sprites/Effects/FireAreaIcon.png`를 쓴다. 워터볼 아이콘의 외곽선·색면을 참조해 내장 imagegen으로 투명 아이콘을 만들었으며 Unity 임포트 크기는 256×256이다. 쿨다운 표시, 플레이어의 시전 컴포넌트, 장판 프리팹 참조와 비활성 미리보기를 유지한다.
- **확정 · 피격 효과 에셋** — `Assets/Prefab/Effects/FireAreaHit.prefab`은 적 발밑에서 재생할 짧은 불꽃과 자동 정리를 구성했다. 현재 장판의 주기 피해는 공통 피격 반응을 사용한다. 전용 프리팹의 재생은 피해 처리 코드에 연결하는 후속 작업이다.
- **확정 · 불꽃 소용돌이** — 저장소 루트의 `귀여운 불꽃 소용돌이 애니메이션.gif`를 참고해 기본 점화·유지 연출을 아래 소용돌이 연출로 갱신했다.

### 아이콘 제작 프롬프트

```text
Use case: stylized-concept. Make one transparent square 2D Unity mobile game skill icon for Fire Area, a burning patch on the floor. Image 1 is style reference for the neighboring water spell icon, for its strong colored outline, rounded silhouette, simple cel-shaded color masses and bright highlight. Draw an original warm amber-orange circular ground ring seen as a shallow ellipse, red-orange interior and three merged chunky curling flame tongues rising distinctly above the ground ring. Biggest tongue near middle, creamy yellow inner flame, a deep reddish-brown consistent outline, softly irregular hand-drawn contours. The ground ring is visibly part of the icon and indicates the area. A single compact graphic centered at about 75–80% of the canvas, generous transparent padding, readable at 48 pixels. True transparent RGBA background. No slot frame, backdrop, lettering, watermark, logo, gradients outside the silhouette, extraneous sparks, glow halo, photoreal fire, or fine details.
```

## 2초 조정 전 실행 검증

- Unity `6000.5.10f1`에서 `Assets/Scenes/IngameScene.unity`를 저장 후 다시 열어 플레이어 → 시전 컴포넌트 → 프리팹 참조, 두 번째 슬롯, 테두리 지름 3유닛과 미리보기 비활성 상태를 확인했다.
- 실제 전투 중 자동 발동과 워터볼 쿨다운을 관찰했다. 생성 순간부터 관찰한 장판 두 개는 각각 3.02초와 3.03초 후 제거됐고, 두 번째 슬롯에서 5초 쿨다운과 원형 표시를 확인했다.
- 통제된 Play Mode에서 워터볼 자동 모드를 끈 상태로 장판이 현재 표적 위치에 자동 생성됐다. 별도로 배치한 범위 내 적의 HP는 50에서 45, 40으로 줄었고 범위 밖 적은 50을 유지했다. 표적이 없을 때는 쿨다운이 시작되지 않았다.
- 저장 후 재실행한 Game View에서 주황색 테두리와 바닥, 불씨를 확인했다. 기존 기본 공격 투사체, 워터볼 쿨다운, 장판 슬롯 표시가 같은 전투에서 동작했고 Unity Console 오류는 0건이었다.
- 연출 보강 후 `IngameScene`의 Unity Play Mode에서 0.15초 점화, 0.90초 유지, 2.73초 잔불을 확인했다. 장판은 3.02초에 제거됐다. 두 번째 슬롯의 전용 아이콘과 쿨다운, 기본 공격과 워터볼을 함께 확인했으며 최종 재생 Console 오류는 0건이었다.
- 피격 프리팹은 테스트 중 독립적으로 생성해 불꽃 네 개 재생과 약 0.55초 뒤 자동 제거를 확인했다. 장판 피해 이벤트 연결과 모바일 실기기 검증은 남아 있다.

## 두 번째 슬롯 표시 보정

- 아이콘과 쿨다운 표시가 보이지 않는다는 보고를 받고 현재 `IngameScene`의 Play Mode를 조사했다. 아이콘 임포트와 참조, 슬롯 활성, 쿨다운 감소와 숫자 활성은 정상으로 확인했다. 전체가 보이지 않는 상태는 2160×2160 Game View에서 재현되지 않았다.
- 쿨다운 시작 직후 어두운 덮개와 숫자가 중앙 불꽃을 가리는 것을 화면에서 확인했다. `SkillSlot02`의 아이콘 영역을 슬롯 폭의 76%에서 89%로 확대했다. 쿨다운은 기존 테두리 스프라이트를 사용한 방사형 링과 우하단 숫자로 표시하도록 바꿨다. 두 번째 슬롯의 씬 인스턴스에만 적용했다.
- 씬을 저장한 뒤 Play Mode에서 대기 중 아이콘과, 쿨다운 시작 및 종료 직전의 아이콘·숫자·링을 캡처했다. 링 채움은 약 96%에서 22%로 줄었고, 워터볼 슬롯의 기존 덮개도 확인했다.
- 후속 수정에서 링 가까이에 배치한 숫자의 가독성과 두 슬롯의 표시 차이를 확인했다. 화염 장판도 워터볼과 같은 원형 덮개, 채움 시작 위치와 방향, 중앙 숫자를 사용하도록 변경했다. 아이콘 크기는 유지하고 화염 장판 덮개의 불투명도를 38%로 낮췄다.
- 수정한 씬의 Play Mode에서 대기 중 아이콘과 쿨다운 중 중앙 숫자 `5`, `1`을 확인했다. 원형 덮개 채움은 약 96%에서 18%로 줄었고, 테스트한 장판 쿨다운은 5초였다.

## 불꽃 소용돌이 최종 연출

- **확정** — 사용자 결정대로 8방향에서 불씨가 감겨 들어오고 중앙 주변에는 작은 불꽃 여섯 개가 서로 다른 시간에 솟는다. 중앙에는 표정이 있는 큰 불꽃을 두고, 마지막에 안쪽 빛 고리와 잔불이 사그라든다.
- `Assets/Animations/Effects/FireAreaVortex.anim`과 같은 이름의 Animator Controller를 `Assets/Prefab/FireArea.prefab` 루트에 연결했다. 60Hz 위치 키로 약 205~221° 회전하며 반지름 1.39에서 0.045유닛까지 수렴한다. 궤적은 실시간 꼬리의 각진 부분을 줄이기 위해 곡선마다 32개 점을 둔 Line Renderer와 키프레임 방향·크기로 만든다. `Assets/Materials/Effects/FireAreaVortexTrail.mat`은 기존 워터볼의 무채색 마스크를 끝 색상과 폭이 줄어드는 주황색 곡선에 사용한다.
- 중앙 불꽃은 내장 imagegen으로 만든 `Assets/Sprites/Effects/FireAreaVortexFlipbook.png`의 투명 스프라이트 8개를 `SpriteRenderer` 키로 교체한다. 현재 클립은 2초이며 반복은 꺼져 있다. 최신 타이밍은 아래 2초 조정을 따른다.
- Particle System은 생성 시 점화 불씨 24개, 중앙 작은 불꽃 6회, 마지막 잔불 18개를 방출한다. 옛 수치가 남아 있던 씬 미리보기 인스턴스는 비활성화했고 실제 자동 발동은 갱신한 프리팹을 사용한다. 장판 범위, 피해 판정과 쿨다운 C#은 유지했다.
- Unity `6000.5.10f1`의 `IngameScene`에서 초기 소용돌이 구현의 자동 발동을 확인했다. 0.45초에 곡선 소용돌이, 0.72초에 중앙 수렴, 1.00초와 1.31초에 다른 중앙 불꽃 프레임, 2.67초에 소멸 고리를 화면에서 확인했다. 같은 실행에서 기본 공격·워터볼·두 번째 슬롯 쿨다운 표시가 동작했다. 모바일 실기기 성능과 가독성은 남아 있다.

### 소멸 타이밍 보정

- 중앙 불꽃이 약 1.77초에 꺼지고 바닥과 테두리가 장판 만료 시까지 남는 문제가 있었다. 피해가 진행되는 동안 범위를 보이게 하려고 3초 수명과 5초 쿨다운을 유지하고, 중앙 플립북의 뒷부분을 늘렸다.
- 중앙 불꽃·바닥·테두리에 2.21~2.98초 공통 알파 보간을 적용했다. 중앙 불꽃은 크기도 줄어든다. 안쪽 고리와 빛은 같은 시점에 투명해지고 잔불은 입자 수명에 따라 먼저 약해진다. 장판 수명·피해 C#은 유지했다.
- `IngameScene` Play Mode에서 중앙 불꽃·테두리·바닥의 알파가 2.30초에 약 `0.95 / 0.88 / 0.38`, 2.55초에 `0.57 / 0.53 / 0.23`, 2.81초에 `0.13 / 0.12 / 0.05`로 줄었다. 저장된 다섯 투명도 곡선은 2.98초에 모두 0이며 최종 재생 Console 오류는 0건이었다.

### 2초 재생과 수렴 오브젝트 정리

- **확정** — 기존 2.98초 연출을 2초로 압축한다. 소용돌이의 경로와 색, 중앙 불꽃 프레임, 시간차 방출 횟수, 보간 형태를 유지하고 키프레임과 파티클 시간을 함께 줄였다.
- 꼬리가 작아진 뒤 중앙에 겹쳐 남는 것을 방지하기 위해 `Vortex/Arm1`~`Arm8`을 0.65초에 비활성화하는 키를 추가했다. 연출을 시작할 때는 여덟 갈래를 활성화한다. 중앙 불꽃·테두리·바닥·고리는 약 1.47초부터 함께 옅어지고 1.98초까지 사라진다.
- `IngameScene`에서 화면과 피해 범위의 종료 시점을 맞추려고 장판의 시전 설정 수명을 3초에서 2초로 바꿨다. 0.5초 피해 간격과 5초 쿨다운은 동일하다. 수명이 줄어 시전당 피해 횟수도 감소한다.
- Unity Play Mode에서 초반에 8갈래가 렌더링되고 수렴 뒤에는 0개가 되며, 다음 자동 발동에서 다시 8개가 켜지는 것을 확인했다. 1.72초에 중앙 불꽃과 테두리가 함께 옅어졌고, 1.97초에는 두 요소와 잔불이 화면에서 사라졌다. 별도 피해 시험에서 장판이 약 2.01초에 제거됐고, 범위 안의 적은 5점씩 네 번 피해를 받았다. 기본 공격·워터볼·5초 쿨다운을 확인했고 Console 오류는 0건이었다.

### 중앙 불꽃 생성 프롬프트

```text
Use case: stylized-concept. Production transparent game VFX sprite sheet for Unity, 4 columns by 2 rows, exactly eight equal tiles with generous gutters, square canvas. All tiles show ONLY a single small adorable flame character over transparent alpha, no ground or shadow. Simple hand-painted chibi shape, rich vermilion outside, bright golden yellow center, warm ivory highlights, dark rust outline, expressive two tiny eyes and tiny smile in the lower center. All frames share a fixed center baseline, similar width, third-person 2D game icon style, good at small size. Chronological animation order left to right, top row then bottom row. Tile 1 small ignition, 2 flame rises, 3 flame full tall with smiling face, 4 bends gently left, 5 bends gently right, 6 rounded flame flickers, 7 shrinks, 8 tiny dying flame. Maintain cell alignment. Fit every visible mark inside central 68 percent of its tile; do not let edges touch another tile. Canvas truly transparent RGBA outside the eight silhouettes. No grid lines, text, labels, example UI, shadows, floor rings, scenery, separate sparks, background color or character accessories.
```

## 다음 작업 · 레벨업

- **확정 · 현재 연결** — `Enemy`의 처치 경로에서 `Player.GainExperience()`로 경험치를 지급한다. `PlayerInfo.SetInfo()`는 레벨, 현재/필요 경험치, 게이지와 버튼 클릭 가능 상태를 갱신한다. `IngameScene`의 레벨업 버튼은 `Player.TryLevelUp()`에 연결돼 있다. 첫 기준은 경험치 100이며, 클릭 시 필요 경험치를 차감하고 한 단계 올린 뒤 다음 기준을 1.1배로 갱신해 정수로 저장한다. [기존 레벨업 범위](2026-11-12-RESTART-REVIEW-AND-NEXT-SCOPE.md)
- **확정 · 작업 순서** — 다음 작업은 레벨업이다. 경험치 차감과 HUD 갱신을 먼저 확인한다. 이전에 요청한 레벨업 가능 알림·클릭 반응과 캐릭터 주위에서 솟는 짧은 빛 기둥을 연출 작업의 출발점으로 삼는다. [이전 요청과 구현 상태](2026-11-18-IMPROVEMENT-BACKLOG.md)
- **검토 중** — 레벨업 성공 시 VFX를 부르는 연결과 카드가 등장하는 시점, 카드의 선택·보상 규칙. 현재는 성공 이벤트, 전용 VFX 프리팹·클립, 카드 선택과 보상 적용을 파일에서 확인하지 못했다.
- **보류** — 스킬 제작 자동화·스킬 트리는 최후순위로 유지한다. 화염 장판의 전용 피격 불꽃 연결과 모바일 가독성 확인은 후속 항목에 남겨 둔다.

## 다음 작업 인수인계

- **결정** — 화염 장판의 현재 연출과 설정을 마무리하고 레벨업을 다음에 진행한다. 스킬 제작 자동화와 스킬 트리는 최후순위로 둔다.
- **적용** — 2초 클립과 씬의 장판 수명을 맞췄다. 8갈래 소용돌이는 수렴 뒤 비활성화되고, 중앙 플립북과 바닥은 함께 옅어진다. 슬롯의 원형 쿨다운과 전용 아이콘을 사용한다. 피격 불꽃 프리팹을 준비했다.
- **검증** — 기존 Play Mode에서 2초 클립, 8갈래 비활성화 및 재활성화, 공통 페이드, 피해 네 번과 약 2.01초에 제거되는 것을 확인했다. 이번 문서화는 현재 씬·프리팹·클립 및 레벨업 코드와 버튼 참조를 대조했다. 레벨업 Play Mode 확인은 다음 작업이다.
- **다음 작업** — `IngameScene`에서 레벨업 버튼의 경험치 차감과 HUD 갱신을 재확인한 뒤, 성공 후 빛 기둥 연출을 연결할 지점을 정한다. 카드 등장 시점과 보상 규칙은 사용자 판단을 기다린다. 장판 전용 피격 효과 연결과 모바일 검증은 별도로 추적한다.
- **공유 기준** — 이번 업로드의 기준은 원격 `main`의 `4eb993e`다. 씬·장판 및 피격 프리팹·아이콘·소용돌이 애니메이션과 머티리얼·플립북·참고 GIF·문서 변경은 이 기록을 포함하는 커밋으로 공유한다.
