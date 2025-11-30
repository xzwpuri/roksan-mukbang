using System.Collections;
using UnityEngine;

public class DefaultWSkill : MonoBehaviour
{
    [Header("Default W")]
    [SerializeField] private float defaultWSpeed = 5f;
    [SerializeField] private float defaultWWidth = 1.7f;
    [SerializeField] private float defaultWHeight = 0.5f;
    public GameObject DefaultWPrefab;

    private bool isDefaultWActive = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) && !isDefaultWActive)
        {
            StartCoroutine(W());
        }
    }

    IEnumerator W()
    {
        isDefaultWActive = true;
        var (_, angleToMouse, skillPos) = MouseDirection.Mouse(transform);

        GameObject wSkill = Instantiate(DefaultWPrefab, skillPos, Quaternion.Euler(0, 0, angleToMouse));
        wSkill.transform.SetParent(transform);

        float t = 0f;

        while (t < 1f)
        {
            t = Mathf.MoveTowards(t, 1f, defaultWSpeed * Time.deltaTime);

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

            wSkill.transform.localScale = new Vector3(scale * defaultWWidth, defaultWHeight, 1f);

            yield return null;
        }
        Destroy(wSkill);
        isDefaultWActive = false;
    }
}
