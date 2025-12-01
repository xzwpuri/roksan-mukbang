using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonOpacity : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Set(1f);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        Set(0.5f);
    }

    private void Set(float v)
    {
        Color c = text.color;
        c.a = v;
        text.color = c;
    }
}
