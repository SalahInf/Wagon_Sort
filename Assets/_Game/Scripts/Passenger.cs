using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Passenger : MonoBehaviour
{
    [SerializeField] Animator _animator;

    public void Jump(Vector3 startPos, Transform endPos, float height, float duration)
    {
        StartCoroutine(JumpRoutine(this.gameObject, startPos, endPos, height, duration));
    }

    private IEnumerator JumpRoutine(GameObject obj, Vector3 startPos, Transform endPos, float height, float duration)
    {
        float time = 0;

        _animator.SetTrigger("Jump");
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            float parabolicHeight = 4 * height * t * (1 - t);
            obj.transform.position = Vector3.Lerp(startPos, endPos.position, t) + Vector3.up * parabolicHeight;

            yield return null;
        }
        _animator.SetTrigger("SitDown");
        obj.transform.position = endPos.position;
    }

}
