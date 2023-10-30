

using UnityEngine;
using UnityEngine.Animations.Rigging;

public partial class Player : MonoBehaviour
{

    [Header("Movement and Animations")]

    [Tooltip("For Movement")] public CharacterController controller;


    [Tooltip("Character Animatior")] public Animator animator;
    [Tooltip("Player Rig")] public Rig Rig;
    [Tooltip("Spine Aim")] public GameObject SpineSource;


    [HideInInspector] public Vector3 Velocity;

    public float Speed = 4f;
    public float MaxSpeed = 10;


    public bool EnabledGravity = true;
    public bool EnabledMovement = true;

    void Movement()
    {
        if (!EnabledMovement) return;


        float x = Input.GetAxis("Horizontal");



        if (x != 0)
        {

            int state = 0;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                Speed = MaxSpeed;
                state = 1;
            }
            else if (IsGrounded()) Speed = 4f;

            Speed = Mathf.Clamp(Speed, 0, MaxSpeed);
            x *= Speed;

            state++;

            controller.Move(new Vector3(x, 0, 0) * Time.deltaTime);
            animator.SetInteger("MovementState", state);

            if (x < 0) RotateCharacter(270);
            else RotateCharacter(90);
        }
        else animator.SetInteger("MovementState", 0);



    }


    void Gravity()
    {

        bool grounded = IsGrounded();

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && grounded)
        {

            Collider[] hitcolliders = Physics.OverlapSphere(Mesh.gameObject.transform.position + Vector3.up * 5, 0.1f);
            bool isAbleToJump = true;
            foreach (var c in hitcolliders)
            {
                if (c.isTrigger) continue;
                else
                {
                    isAbleToJump = false;
                    break;
                }
            }

            if (isAbleToJump) Velocity.y += Mathf.Sqrt(1.65f * -3.0f * Physics.gravity.y);
            animator.SetTrigger("Jump");
        }


        if (EnabledGravity == false) return;

        if (grounded && Velocity.y < 0) Velocity.y = 0;

        float _gravity = Physics.gravity.y * Time.deltaTime;


        animator.SetBool("isLanded", grounded);



        Velocity.y += _gravity;


        controller.Move(Velocity * Time.deltaTime);



    }


    public void RotateCharacter(float yRot, bool lerp = true)
    {
        if (PlayerParkourState == ParkourState.Sliding) return;
        if (lerp == true)
        {
            float currentAngle = Mesh.transform.eulerAngles.y;
            currentAngle = Mathf.Lerp(currentAngle, yRot, Time.deltaTime * 9);
            Mesh.transform.eulerAngles = new Vector3(0, currentAngle, 0);
        }
        else
        {
            Mesh.transform.eulerAngles = new Vector3(0, yRot, 0);
        }
    }

    bool IsGrounded(float distance = 0.5f)
    {
        //   if (animator.GetBool("isVaulting") || animator.GetBool("isEdgeClimbing")) return true;
        if (Velocity.y > 0.05f) return false;
        return Physics.Raycast(Mesh.transform.position, Vector3.down, distance);
    }
}
