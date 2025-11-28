using System.Collections;
using UnityEngine;

public class DefaultWSkill : MonoBehaviour
{
    [Header("Default W")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float cooldown = 2;
    [SerializeField] private float width = 1.7f;
    [SerializeField] private float height = 0.5f;
    public GameObject DefaultWPrefab;

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
        var (_, angleToMouse, skillPos) = MouseDirection.Mouse(transform);

        GameObject wSkill = Instantiate(DefaultWPrefab, skillPos, Quaternion.Euler(0, 0, angleToMouse));
        wSkill.transform.SetParent(transform);
        StartCoroutine(Cooldown());

        float t = 0f;

        while (t < 1f)
        {
            t = Mathf.MoveTowards(t, 1f, speed * Time.deltaTime);

            float tt = t < 0.5f ? t * 2f : (t - 0.5f) * 2f;
            float scale;

            if (t < 0.5f)
            {
                scale = Mathf.Pow(tt, 5);
            }
            else
            {
                scale = 1f - Mathf.Pow(tt, 5);
            }

            wSkill.transform.localScale = new Vector3(scale * width, height, 1f);

            yield return null;
        }
        Destroy(wSkill);
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldown);
        isWActive = false;
    }
}
