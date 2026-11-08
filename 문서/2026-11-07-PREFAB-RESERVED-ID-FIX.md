# 속성 Prefab 예약 ID 충돌 수정

작업일: 2026-11-07. 사용자가 속성 Prefab을 Enemy 슬롯에 넣을 수 없고 실제 생성도 되지 않는다고 보고했다. 원인은 AI가 작성한 Prefab 파일 구조에 있었다.

## 발생한 문제와 직접 원인

Unity의 원본 프로젝트 `Logs/Editor.log`에서 18종 모두 다음 오류를 확인했다.

```text
Prefab file is corrupt: it contains an object using fileID 100100000,
which is reserved for the Prefab asset.
```

Prefab Variant 파일을 직접 작성하면서 `--- !u!1001 &100100000`으로 내부 PrefabInstance를 선언했다. `100100000`은 Prefab 에셋 참조에 예약된 값이므로 내부 오브젝트 선언에 사용하면 Unity가 파일을 손상된 것으로 거부한다. 이미지 생성이나 사용자의 랜덤 선택 코드가 이 오류의 원인은 아니다. 에셋이 로드되지 않으므로 Enemy 슬롯 등록과 후보 검색도 정상적으로 동작할 수 없었다.

## 왜 이전 검사에서 놓쳤는가

이전 검사는 GUID·내부 참조의 존재·중복 조합 여부만 확인했다. 잘못된 ID를 파일과 검사 양쪽에서 동일하게 사용해 참조 일치 검사는 통과했지만 Unity의 예약 ID 제약은 검사하지 않았다. 실제 네이티브 PrefabImporter의 오류도 완료 보고 전에 확인하지 않았다. 따라서 이전의 파일 검사 통과를 정상 사용 가능한 Prefab이라는 근거로 삼은 판단은 잘못이었다.

## 수정한 내용과 유지한 부분

- 18종 내부 PrefabInstance ID를 `7300000000000000100`으로 변경했다.
- 상속된 Enemy 참조의 `m_PrefabInstance`도 같은 ID로 맞췄다.
- `m_SourcePrefab`의 `fileID: 100100000`은 외부 Prefab 에셋을 가리키는 올바른 용도이므로 유지했다. 숫자 전체를 무조건 치환하지 않았다.
- 씬의 Spawner가 기본 3종으로 돌아가 있는 것을 확인하고 18종의 Enemy 참조 목록을 복원했다.

Prefab의 GUID, Enemy 컴포넌트 참조 ID, 크기·속성 값, 몸·얼굴 이미지와 모션 설정은 유지했다. C#·PNG·원본 Prefab·Animation·생성 수와 대기 시간 코드는 수정하지 않았다.

## 실제 Unity 확인

동일 에셋을 `.utmp/PrefabReservedIdFix-20261107/UnityProject`에 복사해 Unity 6000.5.10f1을 batchmode·nographics로 실행했다. 사용자 코드도 복사본 그대로 사용했으며 검증용 C#을 작성하지 않았다. uGUI는 기존 로컬 패키지와 기본 모듈을 사용했다. 원본 프로젝트의 전체 패키지·렌더링 환경과 동일한 검증은 아니다.

첫 실행은 권한 제한으로 패키지 서버 IPC 연결을 완료하지 못했다. 권한을 허용받아 재실행한 결과, 18종 모두 네이티브 PrefabImporter의 임포트 기록이 생겼고 예약 ID 손상 오류·C# 컴파일 오류 없이 `Exiting batchmode successfully now!`로 종료했다.

[검증 결과·에셋 해시](검증자료/2026-11-07-Prefab예약ID오류/VERIFICATION.json)와 [18종 임포트 기록](검증자료/2026-11-07-Prefab예약ID오류/PREFAB-IMPORT-LINES.txt)을 남겼다. 원본 전체 로그에는 환경·인증 정보가 섞일 수 있으므로 문서에는 필요한 임포트 행만 보존했다.

파일 검사에서도 예약 ID의 내부 선언 제거, 변경된 ID와 참조 일치, 18개 크기·속성 조합, 씬 후보 중복·누락 여부를 확인했다. 작업 전후 C# 해시도 동일하다.

## 남은 확인과 재발 방지

원본 Unity의 Inspector 드래그·Play Mode·실제 전투는 수행하지 않았다. 원본 에디터가 수정 전 씬을 열고 있다면 변경된 씬을 다시 읽어 18종 등록 상태를 확인해야 한다. 새 에셋 생성 시에는 가능한 한 Unity의 Prefab API로 저장하고, 직접 직렬화 파일을 변경한다면 예약 ID 선언과 실제 임포트 성공을 함께 검사해야 한다.

문서 상태는 `파일 구조 확인`, `Unity 임포트 확인`, `실제 실행 확인`을 구분한다. 파일 검사만 통과한 결과로 슬롯 등록이나 게임 실행까지 확인했다고 보고하지 않는다. 성능 변화는 측정하지 않았다.
