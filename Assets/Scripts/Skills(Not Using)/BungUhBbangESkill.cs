using System.Collections;
using UnityEngine;

public class BungUhBbangESkill : MonoBehaviour
{
    public static bool isCustardCream = false;

    private bool isBungUhBbangEActive = false;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isBungUhBbangEActive)
        {
            StartCoroutine(E());
        }
    }
    IEnumerator E()
    {
        isBungUhBbangEActive = true;
        isCustardCream = !isCustardCream;
        Debug.Log("½´Å©¸² " + isCustardCream);
        isBungUhBbangEActive = false;
        yield return null;
    }
}
