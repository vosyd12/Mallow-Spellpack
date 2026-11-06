public struct EnemyState
{
    public int HP;

    public int MP;  
    public int Attack;      // 공격
    public int Defense;     // 방어

    public int Exp;         // 적이 제공할 경험치량

    public float MoveSpeed;   // 이동속도
    public float AttackSpeed; // 공격속도
}

// 적들의 속성
public enum Element
{
    Fire,       // 불
    Water,      // 물
    Earth,      // 땅
    Wind,       // 바람
    Lightning   // 번개

}