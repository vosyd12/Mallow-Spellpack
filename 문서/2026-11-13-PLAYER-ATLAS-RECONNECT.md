# 리깅 아틀라스 기준 통일과 표시 연결 복구

## 문제·원인

사용자는 Sprite Editor로 리깅한 PlayerPartsAtlas를 유지하고 Game 화면에 보이지 않는 플레이어의 연결 복구를 요청했다. 확인 시 V3 폴더에는 해당 PNG와 .meta만 남았지만 PlayerVisual의 14개 Renderer가 삭제된 이전 아틀라스 GUID c2d5c43dc099ba742a9ea5c1dee956f2를 참조하고 있었다. 이 상태에서는 Sprite 참조가 해석되지 않는다.

추가로 Player Controller에는 조건 없는 Any State → Death, Blend Tree → Hit 전환이 남아 있었다. 특히 Death는 페이드로 외형을 사라지게 하므로 정상 시작 상태에서도 비표시를 유발할 수 있는 설정이다. 실제 Unity 재생으로 해당 현상을 관찰한 결과와는 구분한다.

## 선택한 해결 방법·달라진 점

- 기준은 `Assets/Sprites/Characters/Player/V3/PlayerPartsAtlas.png`, GUID `5412993dcc56fcb4098bdbfff14b744d`다. PNG와 .meta를 재생성하거나 덮어쓰지 않았다. 사용자 Sprite ID·피벗·PPU·메시·뼈·가중치 데이터를 그대로 보존했다.
- 각 기존 Sprite ID가 기준 아틀라스에 실제 존재하는 것을 확인하고 `Assets/Prefab/Player/PlayerVisual.prefab`의 아틀라스 GUID 참조만 변경했다. 객체 ID·계층·씬 보정·FirePos·HitEffect 연결은 유지한다. 중복 아틀라스는 이번 작업 시작 시 이미 없었으므로 추가 삭제하지 않았다.
- `Assets/Animations/Player/Player.controller`에서 조건 없는 Death·Hit 전환 두 개와 해당 참조만 제거했다. 사용자가 구성 중인 Base/Attack 레이어·이동 Blend Tree·클립 연결은 유지한다.
- AGENTS에 기준 아틀라스 GUID와 사용자 리깅 보존·연결 검증 규칙을 추가했다. C#·모션 클립·재질·셰이더·씬은 변경하지 않았다.

## 확인 결과·남은 문제

[검증 기록](검증자료/2026-11-13-PLAYER-ATLAS-RECONNECT/Verification.json)에 대조 결과를 남겼다. 6개 Sprite Skin의 뼈 13개에 대해 Sprite 데이터 순서·이름·부모·기본 위치/회전을 현재 Transform과 대조했다. 위치는 PPU 804와 Sprite 피벗 보정을 반영해 확인했다. 14개 Sprite 참조의 ID가 유효하며 폐기 GUID 참조가 Assets에 남지 않는다. 보호 대상 273개 파일의 해시와 아틀라스 원본·메타가 유지된다.

MCP·Unity 임포트·Play 검증은 수행하지 않았다. 파일 수준의 표시 연결 원인은 수정했지만 Game 화면 출력 성공으로 기록하지 않는다.

현재 PlayerMotion 코드의 Empty State.Idle/Walk/Hit/Death 경로와 Base 레이어의 Blend Tree/Hit/Death 구성이 맞지 않는다. Attack.Attack 경로도 별도 레이어인데 기존 상태 변경 함수는 레이어 0을 사용한다. 걷기·상체 공격 작업에서 사용자가 코드 경로·Blend 파라미터·Attack 레이어 제어를 맞춰야 한다. 이 문제를 해결하기 위해 사용자 코드나 작업 중인 레이어 구조를 임의로 재작성하지 않았다. 상체 마스크 연결·게임 재생 검증은 별도 후속이다.

이전 통합 기록의 기준 경로·GUID 설명 중 아틀라스 기준은 이번 기록으로 대체한다. 복구용 변경 전 파일은 `Temp/PlayerAtlasReconnect20261113/Before`에 보관했다.
