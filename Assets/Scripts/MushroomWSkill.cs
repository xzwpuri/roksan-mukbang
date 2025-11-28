using System.Collections;
using UnityEngine;

public class MushroomWSkill : MonoBehaviour
{
    [Header("Mushroom W")]
    [SerializeField] private float cooldown = 8f;
    [SerializeField] private float duration = 6f;
    [SerializeField] private float radius = 4.5f;

    public GameObject MushroomWPrefab;
    private bool isWActive = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) && !isWActive)
        {
            StartCoroutine(W());
        }
    }
    IEnumerator W()
    {
        isWActive = true;

        var (dir, _, _) = MouseDirection.Mouse(transform);

        GameObject wSkill = Instantiate(MushroomWPrefab, transform.position + dir * 3f, Quaternion.identity);
        wSkill.transform.localScale = new Vector3(radius, radius, 1f);
        StartCoroutine(Cooldown());

        yield return new WaitForSeconds(duration);
        Destroy(wSkill);
    }
    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldown);
        isWActive = false;
    }
}
