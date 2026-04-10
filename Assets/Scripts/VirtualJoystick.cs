using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public static VirtualJoystick Instance { get; private set; }

    public RectTransform background;
    public RectTransform handle;
    public float handleRange = 80f;

    public Vector2 InputVector { get; private set; }

    private Canvas canvas;
    private Camera uiCamera;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        canvas = GetComponentInParent<Canvas>();

        if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            uiCamera = canvas.worldCamera;
        }
        else
        {
            uiCamera = null;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background,
            eventData.position,
            uiCamera,
            out localPoint))
        {
            Vector2 clamped = Vector2.ClampMagnitude(localPoint, handleRange);

            handle.anchoredPosition = clamped;
            InputVector = clamped / handleRange;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        InputVector = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
    }
}