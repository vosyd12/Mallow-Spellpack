# 속성 슬라임 18종 시안

후속 작업: 승인한 이미지를 [Prefab Variant 18종에 장착](../../2026-11-07-ELEMENTAL-SLIME-PREFABS.md)했다. 이 폴더는 원본 시안·프롬프트·비교 갤러리를 보존하며, 게임용 사본은 `Assets/Sprites/Characters/Slimes/`에 있다.

현재 Variant는 [몸·얼굴이 분리된 Layers 이미지](../../2026-11-07-ELEMENTAL-SLIME-FACE-MOTION.md)를 사용한다. 이 폴더의 한 장짜리 PNG는 승인 원본으로 보존한다.

2026-11-07. [미리보기 갤러리](PREVIEW.html)에서 전체 비교와 축소 가독성을 확인한다. [프롬프트·파일 지문](PROMPTS.json)을 보존했다. 빌트인 image_gen을 사용했다.

| 속성 | Small | Medium | Large |
| --- | --- | --- | --- |
| 무속성 | [보기](Pure_Small.png) | [보기](Pure_Medium.png) | [보기](Pure_Large.png) |
| 불 | [보기](Fire_Small.png) | [보기](Fire_Medium.png) | [보기](Fire_Large.png) |
| 물 | [보기](Water_Small.png) | [보기](Water_Medium.png) | [보기](Water_Large.png) |
| 땅 | [보기](Earth_Small.png) | [보기](Earth_Medium.png) | [보기](Earth_Large.png) |
| 바람 | [보기](Wind_Small.png) | [보기](Wind_Medium.png) | [보기](Wind_Large.png) |
| 번개 | [보기](Lightning_Small.png) | [보기](Lightning_Medium.png) | [보기](Lightning_Large.png) |

## 검토 기준

- 같은 픽셀 윤곽·눈·명암 기준이 유지되는지 확인한다.
- Small의 얼굴과 단일 속성 장식이 작게 봐도 읽히는지 확인한다.
- Medium의 눈썹과 Large의 콧수염이 장식에 가려지지 않는지 확인한다.
- 불의 곡선, 물의 물결, 땅의 돌, 바람의 소용돌이, 번개의 지그재그가 색상 외에도 구별되는지 확인한다.

현재는 몸·얼굴·장식이 합쳐진 시안 PNG다. 제작용 레이어 분리·Prefab 생성·전투 속성 연동은 하지 않았다. 기존 게임의 Small 크기도 그대로이며 약 20% 확대는 후속 조정안이다. 이미지 해상도와 실제 월드 크기는 별도로 관리한다.
