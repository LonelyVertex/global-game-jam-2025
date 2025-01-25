using System;
using UnityEngine;

public class DuckSpawner : MonoBehaviour
{
    public int PlayerIndex;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(gameObject.transform.position, 0.5f);
        Gizmos.DrawLine(gameObject.transform.position, gameObject.transform.position + gameObject.transform.up);
    }
}