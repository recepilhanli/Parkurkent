using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activer : MonoBehaviour
{

    [SerializeField] GameObject ToBeActivetedObject;
    void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        ToBeActivetedObject.SetActive(true);
        Destroy(gameObject);
    }
}