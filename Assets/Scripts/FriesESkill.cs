using System.Collections;
using UnityEngine;

public class FriesESkill : MonoBehaviour
{
    [Header("Fries E")]
    public static bool isUpgraded = false;
    public float cooldown = 5f;
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
        isUpgraded = true;
        yield return new WaitForSeconds(cooldown);
        isEActive = false;
    }
}
