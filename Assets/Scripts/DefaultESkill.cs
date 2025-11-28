using System.Collections;
using UnityEngine;

public class DefaultESkill : MonoBehaviour
{
    [Header("Default E")]
    [SerializeField] private float speed = 540f;
    [SerializeField] private float angle1 = 60f;
    [SerializeField] private float angle2 = -60f;
    [SerializeField] private float cooldown = 3;
    [SerializeField] private float width = 1.2f;
    [SerializeField] private float height = 0.3f;
    public GameObject DefaultEPrefab;

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

        float currentAngle;

        var (dir, angleToMouse, skillPos) = MouseDirection.Mouse(transform);

        GameObject eSkill = Instantiate(DefaultEPrefab, skillPos, Quaternion.Euler(0, 0, angleToMouse + angle1));
        eSkill.transform.SetParent(transform);
        eSkill.transform.localScale = new Vector3(width, height, 1f);
        StartCoroutine(Cooldown());

        currentAngle = angle1;

        while (currentAngle > angle2)
        {
            currentAngle = Mathf.MoveTowardsAngle(currentAngle, angle2, speed * Time.deltaTime);
            eSkill.transform.rotation = Quaternion.Euler(0, 0, angleToMouse + currentAngle);
            yield return null;
        }

        Destroy(eSkill);
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldown);
        isEActive = false;
    }
}
