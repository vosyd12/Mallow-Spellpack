# 플레이어 사망 화면 전환 에셋과 씬 연결

후속 변경: 기존 적 HP/위치 유지 방식은 폐기하고 성장 상태를 유지한 전투 재시작으로 변경했다. 사용자 작성 초기화 코드와 추가 Spawner·시작점 연결은 [최신 연결 기록](2026-11-12-STAGE-RESTART-WIRING.md)을 따른다. 아래 UI 제작·임포트 기록은 유효하며 기존 전투 보존 흐름은 당시 구현이다.

작성일: 2026-11-12. 기존 Stage·ScreenTransition·Player·PlayerMotion 코드를 사용해 전환 UI·애니메이션·참조만 구성했다. C# 19개 파일의 변경 전후 해시가 같으며 MCP·Notion은 사용하지 않았다.

## 문제와 원인

사망 후 전환·부활 순서 코드는 있었지만 ScreenTransition 컴포넌트·검은 화면 UI·Controller·클립이 없어 Stage.GameOver가 전환 참조 검사에서 반환했다. 씬의 Stage → Player와 Player → Stage 참조도 저장되지 않았다. Player.CanAct는 Stage 참조가 없으면 false이므로 전환 UI만 만드는 것으로는 충분하지 않았다.

## 선택한 방법과 이유

- `Assets/Prefab/UI/ScreenTransition.prefab`에 별도 Screen Space Overlay Canvas를 구성했다. 정렬 순서는 1000으로 기존 HUD의 200보다 높아 HUD까지 덮는다. UI 하위에 인스턴스를 두며 화면 전체 비율을 따른다.
- 자식 `BlackCurtain`은 검은 Image다. RectTransform의 세로 앵커만 애니메이션화해 화면 크기에 상관없이 덮는다. Cover는 위쪽 끝에서 아래로 확장하고, Reveal은 아래쪽에 검은 영역을 남기며 위쪽부터 드러낸다. 고정 픽셀 이동·텍스처·새 Shader는 필요하지 않다.
- `Assets/Animations/ScreenTransition/`의 Clear·Cover·Reveal과 Controller를 만들었다. Clear는 기본 상태, Cover·Reveal은 각각 0.4초·비반복·자동 전환 없음이다. 0.4초는 이번 제작의 조정값이며 확정한 암전 유지 0.5초와 다르다.
- Animator는 Unscaled Time으로 동작한다. Cover·Reveal 마지막의 `FinishTransition` 이벤트와 기존 normalizedTime 완료 검사를 함께 사용한다. 정상 전환 시간과 오류 대기 상한 10초를 구분한다.
- 투명한 전체 화면 `InputBlocker`와 CanvasGroup으로 전환 중 UI 입력을 막는다. Clear와 Reveal 종료에는 입력 차단을 해제한다. 전투 차단은 UI가 아닌 기존 CanAct·IsRestarting 코드가 담당한다.
- SampleScene의 Stage 인스턴스에 Player·ScreenTransition·암전 유지 0.5초를 연결하고 Player의 Stage 참조를 같은 인스턴스에 연결했다. Stage Prefab에는 씬 외부 참조를 저장하지 않는다.

## 변경 후 흐름

플레이어 HP 0 → 기존 사망 처리 → 다음 Stage.Update에서 Cover 0.4초 → 보스 게이지 초기화·Revive → 완전 암전 0.5초 → Reveal 0.4초 → IsRestarting 해제다. 레벨·경험치·스테이지·기존 적 HP와 위치는 기존 코드대로 유지한다. 배경 재배치나 새 보스 생성은 추가하지 않았다.

## 확인 결과와 남은 문제

에셋 파일에서 앵커 시작/끝 값, 기본 Clear 상태, 비반복 클립, 이벤트 종료 시각, Unscaled Time, Canvas 정렬과 양방향 Stage/Player 참조를 대조했다. [파일 검사 결과](검증자료/2026-11-12-사망화면전환/AssetChecks.json). 소스 검토·임포트와 원본 게임의 Play 성공은 구분한다.

별도 Unity 6000.5.10f1 프로젝트에서 새 Prefab·Controller·클립 3개와 변경하지 않은 ScreenTransition.cs의 임포트·컴파일 후 정상 종료를 확인했다. 첫 시도에서는 임시 프로젝트의 Animation 모듈 누락으로 Animator 타입 컴파일 오류가 발생했고, 임시 manifest에 모듈을 추가한 재시도에서 해소됐다. 원본 프로젝트의 패키지·C#은 변경하지 않았다. [임포트 결과 발췌](검증자료/2026-11-12-사망화면전환/UnityImport.txt). 원본 씬의 Play·실제 화면·연속 사망은 검증하지 않았다.

전환 중 PlayerMotion 일반 행동·SlimeMotion 타이머/Animator의 완전 정지는 아직 코드 보완 대상이다. 기존 탄환·적 사망 코루틴도 전체 정지 대상으로 바꾸지 않았다. 전환 실패 시 기존 코드는 정지 상태를 유지하며 자동 복구하지 않는다.

실제 게임에서 반복 사망, 덮기/걷히기 방향, HUD 전체 가림, 부활 시 HP·표정·알파·그림자 복구, 성장·적 상태 보존, 입력 차단 해제를 확인해야 한다. 현재 코드는 Death 클립 종료를 기다리지 않으므로 사망 비행과 Cover가 겹친다. 이 순서는 이번 에셋 작업에서 변경하지 않았다. 성능 측정이나 개선 수치는 없다.

## 후속 조절 위치

덮기·걷히기 속도는 Cover/Reveal의 마지막 키와 종료 이벤트 시각을 함께 조절한다. 암전 유지는 씬의 Stage `_blackHoldDuration`, 오류 대기 상한은 전환 Prefab의 `_maxWaitDuration`이다. 재사용할 때에는 Stage·Player의 상호 참조와 전환 참조를 빠짐없이 연결하고, 새 UI의 Canvas 순서가 전환 Canvas를 넘지 않도록 확인한다.
