using UnityEngine;

public enum ConsumableTier
{
    Small,
    Medium,
    Goal
}

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class ConsumableObjectInfo : MonoBehaviour
{
    public ConsumableTier Tier = ConsumableTier.Small;
    public int PointValue = 1;

    private Vector3 startPosition;
    private Quaternion startRotation;
    private Rigidbody rb;
    private Collider col;

    private void Awake()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    public void ResetToStart(HoleSystemController holeScript)
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (holeScript != null)
        {
            Physics.IgnoreCollision(col, holeScript.groundCollider, false);
            Physics.IgnoreCollision(col, holeScript.generatedMeshCollider, true);
        }

        transform.position = startPosition;
        transform.rotation = startRotation;
    }
}