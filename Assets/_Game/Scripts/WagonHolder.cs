using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WagonHolder : MonoBehaviour
{
    public List<Wagon> wagonList;
    public Transform WagolMidel;


    public bool isVertical = false;
    public bool isHorizontal = false;

    [SerializeField] GameObject _arrowSprite;
    [SerializeField] GameObject whole;
    [SerializeField] GameObject VanishEffect;

    public float followDistance = 0.1f;
    public float followSpeed = 25f;

    private bool stopMovement = false;
    public bool isActive = true;

    GameObject tmpwhole;
    int _wagonsCount = 0;
    private void OnEnable()
    {
        Init();
    }

    void Init()
    {
        _arrowSprite.SetActive(isVertical || isHorizontal);
        _wagonsCount = wagonList.Count;
    }

    public void RefilWagon(Transform pasenger)
    {
        for (int i = 0; i < wagonList.Count; i++)
        {
            for (int j = 0; j < wagonList[i].wagonStat.chairsPoints.Count; j++)
            {
                if (wagonList[i].wagonStat.wagonCurentCapacity != wagonList[i].wagonStat.wagonCapacity)
                {
                    Vector3 startpos = pasenger.transform.position;
                    pasenger.GetComponent<Passenger>().Jump(startpos, wagonList[i].wagonStat.chairsPoints[0], 2f, 0.3f);
                    wagonList[i].wagonStat.wagonCurentCapacity++;
                    pasenger.parent = wagonList[i].wagonStat.chairsPoints[0].transform;
                    wagonList[i].wagonStat.chairsPoints.RemoveAt(0);

                    WagonFullVanish();
                    return;

                }
                else
                {
                    break;
                }


            }

        }

    }


    void WagonFullVanish()
    {
        foreach (var item in wagonList)
        {
            if (item.wagonStat.chairsPoints.Count > 0)
            {
                return;
            }
        }

        var emptyCells = wagonList[0].currentCell.neighbors.Where(c => c.Value.isFull == false).ToList();
        wagonList[0].transform.DOJump((emptyCells[0].Value.transform.position + Vector3.down * 2f), 4, 1, 1f);
        tmpwhole = Instantiate(whole, emptyCells[0].Value.transform.position, Quaternion.identity);
        isActive = false;
        StartCoroutine(VanishAnimation(emptyCells[0].Value.transform.position));

    }
   
    IEnumerator VanishAnimation(Vector3 pos)
    {
        while (!stopMovement)
        {
            for (int i = 1; i < wagonList.Count; i++)
            {
                Transform wagon = wagonList[i].transform;

                Vector3 targetPos = wagonList[i - 1].transform.position;

                Vector3 directionToTarget = (wagon.position - targetPos).normalized;
                targetPos += directionToTarget * followDistance;

                wagon.position = Vector3.Lerp(wagon.position, targetPos, followSpeed * Time.deltaTime);

                Vector3 direction = (targetPos - wagon.position).normalized;
                if (direction != Vector3.zero)
                {
                    wagon.rotation = Quaternion.LookRotation(direction);
                }


                if (wagon.position.y <= -0.2f)
                {
                    GameObject e = Instantiate(VanishEffect, pos, Quaternion.identity);
                    Destroy(e, 1f);
                    Destroy(wagonList[i].gameObject, 0.05f);
                    wagonList[i].currentCell.isFull = false;
                    wagonList.RemoveAt(i);
                    if (wagonList.Count == 1)
                    {
                        tmpwhole.transform.DOScale(0, 1f).OnComplete(() =>
                        {
                            wagonList[0].currentCell.isFull = false;
                            Destroy(tmpwhole);
                            Destroy(wagonList[0].gameObject);
                            wagonList.RemoveAt(0);
                            stopMovement = true;
                        });
                    }

                }
            }
            yield return null; 
        }
        GameManager.Instance.Win();
    }
}


