using DualQuaternions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Sclerp : MonoBehaviour
{
    public List<Transform> Waypoints;
    public List<float> times;

    [SerializeField]
    private float param, adjParam;
    [SerializeField]
    private int paramInd, paramNext;
    

    private readonly List<DualQuaternion> dualQuaternions = new List<DualQuaternion>();

    void Start()
    {
        transform.position = Waypoints[0].position;
        transform.rotation = Waypoints[0].rotation;

        foreach (var tf in Waypoints)
        {
            var dq = new DualQuaternion(tf.position, tf.rotation);
            dualQuaternions.Add(dq);
        }
    }

    void Update()
    {
        float elapsed = Time.time;
        param = elapsed % times.Last();

        if (param < times[0])
        {
            paramInd = 0;
            paramNext = 1;
            adjParam = param / times[0];
            
        }
        else
        {
            paramInd = times.BinarySearch(param);
            paramInd = paramInd < 0 ? ~paramInd : paramInd;
            paramNext = paramInd + 1;
            adjParam = (param - times[paramInd - 1]) / (times[paramInd] - times[paramInd - 1]);
        }

        var res = dualQuaternions[paramInd].Sclerp(dualQuaternions[paramNext], adjParam);
        transform.position = res.Translation;
        transform.rotation = res.Real;
    }
}
