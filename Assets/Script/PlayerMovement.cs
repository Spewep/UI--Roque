using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController cc;

    [Header("Movimento")]
    public float walkSpeed = 8f;
    public float runSpeed = 14f;
    public float airControl = 0.5f;
    public float jumpForce = 8f;
    public float gravity = 20f;
    private bool canMove = true;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float stamina;
    public float staminaDrainRate = 25f;
    public float staminaRecoverRate = 20f;
    public float zeroStaminaRecoverRate = 5f;
    private bool canRun = true;
    private bool zeroStamina = false;

    [Header("Later")]


    private Vector3 moveDirection = Vector3.zero;
    private Vector3 inputDirection = Vector3.zero;
    private bool isRunning = false;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        stamina = maxStamina;
    }

    void Update()
    {
        if (!canMove)
        {
            moveDirection.x = 0f;
            moveDirection.z = 0f;
            moveDirection.y -= gravity * Time.deltaTime;
            cc.Move(moveDirection * Time.deltaTime);
            return;
        }

        // ===== Código normal de movimento e stamina =====
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");

        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;
        forward.y = 0f; right.y = 0f;
        forward.Normalize(); right.Normalize();

        inputDirection = (forward * v + right * h).normalized;

        isRunning = (Mathf.Abs(v) > 0.1f || Mathf.Abs(h) > 0.1f)
                    && Input.GetKey(KeyCode.LeftShift)
                    && canRun;

        float speed = isRunning ? runSpeed : walkSpeed;

        if (cc.isGrounded)
        {
            moveDirection = inputDirection * speed;
            if (Input.GetButtonDown("Jump"))
                moveDirection.y = jumpForce;
        }
        else
        {
            Vector3 airMove = inputDirection * speed * airControl;
            moveDirection.x = Mathf.Lerp(moveDirection.x, airMove.x, airControl * Time.deltaTime * 5f);
            moveDirection.z = Mathf.Lerp(moveDirection.z, airMove.z, airControl * Time.deltaTime * 5f);
        }

        moveDirection.y -= gravity * Time.deltaTime;
        cc.Move(moveDirection * Time.deltaTime);

        if (inputDirection.magnitude > 0.1f)
            transform.rotation = Quaternion.LookRotation(inputDirection);

        // ===== Stamina =====
        if (isRunning)
        {
            stamina -= staminaDrainRate * Time.deltaTime;
            if (stamina <= 0f)
            {
                stamina = 0f;
                canRun = false;
                zeroStamina = true;
            }
        }
        else
        {
            if (zeroStamina)
            {
                stamina += zeroStaminaRecoverRate * Time.deltaTime;
                if (stamina >= maxStamina)
                {
                    stamina = maxStamina;
                    zeroStamina = false;
                    canRun = true;
                }
            }
            else
            {
                stamina += staminaRecoverRate * Time.deltaTime;
                if (stamina > maxStamina) stamina = maxStamina;
            }
        }
    }
    public void SetMovementEnabled(bool enabled)
    {
        canMove = enabled;
    }
    public void ResetPlayer()
    {
        stamina = maxStamina;
        canRun = true;
        zeroStamina = false;
    }

}