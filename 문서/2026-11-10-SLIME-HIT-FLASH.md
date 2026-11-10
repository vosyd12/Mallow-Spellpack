# 슬라임 공통 피격 색상 연결

작성일: 2026-11-10. **MCP로 새 셰이더의 정상 로드·오류 없음과 실제 피격을 확인하고, 기본 3종·속성 Variant 18종의 몸 재질을 연결 완료했다.** 동일한 HitReaction을 사용하는 플레이어 몸도 연결해 피격 표시 누락을 막았다. 사용자 C# 소스는 변경하지 않았다.

## 문제와 원인

물슬라임의 피격 색이 다른 슬라임과 달라 보인다는 사용자 보고가 있었다. 기존 SpriteRenderer 틴트는 원본 이미지 색과 곱해지므로 같은 피격 RGB라도 출력이 달라질 수 있다. 실제 화면의 원인을 이번 작업에서 재검증한 것은 아니다.

사용자는 HitReaction을 MaterialPropertyBlock의 `_HitColor`·`_HitAmount` 전달 방식으로 수정했다. 기존 Sprite Lit 재질은 이 값을 처리하지 않으므로 대응 셰이더 연결이 필요했다.

MCP 연결 후 피격색이 전혀 안 나오던 원인도 확인했다. 마젠타 복구를 위해 몸을 기본 Sprite-Lit-Default로 되돌린 상태에서, 코드는 새 프로퍼티를 전달하고 있었다. 기본 재질에는 `_HitAmount`가 없어 값이 전달돼도 색을 표시하지 못했다. 당시 플레이어 몸에서도 같은 재질 불일치를 확인했다.

## 적용한 변경과 이유

- `Assets/Shaders/Characters/SlimeHitFlash.shadergraph`: 프로젝트에 설치된 URP 17.6의 Sprite Lit 그래프 구성을 바탕으로 작성했다. 기존 2D 조명 경로를 사용한다.
- 원본 Sprite 텍스처 × Vertex Color를 평상시 색으로 사용하고, `_HitAmount`에 따라 `_HitColor`와 Lerp한다. 색상 혼합 뒤에 기존 틴트를 다시 곱하지 않도록 기본 자동 틴트를 끄고 그래프 안에서 먼저 적용한다.
- Alpha는 원본 텍스처 × Vertex Color의 알파를 별도로 전달한다. 피격 색으로 투명 여백이 채워지지 않도록 RGB와 분리했다.
- `Assets/Materials/Characters/SlimeHitFlash.mat`: `_HitAmount=0`, `_HitColor=(1, 0.4, 0.4, 1)`로 준비했다. 실제 피격값은 사용자 코드가 개체별 MaterialPropertyBlock으로 전달한다.
- SlimeSmall·SlimeMedium·SlimeLarge Prefab에서 HitReaction이 참조하는 몸 SpriteRenderer의 Material 슬롯만 교체했다. 속성 Variant 18종에는 해당 Material·피격색의 개별 override가 없어 기본 설정을 상속하는 구조다.
- SampleScene의 플레이어 몸에도 같은 Material을 연결했다. 사용자 코드가 공유 HitReaction을 변경한 상태여서 기본 재질을 그대로 두면 기존 플레이어 피격 표시도 사라지기 때문이다. 변경 전의 저장되지 않은 씬 내용까지 Temp에 복사 보존한 뒤 연결·저장했다.
- 얼굴·속성 장식·그림자·HP바 재질과 사용자 C# 소스는 변경하지 않았다. 손으로 작성한 `.shader`나 추가 C# 파일 대신 편집 가능한 Shader Graph 에셋으로 구성했다.

## 확인한 결과

- 그래프 JSON을 읽어 노드 15개·연결 9개와 연결 대상 노드 참조 누락 0개를 확인했다.
- 기존 Prefab 3개와 비교해 몸 Material 참조 교체 외 내용이 동일함을 확인했다.
- 작업 시작 시점 대비 C# 파일 해시 변경은 0개다.
- Shader Graph와 Material 및 폴더의 `.meta`를 함께 준비했다.

위 검사는 파일 구조 확인이며 Unity 셰이더 컴파일 성공이나 실제 표시 검증을 대신하지 않는다.

## 오류와 복구 이력

### 마젠타 표시 대응

사용자가 새 연결 뒤 슬라임이 네모난 마젠타로 보인다고 보고했다. 이후 프로젝트의 `Logs/Editor.log`에서 해당 Graph의 `JSON parse error: Invalid value`와 임포트 실패를 확인했다. 파일 구조 검사만으로 정상 작동을 판단할 수 없었던 사례다.

당시 기본 3종 몸을 기존 GUID `a97c105638bdf8b4a8650670310a4cd3`로 복원하고 사용자 C#은 보존했다. 기존 재질에는 새 피격 프로퍼티가 없어 피격색이 표시되지 않는 임시 복구 상태였다. 이후 아래 실제 Unity 검증을 거쳐 새 재질을 다시 연결했다.

### 후속 JSON 수정과 임포트 확인

- 원인: PowerShell 직렬화의 오브젝트 구분자와 마지막 오브젝트 내부 빈 줄이 Shader Graph의 MultiJson 파서 구분 방식과 충돌했다. 일부 노드 좌표도 숫자 대신 문자열이었다.
- 수정: 각 JSON 오브젝트를 한 줄로 저장하고 동일한 빈 줄 구분자를 마지막까지 사용했다. 노드 좌표를 숫자로 정규화했다. 사용자 C#과 Material·Prefab 연결은 바꾸지 않았다.
- 결과: Unity 파서와 같은 구분 방식으로 53개 오브젝트를 읽었고 문자열 좌표는 0개였다. 수정 후 프로젝트 Editor.log에 해당 Graph의 정상 임포트가 기록됐다. 실제 렌더링·피격 동작의 성공과는 구분한다.
- MCP: 기존 서버가 응답하지 않아 캐시된 MCP for Unity 10.3 서버를 localhost:8099로 실행했다. 서버 초기화는 성공했으나 인스턴스 조회는 0개다. Unity의 MCP 창에서 Connect가 필요하다는 사용자 입력 요청을 보냈다. 권한 승인 미응답과 에디터 연결 부재를 혼동하지 않는다.

### MCP 연결 후 완료한 검증

- 사용자의 에디터 연결 후 Shader.isSupported=true, ShaderUtil 메시지 0개, Material이 실제 새 Shader를 참조함을 확인했다. 확인 전 기존 재질은 변경하지 않았다.
- 기본 3종을 Unity Prefab API로 연결·저장하고 속성 Variant 18종의 몸이 모두 같은 Material과 피격 프로퍼티를 갖는지 조회했다.
- 실행 중 임시 물·불 Medium 개체를 같은 화면에 배치해 Enemy.TakeDamage(1)를 호출했다. 물만 맞았을 때 물의 `_HitAmount=1`, 불은 0이었고 공유 Material의 값도 0으로 유지됐다. 불도 맞게 한 뒤 양쪽 몸이 같은 붉은색으로 표시되는 것을 직접 확인했다. 얼굴·속성 장식과 투명한 윤곽은 유지됐다.
- 18종 임시 개체 모두에서 실제 피해의 피격값 1, HitReaction 비활성화 시 0, 재활성화 후 연속 Play 시 1을 확인했다. 미리보기 2개를 포함한 20개에서 시간 경과 후 0으로 복원됐다. 기존 Renderer 틴트와 공유 Material 값은 바뀌지 않았다.
- 플레이어도 TakeDamage(1) 직후 1, 이후 0 복원을 확인했다. 일반 전투 화면에서 마젠타 표시가 없음을 확인했다.
- 검증용 카메라·시간 배율을 복원하고 임시 개체를 제거했다. 자동 공격·EnemySpawner는 활성 상태이며, 편집 모드·시간 배율 1·씬 저장 완료·임시 루트 없음으로 종료했다. C# 해시 변경은 0개다.

[평상시 물·불](검증자료/2026-11-10-SLIME-HIT-FLASH/normal.png) · [물만 피격](검증자료/2026-11-10-SLIME-HIT-FLASH/water-hit-only.png) · [동일 피격색](검증자료/2026-11-10-SLIME-HIT-FLASH/both-hit.png) · [복원](검증자료/2026-11-10-SLIME-HIT-FLASH/restored.png) · [일반 전투](검증자료/2026-11-10-SLIME-HIT-FLASH/gameplay-final.png)

목표는 원본 몸 색에 따른 피격색 편차를 없애는 것이다. Sprite Lit이므로 서로 다른 조명 아래에서 최종 화면 RGB까지 항상 같다고 보장하지 않는다. 개별 18종의 시각 비교 전부가 아니라 물·불의 시각 비교와 18종의 상태·연결 검증을 수행했다. 성능 측정·GitHub 업로드는 하지 않았다.
