public interface IUnit
{
    float Hp { get; set; }
    float MoveSpeed { get; set; }
    int Element { get; set; } // 1 -> 2 -> 3 -> 1 (f(x) = (x % 3) + 1)

    int Stomach{ get; set; }

    void Init(float hp, float moveSpeed, int element, int stomach);
    void GetDamage(float damage);
}
