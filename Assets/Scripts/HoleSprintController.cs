using UnityEngine;

public class HoleSprintController : MonoBehaviour
{
    public HoleSystemController holeController;

    public float sprintMultiplier = 1.8f;
    public float sprintDuration = 1.25f;
    public float cooldownDuration = 3.5f;

    public bool IsSprinting { get; private set; }

    private float sprintTimer = 0f;
    private float cooldownTimer = 0f;

    public bool IsReady
    {
        get { return !IsSprinting && cooldownTimer <= 0f; }
    }

    public bool IsCoolingDown
    {
        get { return !IsSprinting && cooldownTimer > 0f; }
    }

    public float CurrentFillAmount
    {
        get
        {
            if (IsSprinting && sprintDuration > 0f)
            {
                return sprintTimer / sprintDuration;
            }

            if (cooldownTimer > 0f && cooldownDuration > 0f)
            {
                return cooldownTimer / cooldownDuration;
            }

            return 0f;
        }
    }

    private void Update()
    {
        if (holeController == null)
        {
            return;
        }

        if (!holeController.CanMove)
        {
            IsSprinting = false;
            sprintTimer = 0f;
            cooldownTimer = 0f;
            holeController.ResetMoveMultiplier();
            return;
        }

        if (IsSprinting)
        {
            sprintTimer -= Time.deltaTime;

            if (sprintTimer <= 0f)
            {
                sprintTimer = 0f;
                IsSprinting = false;
                holeController.ResetMoveMultiplier();
                cooldownTimer = cooldownDuration;
            }
        }
        else if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;

            if (cooldownTimer < 0f)
            {
                cooldownTimer = 0f;
            }
        }
    }

    public bool TryActivateSprint()
    {
        if (holeController == null)
        {
            return false;
        }

        if (!holeController.CanMove)
        {
            return false;
        }

        if (IsSprinting || cooldownTimer > 0f)
        {
            return false;
        }

        IsSprinting = true;
        sprintTimer = sprintDuration;
        holeController.SetMoveMultiplier(sprintMultiplier);
        return true;
    }
}