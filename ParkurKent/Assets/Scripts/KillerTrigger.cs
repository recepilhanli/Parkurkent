using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillerTrigger : MonoBehaviour
{

    void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        Pause.Instance.KillPlayer();
    }

}
