using UnityEngine;

public class CubeSpawner : MonoBehaviour
{
    public GameObject cubePrefab;

    public int amount = 20;
    public float spacing = 1.1f;

    public int gridSizeX = 5;
    public int gridSizeZ = 5;

    public bool stackUpwards = false;

    private void Start()
    {
        SpawnCubes();
    }

    public void SpawnCubes()
    {
        int count = 0;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int z = 0; z < gridSizeZ; z++)
            {
                for (int y = 0; y < (stackUpwards ? amount : 1); y++)
                {
                    if (!stackUpwards && count >= amount)
                        return;

                    Vector3 pos = transform.position + new Vector3(
                        x * spacing,
                        y * spacing,
                        z * spacing
                    );

                    Instantiate(cubePrefab, pos, Quaternion.identity);

                    count++;
                }
            }
        }
    }
}