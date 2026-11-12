# 사망 비행 중 점진적인 투명도 감소

작성일: 2026-11-11. 사용자가 사망 애니메이션 재생을 확인한 뒤, 비행 중 점점 흐려지는 표현을 요청했다. 이번에는 페이드만 적용하고 사망 전용 표정 제작은 다음 작업으로 남겼다.

## 문제·원인·선택 이유

기존 Death 클립은 회전·이동만 제어해 비행이 끝날 때까지 불투명했다. 새 런타임 코드나 Material 복제 없이 기존 클립의 SpriteRenderer `m_Color.a`를 애니메이션으로 제어했다. 몸과 얼굴 일부가 남지 않도록 현재 외형의 활성 Renderer 전체에 같은 곡선을 연결했다.

## 달라진 점과 조절 위치

| 클립 | 페이드 대상 |
| --- | --- |
| Assets/Animations/Player/PlayerDeath.anim | 몸·팔다리·머리·얼굴·머리카락·모자·지팡이와 얼굴 보완용 Sprite 총 14개 |
| Assets/Animations/Slime/SlimeDeathSmall.anim | Body·Eyes·Smile 3개 |
| Assets/Animations/Slime/SlimeDeathMedium.anim | Body·Eyes·Smile 3개 |
| Assets/Animations/Slime/SlimeDeathLarge.anim | Body·Eyes·Moustache 3개 |

- 길이는 기존 0.90초를 유지했다. 시작~0.18초는 원래 알파를 유지하고 0.90초에 알파 0이 된다. 양끝 기울기를 0으로 둬 부드럽게 감소한다.
- 원래 알파 대비 0.36초 84.375%, 0.54초 50%, 0.72초 15.625%다.
- Animation 창의 Death 클립에서 각 파츠의 `Sprite Renderer > Color.a` 곡선을 조절한다. 시작 지연은 0.18초 키, 사라지는 시각은 마지막 0.90초 키다. 파츠별 값을 함께 조절해야 한다.
- 슬라임 속성 Variant 18종은 기존 크기별 Death 클립을 그대로 상속한다. 그림자는 기존 사망 처리에서 숨긴다.
- 변경한 게임 에셋은 Death 클립 4개뿐이다. C#·Prefab·씬·Controller·Material·Sprite·다른 행동 클립·기존 회전과 이동 키는 수정하지 않았다.

## 확인 결과와 남은 문제

[검증 수치·바인딩 경로·해시](검증자료/2026-11-11-DEATH-FADE/Verification.json)를 남겼다.

- 실행용 FloatCurves와 편집용 EditorCurves에 같은 알파 곡선을 기록하고, 기존 Unity 알파 곡선의 형식과 CRC32 바인딩을 대조했다.
- 파츠 경로와 Renderer 종류, 기본 3종 Controller·Death 참조, Variant 18종의 상속을 파일로 확인했다.
- 추가한 알파 데이터만 제거하면 기존 클립과 완전히 일치함을 확인했다. 길이·회전·이동·이벤트·기존 곡선은 유지한다.
- 파츠별 곡선을 181시점에서 계산해 알파가 원래 값부터 0까지 범위를 벗어나거나 다시 증가하지 않는지 확인했다. 이는 오프라인 곡선 계산이며 Unity 게임 재생 결과는 아니다.
- 게임 C#·씬·Prefab·Controller·다른 클립과 .meta의 SHA-256은 작업 전후 일치했다.
- MCP는 사용하지 않았다. 별도 Unity 6000.5.10f1 프로젝트에서 네이티브 임포트·샘플링 검증을 시도했지만 LicensingClient 채널 연결 거부로 프로젝트 임포트 전에 멈춰 해당 보조 프로세스를 종료했다. Unity 임포트 성공·실제 게임 화면 확인으로 기록하지 않는다.

예상 결과는 사망 비행 후반에 외형 전체가 부드럽게 사라지는 것이다. 실제 게임에서 파츠가 함께 흐려지는지, 피격색과 겹쳤을 때의 외형, 다양한 화면 비율의 이탈을 확인해야 한다. 사망 전용 표정, 좌우 비행 선택, 궤적·회전량 변경, 게임오버·재시작은 이번 구현에 포함하지 않았다.
