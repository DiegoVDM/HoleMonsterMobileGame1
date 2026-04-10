using System.Collections;
using UnityEngine;

public class HoleSystemController : MonoBehaviour
{
    public PolygonCollider2D hole2DCollider;
    public PolygonCollider2D ground2DCollider;
    public MeshCollider generatedMeshCollider;
    public Collider groundCollider;

    public float initialScale = 0.5f;
    public float moveSpeed = 6f;
    public float boundaryPadding = 1.0f;
    public int holeSides = 20;

    public bool CanMove = true;

    public float MoveMultiplier { get; private set; } = 1f;

    private Mesh generatedMesh;
    private Vector3 lastPosition;
    private Vector3 lastScale;
    private bool isScaling = false;

    private void Start()
    {
        CreateGround2DShape();
        CreateHole2DShape();
        RebuildHoleSystem();

        IgnoreObstacleCollisionsWithGeneratedMesh(true);

        lastPosition = transform.position;
        lastScale = transform.localScale;
    }

    private void FixedUpdate()
    {
        if (CanMove)
        {
            HandleMovement();
        }

        if (transform.position != lastPosition || transform.localScale != lastScale)
        {
            RebuildHoleSystem();
            lastPosition = transform.position;
            lastScale = transform.localScale;
        }
    }

    private void HandleMovement()
    {
        Vector2 input = Vector2.zero;

        if (VirtualJoystick.Instance != null)
        {
            input = VirtualJoystick.Instance.InputVector;
        }

        if (input == Vector2.zero)
        {
            input = GetKeyboardInput();
        }

        if (input == Vector2.zero)
        {
            return;
        }

        Vector3 move = new Vector3(input.x, 0f, input.y);
        float currentSpeed = moveSpeed * MoveMultiplier;

        Vector3 newPosition = transform.position + move * currentSpeed * Time.fixedDeltaTime;

        Bounds bounds = groundCollider.bounds;
        float padding = boundaryPadding * transform.localScale.x;

        newPosition.x = Mathf.Clamp(newPosition.x, bounds.min.x + padding, bounds.max.x - padding);
        newPosition.z = Mathf.Clamp(newPosition.z, bounds.min.z + padding, bounds.max.z - padding);
        newPosition.y = 0f;

        transform.position = newPosition;
    }

    public void SetMoveMultiplier(float newMultiplier)
    {
        MoveMultiplier = Mathf.Max(1f, newMultiplier);
    }

    public void ResetMoveMultiplier()
    {
        MoveMultiplier = 1f;
    }

    private Vector2 GetKeyboardInput()
    {
        Vector2 input = Vector2.zero;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            input.x -= 1f;

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            input.x += 1f;

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            input.y -= 1f;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            input.y += 1f;

        return input.normalized;
    }

    private void CreateGround2DShape()
    {
        Bounds bounds = groundCollider.bounds;

        Vector2[] outline = new Vector2[4];
        outline[0] = new Vector2(bounds.min.x, bounds.min.z);
        outline[1] = new Vector2(bounds.max.x, bounds.min.z);
        outline[2] = new Vector2(bounds.max.x, bounds.max.z);
        outline[3] = new Vector2(bounds.min.x, bounds.max.z);

        ground2DCollider.pathCount = 1;
        ground2DCollider.SetPath(0, outline);
    }

    private void CreateHole2DShape()
    {
        Vector2[] points = new Vector2[holeSides];

        for (int i = 0; i < holeSides; i++)
        {
            float angle = (Mathf.PI * 2f * i) / holeSides;
            points[i] = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }

        hole2DCollider.pathCount = 1;
        hole2DCollider.SetPath(0, points);
    }

    private void RebuildHoleSystem()
    {
        hole2DCollider.transform.position = new Vector3(transform.position.x, transform.position.z, 0f);

        hole2DCollider.transform.localScale = new Vector3(
            transform.localScale.x * initialScale,
            transform.localScale.z * initialScale,
            1f
        );

        MakeHole2D();
        Make3DMeshCollider();
    }

    private void MakeHole2D()
    {
        Vector2[] pointPositions = hole2DCollider.GetPath(0);

        for (int i = 0; i < pointPositions.Length; i++)
        {
            pointPositions[i] = hole2DCollider.transform.TransformPoint(pointPositions[i]);
        }

        ground2DCollider.pathCount = 2;
        ground2DCollider.SetPath(1, pointPositions);
    }

    private void Make3DMeshCollider()
    {
        if (generatedMesh != null)
        {
            Destroy(generatedMesh);
        }

        generatedMesh = ground2DCollider.CreateMesh(true, true);

        generatedMeshCollider.sharedMesh = null;
        generatedMeshCollider.sharedMesh = generatedMesh;
    }

    private void IgnoreObstacleCollisionsWithGeneratedMesh(bool ignoreValue)
    {
        int obstacleLayer = LayerMask.NameToLayer("Obstacles");
        Collider[] allColliders = FindObjectsByType<Collider>(FindObjectsSortMode.None);

        foreach (Collider col in allColliders)
        {
            if (col.gameObject.layer == obstacleLayer)
            {
                Physics.IgnoreCollision(col, generatedMeshCollider, ignoreValue);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Obstacles"))
            return;

        Physics.IgnoreCollision(other, groundCollider, true);
        Physics.IgnoreCollision(other, generatedMeshCollider, false);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Obstacles"))
            return;

        Physics.IgnoreCollision(other, groundCollider, false);
        Physics.IgnoreCollision(other, generatedMeshCollider, true);
    }

    public IEnumerator ScaleHole()
    {
        if (isScaling)
            yield break;

        isScaling = true;

        Vector3 startScale = transform.localScale;
        Vector3 endScale = startScale * 2f;

        float t = 0f;

        while (t < 0.4f)
        {
            t += Time.deltaTime;

            float lerpValue = t / 0.4f;
            transform.localScale = Vector3.Lerp(startScale, endScale, lerpValue);

            RebuildHoleSystem();

            yield return null;
        }

        transform.localScale = endScale;
        RebuildHoleSystem();

        isScaling = false;
    }

    private void OnDestroy()
    {
        if (generatedMesh != null)
        {
            Destroy(generatedMesh);
        }
    }
}