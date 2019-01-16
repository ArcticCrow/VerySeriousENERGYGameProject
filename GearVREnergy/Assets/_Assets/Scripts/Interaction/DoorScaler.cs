using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScaler : MonoBehaviour {

    public Transform target;
    public Vector3 maxScale = Vector3.one;
    public Vector3 minScale = Vector3.one;
    public float duration = 1f;

    float time = 0;

    public void Awake()
    {
        if (target == null)
        {
            target = transform;
        }
    }

    public void ScaleUp()
    {
        StopAllCoroutines();
        StartCoroutine(ScaleTo(maxScale));
    }

    public void ScaleDown()
    {
        StopAllCoroutines();
        StartCoroutine(ScaleTo(minScale));
    }

    public IEnumerator ScaleTo(Vector3 scale)
    {
        time = 0;
        while (target.localScale != scale)
        {
            target.localScale = Vector3.Lerp(target.localScale, scale, time / duration);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }
}
