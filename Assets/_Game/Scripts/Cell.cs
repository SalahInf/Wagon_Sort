using System;
using System.Collections.Generic;
using UnityEngine;


public class Cell : MonoBehaviour
{
    [HideInInspector] public int xIndex, yIndex;
    public List<Vector3> corners;
    [HideInInspector] public int indexCell;

    [Header("Bool Verification")]
    public bool isFull = false;
    public bool isStation = false;
    public bool isExit = false;
    public bool isCorner = false;

    public CellData cellData;

    private readonly List<Vector2> Dirs = new List<Vector2>() {
            new Vector2(0, 1), new Vector2(-1, 0), new Vector2(0, -1), new Vector2(1, 0),
            new Vector2(1, 1), new Vector2(1, -1), new Vector2(-1, -1), new Vector2(-1, 1)
        };


    public Dictionary<(int, int), Cell> neighbors;

    public void SetCellNeibors()
    {

        neighbors = new Dictionary<(int, int), Cell>();

        foreach (Vector2 dir in Dirs)
        {
            (int, int) index = Grid.instance.GetBlockFromPosition(new Vector3(transform.position.x + dir.x, transform.position.y, transform.position.z + dir.y));
            if (index.Item1 != -1)
            {
                neighbors.Add((Mathf.RoundToInt(dir.x), Mathf.RoundToInt(dir.y)), Grid.instance.gridCells[index.Item1, index.Item2]);
            }
        }

    }
    public void InitCell()
    {
        isFull = false;
        isStation = false;
        isExit = false;
        isCorner = false;
        corners = new List<Vector3>();
    }

    public void SetCell(int x, int y)
    {
        xIndex = x;
        yIndex = y;
    }

    public void SetAdjustSells(Vector3 dir)
    {
        corners.Add(dir);
    }

    public Vector2 GetNeighborDirection(Cell targetCell)
    {
        foreach (var item in neighbors)
        {
            if (item.Value == targetCell)
            {
                return new Vector2(item.Key.Item1, item.Key.Item2);
            }
        }
        return Vector2.zero;
    }
}

[Serializable]
public struct CellData
{
    // Walls
    public GameObject upWall;
    public GameObject rightWall;
    public GameObject leftWall;
    public GameObject downWall;
    // Corners
    public GameObject cornerUp;
    public GameObject cornerRight;
    public GameObject cornerLeft;
    public GameObject cornerDown;
    // Other blocs
    public GameObject block;
}
