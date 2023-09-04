using UnityEngine;
using UnityEngine.EventSystems;

public class MovePanel : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler
{
    private RectTransform panelTransform;   // �г��� RectTransform ������Ʈ
    private RectTransform topBarTransform; // Top_bar�� RectTransform ������Ʈ

    private Vector2 pointerOffset;  // ���콺 ������ offset
    private bool isDragging = false;    // �巡����

    private void Start()
    {
        panelTransform = GetComponent<RectTransform>(); // RectTransform ������Ʈ Get
        topBarTransform = panelTransform.Find("Top_Bar").GetComponent<RectTransform>(); // �г��� �ڽĿ��� topbar ã��
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.rawPointerPress == topBarTransform.gameObject)
        {
            panelTransform.parent.SetAsLastSibling();   // �г��� �θ� �� ���� �ø���
            panelTransform.SetAsLastSibling(); // Ŭ���� �г��� �� ���� �ø���
            isDragging = true; // �巡�� ����
            // ��ũ�� ��ǥ�迡�� �г��� ���� ��ǥ��� ��ȯ�Ͽ� offset ���
            RectTransformUtility.ScreenPointToLocalPointInRectangle(panelTransform, eventData.position, eventData.pressEventCamera, out pointerOffset);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {


            Vector2 localPointerPosition;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(panelTransform.parent as RectTransform, eventData.position, eventData.pressEventCamera, out localPointerPosition))
            {
                // �г��� ���� ��ġ�� �����Ͽ� �巡���� ��ġ�� �̵�
                panelTransform.localPosition = localPointerPosition - pointerOffset;

                // �г��� ���ο� ���� ũ�⸦ ����Ͽ� ��ũ�� ��� ���� �ִ��� ����
                Vector2 clampedPosition = panelTransform.localPosition;

                // Mathf.Clamp�� �ּ� �ִ�� ����
                clampedPosition.x = Mathf.Clamp(clampedPosition.x, -Screen.width / 2f + panelTransform.rect.width / 2f, Screen.width / 2f - panelTransform.rect.width / 2f);
                clampedPosition.y = Mathf.Clamp(clampedPosition.y, -Screen.height / 2f + panelTransform.rect.height / 2f, Screen.height / 2f - panelTransform.rect.height / 2f);

                panelTransform.localPosition = clampedPosition;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false; // �巡�� ���� ó��
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        panelTransform.parent.SetAsLastSibling();   // �г��� �θ� �� ���� �ø���
        panelTransform.SetAsLastSibling(); // Ŭ���� �г��� �� ���� �ø���
        RectTransformUtility.ScreenPointToLocalPointInRectangle(panelTransform, eventData.position, eventData.pressEventCamera, out pointerOffset);
    }
}
