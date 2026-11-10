# 플레이어 v3 파츠 Idle 적용

후속: [Walk 단독 클립](2026-11-10-PLAYER-WALK-CLIP.md)을 제작했고 기본 행동 5종과 사망 방향을 확정했다. [최신 플레이어 요약](2026-11-10-PLAYER-WORK-SUMMARY.md)을 따르며, 아래 남은 범위는 Idle 적용 당시 기록이다.
작성일: 2026-11-10

## 요청과 변경
사용자가 확정한 v3와 파츠 PNG를 기준으로 Idle까지만 구현하고 Unity MCP 연결·실행 검증을 허용했다. 이전 v2 통짜 프레임 교체 방식에서 v3 파츠 리그로 교체했다. C# 소스는 변경하지 않았다.

- `Assets/Sprites/Characters/Player/V3/PlayerMageV3PartsAtlas.png`: 기존 파츠의 투명 여백을 줄여 2048×1024 아틀라스 한 장으로 구성했다. 파츠 12개와 원화의 피부 영역에서 추출한 눈 뒤 보완용 Sprite 1개, PPU 804, Bilinear, 무압축·MipMap 없음.
- `Assets/Prefab/Player/PlayerMageV3Visual.prefab`: SortingGroup과 IdleMotion 아래에 12개 파츠, 눈 뒤 피부 보완 Sprite 2개를 배치했다. 기존 Visual Renderer는 비활성화하고 Sprite 참조를 비웠다.
- `Assets/Animations/Player/PlayerMageV3Idle.anim`, `PlayerMageV3Idle.controller`: Idle 한 상태, 3.6초 반복, Transform 곡선 6개.
- `Assets/Scenes/SampleScene.unity`: 기존 Player 아래 Visual 프리팹 연결. 이동·공격·Collider·HP바의 기존 연결은 유지하고 HitReaction의 단일 몸통 Renderer 참조를 새 Torso로 연결했다. 따라서 현재 피격색 대상은 몸통이며, 다른 파츠를 함께 피격시키는 기능은 추가하지 않았다.

## 문제·원인·해결 방법
분리 PNG는 보이는 픽셀만 가지고 있어 큰 회전에서 연결부가 벌어질 수 있다. 발 기준 호흡(세로 최대 +1.2%, 가로 최대 -0.3%), 모자 ±0.12도, 양갈래 ±0.35도로 움직임을 제한했다. 큰 자세 변형용 연결부 전체를 다시 그리지는 않았다.

눈을 세로로 줄이면 기존 분리 경계의 잔선이 남았다. 원화의 작은 피부 영역을 Sprite로 추출해 눈 뒤에만 배치하고, Head 위·Eyes 아래 순서로 가렸다. 눈은 2.54~2.82초 사이에 세로 크기 1→0.35→0.045→0.35→1로 변한다. 별도 표정 그림이나 C# 없이 현재 원화를 이용했다.

## 실제 확인
- Unity Play 모드에서 기본 Idle 상태와 반복 재생을 확인했다. 조회 시 normalizedTime이 약 29를 넘어 루프가 진행 중이었다.
- 17개 시점의 실제 리그 샘플에서 호흡·모자·눈 곡선과 0/3.6초 복원을 확인했다. 발 기준점의 월드 좌표는 모든 샘플에서 동일했다.
- 확대 캡처에서 눈을 뜬 모습과 감은 모습, 이번 Idle 범위의 연결부를 확인했다. 일반 게임 화면도 캡처했다.
- HitReaction 호출 직후 Torso의 _HitAmount=1, Head=0을 확인했다. 모든 활성 파츠의 셰이더가 지원 상태이며 Unity Console 오류는 0건이었다.
- Player·PlayerAttack·HealthBar와 EnemySpawner는 활성 상태였다. 캡처 때만 시간을 멈추고 적의 표시 레이어와 카메라를 바꿨으며 finally에서 복원했다. TimeScale=1, 카메라 크기=7.5, Animator 활성 상태를 확인했다.
- 검증 종료 후 Edit 모드, Player 위치 (0,0,0), 씬 저장·dirty=false를 확인했다. 기존 C# 파일의 작업 전후 해시가 모두 일치했다. 최종 프리팹은 활성 Renderer 14개 모두 Sprite가 연결되어 있고 기본 상태는 Idle 하나였다.

## 미리보기
[원본 PNG 품질로 반복 보기](플레이어IdleV3/Preview.html) · [Unity 캡처 GIF](플레이어IdleV3/PlayerMageV3_Idle_Unity.gif) · [일반 게임 화면](플레이어IdleV3/Ingame_Idle_Final.png)

확대 미리보기는 실제 Unity 리그를 시간별로 샘플링한 캡처이며 연속 녹화 영상은 아니다. 주변 적 표시를 임시로 가린 검증 화면이다. 일반 게임 캡처와 구분하며 GIF는 색상 양자화로 원본보다 품질이 낮을 수 있다.

## 남은 범위
이동·공격·피격·사망 전용 애니메이션과 상태 전환은 미구현이다. 기존 자동 이동 중에도 현재 Idle이 재생된다. 큰 회전·새 자세에는 경계 마스크·가려진 연결부 보완이 여전히 필요하다. 원본 PSD·분리 PNG·이전 v2 에셋은 보존했으며 PSD/Krita 실앱 호환성 검증은 이번에도 진행하지 않았다. 아틀라스 통합은 적용 사실이며 FPS 향상이나 Draw Call 감소량은 측정하지 않았다.
