using System.Collections;
using UnityEngine;

public class BungUhBbangESkill : MonoBehaviour
{
    public static bool isCustardCream = false;

    [Header("BungUhBbang E")]
    [SerializeField] private float cooldown = 1f;

    private bool isEActive = false;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isEActive)
        {
            StartCoroutine(E());
        }
    }
    IEnumerator E()
    {
        isEActive = true;
        isCustardCream = !isCustardCream;
        Debug.Log("½´Å©¸² " + isCustardCream);
        StartCoroutine(Cooldown());
        yield return null;
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldown);
        isEActive = false;
    }
}
