using UnityEngine;
using UnityEngine.EventSystems;

public class MovePanel : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler
{
    private RectTransform panelTransform;   // 패널의 RectTransform 컴포넌트
    private RectTransform topBarTransform; // Top_bar의 RectTransform 컴포넌트

    private Vector2 pointerOffset;  // 마우스 움직직 offset
    private bool isDragging = false;    // 드래그중

    private void Start()
    {
        panelTransform = GetComponent<RectTransform>(); // RectTransform 컴포넌트 Get
        topBarTransform = panelTransform.Find("Top_Bar").GetComponent<RectTransform>(); // 패널의 자식에서 topbar 찾기
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.rawPointerPress == topBarTransform.gameObject)
        {
            panelTransform.parent.SetAsLastSibling();   // 패널의 부모도 젤 위로 올리기
            panelTransform.SetAsLastSibling(); // 클릭한 패널을 맨 위로 올리기
            isDragging = true; // 드래그 시작
            // 스크린 좌표계에서 패널의 로컬 좌표계로 변환하여 offset 계산
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
                // 패널의 로컬 위치를 설정하여 드래그한 위치로 이동
                panelTransform.localPosition = localPointerPosition - pointerOffset;

                // 패널의 가로와 세로 크기를 고려하여 스크린 경계 내에 있는지 제한
                Vector2 clampedPosition = panelTransform.localPosition;

                // Mathf.Clamp는 최소 최대로 제한
                clampedPosition.x = Mathf.Clamp(clampedPosition.x, -Screen.width / 2f + panelTransform.rect.width / 2f, Screen.width / 2f - panelTransform.rect.width / 2f);
                clampedPosition.y = Mathf.Clamp(clampedPosition.y, -Screen.height / 2f + panelTransform.rect.height / 2f, Screen.height / 2f - panelTransform.rect.height / 2f);

                panelTransform.localPosition = clampedPosition;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false; // 드래그 종료 처리
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        panelTransform.parent.SetAsLastSibling();   // 패널의 부모도 젤 위로 올리기
        panelTransform.SetAsLastSibling(); // 클릭한 패널을 맨 위로 올리기
        RectTransformUtility.ScreenPointToLocalPointInRectangle(panelTransform, eventData.position, eventData.pressEventCamera, out pointerOffset);
    }
}
