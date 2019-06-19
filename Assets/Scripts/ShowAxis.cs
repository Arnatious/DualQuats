using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowAxis : MonoBehaviour
{
    public float size = .5f;
    void OnDrawGizmos()
    {
        foreach (Transform tf in transform)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(tf.position, tf.position + tf.right * size);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(tf.position, tf.position + tf.up * size);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(tf.position, tf.position + tf.forward * size);
        }

        int numChildren = transform.childCount;
        Gizmos.color = Color.yellow;
        for (int i = 0; i < numChildren - 1; ++i)
        {

            Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(i + 1).position);
        }
        Gizmos.color = Color.white;
    }
}
