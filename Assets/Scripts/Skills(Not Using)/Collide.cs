using UnityEngine;

public class Collide : MonoBehaviour
{
    public System.Action<Collider2D> onHit;

    void OnTriggerEnter2D(Collider2D other)
    {
        onHit?.Invoke(other);
    }
}
