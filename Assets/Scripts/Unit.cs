using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    protected float hp;
    protected float movespeed;
    protected int element;    
    // 1 -> 2 -> 3 -> 1
    // f(x)=(x%3)+1


    public void Init(float hp, float movespeed)
    {
        this.hp = hp;
        this.movespeed = movespeed;
    }

    public void GetDamage(float dam)
    {
        this.hp -= dam;
        if (this.hp <= 0)
        {
            //Destroy(gameObject);
        }
    }
}
