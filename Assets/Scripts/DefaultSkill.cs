using System;
using System.Collections;
using UnityEngine;

public class DefaultSkill : MonoBehaviour
{
    [Header("Swing")]
    public float swingSpeed = 180f;
    public float swingAngle1 = 60f;
    public float swingAngle2 = -60f;
    public float currentAngle;
    public int swingCooldown = 3;
    public float swingReach = 1.2f;
    public float swingWidth = 0.3f;
    public GameObject SwingPivot;

    [Header("Jab")]
    public float jabSpeed = 5f;
    public int jabCooldown = 2;
    public float jabReach = 1.7f;
    public float jabWidth = 0.5f;
    public GameObject JabPivot;

    private bool isSwinging = false;
    private bool isJabbing = false;

    void Update()
    {
        if (Input.GetKey(KeyCode.Q) && !isJabbing)
        {
            StartCoroutine(Jabbing());
        }
        if (Input.GetKey(KeyCode.E) && !isSwinging)
        {
            StartCoroutine(Swing());
        }
    }

    private (Vector3 dir, float angleToMouse, Vector3 skillPos) MouseDirection()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        Vector3 dir = (mousePos - transform.position).normalized;
        float angleToMouse = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Vector3 skillPos = transform.position + dir * 0.3f;
        return (dir, angleToMouse, skillPos);
    }

    IEnumerator Jabbing()
    {
        isJabbing = true;

        var (dir, angleToMouse, skillPos) = MouseDirection();

        GameObject jab = Instantiate(JabPivot, skillPos, Quaternion.Euler(0, 0, angleToMouse));
        jab.transform.SetParent(transform);

        float t = 0f;

        while (t < 1f)
        {
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

            jab.transform.localScale = new Vector3(scale * jabReach, jabWidth, 1f);

            t += Time.deltaTime * jabSpeed;
            yield return null;
        }

        Destroy(jab);
        yield return new WaitForSeconds(jabCooldown);
        isJabbing = false;
    }

    IEnumerator Swing()
    {
        isSwinging = true;

        var (dir, angleToMouse, skillPos) = MouseDirection();

        GameObject swing = Instantiate(SwingPivot, skillPos, Quaternion.Euler(0, 0, angleToMouse + swingAngle1));
        swing.transform.SetParent(transform);
        swing.transform.localScale = new Vector3(swingReach, swingWidth, 1f);

        currentAngle = swingAngle1;

        while (currentAngle > swingAngle2)
        {
            currentAngle -= swingSpeed * Time.deltaTime;
            swing.transform.rotation = Quaternion.Euler(0, 0, angleToMouse + currentAngle);
            yield return null;
        }

        Destroy(swing);
        yield return new WaitForSeconds(swingCooldown);
        isSwinging = false;
    }
}
