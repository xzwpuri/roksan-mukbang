using System.Collections;
using UnityEngine;

public class MushroomWSkill : MonoBehaviour
{
    [Header("Mushroom W")]
    [SerializeField] private float mushroomWDuration = 6f;
    [SerializeField] private float mushroomWRadius = 4.5f;

    public GameObject MushroomWPrefab;
    private bool isMushroomWActive = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) && !isMushroomWActive)
        {
            StartCoroutine(W());
        }
    }
    IEnumerator W()
    {
        isMushroomWActive = true;

        var (dir, _, _) = MouseDirection.Mouse(transform);

        GameObject wSkill = Instantiate(MushroomWPrefab, transform.position + dir * 3f, Quaternion.identity);
        wSkill.transform.localScale = new Vector3(mushroomWRadius, mushroomWRadius, 1f);

        yield return new WaitForSeconds(mushroomWDuration);
        Destroy(wSkill);
        isMushroomWActive = false;
    }
}
