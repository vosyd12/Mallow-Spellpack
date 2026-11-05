# 착지 먼지 시각 자원

후속 상태: [공용 풀 연결·실행 확인](2026-11-06-LANDING-DUST-POOL-CHECK.md)을 완료했다. 아래는 시각 자원 준비 당시의 설정과 미구현 범위다. 사용자가 작성한 코드가 실행 시 Stop Action을 Callback으로 바꾸므로 현재 풀링 경로에서는 재생 후 삭제 대신 반환한다.

## 범위와 파일

사용자가 MCP를 이용한 이미지 작업만 요청했다. C#·슬라임 동작·착지 호출·씬 배치는 변경하지 않았다. `Assets/Scripts/Effects/LandingDust.cs`는 사용자가 작성할 기존 파일로 유지했다.

- 이미지: `Assets/Sprites/Effects/Dust.png`
- 재질: `Assets/Materials/Effects/LandingDust.mat`
- Prefab: `Assets/Prefab/Effects/LandingDust.prefab`
- 셰이더: URP Particles/Unlit. 게임의 2D 바닥 위에서 표시를 확인했다.

## 외형과 설정

기본 내장 imagegen으로 투명 PNG 한 장을 생성했다. 프롬프트는 작은 2D 슬라임 게임의 먼지 입자용으로, 부드러운 둥근 덩어리 3개가 합쳐진 단일 먼지 구름, 옅은 흰색/회색, 단순한 명암, 투명 배경, 바닥·그림자·캐릭터·글자·테두리·발광 제외를 지정했다. 원본 PNG는 편집 없이 복사했고 Particle System에서 회녹색으로 착색한다.

3배 입자 수 범위의 상단값으로 소형 9·중형 15·대형 24개를 미리보기에서 비교했다. Prefab 기본 Burst는 중형 15개다. 하나의 Prefab을 공유하고 코드를 통해 개수를 바꾸는 구성이다.

- Loop 꺼짐, Play On Awake 꺼짐.
- 시간당/거리당 방출 0, 시간 0의 Burst 1개.
- 입자 수명 0.18~0.32초, 크기 0.13~0.23, 좌우 퍼짐과 약한 위쪽 이동.
- 크기가 약간 커지면서 알파가 0으로 감소한다. 기본 정렬 순서는 -1로 바닥(-100) 위·캐릭터(0) 아래다.
- World 공간에서 재생하며 완료 후 Stop Action=Destroy로 제거한다. maxParticles=32다.

## 사용자 코드에서 연결할 부분

이 Prefab에는 ParticleSystem과 ParticleSystemRenderer만 있다. 아직 LandingDust 스크립트는 부착하지 않았다. 사용자가 해당 컴포넌트를 작성·부착하고 ParticleSystem을 연결하면 된다.

호출 측은 Enemy 루트의 지면 위치에서 Prefab을 생성하고, LandingDust는 Burst 0의 입자 수를 9/15/24 등으로 설정한 뒤 ParticleSystem.Play()를 호출하는 방식에 맞춰 준비했다. Play On Awake가 꺼져 있으므로 생성만으로는 재생되지 않는다. Emit(count)를 별도로 추가하면 기존 Burst와 중복될 수 있으므로 한 가지 방식을 사용한다.

SlimeMotion의 Jump → Land 전환 순간 한 번 호출해야 한다. 매 Land 프레임에 호출하는 방식은 피한다. 소형의 경우 바닥 위치 기준 예시 오프셋은 Y=-0.11, 중형 -0.16, 대형 -0.245이며 실제 Visual 발 위치에 맞춰 조정할 수 있다. 제시한 값은 외형 미리보기용이며 충돌 규칙이 아니다.

## 확인

MCP의 편집기 임시 오브젝트에서 0.12초 시점에 9/15/24개를 확인하고, 추가 0.5초 시뮬레이션 후 각각 0개가 됨을 확인했다. [미리보기](LANDING-DUST-PREVIEW.png)는 실제 착지 동작이 아닌 시각 비교다. 확인 후 임시 오브젝트·카메라를 제거했고 Play는 실행하지 않았다. 씬 변경 상태는 false다.

C# 파일 해시가 모두 동일함을 확인했다. 자동 착지 연결·실제 게임에서의 생성/제거·모바일 성능 검증은 아직 하지 않았다. 이번 완료 범위는 이미지·재질·파티클 Prefab 준비까지다.
