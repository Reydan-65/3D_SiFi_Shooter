using Unity.VisualScripting;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;
    [Space(10)]

    [Header("Movement")]
    [SerializeField] private float accelerationRate;
    [Space(10)]
    [SerializeField] private float rifleBaseMoveSpeed;
    [Space(10)]
    [SerializeField] private float rifleWalkSpeed;
    [SerializeField] private float aimingRifleWalkSpeed;
    [Space(10)]
    [SerializeField] private float rifleSprintSpeed;
    [Space(10)]
    [SerializeField] private float rifleCrouchWalkSpeed;
    [SerializeField] private float AimingRifleCrouchWalkSpeed;
    [Space(10)]
    [SerializeField] private float rifleJumpSpeed;
    [Space(10)]
    [SerializeField] private float currentMoveSpeed;

    [Header("State")]
    [SerializeField] private float crouchHeight;

    private bool isIdle;
    private bool isAiming;
    private bool isJump;
    private bool isCrouch;
    private bool isSprint;

    private float baseCharacterHeight;
    private float baseCharacterHeightOffset;

    private Vector3 movementDirection;
    private Vector3 directionControl;

    public bool IsIdle => isIdle;
    public bool IsAiming => isAiming;
    public bool IsCrouch => isCrouch;
    public bool IsSprint => isSprint;

    [HideInInspector]
    public Vector3 TargetDirectionControl;

    // Unity Events
    private void Start()
    {
        baseCharacterHeight = characterController.height;
        baseCharacterHeightOffset = characterController.center.y;
        currentMoveSpeed = rifleWalkSpeed;

        ResetState();
    }

    private void Update()
    {
        CheckIdleState();
        Move();
    }

    // Private
    private void Move()
    {
        directionControl = Vector3.MoveTowards(directionControl, TargetDirectionControl, Time.deltaTime * accelerationRate);

        if (characterController.isGrounded)
        {
            movementDirection = directionControl * GetCurrentSpeedByState();

            if (isJump)
            {
                movementDirection.y = rifleJumpSpeed;
                isJump = false;
            }
        }
        else
        {
            Vector3 jumpDirection = directionControl * GetCurrentSpeedByState();
            jumpDirection.y = 0;
            movementDirection.x = jumpDirection.x;
            movementDirection.z = jumpDirection.z;
        }

        if (isIdle) movementDirection.y = 0;
        else
        movementDirection += Physics.gravity * Time.deltaTime;


        characterController.Move(movementDirection * Time.deltaTime);
    }

    private bool CheckIdleState()
    {
        if (movementDirection.x < 0.03f && movementDirection.z < 0.03f &&
            movementDirection.x > -0.03f && movementDirection.z > -0.03f && !isJump) isIdle = true;
        else isIdle = false;

        return isIdle;
    }

    private void ResetState()
    {
        isIdle = false;
        isAiming = false;
        isJump = false;
        isCrouch = false;
        isSprint = false;
    }

    private float GetCurrentSpeedByState()
    {
        float speed;
        speed = rifleWalkSpeed;

        if (characterController.isGrounded)
        {
            if (isSprint)
            {
                if (directionControl.z >= 0) speed = rifleSprintSpeed;
                else if (directionControl.z < 0) speed = rifleSprintSpeed;
            }
            else if (isCrouch && !isAiming)
            {
                if (directionControl.z >= 0) speed = rifleCrouchWalkSpeed;
                else if (directionControl.z < 0) speed = rifleCrouchWalkSpeed;
            }
            else if (isAiming && !isCrouch)
            {
                if (directionControl.z >= 0) speed = aimingRifleWalkSpeed;
                else if (directionControl.z < 0) speed = aimingRifleWalkSpeed;
            }
            else if (isAiming && isCrouch)
            {
                if (directionControl.z >= 0) speed = AimingRifleCrouchWalkSpeed;
                else if (directionControl.z < 0) speed = AimingRifleCrouchWalkSpeed;
            }
            else if (isJump)
            {
                if (directionControl.z >= 0) speed = rifleJumpSpeed;
                else if (directionControl.z < 0) speed = rifleJumpSpeed;
            }
        }

        currentMoveSpeed = Mathf.Lerp(currentMoveSpeed, speed, accelerationRate * Time.deltaTime);

        return currentMoveSpeed;
    }

    // Public
    public void Jump()
    {
        if (!characterController.isGrounded) return;

        if (isCrouch)
        {
            UnCrouch();
        }

        if (isAiming)
        {
            UnAiming();
        }

        isJump = true;
    }

    public void Crouch()
    {
        if (!characterController.isGrounded) return;

        if (!isCrouch)
        {
            if (isSprint) UnSprint();

            characterController.height = crouchHeight;
            characterController.center = new Vector3(0, baseCharacterHeightOffset / 2, 0);

            accelerationRate *= 0.8f;
            isCrouch = true;
        }
    }

    public void UnCrouch()
    {
        if (isCrouch)
        {
            characterController.height = baseCharacterHeight;
            characterController.center = new Vector3(0, baseCharacterHeightOffset, 0);
            accelerationRate /= 0.8f;

            isCrouch = false;
        }
    }

    public void Sprint()
    {
        if (!characterController.isGrounded) return;

        if (!isSprint && !isAiming)
        {
            if (isCrouch)
            {
                UnCrouch();
            }

            isSprint = true;
            accelerationRate *= 1.2f;
        }
    }

    public void UnSprint()
    {
        if (isSprint)
        {
            isSprint = false;
            accelerationRate /= 1.2f;
        }
    }

    public void Aiming()
    {
        if (!characterController.isGrounded) return;

        if (!isAiming)
        {
            isSprint = false;
            isAiming = true;
        }
    }

    public void UnAiming()
    {
        if (isAiming)
        {
            isAiming = false;
        }
    }
}
