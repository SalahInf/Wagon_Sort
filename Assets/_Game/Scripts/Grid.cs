using MatrixAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;

public class Grid : MonoBehaviour
{
    public PPin2dCellArr gridCells;
    public Int2dArray matrix;

    public float sizeCell = 1;

    List<List<int>> _wagonsToSpown;
    List<List<Cell>> _cellWagon;
    int colums = 10;
    int rows = 10;

    [SerializeField] Cell _cell;
    [SerializeField] GameObject _passenger;
    [SerializeField] List<Passage> _passages;
    [SerializeField] WagonHolder[] wagonHolder;

    [SerializeField] Wagon wagonHead;
    [SerializeField] Wagon wagonMid;
    [SerializeField] Wagon wagonTail;

    public static List<Cell> cellsInit;

    public static Grid instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }

        instance = this;
    }
 
    public void Init()
    {
        if (cellsInit != null)
            foreach (var item in cellsInit)
            {
                Destroy(item.gameObject);
            }

        foreach (var passage in _passages)
        {
            foreach (var passengers in passage.passengers)
            {
                Destroy(passengers.Key);
            }

            Destroy(passage.gameObject);
        }
        _passages.Clear();
        _wagonsToSpown = new List<List<int>>();
        _cellWagon = new List<List<Cell>>();
        cellsInit = new List<Cell>();

    }
    public void SpownLevel()
    {
        for (int i = 0; i < 4; i++)
        {
            _wagonsToSpown.Add(new List<int>());
            _cellWagon.Add(new List<Cell>());
        }

        SpownGrid();
        SetCorners();
        FindNoAdjacentCells();
        SpownWalls();
        SpownWagons();
        SpownPasengers();
        GameManager.Instance.InitCamera();
    }
    void SpownPasengers()
    {
        List<int> passengerColors = new List<int>(GameManager.Instance.passagerColorIndexs);

        ShuffleList(passengerColors);

        int[] distribution = DistributePeople(passengerColors.Count, _passages.Count);

        int currentIndex = 0;

        for (int i = 0; i < _passages.Count; i++)
        {
            _passages[i].passengers = new Dictionary<GameObject, int>();
            for (int j = 0; j < distribution[i]; j++)
            {
                int colorIndex = passengerColors[currentIndex];
                Vector3 pos = _passages[i].transform.position + _passages[i].dirPassage * (j * 0.5f) * -1 + Vector3.up * 0.5f;
                GameObject passenger = Instantiate(_passenger, pos, Quaternion.identity, transform);
                _passages[i].passengers.Add(passenger, colorIndex);
                passenger.transform.forward = _passages[i].dirPassage;
                SetPassengerColor(passenger, colorIndex);
                currentIndex++;

            }
        }

    }
    private int[] DistributePeople(int total, int places)
    {
        int[] result = new int[places];

        int baseAmount = total / places;
        int remainder = total % places;

        for (int i = 0; i < places; i++)
        {
            result[i] = baseAmount;
        }

        for (int i = 0; i < remainder; i++)
        {
            result[i]++;
        }

        return result;
    }
    private void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }
    private void SetPassengerColor(GameObject passenger, int colorIndex)
    {
        Renderer renderer = passenger.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            renderer.material = GameManager.Instance.colorsWagon[colorIndex];
        }
    }
    void SpownGrid()
    {
      
        rows = matrix.rows;
        colums = matrix.cols;
        gridCells = new PPin2dCellArr(colums, rows);

        for (int i = 0; i < colums; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                Cell _tmpCell = Instantiate(_cell, new Vector3(i - (colums - 1f) / 2f, 0, j - (rows - 1f) / 2f), Quaternion.Euler(90, 0, 0), transform);
                gridCells[i, j] = _tmpCell;
                cellsInit.Add(_tmpCell);
                gridCells[i, j] = _tmpCell;
                _tmpCell.SetCell(i, j);
                _tmpCell.name = (i + j).ToString();
                _tmpCell.InitCell();

               
                switch (matrix[i, j])
                {
                    case 1:
                        _tmpCell.cellData.block.SetActive(true);
                        _tmpCell.isFull= true;
                        break;
                    case 3:// perple = 3
                        _wagonsToSpown[0].Add(matrix[i, j]);
                        _cellWagon[0].Add(_tmpCell);
                        break;
                    case 4:// red = 4
                        _wagonsToSpown[1].Add(matrix[i, j]);
                        _cellWagon[1].Add(_tmpCell);
                        break;
                    case 5: // Orange = 5
                        _wagonsToSpown[2].Add(matrix[i, j]);
                        _cellWagon[2].Add(_tmpCell);
                        break;
                    case 6: // Bleu = 6
                        _wagonsToSpown[3].Add(matrix[i, j]);
                        _cellWagon[3].Add(_tmpCell);
                        break;
                }

            }
        }

        foreach (var c in cellsInit)
        {
            c.SetCellNeibors();
        }

    }
    public (int, int) GetBlockFromPosition(Vector3 vector2, float padding = 0.8f)
    {
        float sizeSlotx = (sizeCell * (colums + 1)) / 2;
        float sizeSloty = (sizeCell * (rows + 1)) / 2;

        Vector3 veTLocal = transform.InverseTransformPoint(vector2);
        Vector2 center1 = new Vector2(veTLocal.x, veTLocal.z);

        veTLocal.x += sizeSlotx;
        veTLocal.z += sizeSloty;

        veTLocal.x /= sizeCell;
        veTLocal.z /= sizeCell;

        veTLocal.x -= 1;
        veTLocal.z -= 1;

        int indexX = Mathf.RoundToInt(veTLocal.x);
        int indexY = Mathf.RoundToInt(veTLocal.z);

        if (indexX >= 0 && indexX < colums && indexY >= 0 && indexY < rows)
        {
            Vector3 center2 = new Vector2(sizeCell * (indexX + 1) - sizeSlotx, sizeCell * (indexY + 1) - sizeSloty);

            if (Vector3.Distance(center1, center2) < (sizeCell * padding))
                return (indexX, indexY);
        }
        return (-1, -1);
    }
    void SpownWalls()
    {
        int _index = 0;
        Vector3 _dir = Vector3.zero;
        _passages = new List<Passage>();
        for (int i = 0; i < colums; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                _index++;
                Cell _tmpCell = gridCells[i, j];
                if (_tmpCell.corners.Count > 0)
                {
                    foreach (var obj in _tmpCell.corners)
                    {
                        _dir += obj;

                        if (obj == Vector3.left)
                        {
                            _tmpCell.cellData.leftWall.SetActive(true);
                            if (matrix[i, j] == 2)
                            {
                                _tmpCell.cellData.leftWall.transform.GetChild(0).gameObject.SetActive(true);
                                _passages.Add(_tmpCell.cellData.leftWall.transform.GetChild(0).GetComponent<Passage>());
                                _passages[_passages.Count - 1].dirPassage = Vector3.left * -1;
                            }
                        }
                        if (obj == Vector3.right)
                        {
                            _tmpCell.cellData.rightWall.SetActive(true);
                            if (matrix[i, j] == 2)
                            {
                                _tmpCell.cellData.rightWall.transform.GetChild(0).gameObject.SetActive(matrix[i, j] == 2);
                                _passages.Add(_tmpCell.cellData.rightWall.transform.GetChild(0).GetComponent<Passage>());
                                _passages[_passages.Count - 1].dirPassage = Vector3.right * -1;
                            }

                        }
                        if (obj == Vector3.up)
                        {
                            _tmpCell.cellData.upWall.SetActive(true);
                            if (matrix[i, j] == 2)
                            {
                                _tmpCell.cellData.upWall.transform.GetChild(0).gameObject.SetActive(matrix[i, j] == 2);
                                _passages.Add(_tmpCell.cellData.upWall.transform.GetChild(0).GetComponent<Passage>());
                                _passages[_passages.Count - 1].dirPassage = Vector3.forward * -1;
                            }

                        }
                        if (obj == Vector3.down)
                        {
                            _tmpCell.cellData.downWall.SetActive(true);
                            if (matrix[i, j] == 2)
                            {
                                _tmpCell.cellData.downWall.transform.GetChild(0).gameObject.SetActive(matrix[i, j] == 2);
                                _passages.Add(_tmpCell.cellData.downWall.transform.GetChild(0).GetComponent<Passage>());
                                _passages[_passages.Count - 1].dirPassage = Vector3.forward;
                            }


                        }
                    }

                    Vector3 posCell = _tmpCell.transform.position;

                    if (_tmpCell.isCorner)
                    {
                        if (posCell.x == -(colums - 1f) / 2f && posCell.z == -(rows - 1f) / 2f)
                        {
                            _tmpCell.cellData.cornerLeft.SetActive(true);
                        }
                        if (posCell.x == (colums - 1) - (colums - 1f) / 2f && posCell.z == -(rows - 1f) / 2f)
                        {
                            _tmpCell.cellData.cornerRight.SetActive(true);
                        }
                        if (posCell.z == (rows - 1) - (rows - 1f) / 2f && posCell.x == -(colums - 1f) / 2f)
                        {
                            _tmpCell.cellData.cornerUp.SetActive(true);
                        }
                        if (posCell.z == (rows - 1) - (rows - 1f) / 2f && posCell.x == (colums - 1) - (colums - 1f) / 2f)
                        {
                            _tmpCell.cellData.cornerDown.SetActive(true);
                        }
                    }
                }
            }
        }
    }
    void SpownWagons()
    {
        WagonHolder _tmpWagonHolder = null;
        for (int i = 0; i < _wagonsToSpown.Count; i++)
        {
            foreach (var item in wagonHolder)
            {
                if (item.wagonList.Count == _wagonsToSpown[i].Count)
                {
                    _tmpWagonHolder = Instantiate(item, Vector3.zero, Quaternion.identity, transform);
                    GameManager.Instance.activeWagonHolders.Add(_tmpWagonHolder);
                }
            }

            if (_tmpWagonHolder != null)
                for (int j = 0; j < _tmpWagonHolder.wagonList.Count; j++)
                {
                    _tmpWagonHolder.wagonList[j].transform.position = _cellWagon[i][j].transform.position;
                    init(_tmpWagonHolder.wagonList[j]);
                    Material mat = new Material(GameManager.Instance.colorsWagon[i]);
                    _tmpWagonHolder.wagonList[j].wagonStat.wagonColorIndex = i;
                    _tmpWagonHolder.wagonList[j].meshRenderer.material = mat;
                }
            _tmpWagonHolder = null;
        }

        void init(Wagon wa)
        {
            (int, int) indexWagon = GetBlockFromPosition(wa.transform.position);
            wa.currentCell = gridCells[indexWagon.Item1, indexWagon.Item2];
            wa.transform.position = wa.currentCell.transform.position;
            print(wa.currentCell.name);
            wa.currentCell.isFull = true;
        }

        GameManager.Instance.SetPassengers();
    }
    void FindNoAdjacentCells()
    {
        for (int i = 0; i < colums; i++)
        {
            for (int j = 0; j < rows; j++)
            {

                if (i == 0 || gridCells[i - 1, j] == null)
                    gridCells[i, j].SetAdjustSells(Vector3.left);


                if (i == colums - 1 || gridCells[i + 1, j] == null)
                    gridCells[i, j].SetAdjustSells(Vector3.right);


                if (j == 0 || gridCells[i, j - 1] == null)
                    gridCells[i, j].SetAdjustSells(Vector3.down);


                if (j == rows - 1 || gridCells[i, j + 1] == null)
                    gridCells[i, j].SetAdjustSells(Vector3.up);
            }
        }
    }
    void SetCorners()
    {
        gridCells[0, 0].isCorner = true;
        gridCells[0, rows - 1].isCorner = true;
        gridCells[colums - 1, 0].isCorner = true;
        gridCells[colums - 1, rows - 1].isCorner = true;
    }
}


