using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Matinator : MonoBehaviour
{
    /* Curves */
    private AnimationCurve[] position;
    private List<Key> eventList;

    /* Time */
    private float timeBegin;
    private bool timeStart;
    private float time;

    void Start()
    {
        timeStart = true;
    }

    public void SetParameters()
    {
        timeStart = true;
        timeBegin = 0;
        time = 0;
        position = new AnimationCurve[9];
    }

    public void SetEvent(List<Key> _eventList)
    {
        eventList = _eventList;
    }

    public void SetCurves(AnimationCurve curve, int type)
    {
        position[type] = curve;
    }

    public void StartTime()
    {
        this.transform.position = new Vector3(position[0].Evaluate(0.0f), position[1].Evaluate(0.0f), position[2].Evaluate(0.0f));
        this.transform.rotation = Quaternion.Euler(new Vector3(position[3].Evaluate(0.0f), position[4].Evaluate(0.0f), position[5].Evaluate(0.0f)));
        this.transform.localScale = new Vector3(position[6].Evaluate(0.0f), position[7].Evaluate(0.0f), position[8].Evaluate(0.0f));
        timeStart = true;
        timeBegin = 0;
        time = 0;
        foreach (Key k in eventList)
            k.eventTrigger = false;
    }

    public void StopTime()
    {
        timeStart = false;
        timeBegin = 0;
        time = 0;
        this.transform.position = new Vector3(position[0].Evaluate(0.0f), position[1].Evaluate(0.0f), position[2].Evaluate(0.0f));
        this.transform.rotation = Quaternion.Euler(new Vector3(position[3].Evaluate(0.0f), position[4].Evaluate(0.0f), position[5].Evaluate(0.0f)));
        this.transform.localScale = new Vector3(position[6].Evaluate(0.0f), position[7].Evaluate(0.0f), position[8].Evaluate(0.0f));
        foreach (Key k in eventList)
            k.eventTrigger = false;
    }

    public void Update()
    {
        if (timeStart)
        {
         //   float time = Time.time - timeBegin;
         //   Debug.Log("Time Begin = " + timeBegin + "Time current = " + Time.time + "Time = " + time);

            transform.position = fillVector(transform.position, 0);
            transform.rotation = Quaternion.Euler(fillVector(transform.rotation.eulerAngles, 3));
            transform.localScale = fillVector(transform.localScale, 6);
            CheckEvent();
            time += Time.deltaTime;
        }
    }

    private Vector3 fillVector(Vector3 parameter, int id)
    {
        Vector3 pos = new Vector3(parameter.x, parameter.y, parameter.z);
        if (position[id] != null)
            pos.x = position[id].Evaluate(time);
        if (position[id + 1] != null)
            pos.y = position[id + 1].Evaluate(time);
        if (position[id + 2] != null)
            pos.z = position[id + 2].Evaluate(time);
        return pos;
    }

    private void CheckEvent()
    {
        if (eventList == null)
            return;
        foreach (Key k in eventList)
        {
            if (time > k.time && k.eventTrigger == false && k.function != null)
            {
                k.function.Invoke(null, k.parameterArray);
                k.eventTrigger = true;
            }
        }
    }
}
