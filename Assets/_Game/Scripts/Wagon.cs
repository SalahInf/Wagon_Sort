using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using DG.Tweening;

public class Wagon : MonoBehaviour
{
    public WagonStatistique wagonStat;
    public bool isWagonHead;
    public bool isWagonTail;
    public bool isSelected;
    float roationTime = 0.5f;
    public Cell currentCell;
    public Cell previousCell;
    public bool Moving { get; private set; } = false;
    Coroutine moveCoroutine;

    public Transform _dirTransform;

    public MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        wagonStat.wagonCapacity = wagonStat.chairsPoints.Count;
    }
    public void MoveWagen(Cell targetCell)
    {
        Vector2 dir = currentCell.GetNeighborDirection(targetCell);
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(RotateWagon(dir));
        transform.DOKill();
        transform.DOMove(targetCell.transform.position, 0.1f).SetEase(Ease.Linear);
        previousCell = currentCell;
        previousCell.isFull = false;
        currentCell = targetCell;
        currentCell.isFull = true;
    }

    IEnumerator RotateWagon(Vector3 Target)
    {
        float t = 0;
        Target = new Vector3(Target.x, 0, Target.y);

        if (isWagonTail)
        {
            Target *= -1;
        }

        while (t < roationTime)
        {
            t += Time.deltaTime;
            transform.forward = Vector3.Lerp(transform.forward, Target, t / roationTime);
            yield return null;
        }
    }
    
}


[Serializable]
public struct WagonStatistique
{
    public int wagonCurentCapacity;
    public int wagonColorIndex;
    public int wagonCapacity;
    public List<Transform> chairsPoints;
}