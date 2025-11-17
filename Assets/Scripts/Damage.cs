using UnityEngine;

public class Damage : MonoBehaviour
{
    public int damage = 30;
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 대충 적 health 구하기
        //if (health != null)
        //{
        //    대충 데미지 주기
        //}
        Debug.Log("아야");
    }
}
