public struct PlayerState
{
    public int LEVEL;

    public int EXP;

    public int HP;
    public int MP;

    public int STR;    // 힘 - 물리공격에 영향

    public int DEX;    // 민첩 - 이동속도, 공격속도 영향

    public int CON;    // 건강 - HP, 물리방어에 영향

    public int INT;    // 지능 - 마법공격에 영향

    public int WIS;    // 지혜 - MP, 마법방어에 영향

    public int LUK;    // 운 - 치명타확률, 치명타 데미지 영향

    public int AFF;    // 애정 - 펫 수용량에 영향

    int MAX;
}

public enum EPlayerJob
{
    ESward,
    EArcher,
    EWizard
}