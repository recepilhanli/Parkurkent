
using System.Collections;
using UnityEngine;

public partial class Player : MonoBehaviour
{

    [Header("Doing Parkour")]

    public bool EnabledParkour = true;

    public const int NonParkourLayer = 7;


    enum ParkourState
    {
        None,
        Vaulting,
        EdgeClimbing,
        Sliding,
        WallRunning,
        RopeSwinging,
    }

    ParkourState PlayerParkourState = ParkourState.None;

    Vector3 LerpingAxis = Vector3.zero;
    Vector3 LastPos = Vector3.zero;

    float lerpMultiplier = 1f;

    Coroutine StartedCoroutine = null;

    float lastAxis = 0f;

    float SlidingCooldown = 0f;
    GameObject LastSlidedObject = null;
    GameObject DashedObject = null;

    void DoParkour()
    {

        ParkourMovementUpdate();

        if (!EnabledParkour || !EnabledMovement || PlayerParkourState != ParkourState.None
         || Input.GetAxis("Horizontal") == 0 || Mathf.Abs(Mesh.transform.forward.x) < 0.9) return;

        RaycastHit hit;
        if (Physics.Raycast(Mesh.transform.position, Mesh.transform.forward, out hit, 1))
        {

            GameObject hitObject = hit.collider.gameObject;

            if (hitObject.layer == NonParkourLayer) return;

            Vector3 scale = hitObject.transform.lossyScale;

            float space = 0.75f * Input.GetAxis("Horizontal");

            bool grounded = IsGrounded();

            if (scale.x <= 1.3 && scale.y < 1.85f && (animator.GetInteger("MovementState") == 2 || !grounded)) Vault(new Vector3(scale.x * Mesh.transform.forward.x * 3f + space, scale.y, 0));
            else if (!grounded)
            {

                Debug.DrawRay(Mesh.transform.position + (Mesh.transform.up * 1.5f) + Mesh.transform.forward, Vector3.down, Color.red);
                if (Physics.Raycast(Mesh.transform.position + (Mesh.transform.up * 1.5f) + Mesh.transform.forward, Vector3.down, out hit, 5))
                {
                    Vector3 hitpoint = hit.point;

                    hitpoint.y += 0.85f;
                    Collider[] hitcolliders = Physics.OverlapSphere(hitpoint, 0.1f);
                    if (hitcolliders.Length > 0)
                    {
                        Collider[] SlidingHits = Physics.OverlapSphere(Mesh.gameObject.transform.position + Mesh.gameObject.transform.forward * 3, 0.1f);
                        if (SlidingHits.Length > 0) Sliding(SlidingHits[0].gameObject);
                        return; //prevents inside climbing
                    }
                    hitpoint = hit.point; // reset
                    hitpoint.y += 0.85f;
                    hitpoint.x += Input.GetAxis("Horizontal") / 2;
                    hitpoint.z = 0;

                    hit.collider.bounds.Contains(hit.point);
                    float y = hit.point.y;
                    Vector3 pos = Mesh.transform.position;
                    pos.y = y;
                    EnabledMovement = false;
                    EnabledGravity = false;
                    transform.position = pos;
                    Velocity.y = 0;

                    EdgeClimbing(hitpoint);

                }
                else
                {
                    Collider[] SlidingHits = Physics.OverlapSphere(Mesh.gameObject.transform.position + Mesh.gameObject.transform.forward * 3, 0.1f);
                    Sliding(SlidingHits[0].gameObject);
                }

            }



        }


    }


    void ParkourMovementUpdate()
    {
        if (PlayerParkourState == ParkourState.None) return;

        Vector3 lerpPos = LastPos + LerpingAxis;
        if (LerpingAxis != Vector3.zero)
        {
            Vector3 lerpedPos = Vector3.Lerp(gameObject.transform.position, lerpPos, Time.deltaTime * lerpMultiplier);
            controller.Move(-(gameObject.transform.position - lerpedPos));
        }


        switch (PlayerParkourState)
        {

            case ParkourState.Vaulting:
                {


                    if (Mathf.Abs(gameObject.transform.position.x - lerpPos.x) < 1)
                    {
                        if (LerpingAxis != null) StopCoroutine(StartedCoroutine);
                        ResetState();
                        Debug.LogWarning("Parkour movement stopped becase of distance.");
                    }
                    break;
                }

            case ParkourState.RopeSwinging:
                {

                    if (Input.GetAxis("Horizontal") == 0 || (Mathf.Abs(gameObject.transform.position.x - lerpPos.x) < 0.5))
                    {
                        if (StartedCoroutine != null) StopCoroutine(StartedCoroutine);
                        ResetState();
                        Debug.LogWarning("Parkour movement stopped becase of reverse movement or distance.");
                    }
                    break;
                }
            case ParkourState.EdgeClimbing:
                {

                    if ((Mathf.Abs(gameObject.transform.position.x - lerpPos.x) < 0.5))
                    {
                        if (StartedCoroutine != null) StopCoroutine(StartedCoroutine);
                        ResetState();
                        Debug.LogWarning("Parkour movement stopped becase of distance.");
                    }
                    break;
                }
            case ParkourState.WallRunning:
                {
                    if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || (Mathf.Abs(gameObject.transform.position.x - lerpPos.x) < 1))
                    {
                        if (StartedCoroutine != null) StopCoroutine(StartedCoroutine);
                        ResetState();
                        Debug.LogWarning("Parkour movement stopped becase of jumping or distance.");
                    }
                    break;
                }
            case ParkourState.Sliding:
                {
                    if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || IsGrounded() || !Physics.Raycast(Mesh.transform.position, Mesh.transform.forward, 1))
                    {
                        if (StartedCoroutine != null) StopCoroutine(StartedCoroutine);
                        ResetState();
                        Velocity.y += Time.deltaTime * 1000 * 3;
                        Debug.LogWarning("Parkour movement stopped becase of jumping.");
                    }
                    break;
                }


            default: break;
        }

    }


    void Sliding(GameObject slidingObject)
    {
        if (Velocity.y >= -1f || PlayerParkourState != ParkourState.None || IsGrounded()) return;
        lastAxis = Input.GetAxis("Horizontal");
        if (lastAxis == 0) return;
        if (LastSlidedObject == slidingObject && SlidingCooldown > Time.time) return;
        LastSlidedObject = slidingObject;

        PlayerParkourState = ParkourState.Sliding;
        animator.SetBool("isWallSliding", true);
    }


    public void RopeSwinging(Vector3 ToPosition) //public becase of rope.cs
    {
        ToPosition.z = 0;
        MaxSpeed = 16;
        lerpMultiplier = 2;
        LerpingAxis = ToPosition;
        LastPos = gameObject.transform.position;
        PlayerParkourState = ParkourState.RopeSwinging;
        Velocity.y = Mathf.Sqrt(0.25f * -3.0f * Physics.gravity.y);
        StartedCoroutine = StartCoroutine(ResetParkourState(1.75f));
    }

    public void WallRunning(float scaleX)
    {
        float x = Input.GetAxis("Horizontal");

        if (x == 0) return;

        EnabledMovement = false;
        EnabledGravity = false;

        MaxSpeed = 10;
        lerpMultiplier = 1.25f;
        Velocity.y = -(Physics.gravity.y * Time.deltaTime * 5);
        animator.SetBool("isWallSliding", false); //just in case
        animator.SetBool("isWallRunning", true);
        LerpingAxis = new Vector3(x * scaleX, 0.75f);
        LastPos = gameObject.transform.position;
        PlayerParkourState = ParkourState.WallRunning;
        StartedCoroutine = StartCoroutine(ResetParkourState(0.95f));
    }

    void EdgeClimbing(Vector3 ToPosition)
    {
        ToPosition.z = 0;
        EnabledMovement = false;
        EnabledGravity = false;
        Physics.IgnoreLayerCollision(0, 6, true);
        controller.detectCollisions = false;
        animator.SetBool("isWallSliding", false); //just in case
        animator.SetBool("isEdgeClimbing", true);
        lerpMultiplier = 3.0f;
        LerpingAxis = ToPosition;
        PlayerParkourState = ParkourState.EdgeClimbing;

        StartedCoroutine = StartCoroutine(ResetParkourState(1.3f));
    }

    void Vault(Vector3 ToPosition)
    {
        Physics.IgnoreLayerCollision(0, 6, true);
        EnabledMovement = false;
        controller.detectCollisions = false;
        MaxSpeed = 3f;
        lerpMultiplier = 3.25f;
        animator.SetBool("isWallSliding", false); //just in case
        animator.SetBool("isVaulting", true);
        StartedCoroutine = StartCoroutine(ResetParkourState(1.3f));
        LerpingAxis = ToPosition;
        LerpingAxis.y = Mathf.Clamp(LerpingAxis.y, 0, 4);
        LastPos = gameObject.transform.position;
        Velocity.y = -(Physics.gravity.y * Time.deltaTime * 10);
        PlayerParkourState = ParkourState.Vaulting;
    }


    IEnumerator ResetParkourState(float time)
    {

        yield return new WaitForSeconds(time);

        ResetState();

        yield return null;

    }

    void ResetState()
    {
        switch (PlayerParkourState)
        {

            case ParkourState.Vaulting:
                {
                    Physics.IgnoreLayerCollision(0, 6, false);
                    controller.detectCollisions = true;
                    EnabledGravity = true;
                    EnabledMovement = true;
                    animator.SetBool("isVaulting", false);
                    break;
                }

            case ParkourState.EdgeClimbing:
                {
                    Physics.IgnoreLayerCollision(0, 6, false);
                    controller.detectCollisions = true;
                    EnabledGravity = true;
                    EnabledMovement = true;
                    animator.SetBool("isEdgeClimbing", false);
                    break;
                }
            case ParkourState.WallRunning:
                {
                    Velocity.y += Mathf.Sqrt(1f * -3.0f * Physics.gravity.y);
                    EnabledGravity = true;
                    EnabledMovement = true;
                    animator.SetBool("isWallRunning", false);
                    break;
                }

            case ParkourState.RopeSwinging:
                {
                    //do nothing
                    break;
                }

            case ParkourState.Sliding:
                {
                    if (!IsGrounded() && Input.GetAxis("Horizontal") != 0 && Mathf.Sign(lastAxis) != Mathf.Sign(Input.GetAxis("Horizontal")))
                    {
                        if (Physics.OverlapSphere(Mesh.gameObject.transform.position + Vector3.up * 6, 0.1f).Length == 0)
                        {
                            Velocity.y += Mathf.Sqrt(1.1f * -3f * Physics.gravity.y);
                            controller.Move(new Vector3(-lastAxis * 1.4f, 0, 0));
                        }

                        controller.Move(new Vector3(-lastAxis * 0.1f, 0, 0));
                        if (-lastAxis < 0) RotateCharacter(270, false);
                        else RotateCharacter(90, false);

                        Debug.Log("Wall Jump");
                    }


                    animator.SetBool("isWallSliding", false);
                    Vector3 tempVector = transform.localPosition;
                    tempVector.z = 0;
                    transform.localPosition = tempVector;
                    SlidingCooldown = Time.time + 1.5f;
                    break;
                }

            default: break;
        }

        PlayerParkourState = ParkourState.None;
        LerpingAxis = Vector3.zero;
        LastPos = Vector3.zero;
        MaxSpeed = 10f;
        animator.ResetTrigger("Jump");
    }


}
