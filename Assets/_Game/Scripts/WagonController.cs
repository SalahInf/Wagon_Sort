using DG.Tweening;
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
    public Material _tmpmat;
    #region Cardinal Dir
    private readonly Vector2 UP = new Vector2(0, 1);
    private readonly Vector2 LEFT = new Vector2(-1, 0);
    private readonly Vector2 DOWN = new Vector2(0, -1);
    private readonly Vector2 RIGHT = new Vector2(1, 0);
    #endregion

    private void Update()
    {
        if (!GameManager.Instance.lose && !GameManager.Instance.win)
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


                    foreach (var item in holder.wagonList)
                    {
                        item.transform.DOScale(1.1f, 0.1f).SetEase(Ease.InBounce);
                        Material newMaterial = new Material(item.meshRenderer.material);
                        Material[] mats = item.meshRenderer.materials;
                        //_tmpmat = mats[1];
                        mats[mats.Length - 1] = newMaterial;
                        mats[mats.Length - 1].DOColor(mats[0].color, 0.05f);
                        item.meshRenderer.materials = mats;

                    }

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
            if (holder == null)
                return;

            foreach (var item in holder.wagonList)
            {
                item.transform.DOScale(1f, 0.1f).SetEase(Ease.InBounce);
                Material[] mats = item.meshRenderer.materials;
                mats[mats.Length - 1] = _tmpmat;
                mats[mats.Length - 1].DOColor(_tmpmat.color, 0.05f);
                item.meshRenderer.materials = mats;
            }
            isDragging = false;
            wagon = null;
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


