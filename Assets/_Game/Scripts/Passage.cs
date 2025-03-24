using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;

public class Passage : MonoBehaviour
{
    public Dictionary<GameObject, int> passengers;
    public Vector3 dirPassage;

    public void UpdatePassengersPosition()
    {
        int index = 0;
        foreach (var item in passengers)
        {
            Vector3 pos = transform.position + dirPassage * (index * 0.5f) * -1 + Vector3.up * .5f;

            item.Key.transform.DOMove(pos, 0.2f);
            index++;
        }
    }
    IEnumerator Refill(Wagon wagon)
    {
        yield return new WaitForSeconds(0.3f);

        List<GameObject> passengersTpGetIn = new List<GameObject>();      
        foreach (var passenger in passengers.ToList()) 
        {
            if (passenger.Value == wagon.wagonStat.wagonColorIndex)
            {
                passengersTpGetIn.Add(passenger.Key);
                passengers.Remove(passenger.Key);
            }
            else
            {
                break;
            }
            yield return null;
        }

        foreach (var passenger in passengersTpGetIn)
        {
            if (wagon != null)
            {
                wagon.GetComponentInParent<WagonHolder>().RefilWagon(passenger.transform);
                yield return new WaitForSeconds(0.1f);
                UpdatePassengersPosition();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Wagon")
        {
            Wagon wagon = other.GetComponent<Wagon>();

            StartCoroutine(Refill(other.GetComponent<Wagon>()));
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Wagon")
        {
            Wagon wagon = other.GetComponent<Wagon>();
            StopCoroutine(Refill(wagon));
        }
    }

}
