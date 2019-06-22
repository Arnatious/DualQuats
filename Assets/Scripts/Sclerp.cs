using DualQuaternions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Sclerp : MonoBehaviour
{
    public Transform Targets;
    public List<float> TimeStamps;

    [SerializeField]
    private float param, adjParam;
    [SerializeField]
    private int paramInd, paramNext;
    

    private readonly List<DualQuaternion> dualQuaternions = new List<DualQuaternion>();

    void Start()
    {
        transform.position = Targets.position;
        transform.rotation = Targets.rotation;
        dualQuaternions.Add(new DualQuaternion(transform.position, transform.rotation));
        foreach (Transform tf in Targets)
        {
            var dq = new DualQuaternion(tf.position, tf.rotation);
            dualQuaternions.Add(dq);
        }
    }

    void Update()
    {
        float elapsed = Time.time;
        param = elapsed % TimeStamps.Last();

        if (param < TimeStamps[0])
        {
            paramInd = 0;
            paramNext = 1;
            adjParam = param / TimeStamps[0];
            
        }
        else
        {
            paramInd = TimeStamps.BinarySearch(param);
            paramInd = paramInd < 0 ? ~paramInd : paramInd;
            paramNext = paramInd + 1;
            adjParam = (param - TimeStamps[paramInd - 1]) / (TimeStamps[paramInd] - TimeStamps[paramInd - 1]);
        }

        var res = dualQuaternions[paramInd].Sclerp(dualQuaternions[paramNext], adjParam);
        transform.position = res.Translation;
        transform.rotation = res.Real;
    }
}
