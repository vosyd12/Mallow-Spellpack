# 플레이어 Walk GIF 시안

후속: 현재 파츠로 [Walk 클립을 별도 제작하고 Unity 프리뷰에서 확인](2026-11-10-PLAYER-WALK-CLIP.md)했다. 아래 생성 이미지 GIF와 구분하며 게임 자동 전환은 아직 연결하지 않았다.
작성일: 2026-11-10

## 요청과 제작
현재 v3 원화에 어울리는 작은 보폭의 Walk를 GIF로 검토하도록 제작했다. 기본 제공 imagegen으로 원화를 참고한 8프레임 시트를 생성하고, 네이티브 프레임 분할·GIF 내보내기로 7프레임 반복 파일을 만들었다.

- [Walk GIF](아트시안/2026-11-10-플레이어마법사/WalkPreview/PlayerMage_Walk_Concept.gif)
- [생성 프레임 시트](아트시안/2026-11-10-플레이어마법사/WalkPreview/PlayerMage_Walk_ConceptAtlas.png)

## 문제·선택과 달라진 점
Idle 상태로 이동하면 걷는 느낌이 부족하므로 짧은 발 교대·작은 몸의 상하 움직임·로브와 모자·양갈래의 후행 움직임을 검토한다. 양발이 함께 떠 보이는 원본 6번 프레임은 제외하고 0→1→2→3→4→5→7 순서로 구성했다. 프레임당 0.14초, 한 주기 0.98초, 384×512, 무한 반복이다.

## 확인과 한계
시트에서 전신·지팡이·발 교대와 잘림 여부를 확인했다. GIF를 다시 디코딩해 프레임 7개와 각 지연 14/100초를 확인하고 무한 반복 확장을 넣었다.
이 파일은 생성 프레임을 이어 붙인 동작 제안이며, 현재 Unity 파츠 리그나 실제 게임을 녹화한 결과가 아니다. 원화의 선·비율·세부 색과 프레임별 형태에 차이가 있어 최종 스프라이트로 확정하지 않는다. 실제 게임의 발 접지·이동 속도 일치·파츠 연결부는 별도 구현·검증이 필요하다. 게임 파일·C#·씬·현재 Idle은 변경하지 않았으며 MCP도 사용하지 않았다.

## 생성 프롬프트
참조: PlayerMageConcept_v3.png.

```text
Use case: stylized-concept / game animation preview spritesheet.
Create an EIGHT-FRAME seamlessly looping SMALL-STEP WALK CYCLE for the exact approved mage in the reference. This will be sliced into an animated GIF, so frame-to-frame consistency and walk mechanics are CRITICAL.
Output ONLY a STRICT 4 columns by 2 rows grid, 8 equal rectangular cells, preferably 1536x1024 overall, each cell 384x512. Uniform solid pale warm gray #F3EFE6 background in ALL cells. NO text, grid lines, borders, numbers, diagrams or labels. Each cell contains exactly ONE full-body mage, hat to boots and staff fully inside its cell. Maintain identical scale and fixed horizontal body center in all eight cells. Max character height about 410px per 512px cell. Hat has generous clearance. Same ground contact baseline at y=475 WITHIN EACH CELL, small unobtrusive identical soft oval contact shadow. No objects crossing cell boundaries.
IDENTITY: faithfully preserve the reference's simplified chibi early-teen girl, very large floppy purple gold-band hat drooping right, short brown twin pigtails, purple gold-edged shoulder capelet and A-line robe, small brown boots, simple dark oval eyes and tiny cheerful mouth. Wooden staff with mint diamond crystal ALWAYS held in viewer LEFT hand, other arm relaxed. Same clean plum outline, simple broad cel shading, gold hem, no new costume details, no complex eyes, no shiny 3D rendering. No added accessories. No blinking or changing face this time.
WALK MOTION: near-front view matching reference with only a subtle three-quarter orientation, calm cute short measured steps, walking in place. NOT running, jumping, skating, marching high knees, dance or full-body wobbling. Alternate feet visibly: short forward step, weight transfer with supporting foot grounded, rear foot lifts just enough to pass, opposite foot plants. One full stride cycle in eight frames in LEFT-TO-RIGHT TOP-TO-BOTTOM order. Both feet must NOT lift together.
FRAME mechanics:
0 viewer-left foot forward planted at baseline, viewer-right foot slightly back;
1 weight settles onto left foot, body lowers very subtly, right heel lifts;
2 left support remains planted, right foot passes forward slightly lifted, robe follows;
3 right foot reaches forward toward contact, small body rise;
4 mirrored support phase: right foot forward firmly planted, left foot back;
5 weight settles onto right, body lowers very subtly, left heel lifts;
6 right support stays planted, left foot passes forward slightly lifted;
7 left foot reaches forward into pose0, seamless loop back to0.
Keep upper body and face stable; body bob is only about 4-6px within each cell, little forward lean. Robe hem sways gently in response to alternating legs, no billowing. Hat tip and pigtails lag the body bob slightly, with only tiny bending/rotation, not major shape changes. Holding hand and staff move TOGETHER as one unit with subtle sway, no detached hand or staff sliding in grip. Grounded boot must stay on same baseline; lifted boot just 5-10px higher. Make leg alternation legible despite short robe. No motion trails, attacks, projectiles, special effects or exaggerated squash. All 8 faces, hat size, staff crystal, hair colors and proportions must be consistent. This is concept animation, not a claimed screenshot or game-ready separate parts atlas.
```
