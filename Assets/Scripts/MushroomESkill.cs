using System.Collections;
using UnityEngine;

public class MushroomESkill : MonoBehaviour
{
    [Header("Mushroom E")]
    [SerializeField] private float mushroomEDuration = 5f;

    private bool isMushroomEActive = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isMushroomEActive)
        {
            StartCoroutine(E());
        }
    }
    IEnumerator E()
    {
        isMushroomEActive = true;

        float t = 0f;
        while (t < mushroomEDuration)
        {
            t = Mathf.MoveTowards(t, mushroomEDuration, Time.deltaTime);
            //Èú ÃÊ¸¶´Ù
            Debug.Log("ÃÊ¸¶´Ù Èú");
            yield return null;
        }

        isMushroomEActive = false;
    }
}
