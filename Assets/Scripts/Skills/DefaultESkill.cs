using System.Collections;
using UnityEngine;

public class DefaultESkill : MonoBehaviour
{
    [Header("Default E")]
    [SerializeField] private float defaultESpeed = 540f;
    [SerializeField] private float defaultEAngle1 = 60f;
    [SerializeField] private float defaultEAngle2 = -60f;
    [SerializeField] private float defaultEWidth = 1.2f;
    [SerializeField] private float defaultEHeight = 0.3f;
    public GameObject DefaultEPrefab;

    private bool isDefaultEActive = false;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isDefaultEActive)
        {
            StartCoroutine(E());
        }
    }

    IEnumerator E()
    {
        isDefaultEActive = true;

        float defaultECurrentAngle;

        var (dir, angleToMouse, skillPos) = MouseDirection.Mouse(transform);

        GameObject eSkill = Instantiate(DefaultEPrefab, skillPos, Quaternion.Euler(0, 0, angleToMouse + defaultEAngle1));
        eSkill.transform.SetParent(transform);
        eSkill.transform.localScale = new Vector3(defaultEWidth, defaultEHeight, 1f);

        defaultECurrentAngle = defaultEAngle1;

        while (defaultECurrentAngle > defaultEAngle2)
        {
            defaultECurrentAngle = Mathf.MoveTowardsAngle(defaultECurrentAngle, defaultEAngle2, defaultESpeed * Time.deltaTime);
            eSkill.transform.rotation = Quaternion.Euler(0, 0, angleToMouse + defaultECurrentAngle);
            yield return null;
        }
        Destroy(eSkill);
        isDefaultEActive = false;
    }
}
