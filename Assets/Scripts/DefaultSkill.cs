using System.Collections;
using UnityEngine;

public class DefaultSkillE : MonoBehaviour
{
    public float swingSpeed = 180f;
    public float swingAngle1 = 45f;
    public float swingAngle2 = -45f;
    public float currentAngle;
    public int cooldown = 3;

    private bool isSwinging = false;
    public GameObject SwingPivot;

    void Update()
    {
        if (Input.GetKey(KeyCode.E) && !isSwinging)
        {
            StartCoroutine(Swing());
        }
    }

    IEnumerator Swing()
    {
        isSwinging = true;
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GameObject swing = Instantiate(SwingPivot, transform.position, Quaternion.identity);
        swing.transform.SetParent(transform);
        swing.transform.localPosition = new Vector3(0.3f, 0f, 0f);
        float currentAngle = swingAngle1;
        swing.transform.localRotation = Quaternion.Euler(0, 0, currentAngle);
        while (currentAngle > swingAngle2)
        {
            currentAngle -= swingSpeed * Time.deltaTime;
            swing.transform.rotation = Quaternion.Euler(0, 0, currentAngle);
            yield return null;
        }
        Destroy(swing);
        yield return new WaitForSeconds(3);
        isSwinging = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            // µ©Áö
        }
    }
}
