# 프로젝트 공유와 재개 기록

2026-11-06 후속 최신화. 현재 프로젝트와 문서를 GitHub `vosyd12/Mallow-Spellpack`의 main에 공유한다. 이전 공유 이후 사용자 코드와 회피·공격 연출·HP바 UI 작업을 포함한다.

## 공유 상태

origin은 https://github.com/vosyd12/Mallow-Spellpack.git 이다. 업로드 전 원격 main과 로컬 HEAD는 `19d253a`로 일치했다. 이번 후속 커밋의 최종 식별자는 Git 이력과 종료 보고에서 확인한다. 기존 이력을 덮어쓰지 않고 후속 커밋으로 공유한다.

현재 구현의 기준은 [최신 정리](2026-11-06-CURRENT-IMPLEMENTATION.md), [현황](README.md), [MILESTONES](../MILESTONES.md)다. [CURRENT-SOURCE](2026-11-06-CURRENT-SOURCE.json)는 이번 저장 파일로 갱신했다. 과거 성능 실측에 첨부된 지문과 데이터는 보존한다.

이번 문서화는 C#이나 게임 설정의 결함을 고치지 않는다. 사용자 수정 코드와 이미 요청받아 구성한 UI/연출을 저장 상태 그대로 공유한다. 사용자 합의 규칙만 GAME_DESIGN에 기록하고, 테스트 설정과 미완료 코드는 분리했다.

## 다른 환경에서 재개

1. [작업 규칙](../AGENTS.md)을 먼저 읽는다. 코드 작성은 사용자 주도이며 AI는 명시된 범위에서 보조한다.
2. Unity 6000.5.10f1과 Assets/.meta·Packages/lock·ProjectSettings를 함께 복원한다. LFS 이미지가 포인터로 남으면 실제 파일을 내려받는다.
3. SampleScene을 열고 Console과 Inspector를 확인한다. HP바가 가득 차 있어도 현재 HP가 정상이라는 의미는 아니다.
4. 플레이어 최대 HP/MP, 공격 착지 타격 호출, HP바 갱신 연결, 생성 수 경계/재시도 시각, 회피 여유 거리의 남은 문제부터 확인한다.
5. 구현 기준과 미완료 이유는 최신 정리에서 확인하고, 과거 제안 코드나 측정 시점의 설정으로 덮어쓰지 않는다.

## 보존과 제외

새 공통 HP바 프리팹과 모든 Animation·Prefab·씬·스크립트의 .meta를 함께 공유한다. 날짜별 변경 이유·검사 한계와 기존 성능 자료를 보존한다. Library·Temp·Logs·UserSettings·obj·IDE 캐시·.utmp는 제외한다. Unity에서 저장하지 않은 변경은 Git 업로드에 포함되지 않는다.

## 이번 확인 범위

현재 코드·저장 에셋·문서와 링크를 대조했다. .NET 빌드 확인은 Temp/obj의 project.assets.json 누락으로 NETSDK1004가 발생해 완료하지 못했다. 일반 소스 복원 시도에서도 해당 캐시가 생성되지 않았다. Unity의 새 빌드·Play Mode 검증은 남아 있다. MCP·Notion은 사용하지 않았다. 과거 테스트 성공을 이번 파일의 실행 확인으로 간주하지 않는다.
