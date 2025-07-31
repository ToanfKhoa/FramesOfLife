using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableFrame : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    //phuc vu keo tha
    private Transform originalParent;
    private int originalIndex;
    private GameObject placeholder;
    private LayoutElement layoutElement;
    private CanvasGroup canvasGroup;

    public Image image;       // anh hien thi
    public int correctIndex; // vi tri dung cua anh

    void Start()
    {
        layoutElement = GetComponent<LayoutElement>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        originalIndex = transform.GetSiblingIndex();

        // Create placeholder
        placeholder = new GameObject("Placeholder", typeof(RectTransform), typeof(LayoutElement));
        placeholder.transform.SetParent(originalParent);
        placeholder.transform.SetSiblingIndex(originalIndex);

        var le = placeholder.GetComponent<LayoutElement>();
        le.preferredWidth = layoutElement.preferredWidth;
        le.preferredHeight = layoutElement.preferredHeight;

        // Move dragged object outside of layout
        transform.SetParent(originalParent.parent.parent); // outside layout
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;

        for (int i = 0; i < originalParent.childCount; i++)
        {
            if (originalParent.GetChild(i) == placeholder) continue;

            var child = originalParent.GetChild(i) as RectTransform;
            if (eventData.position.x < child.position.x)
            {
                placeholder.transform.SetSiblingIndex(i);
                break;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(originalParent);
        transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());

        canvasGroup.blocksRaycasts = true;
        Destroy(placeholder);
    }
    public void Setup(Sprite sprite, int index)
    {
        image.sprite = sprite;
        correctIndex = index;
    }
}
