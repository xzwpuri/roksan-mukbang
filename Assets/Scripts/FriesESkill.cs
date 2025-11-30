using System.Collections;
using UnityEngine;

public class FriesESkill : MonoBehaviour
{
    public static bool isFriesUpgraded = false;

    private bool isFriesEActive = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isFriesEActive)
        {
            StartCoroutine(E());
        }
    }
    IEnumerator E()
    {
        isFriesEActive = true;
        isFriesUpgraded = true;
        isFriesEActive = false;
        yield return null;
    }
}
