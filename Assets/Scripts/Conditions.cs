using UnityEngine;

public class Conditions : MonoBehaviour
{
    public int Points = 0;

    public int Stage1Threshold = 5;
    public int Stage2Threshold = 12;

    public int CurrentStage = 0;

    public HoleSystemController HoleScript;
    public GameUIManager GameUIManager;

    public bool ShowHud = false;

    private bool levelWon = false;
    private GUIStyle labelStyle;

    private void OnTriggerEnter(Collider other)
    {
        if (levelWon)
        {
            return;
        }

        if (other.gameObject.layer != LayerMask.NameToLayer("Obstacles"))
        {
            return;
        }

        ConsumableObjectInfo info = other.GetComponent<ConsumableObjectInfo>();

        if (info == null)
        {
            return;
        }

        if (!CanConsume(info.Tier))
        {
            info.ResetToStart(HoleScript);
            return;
        }

        Physics.IgnoreCollision(other, HoleScript.groundCollider, false);
        Physics.IgnoreCollision(other, HoleScript.generatedMeshCollider, true);

        int gainedPoints = info.PointValue;

        Destroy(other.gameObject);

        HandleProgress(info.Tier, gainedPoints);
    }

    private bool CanConsume(ConsumableTier tier)
    {
        if (tier == ConsumableTier.Small)
        {
            return true;
        }

        if (tier == ConsumableTier.Medium)
        {
            return CurrentStage >= 1;
        }

        if (tier == ConsumableTier.Goal)
        {
            return CurrentStage >= 2;
        }

        return false;
    }

    private void HandleProgress(ConsumableTier tier, int gainedPoints)
    {
        Points += gainedPoints;

        if (CurrentStage == 0 && Points >= Stage1Threshold)
        {
            CurrentStage = 1;
            StartCoroutine(HoleScript.ScaleHole());
        }

        if (CurrentStage == 1 && Points >= Stage2Threshold)
        {
            CurrentStage = 2;
            StartCoroutine(HoleScript.ScaleHole());
        }

        if (tier == ConsumableTier.Goal && CurrentStage >= 2)
        {
            levelWon = true;
            HoleScript.CanMove = false;
            ShowHud = false;

            if (GameUIManager != null)
            {
                GameUIManager.ShowWinPanel();
            }
        }
    }

    private void OnGUI()
    {
        if (!ShowHud)
        {
            return;
        }

        if (labelStyle == null)
        {
            labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.fontSize = 40;
            labelStyle.normal.textColor = Color.white;
        }

        GUI.Label(new Rect(30, 30, 600, 50), "Score: " + Points, labelStyle);
        GUI.Label(new Rect(30, 80, 1200, 50), GetStageText(), labelStyle);
    }

    private string GetStageText()
    {
        if (CurrentStage == 0)
        {
            return "Get " + Stage1Threshold + " points to unlock medium cubes";
        }

        if (CurrentStage == 1)
        {
            return "Get " + Stage2Threshold + " points to unlock the goal";
        }

        if (CurrentStage == 2)
        {
            return "Now consume the gold cube";
        }

        return "";
    }
}