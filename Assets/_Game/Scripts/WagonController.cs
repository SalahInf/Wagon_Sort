//using DG.Tweening;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Color = UnityEngine.Color;

//public class WagonController : MonoBehaviour
//{
//    public List<WagonHolder> wagonHolder;
//    [SerializeField] LayerMask _layer;
//    float sizeCell = 1;
//    Camera _mainCam => Camera.main;
//    Wagon wagon;
//    public Grid grid;
//    RaycastHit mouseHit;

//    public bool isDragging = false;

//    [SerializeField] Cell targetCell;
//    WagonHolder holder;
//    private void Update()
//    {
//        Fetch();
//    }
//    void Fetch()
//    {
//        Physics.Raycast(_mainCam.ScreenPointToRay(Input.mousePosition), out mouseHit, 100, _layer);
//        if (Input.GetMouseButtonDown(0))
//        {
//            if (mouseHit.collider != null && !isDragging)
//            {
//                holder = mouseHit.collider.GetComponentInParent<WagonHolder>();

//                if (holder)
//                {
//                    float distance1 = Vector3.Distance(holder.wagonList[0].transform.position, mouseHit.point);
//                    float distance2 = Vector3.Distance(holder.wagonList[holder.wagonList.Count - 1].transform.position, mouseHit.point);

//                    wagon = distance1 < distance2 ? holder.wagonList[0] : holder.wagonList[holder.wagonList.Count - 1];

//                    isDragging = true;
//                    print(wagon.name);

//                    StartCoroutine(FetchNewPosition());

//                }
//            }
//        }

//        if (Input.GetMouseButtonUp(0))
//        {
//            //CheckDirection();
//            RestWagonChangeHead();
//            wagon = null;
//            isDragging = false;
//            if (_road != null)
//                _road.Clear();




//        }

//    }

//    public List<Cell> _road = new List<Cell>();
//    IEnumerator FetchNewPosition()
//    {
//        while (Input.GetMouseButton(0))
//        {

//            float distance1 = Vector3.Distance(wagon.transform.position, mouseHit.point);

//            if (distance1 > 1.5f)
//            {
//                if (wagon != null)
//                {
//                    Vector3 point = mouseHit.point;

//                    bool isReversed = WagonChangeHead(point, wagon);

//                    for (int i = 0; i < holder.wagonList.Count; i++)
//                    {
//                        Vector3 dir = Vector3.zero;
//                        Cell t = CheckDirection(point, holder.wagonList[i].currentCell, holder.wagonList[i], isReversed);
//                        if (t != null)
//                        {

//                            if (holder.wagonList[i].isWagonHead || holder.wagonList[i].isWagonHead && holder.wagonList[i].isWagonTail/*&& holder.wagonList[i].isSelected*/)
//                            {
//                                if (!t.isFull)
//                                {

//                                    holder.wagonList[i].MoveWagen(t);
//                                    isReversed = false;
//                                }
//                            }
//                            else
//                            {
//                                if (!holder.wagonList[i - 1].previousCell.isFull)
//                                    holder.wagonList[i].MoveWagen(holder.wagonList[i - 1].previousCell);
//                            }
//                        }
//                        //  yield return null;

//                        //holder.wagonList[i].MoveWagen(point);
//                    }

//                }


//                //CheckDirection();
//            }
//            yield return new WaitForSeconds(0.1f);
//            //yield return null;


//        }
//    }

//    void CheckWagonIsOnTrack(Vector3 point)
//    {
//        for (int i = 1; i < holder.wagonList.Count; i++)
//        {

//            if (holder.wagonList[i].currentCell != holder.wagonList[i - 1].previousCell)
//            {
//                //point = holder.wagonList[i - 1].previousCell.transform.position;
//                holder.wagonList[i].MoveWagen(holder.wagonList[i - 1].previousCell);
//            }
//        }
//    }

//    bool WagonChangeHead(Vector3 point, Wagon currentWagon)
//    {

//        Cell targetCell = CheckDirection(point, currentWagon.currentCell, currentWagon);

//        if (targetCell == holder.wagonList[1].currentCell)
//        {
//            currentWagon.isWagonHead = false;
//            holder.wagonList.Reverse();
//            holder.wagonList[0].isWagonHead = true;
//            wagon = holder.wagonList[0];
//            return true;
//        }
//        return false;
//    }


//    void RestWagonChangeHead()
//    {
//        if (!holder.wagonList[0].isWagonTail)
//            return;

//        holder.wagonList[0].isWagonHead = false;
//        holder.wagonList.Reverse();
//        holder.wagonList[0].isWagonHead = true;
//        wagon = holder.wagonList[0];
//    }

//    #region Chech MovmentPosibility

//    private readonly Vector2 UP = new Vector2(0, 1);
//    private readonly Vector2 LEFT = new Vector2(-1, 0);
//    private readonly Vector2 DOWN = new Vector2(0, -1);
//    private readonly Vector2 RIGHT = new Vector2(1, 0);

//    private Vector2 GetCardinalDirectionVector(Vector2 direction)
//    {
//        direction.Normalize();

//        float angleUp = Vector2.Angle(direction, UP);
//        float angleLeft = Vector2.Angle(direction, LEFT);
//        float angleDown = Vector2.Angle(direction, DOWN);
//        float angleRight = Vector2.Angle(direction, RIGHT);

//        float minAngle = Mathf.Min(angleUp, angleLeft, angleDown, angleRight);

//        if (minAngle == angleUp)
//            return UP;
//        else if (minAngle == angleLeft)
//            return LEFT;
//        else if (minAngle == angleDown)
//            return DOWN;
//        else if (minAngle == angleRight)
//            return RIGHT;

//        return Vector2.zero;
//    }

//    Cell CheckDirection(Vector3 disertPosition, Cell currentCell, Wagon currentWagon, bool isReversed = false)
//    {
//        Vector2 adjustMouseHit = new Vector2(disertPosition.x, disertPosition.z);
//        Vector2 adjustWagonpos = new Vector2(currentWagon.transform.position.x, currentWagon.transform.position.z);
//        Vector2 dir = adjustMouseHit - adjustWagonpos;
//        Vector2 validDir = GetCardinalDirectionVector(dir);
//        int multiplayer = isReversed ? -1 : 1;
//        int x = Mathf.RoundToInt(validDir.x) * multiplayer;
//        int y = Mathf.RoundToInt(validDir.y) * multiplayer;

//        (int, int) index = Grid.Instance.GetBlockFromPosition(new Vector3(currentCell.transform.position.x + x, currentCell.transform.position.y, currentCell.transform.position.z + y));
//        print(validDir);
//        bool notValid = dir.x == 0 && dir.y == 0;
//        if (index.Item1 != -1 && !notValid)
//        {

//            Cell targetCell = currentCell.neighbors[(x, y)];

//            return targetCell;

//        }
//        return null;
//    }


//    #endregion


//    private void OnDrawGizmos()
//    {
//        Gizmos.color = Color.green;
//        Gizmos.DrawRay(_mainCam.ScreenToWorldPoint(Input.mousePosition), Vector3.down * 1000);
//        Gizmos.color = Color.red;

//        Gizmos.DrawSphere(mouseHit.point, 0.3f);
//    }


//}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class WagonMovementController : MonoBehaviour
{
    public List<WagonHolder> wagonHolder;
    [SerializeField] LayerMask _layer;
    Camera _mainCam => Camera.main;
    Wagon wagon;
    RaycastHit mouseHit;

    public bool isDragging = false;

    [SerializeField] Cell targetCell;
    WagonHolder holder;

    #region Cardinal Dir
    private readonly Vector2 UP = new Vector2(0, 1);
    private readonly Vector2 LEFT = new Vector2(-1, 0);
    private readonly Vector2 DOWN = new Vector2(0, -1);
    private readonly Vector2 RIGHT = new Vector2(1, 0);
    #endregion

    private void Update()
    {
        if (!GameManager.Instance.lose && !GameManager.Instance.win )
            Fetch();
    }

    void Fetch()
    {
        Physics.Raycast(_mainCam.ScreenPointToRay(Input.mousePosition), out mouseHit, 100, _layer);
        if (Input.GetMouseButtonDown(0))
        {
            if (mouseHit.collider != null && !isDragging)
            {
                holder = mouseHit.collider.GetComponentInParent<WagonHolder>();

                if (holder && holder.isActive)
                {
                    float distance1 = Vector3.Distance(holder.wagonList[0].transform.position, mouseHit.point);
                    float distance2 = Vector3.Distance(holder.wagonList[holder.wagonList.Count - 1].transform.position, mouseHit.point);

                    if (distance1 < distance2)
                    {

                        wagon = holder.wagonList[0];

                        if (!wagon.isWagonHead)
                        {
                            ReverseWagonsOrder(holder);
                        }
                    }
                    else
                    {

                        wagon = holder.wagonList[holder.wagonList.Count - 1];

                        if (!wagon.isWagonHead)
                        {
                            ReverseWagonsOrder(holder);
                        }
                    }

                    isDragging = true;

                    StartCoroutine(FetchNewPosition());
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            wagon = null;
            if (_road != null)
                _road.Clear();
        }
    }

    void ReverseWagonsOrder(WagonHolder wagesHolder)
    {
        wagesHolder.wagonList.Reverse();
        for (int i = 0; i < wagesHolder.wagonList.Count; i++)
        {
            wagesHolder.wagonList[i].isWagonHead = (i == 0);
            wagesHolder.wagonList[i].isWagonTail = (i == wagesHolder.wagonList.Count - 1);
        }
        wagon = wagesHolder.wagonList[0];
    }

    public List<Cell> _road = new List<Cell>();

    bool isReversed = false;
    IEnumerator FetchNewPosition()
    {
        while (Input.GetMouseButton(0))
        {

            if (wagon == null || !holder.isActive)
                break;


            float distance1 = Vector3.Distance(wagon.transform.position, mouseHit.point);

            if (distance1 > 1f)
            {
                if (wagon != null)
                {
                    Vector3 point = mouseHit.point;
                    Cell nextCell = CheckDirection(point, wagon.currentCell, wagon);

                    if (nextCell != null)
                    {
                        if (nextCell == holder.wagonList[1].currentCell && !isReversed)
                        {
                            Cell c = CheckDirection(point, holder.wagonList[1].currentCell, wagon);
                            if (c != null) isReversed = !c.isFull;
                            if (isReversed)
                            {
                                wagon.MoveWagen(c);

                            }
                            else
                            {
                                ReverseWagonsOrder(holder);
                                nextCell = CheckDirection(point, wagon.currentCell, wagon, true);
                            }
                        }


                        if (nextCell != null && !isReversed)
                            if (!nextCell.isFull)
                            {
                                Cell headPreviousCell = wagon.currentCell;

                                wagon.MoveWagen(nextCell);

                                for (int i = 1; i < holder.wagonList.Count; i++)
                                {
                                    Cell wagonPreviousCell = holder.wagonList[i].currentCell;
                                    holder.wagonList[i].MoveWagen(headPreviousCell);
                                    headPreviousCell = wagonPreviousCell;

                                    //yield return null;
                                }
                            }
                    }
                }
            }
            yield return new WaitForSeconds(0.1f);
            isReversed = false;
        }

    }
    private Vector2 GetDirection(Vector2 direction)
    {
        if (direction.magnitude < 0.1f)
            return Vector2.zero;

        direction.Normalize();

        float angleUp = Vector2.Angle(direction, UP);
        float angleLeft = Vector2.Angle(direction, LEFT);
        float angleDown = Vector2.Angle(direction, DOWN);
        float angleRight = Vector2.Angle(direction, RIGHT);

        float minAngle = Mathf.Min(angleUp, angleLeft, angleDown, angleRight);

        if (minAngle == angleUp && !holder.isVertical)
            return UP;
        else if (minAngle == angleLeft && !holder.isHorizontal)
            return LEFT;
        else if (minAngle == angleDown && !holder.isVertical)
            return DOWN;
        else if (minAngle == angleRight && !holder.isHorizontal)
            return RIGHT;


        return Vector2.zero;
    }
    Cell CheckDirection(Vector3 desiredPosition, Cell currentCell, Wagon currentWagon, bool isreversed = false)
    {
        Vector2 adjustMouseHit = new Vector2(desiredPosition.x, desiredPosition.z);
        Vector2 adjustWagonpos = new Vector2(wagon.transform.position.x, wagon._dirTransform.transform.position.z);

        Vector2 dir = adjustMouseHit - adjustWagonpos;
        Vector2 validDir = GetDirection(dir);

        if (validDir == Vector2.zero)
            return null;

        int multiplayer = isreversed ? -1 : 1;

        int x = Mathf.RoundToInt(validDir.x) * multiplayer;
        int y = Mathf.RoundToInt(validDir.y) * multiplayer;

        (int, int) index = Grid.instance.GetBlockFromPosition(new Vector3(
            currentCell.transform.position.x + x,
            currentCell.transform.position.y,
            currentCell.transform.position.z + y)
            );

        if (index.Item1 != -1 && currentCell.neighbors.ContainsKey((x, y)))
        {
            Cell targetCell = currentCell.neighbors[(x, y)];
            return targetCell;
        }

        return null;
    }
}


