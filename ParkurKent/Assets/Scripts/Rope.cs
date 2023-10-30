

using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Rope : MonoBehaviour
{
    [SerializeField, Tooltip("Player will attach this body.")]
    Rigidbody[] body;

    byte attachedIndex = 0;

    public bool isPlayerHoldingRope = false;

    public float cooldown = 0f;

    void Update()
    {
        if (!isPlayerHoldingRope || Input.GetAxis("Horizontal") == 0) return;


        float x = Input.GetAxis("Horizontal");

        for (int i = 0; i < body.Length; i++)
        {
            if (i == attachedIndex) continue;
            if (body[i].velocity.magnitude > 100) body[attachedIndex].velocity = Vector3.ClampMagnitude(body[attachedIndex].velocity, 5);
            body[i].AddForce(100 * Time.deltaTime * x, 0, 0);
        }
        body[attachedIndex].AddForce(10000 * Time.deltaTime * x, 0, 0);


        int aimedIndex = attachedIndex - 1;

        if (aimedIndex >= 0)
        {

            Player.Instance.Mesh.transform.LookAt(body[aimedIndex].transform);
            Vector3 neweuler = new Vector3(Player.Instance.Mesh.transform.eulerAngles.x + 90, Player.Instance.Mesh.transform.eulerAngles.y, Player.Instance.Mesh.transform.eulerAngles.z);
            Player.Instance.Mesh.transform.eulerAngles = neweuler;

        }

        if (body[attachedIndex].velocity.magnitude > 100) body[attachedIndex].velocity = Vector3.ClampMagnitude(body[attachedIndex].velocity, 10);
        Player.Instance.gameObject.transform.position = body[attachedIndex].gameObject.transform.position + Vector3.down;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            isPlayerHoldingRope = false;

            Vector3 tempVector = Player.Instance.gameObject.transform.localPosition;
            tempVector.z = 0;
            Player.Instance.gameObject.transform.localPosition = tempVector;

            Player.Instance.transform.SetParent(null);
            Player.Instance.CineMachineCamera.SetActive(true);
            Player.Instance.EnabledGravity = true;
            Player.Instance.EnabledMovement = true;
            Player.Instance.animator.SetBool("isSwinging", false);

            Player.Instance.gameObject.transform.eulerAngles = new Vector3(0, 90, 0);

            Rigidbody selectedBody = body[attachedIndex];

            Vector3 toPos = new Vector3(x * selectedBody.velocity.magnitude / 2.25f, 0.5f, 0) * 2;
            Player.Instance.RopeSwinging(toPos);

            cooldown = Time.time + 0.75f;



            Debug.Log("Player deattached from the rope.");
        }

    }


    public void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player") || !Player.Instance.EnabledMovement) return;

        attachedIndex = FindNearestBody();

        Debug.Log($"Player attached to the rope. {attachedIndex}");

        Player.Instance.EnabledGravity = false;

        Player.Instance.EnabledMovement = false;

        other.gameObject.transform.position = body[attachedIndex].gameObject.transform.position + Vector3.down;

        other.gameObject.transform.SetParent(body[attachedIndex].gameObject.transform, true);

        //Player.Instance.CineMachineCamera.SetActive(false);

        isPlayerHoldingRope = true;

        body[attachedIndex].velocity = Vector2.zero;
        body[attachedIndex].AddForce(500000 * Time.deltaTime * Input.GetAxis("Horizontal"), 0, 0);

        Player.Instance.animator.SetBool("isSwinging", true);
    }


    byte FindNearestBody()
    {

        Vector3 playerPos = Player.Instance.transform.position;

        float TempDist = 100;
        byte index = 0;
        for (byte i = 0; i < body.Length; i++)
        {
            Vector3 bodyPos = body[i].transform.position;
            float dist = Vector3.Distance(playerPos, bodyPos);
            if (dist < TempDist)
            {
                TempDist = dist;
                index = i;
            }

        }


        return index;
    }


}

