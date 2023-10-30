
using UnityEngine;

public class RopeBody : MonoBehaviour
{
    [SerializeField]
    Rope rope;

    void OnTriggerEnter(Collider other)
    {
        if(rope.isPlayerHoldingRope) return;
        if(rope.cooldown > Time.time) return;
        rope.OnTriggerEnter(other);
    }


}
