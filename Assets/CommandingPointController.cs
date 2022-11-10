using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandingPointController : MonoBehaviour
{
    public GameObject MovingObject;
    public GameObject BulletPrefab;
    int _commandingPoint;
    public Vector3 DefaulBulletPos = new Vector3(-201, -7, 0);
    public Vector3 TargetMovingPos = new Vector3(-15, -15.2f, 0);
    public Vector3 DefaulMovingPos = new Vector3(240, -15.2f, 0);
    public Vector3 DefaulBulletMovingPos = new Vector3(-186, 8.2f, 0);
    CoroutineQueue _queue;
    List<GameObject> _cP = new List<GameObject>();
    public int CommandingPoint
    {
        get => _commandingPoint;
        set
        {
            if(_queue == null)
                _queue = new CoroutineQueue(1, StartCoroutine);
            if (_commandingPoint == value) return;
            if (value > CommandingPoint)
                _queue.Run(CPAdd(value - _commandingPoint));
            else
                _queue.Run(CPRemove(value - _commandingPoint));
            _commandingPoint = value;
        }
    }
    float EaseOutQuint(float t) => 1 - Mathf.Pow(1 - t, 5);
    float EaseInQuint(float t) => t * t * t * t * t;
    IEnumerator CPRemove(int difference)
    {
        difference *= -1;
        var cpCount = _cP.Count;
        var startTime = 0.0f;
        var tempPos = DefaulBulletMovingPos;
        var startPos = DefaulMovingPos;
        tempPos = TargetMovingPos;
        tempPos.x += (cpCount - 1) * 26;
        while (startTime < .3f)
        {
            MovingObject.transform.localPosition = Vector3.Lerp(startPos, tempPos, EaseOutQuint(startTime / .3f));
            startTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        MovingObject.transform.localPosition = tempPos;
        startPos = tempPos;
        for (int i = cpCount - 1; i > cpCount - difference-1; i--)
        {
            _cP[i].transform.SetParent(MovingObject.transform, true);
        }
        tempPos = DefaulMovingPos;
        tempPos.x += difference * 26f;
        startTime = 0.0f;
        yield return new WaitForSeconds(.1f);
        while (startTime < .3f)
        {
            MovingObject.transform.localPosition = Vector3.Lerp(startPos, tempPos, EaseInQuint(startTime / .3f));
            startTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        for (int i = cpCount - 1; i > cpCount - difference - 1; i--)
        {
            Destroy(_cP[i].gameObject);
            _cP.RemoveAt(i);
        }
        MovingObject.transform.localPosition = DefaulMovingPos;
    }

    IEnumerator CPAdd(int difference)
    {
        var startTime = 0.0f;
        var tempPos = DefaulBulletMovingPos;
        for (int i = 0; i < difference; i++)
        {
            tempPos = DefaulBulletMovingPos;
            tempPos.x -= (difference-1 - i) * 26;
            var go = Instantiate(BulletPrefab, Vector3.zero, Quaternion.identity, MovingObject.transform);
            go.transform.localPosition = tempPos;
            go.GetComponent<SpriteRenderer>().sortingOrder = 15 - _cP.Count;
            _cP.Add(go);
        }
        var startPos = MovingObject.transform.localPosition;
        tempPos = TargetMovingPos;
        tempPos.x += (_cP.Count-1) * 26;
        while (startTime < .3f)
        {
            MovingObject.transform.localPosition = Vector3.Lerp(startPos, tempPos, EaseOutQuint(startTime / .3f));
            startTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        MovingObject.transform.localPosition = tempPos;
        startPos = tempPos;
        for (int i = 0; i < _cP.Count; i++)
        {
            _cP[i].transform.SetParent(transform, true);
        }
        startTime = 0.0f;
        yield return new WaitForSeconds(.1f);
        while (startTime < .3f)
        {
            MovingObject.transform.localPosition = Vector3.Lerp(startPos, DefaulMovingPos, EaseInQuint(startTime / .3f));
            startTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        MovingObject.transform.localPosition = DefaulMovingPos;
    }
}