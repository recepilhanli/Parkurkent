using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player : MonoBehaviour
{

    [Header("Base")]

    [Tooltip("Character Mesh")]
    public GameObject Mesh;


    [SerializeField, Tooltip("Handle Cinemachine GameObject")]
    public GameObject CineMachineCamera;

    public static Player Instance;



    void Awake()
    {
        Instance = this;
    }

    void Start()
    {

    }

    void Update()
    {

        if(EnabledMovement || transform.parent != null) gameObject.transform.eulerAngles = new Vector3(0,90,0);
        Movement();
        DoParkour();
        Gravity();
    }
}
