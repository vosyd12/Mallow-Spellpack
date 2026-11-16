# Motion 경로·상체 마스크·종료 이벤트 연결

작성일: 2026-11-16. 기존 PlayerVisual·기준 클립·Controller·마스크를 수정했다. C#·씬·Prefab·사용자 아틀라스 PNG/메타·리깅 데이터는 변경하지 않았다. MCP·Notion을 사용하지 않았다.

## 문제와 원인

Prefab 루트는 Motion인데 클립은 IdleMotion을 가리켜 파츠 경로가 맞지 않았다. PlayerUpperBody의 Transform 목록은 비어 있었고 Attack/Hit의 FinishAction 이벤트는 현재 코드에 없는 함수였다. 또한 Controller의 Idle은 중복 Player3Idle을 참조하고, 지팡이는 손 뼈 아래로 이동했는데 클립에는 예전 Motion 직속 Staff 경로와 좌표가 남아 있었다.

## 변경 내용과 선택 이유

| 대상 | 변경 |
| --- | --- |
| PlayerIdle·Walk·Attack·Hit·Death | 에디터/런타임 곡선의 경로 항목 616곳을 Motion 기준으로 변경하고 genericBindings의 경로 해시도 함께 갱신했다. 실제 Prefab Transform 경로와 대조했다. |
| 지팡이 바인딩 | 현재 ArmLeft의 손 뼈 하위 Staff 경로로 이전했다. 과거 절대 위치·회전을 그대로 새 부모 아래에서 재생하면 튀므로 기존 키의 변화량은 유지하되 첫 키를 현재 기본 위치·회전에 맞췄다. FirePos는 기존 지팡이 자식 참조를 유지한다. |
| PlayerUpperBody.mask | Animator 기준 상대 Transform 32개를 등록했다. 양팔·하위 뼈·Staff·FirePos의 10개 경로만 활성화했다. 공통 Motion·다리·몸통·머리·피격 효과 계층은 제외해 Base 이동을 공격이 덮지 않게 구성했다. |
| PlayerAttack 이벤트 | 0.30초 FireProjectile 유지, 0.70초 FinishAction을 FinishAttackMotion으로 변경했다. |
| PlayerHit 이벤트 | 0.42초 FinishAction을 FinishHitMotion으로 변경했다. |
| Player.controller | PlayerIdle과 Player3Idle의 곡선·설정이 이름 외에는 동일함을 확인하고 Idle을 기준 PlayerIdle GUID로 연결했다. Base/Attack 상태·Blend Tree·레이어 설정은 유지했다. |

클립 길이·발사 시점·키 시간·접선·스프라이트/색상 키·사망 효과는 유지했다. 지팡이 기본 자세 보정을 제외한 곡선 값은 원본과 같고, 보정한 키도 동일 변화량을 유지하는지 파일로 대조했다. 새 뼈 애니메이션이나 별도 공격 동작은 제작하지 않았다.

## 중복 에셋과 보존 범위

기준 Controller의 참조를 이전한 후, 외부 Assets 참조가 없는 Player3Idle과 PlayerMageV3 계열 클립·Controller·Prefab 8개 및 각 메타를 `Temp/PlayerMotionRetired-20261116/`로 옮겼다. Assets에는 PlayerVisual 하나와 기준 모션 5개를 사용한다. Temp 사본은 게임에서 참조하지 않으며 영구 백업으로 간주하지 않는다.

기준 Prefab GUID `360b288df2cc6974b92dca49d2c5b4da`, 아틀라스 GUID `5412993dcc56fcb4098bdbfff14b744d`, 기준 클립과 마스크 메타를 보존했다. 보호 대상 368개 파일은 원래 위치 또는 이동 위치에서 동일 해시임을 확인했다. Sprite Skin의 뼈 순서·기본 자세·가중치·Sprite ID를 다시 생성하지 않았다.

## 확인 결과

- **파일 검사:** Controller가 기준 클립 5개를 사용하는지, 활성 Assets에 IdleMotion/FinishAction 모션 바인딩이 남지 않았는지, 각 종료 이벤트의 public 수신 함수가 있는지 확인했다.
- **연결 검사:** 실제 씬의 PlayerVisual GUID, PlayerMotion의 모션 Animator, Staff/FirePos, Sprite Skin 6개·뼈 13개와 별도의 HitReaction 효과 Animator/HitEffectVisual/HitArea 계층이 유지되는지 확인했다. [검사 결과와 활성 마스크 목록](검증자료/2026-11-16-플레이어모션연결/AssetChecks.json)
- **Unity 임포트:** 별도 Unity 6000.5.10f1 임시 프로젝트에서 기준 클립 5개·Controller·Mask를 임포트하고 종료 코드 0을 확인했다. 원본 Prefab의 리깅 변형·게임 재생 검증을 수행한 것은 아니다. [임포트 기록 발췌](검증자료/2026-11-16-플레이어모션연결/UnityImport.txt)
- **미실시:** 원본 프로젝트 전체 컴파일·Play·실제 이동 중 상체 합성·성능 측정. 자연스러운 움직임과 상태 복귀는 기대 결과이며 실제 확인 완료로 기록하지 않는다.

## 다음 확인과 남은 작업

사용자는 정지/이동 공격에서 탄환이 한 발 나오는지, 다리 걷기가 유지되는지, 상체만 종료되는지, Hit 복귀·Death·부활과 반복 재시작이 정상인지 확인한다. 지팡이가 손에서 튀거나 꺾이지 않는지도 확인한다. 현재 클립은 주로 파츠 Transform 키이며 리깅 뼈에 새 키를 작성한 상태가 아니다. 몸통·머리까지 공격 합성에 포함할지와 과장된 뼈 모션·모자/얼굴 추종 개선은 실제 출력을 본 뒤 결정한다.

계층을 다시 바꿀 때에는 클립 문자열만 바꾸지 말고 내부 경로 해시·마스크·기본 로컬 자세·종료 이벤트까지 대조한다. 이전 [11월 13일 미완료 기록](2026-11-13-WORK-SUMMARY.md)의 경로·마스크·이벤트 과제는 이번 작업으로 연결 완료이며, 실행 검증·추가 모션 제작은 별도다.
