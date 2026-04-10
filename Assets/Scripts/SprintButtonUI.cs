using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SprintButtonUI : MonoBehaviour, IPointerDownHandler
{
    public HoleSprintController sprintController;
    public Image buttonImage;
    public Image fillImage;

    public float doubleTapWindow = 0.35f;

    private float lastTapTime = -10f;

    private Color readyColor = new Color(1f, 1f, 1f, 0.45f);
    private Color sprintColor = new Color(0.2f, 1f, 0.2f, 0.75f);
    private Color cooldownColor = new Color(1f, 0.45f, 0.45f, 0.6f);

    public void OnPointerDown(PointerEventData eventData)
    {
        float currentTime = Time.unscaledTime;

        if (currentTime - lastTapTime <= doubleTapWindow)
        {
            if (sprintController != null)
            {
                sprintController.TryActivateSprint();
            }

            lastTapTime = -10f;
            return;
        }

        lastTapTime = currentTime;
    }

    private void Update()
    {
        if (sprintController == null)
        {
            return;
        }

        if (buttonImage != null)
        {
            if (sprintController.IsSprinting)
            {
                buttonImage.color = sprintColor;
            }
            else if (sprintController.IsCoolingDown)
            {
                buttonImage.color = cooldownColor;
            }
            else
            {
                buttonImage.color = readyColor;
            }
        }

        if (fillImage != null)
        {
            fillImage.fillAmount = sprintController.CurrentFillAmount;

            if (sprintController.IsSprinting)
            {
                fillImage.color = new Color(0.2f, 1f, 0.2f, 0.75f);
            }
            else if (sprintController.IsCoolingDown)
            {
                fillImage.color = new Color(1f, 0.45f, 0.45f, 0.65f);
            }
            else
            {
                fillImage.color = new Color(1f, 1f, 1f, 0f);
            }
        }
    }
}