# 워터볼 자동 발사 씬 참조 복구

## 증상과 원인

`Assets/Scenes/IngameScene.unity`에서 자동 모드가 켜져 있어도 워터볼이 발사되지 않았고 첫 슬롯의 쿨다운도 시작되지 않았다. 작업 기준은 저장소의 `Assets/Scenes`에 실제로 존재하는 씬이다. 현재 전투 씬은 `IngameScene.unity`이고 `TitleScene.unity`는 타이틀 씬이다.

씬에는 `m_EditorClassIdentifier: Assembly-CSharp::WaterBall`이 남아 있었지만, 같은 컴포넌트의 `m_Script` GUID는 빈 `FireballSkill.cs`를 가리켰다. 수정 전 Unity MCP에서 플레이어에 `FireballSkill`이 붙어 있고 `PlayerSkill._waterBall`은 `null`임을 확인했다. 이 값이 비어 있으면 `CanUseWaterBall`이 거짓이 되어 적을 찾기 전에 종료한다. 슬롯 쿨다운은 발사 성공 후 시작하므로 함께 멈췄다.

11월 19일 원격 이력을 대조했다. 기존 워터볼 업로드 커밋 `ca6e9d3`의 `WaterBall.cs.meta`에 `FireballSkill.cs.meta`와 동일한 GUID가 들어 있고 씬도 이 GUID를 참조한다. 중복 GUID가 포함된 커밋은 확인했으며, 해당 값을 처음 만든 작업은 확인되지 않았다.

## 수정과 검증 결과

`IngameScene.unity`의 워터볼 컴포넌트에서 `m_Script` 한 곳을 `FireballSkill.cs.meta`의 `390d6c8a129672448aece7a4fd01e360`에서 기존 `WaterBall.cs.meta`의 `3f2ca64508694094487b591bf6b3d754`로 바꿨다. 같은 컴포넌트의 파일 ID와 저장값을 보존하기 위해 참조만 수정했다. 투사체 Prefab, `FirePos`, 사거리 6, 쿨다운 3초와 첫 슬롯 연결을 유지했고 C#과 `.meta`는 수정하지 않았다.

- **파일 확인.** 수정한 GUID를 되돌려 비교했을 때 씬의 나머지 바이트가 원본과 같았다.
- **Unity 편집 모드.** 씬을 다시 열어 플레이어에 실제 `WaterBall` 컴포넌트가 붙고, `PlayerSkill._waterBall`에 같은 타입이 연결되는 것을 확인했다. 자동 모드와 `EnemySpawner` 연결도 확인했다.
- **Unity Play Mode.** 실제 전투를 12초간 관찰해 워터볼 4회와 기본 공격 투사체 13개를 확인했다. `SkillSlot01`은 쿨다운이 시작된 4회에 원형 덮개와 `3 → 2 → 1`을 표시했고, 쿨다운이 끝나면 덮개가 사라졌다. 관찰한 워터볼 명중 효과 4개는 모두 생성 후 자동 정리됐다. 편집 모드로 복귀했고 게임 코드 오류는 확인되지 않았다. 종료 후 Console에는 Unity MCP 연결 경고와 Unity AI 패키지의 `NoSubscription` 예외가 있었다.
- **검증 범위.** 이번 재생은 자동 발사, 슬롯, 명중 효과의 생성과 정리까지 확인했다. 사망 시 쿨다운 초기화는 [후속 과제](2026-11-18-WATERBALL-FOLLOW-UP.md)로 남아 있다. HDR Emission 적용 여부와 모바일 성능은 별도 재검증 항목이다.

## 재발 방지

1. 씬 작업과 검증은 `Assets/Scenes`의 대상 파일 경로를 먼저 확인한다. 전투 연결은 `Assets/Scenes/IngameScene.unity`를 기준으로 한다.
2. 기존 스크립트를 이동하거나 이름을 바꿀 때 `.meta`를 함께 유지한다. 씬 연결을 수정하거나 병합한 뒤에는 컴포넌트에 표시된 이름과 `m_Script` GUID를 함께 대조한다. 워터볼은 `WaterBall.cs.meta`로 해석되어야 하고, 편집 모드에서 `PlayerSkill._waterBall`이 실제 `WaterBall` 컴포넌트를 참조해야 한다.
3. 씬을 저장하고 다시 연 뒤 확인한다. 사거리 안에 적이 들어오면 자동 발사, 첫 슬롯의 3초 표시와 해제, 기존 기본 공격을 Play Mode에서 함께 확인한다. 변경 이력에는 **저장된 파일 참조 확인**과 **실제 재생 결과**를 구분해 기록한다.
