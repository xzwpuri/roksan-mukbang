using System.Collections;
using UnityEngine;

public class MeatWSkill : MonoBehaviour
{
    [Header("Meat W")]
    [SerializeField] private float speed = 720f;
    [SerializeField] private float angle1 = 70f;
    [SerializeField] private float angle2 = -70f;
    [SerializeField] private float cooldown = 3;
    [SerializeField] private float width = 2f;
    [SerializeField] private float height = 0.5f;
    public GameObject MeatWPrefab;

    private float currentAngle;

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

        var (dir, angleToMouse, skillPos) = MouseDirection.Mouse(transform);

        GameObject wSkill = Instantiate(MeatWPrefab, skillPos, Quaternion.Euler(0, 0, angleToMouse + angle1));
        wSkill.transform.SetParent(transform);
        wSkill.transform.localScale = new Vector3(width, height, 1f);
        StartCoroutine(Cooldown());

        currentAngle = angle1;

        while (currentAngle > angle2)
        {
            currentAngle = Mathf.MoveTowardsAngle(currentAngle, angle2, speed * Time.deltaTime);
            wSkill.transform.rotation = Quaternion.Euler(0, 0, angleToMouse + currentAngle);
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
