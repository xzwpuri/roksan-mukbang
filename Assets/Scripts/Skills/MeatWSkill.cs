using System.Collections;
using UnityEngine;

public class MeatWSkill : MonoBehaviour
{
    [Header("Meat W")]
    [SerializeField] private float meatWSpeed = 720f;
    [SerializeField] private float meatWAngle1 = 70f;
    [SerializeField] private float meatWAngle2 = -70f;
    [SerializeField] private float meatWWidth = 2f;
    [SerializeField] private float meatWHeight = 0.5f;
    public GameObject MeatWPrefab;

    private bool isMeatWActive = false;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) && !isMeatWActive)
        {
            StartCoroutine(W());
        }
    }

    IEnumerator W()
    {
        isMeatWActive = true;

        float meatWCurrentAngle;

        var (dir, angleToMouse, skillPos) = MouseDirection.Mouse(transform);

        GameObject wSkill = Instantiate(MeatWPrefab, skillPos, Quaternion.Euler(0, 0, angleToMouse + meatWAngle1));
        wSkill.transform.SetParent(transform);
        wSkill.transform.localScale = new Vector3(meatWWidth, meatWHeight, 1f);

        meatWCurrentAngle = meatWAngle1;

        while (meatWCurrentAngle > meatWAngle2)
        {
            meatWCurrentAngle = Mathf.MoveTowardsAngle(meatWCurrentAngle, meatWAngle2, meatWSpeed * Time.deltaTime);
            wSkill.transform.rotation = Quaternion.Euler(0, 0, angleToMouse + meatWCurrentAngle);
            yield return null;
        }
        Destroy(wSkill);
        isMeatWActive = false;
    }
}
