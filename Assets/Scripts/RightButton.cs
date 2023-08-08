using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RightButton : MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IPointerExitHandler
{
    public GameObject PlayerN;
    public float moveDistance = 6.5f;

    private Image buttonImage;
    private Color originalColor;
    private Color clickedColor = new Color(0.8f, 0.8f, 0.8f, 1f);
    private bool isClicked = false;

    private void Start()
    {
        buttonImage = GetComponent<Image>();
        originalColor = buttonImage.color;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isClicked)
        {
            isClicked = true;
            buttonImage.color = clickedColor;

            float targetX = PlayerN.transform.position.x + moveDistance;
            targetX = Mathf.Clamp(targetX, float.MinValue, 6.5f); // X 위치를 float.MinValue와 6.5 사이로 제한 (양수 방향으로 무한히 이동하지 않도록)

            PlayerN.transform.position = new Vector3(targetX, PlayerN.transform.position.y, PlayerN.transform.position.z);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isClicked = false;
        buttonImage.color = originalColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isClicked = false;
        buttonImage.color = originalColor;
    }
}