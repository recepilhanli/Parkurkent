using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunableWall : MonoBehaviour
{

    void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;

        Player.Instance.WallRunning(gameObject.transform.lossyScale.x);

    }
}
