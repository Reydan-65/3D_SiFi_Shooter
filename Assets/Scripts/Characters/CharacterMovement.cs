using System.Collections;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;
    [Space(10)]

    [Header("Movement")]
    [SerializeField] private float accelerationRate;
    [SerializeField] private float aimingBackwardMoveSpeedReduce;
    [Space(10)]
    [SerializeField] private float rifleBaseMoveSpeed;
    [Space(10)]
    [SerializeField] private float rifleWalkSpeed;
    [SerializeField] private float aimingRifleWalkSpeed;
    [Space(10)]
    [SerializeField] private float rifleRunSpeed;
    [SerializeField] private float aimingRifleRunSpeed;
    [Space(10)]
    [SerializeField] private float rifleCrouchSpeed;
    [SerializeField] private float aimingRifleCrouchSpeed;
    [Space(10)]
    [SerializeField] private float rifleJumpSpeed;
    [Space(10)]
    [SerializeField] private float currentMoveSpeed;

    [Header("State")]
    [SerializeField] private float crouchHeight;

    private bool isAiming;
    private bool isJump;
    private bool isCrouch;
    private bool isRun;
    private bool isGround;
    private bool isFalling;
    private float currentDistanceToGround;
    private float lastDistanceToGround;

    private float baseCharacterHeight;
    private float baseCharacterHeightOffset;

    private Vector3 movementDirection;
    private Vector3 directionControl;

    private LayerMask obstacleLayer;

    public bool IsAiming => isAiming;
    public bool IsCrouch => isCrouch;
    public bool IsRun => isRun;
    public bool IsGround => isGround;
    public bool IsFalling => isFalling;
    public float CurrentDistanceToGround => currentDistanceToGround;

    [HideInInspector]
    public Vector3 TargetDirectionControl;

    // Unity Events
    private void Start()
    {
        baseCharacterHeight = characterController.height;
        baseCharacterHeightOffset = characterController.center.y;
        currentMoveSpeed = rifleWalkSpeed;
        obstacleLayer = LayerMask.GetMask("Obstacle");

        ResetState();
    }

    private void FixedUpdate()
    {
        Move();

        UpdateGroundState();
        UpdateFallingState();
        UpdateDistanceToGround();
        UpdateCharacterControllerHeightState();
    }

    // Private
    private void Move()
    {
        directionControl = Vector3.MoveTowards(directionControl, TargetDirectionControl, Time.fixedDeltaTime * accelerationRate);

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
            Vector3 jumpDirection = directionControl * currentMoveSpeed;

            jumpDirection.y = 0;
            movementDirection.x = jumpDirection.x;
            movementDirection.z = jumpDirection.z;
        }

        movementDirection = transform.TransformDirection(movementDirection);

        movementDirection += Physics.gravity * Time.fixedDeltaTime;

        characterController.Move(movementDirection * Time.fixedDeltaTime);
    }

    private bool UpdateGroundState()
    {
        if (currentDistanceToGround >= -0.001 && currentDistanceToGround <= 0.001)
        {
            lastDistanceToGround = transform.localPosition.y;
            isGround = true;
        }
        if (currentDistanceToGround < -0.001 || currentDistanceToGround > 0.001) isGround = false;

        return isGround;
    }

    private bool UpdateFallingState()
    {
        if (!isGround)
        {
            StartCoroutine(CheckIsFalling(0.25f));
        }
        else isFalling = false;

        return isFalling;
    }

    private IEnumerator CheckIsFalling(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (transform.localPosition.y < lastDistanceToGround) isFalling = true;
    }

    private void ResetState()
    {
        isAiming = false;
        isJump = false;
        isCrouch = false;
        isRun = false;
    }

    private CharacterController UpdateCharacterControllerHeightState()
    {
        CharacterController temp = characterController;

        if (isCrouch && (isGround && movementDirection.y > -0.5f && movementDirection.y < 0.5f))
        {
            temp.height = crouchHeight;
            temp.center = new Vector3(0, baseCharacterHeightOffset / 2, 0);
        }

        if (!isCrouch || isGround && (movementDirection.y < -0.5f || movementDirection.y > 0.5f))
        {
            if (CanStandUp())
            {
                if (temp.height != baseCharacterHeight)
                    temp.height = baseCharacterHeight;

                if (temp.center.y != baseCharacterHeightOffset)
                    temp.center = new Vector3(0, baseCharacterHeightOffset, 0);
            }
        }

        return temp;
    }

    public bool CanStandUp()
    {
        Vector3 topPosition = transform.position + new Vector3(0, baseCharacterHeight, 0);
        Collider[] hitColliders = Physics.OverlapBox(topPosition, new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, obstacleLayer);
        return hitColliders.Length == 0;
    }

    // Public
    public float GetCurrentSpeedByState()
    {
        float speed;
        speed = rifleWalkSpeed;

        if (characterController.isGrounded)
        {
            if (isAiming && !isRun && !isCrouch)
            {
                if (directionControl.z >= 0) speed = aimingRifleWalkSpeed;
                else if (directionControl.z < 0) speed = aimingRifleWalkSpeed * aimingBackwardMoveSpeedReduce;
            }
            else if (isRun && !isAiming)
            {
                if (directionControl.z >= 0) speed = rifleRunSpeed;
                else if (directionControl.z < 0) speed = rifleRunSpeed * aimingBackwardMoveSpeedReduce;
            }
            else if (isRun && isAiming)
            {
                if (directionControl.z >= 0) speed = aimingRifleRunSpeed;
                else if (directionControl.z < 0) speed = aimingRifleRunSpeed * aimingBackwardMoveSpeedReduce;
            }
            else if (isCrouch && !isAiming)
            {
                if (directionControl.z >= 0) speed = rifleCrouchSpeed;
                else if (directionControl.z < 0) speed = rifleCrouchSpeed * aimingBackwardMoveSpeedReduce;
            }
            else if (isCrouch && isAiming)
            {
                if (directionControl.z >= 0) speed = aimingRifleCrouchSpeed;
                else if (directionControl.z < 0) speed = aimingRifleCrouchSpeed * aimingBackwardMoveSpeedReduce;
            }
        }

        currentMoveSpeed = Mathf.Lerp(currentMoveSpeed, speed, accelerationRate * Time.fixedDeltaTime);

        return currentMoveSpeed;
    }

    public void Jump()
    {
        if (!isGround) return;
        isJump = true;
    }

    public void Crouch()
    {
        if (!isCrouch)
        {
            if (isRun) UnRun();

            accelerationRate *= 0.8f;
            isCrouch = true;
        }
    }

    public void UnCrouch()
    {
        if (isCrouch)
        {
            accelerationRate /= 0.8f;
            isCrouch = false;
        }
    }

    public void Run()
    {
        if (!isRun && !isCrouch)
        {
            accelerationRate *= 1.2f;
            isRun = true;
        }
    }

    public void UnRun()
    {
        if (isRun)
        {
            isRun = false;
            accelerationRate /= 1.2f;
        }
    }

    public void Aiming()
    {
        if (!isAiming)
        {
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

    private void UpdateDistanceToGround()
    {
        float radius = characterController.radius;

        // Define points around the capsule
        Vector3[] points = new Vector3[]
        {
        transform.position + new Vector3(radius, 0, 0),
        transform.position + new Vector3(-radius, 0, 0),
        transform.position + new Vector3(0, 0, radius),
        transform.position + new Vector3(0, 0, -radius),
        transform.position + new Vector3(radius, 0, radius),
        transform.position + new Vector3(radius, 0, -radius),
        transform.position + new Vector3(-radius, 0, radius),
        transform.position + new Vector3(-radius, 0, -radius)
        };

        float minDistance = float.MaxValue;

        foreach (var point in points)
        {
            RaycastHit hit;
            if (Physics.Raycast(point, -Vector3.up, out hit, 1000))
            {
                float distance = Vector3.Distance(point, hit.point);
                if (distance < minDistance)
                {
                    minDistance = distance;
                }
            }
        }

        currentDistanceToGround = minDistance;
    }
}
