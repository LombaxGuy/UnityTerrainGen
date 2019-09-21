using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ClutterPlacer
{
    public static void PlaceClutter(ClutterMap[] clutterMaps, MeshSettings meshSettings, GameObject[][] clutterPrefabs, Transform parent)
    {
        float[,] clutterAlreadyPlaced = new float[clutterMaps[0].values.GetLength(0), clutterMaps[0].values.GetLength(1)];

        for (int x = 0; x < clutterAlreadyPlaced.GetLength(0); x++)
        {
            for (int y = 0; y < clutterAlreadyPlaced.GetLength(1); y++)
            {
                clutterAlreadyPlaced[x, y] = 0;
            }
        }

        for (int i = 0; i < clutterMaps.Length; i++)
        {
            GameObject clutterSubParent = new GameObject("ClutterType" + (i + 1));
            clutterSubParent.transform.parent = parent;

            float worldWidth = (meshSettings.meshWorldSize * meshSettings.worldChunkWidth) / 2;

            int width = clutterMaps[i].values.GetLength(0);
            int height = clutterMaps[i].values.GetLength(1);

            float topLeftX = -worldWidth;
            float topLeftZ = worldWidth;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {

                    if (clutterMaps[i].values[x, y] == 1)
                    {
                        if (clutterAlreadyPlaced[x, y] == 0)
                        {
                            CreateClutterInstance(i, x, y, topLeftX, topLeftZ, 1, clutterPrefabs, clutterSubParent.transform);

                            clutterAlreadyPlaced[x, y] = 1;
                        }
                        else
                        {
                            Vector2[] freeAdjacentSquares = GetFreeAdjacentSquares(clutterAlreadyPlaced, x, y);

                            if (freeAdjacentSquares.Length > 0)
                            {
                                Vector2 position2D = freeAdjacentSquares[Random.Range(0, freeAdjacentSquares.Length)];

                                CreateClutterInstance(i, (int)position2D.x, (int)position2D.y, topLeftX, topLeftZ, 1, clutterPrefabs, clutterSubParent.transform);

                                //Debug.Log("Clutter placed in adjacent square: " + x + ", " + y);

                                clutterAlreadyPlaced[(int)position2D.x, (int)position2D.y] = 1;
                            }
                            else
                            {
                                Debug.Log("No free adjacent squares: " + x + ", " + y);
                            }
                        }
                    }
                }
            }
        }
    }

    private static void CreateClutterInstance(int i, int x, int y, float topLeftX, float topLeftZ, float scale, GameObject[][] clutterPrefabs, Transform parent)
    {
        Quaternion rotation = Quaternion.LookRotation(new Vector3(Random.Range(0, 360), 0, Random.Range(0, 360)), Vector3.up);

        Vector3 position = Vector3.zero;
        RaycastHit hit;
        float randomXOffset = Random.Range(-0.4f, 0.4f);
        float randomYOffset = Random.Range(-0.4f, 0.4f);

        float minY = 2.5f;
        float maxY = 8;

        if (Physics.Raycast(new Vector3((topLeftX + x + randomXOffset) * scale, 100, (topLeftZ - y + randomYOffset) * scale), Vector3.down, out hit, Mathf.Infinity))
        {
            position = hit.point;
        }

        if (position.y > minY && position.y < maxY)
        {
            Object.Instantiate(clutterPrefabs[i][Random.Range(0, clutterPrefabs[i].Length)], position, rotation, parent);
        }
    }

    private static Vector2[] GetFreeAdjacentSquares(float[,] values, int x, int y)
    {
        List<Vector2> posiblePositions = new List<Vector2>();

        for (int i = x - 1; i <= x + 1; i++)
        {
            for (int j = y - 1; j <= y + 1; j++)
            {
                if (i > 0 && j > 0 && i < values.GetLength(0) - 1 && j < values.GetLength(1) - 1 && values[i, j] == 0)
                {
                    posiblePositions.Add(new Vector2(i, j));
                }
            }
        }

        Vector2[] positions = posiblePositions.ToArray();

        return positions;
    }
}
