# 속성 착지 효과 표시 오류·셰이더 추가 진단

작성일: 2026-11-07. 이 문서는 당시의 진단·중단 기록이다. 이후 수정과 MCP 확인을 완료했으며 최신 상태는 [속성 착지 셰이더 수정 결과](2026-11-07-ELEMENT-LANDING-SHADER-FIX.md)를 따른다.

## 문제와 확인

사용자가 속성별 착지 효과가 적용되지 않는다고 보고했다. 파일에서는 효과 선택·Prefab 연결이 존재했지만 실제 표시까지 확인하지 않은 상태였다. 재질의 Tiling·Offset으로 몸·장식·입자가 있는 아틀라스의 일부를 선택하도록 구성한 것이 문제였다. 프로젝트에 설치된 URP `ParticlesUnlitForwardPass.hlsl`과 `ShaderLibrary/Particles.hlsl`의 해당 UV 경로는 이 변환을 적용하지 않는다.

사용자 허가 후 MCP로 실제 씬에서 여섯 효과를 재생했다. 여섯 효과 모두 18개 입자가 생성됐지만 Sprite 선택은 꺼져 있었고, 아틀라스가 작은 입자로 표시되어 속성 형태가 뭉개졌다. 기본 먼지는 독립 텍스처이므로 아틀라스 선택 오류의 대상과 구분한다. [수정 전 캡처](검증자료/2026-11-07-속성착지셰이더/01-before-dust.png).

캡처는 비교를 위해 Play Mode에서 전투 개체를 잠시 비활성화하고 여섯 효과를 같은 위치·강도로 배치한 재현 화면이다. 위쪽은 불·물·땅, 아래쪽은 바람·번개·무속성이다. 입자는 0.05초 시점으로 시뮬레이션했고 카메라 크기는 4로 잠시 변경했다. 일반 플레이의 자연 착지 장면이나 수정 후 화면은 아니다.

## 추가 발견과 중단

- 바람 효과에서 `Particle Orbital Velocity curves must all be in the same mode` 경고가 발생했다. `LandingDustWind.prefab`의 orbitalX·Y는 Constant(0), Z만 TwoConstants(3)다. 이전 에셋 구성에서 Z만 변경한 것이 원인이다. 세 축을 같은 모드로 맞춰야 한다. C# 원인이 아니다.
- 새 셰이더 초안은 HLSL 예약어 `point`를 매개변수명으로 사용해 컴파일 오류가 발생했다. AI가 만든 오류이며 `samplePosition` 등으로 수정했다. 최종 MCP의 ShaderUtil 조회에서 현재 셰이더 메시지와 오류는 없었다. 실제 연결·렌더링 성공을 뜻하지 않는다.
- 입자 Sprite 영역을 줄이는 파일 변경은 Windows의 Hidden 속성이 있는 `.meta`를 쓰기 모드로 여는 과정에서 거부됐다. 원본 파일은 변경되지 않았다. 권한 부족으로 단정하지 않으며, 숨김 속성을 보존하는 기존 파일 갱신 방식 또는 Unity 임포터 API가 필요하다.

## 작성한 초안과 남은 작업

`Assets/Shaders/Effects/ElementLanding.shader` 초안은 입자 수명에 따른 반짝임과 바닥 번개 문양을 그리는 두 변형을 준비한다. 새 C# 파일은 만들지 않았고 기존 C#도 수정하지 않았다. 아직 어떤 효과 재질에도 연결하지 않았다. 번개용 변형의 실제 렌더링도 확인하지 않았다.

재개 시 순서는 Sprite의 입자 영역 지정 → Texture Sheet Animation의 Sprites 연결 → 바람 곡선 모드 통일 → 재질·UV Rect·AgePercent 스트림 연결 → 번개를 고정된 바닥 문양 한 개로 설정 → 같은 조건에서 캡처·재사용 확인이다. 파티클의 AgePercent는 현재 Unity에서 값 21임을 MCP로 확인했다. [Unity 공식 참조 소스](https://github.com/Unity-Technologies/UnityCsReference/blob/master/Modules/ParticleSystem/Managed/ParticleSystemEnums.cs)의 Sprite 모드·수명 스트림 정의도 참고했다.

종료 상태는 Play Mode 꺼짐·일시정지 꺼짐·씬 수정 표시 없음·카메라 크기 7.5다. 기존 씬·효과 Prefab·재질·Sprite 메타·C#의 작업 시작 시점 해시가 유지됨을 확인했다. 이번 변경은 새 셰이더 초안과 진단 기록이며, 속성 Dust 수정 완료나 성능 개선으로 기록하지 않는다.
