using System.Collections;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;
    [Space(10)]
    [SerializeField] private LayerMask ignoreTriggerLayerMask;

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

    [Header("Ladder Climbing")]
    [SerializeField] private float ladderClimbSpeed = 2.0f;
    [SerializeField] private float ladderSnapDistance = 0.1f;
    private Vector3 ladderTopPosition;
    private Vector3 ladderBottomPosition;

    private bool isInteractAction;
    private bool isAiming;
    private bool isJump;
    private bool isCrouch;
    private bool isRun;
    private bool isGrounded;
    private bool isFalling;
    private bool isClimbing;
    private bool isStartClimbing;
    private float currentDistanceToGround;
    private float lastDistanceToGround;

    private float baseCharacterHeight;
    private float baseCharacterHeightOffset;

    private Vector3 movementDirection;
    private Vector3 directionControl;
    private Vector3 interactPosition;
    private Vector3 interactRotation;
    private Vector3 lastGrabLadderDirection;
    private LayerMask obstacleLayer;
    private Ladder targetLadder;

    public bool IsInteractAction { get => isInteractAction; set => isInteractAction = value; }
    public bool IsAiming => isAiming;
    public bool IsJump => isJump;
    public bool IsCrouch => isCrouch;
    public bool IsRun => isRun;
    public bool IsGrounded => isGrounded;
    public bool IsFalling => isFalling;
    public bool IsClimbing => isClimbing;
    public bool IsStartClimbing => isStartClimbing;

    public float CurrentDistanceToGround => currentDistanceToGround;
    public Vector3 InteractPosition { get => interactPosition; set => interactPosition = value; }
    public Vector3 interactTarget { get => interactRotation; set => interactRotation = value; }
    public Ladder TargetLadder => targetLadder;

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
        if (IsInteractAction)
        {
            GetComponent<CharacterInputController>().enabled = false;
            movementDirection = Vector3.zero;

            MoveAtInteractTarget();

            Vector3 direction = (interactTarget - transform.position).normalized;
            interactTarget += direction * 0.1f;
            direction.y = 0;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rifleWalkSpeed * Time.fixedDeltaTime);

            if (IsStartClimbing)
            {
                isGrounded = true;

                if (Vector3.Distance(transform.position, interactPosition) < 0.1f &&
                    Quaternion.Angle(transform.rotation, lookRotation) < 1.0f)
                {
                    isClimbing = true;
                    IsInteractAction = false;
                }
            }
        }
        else
        {
            if (isClimbing)
            {
                ClimbLadder();
            }
            else
            {
                Move();
            }
        }

        UpdateGroundState();
        UpdateFallingState();
        UpdateDistanceToGround();
        UpdateCharacterControllerHeightState();
    }

    // Private

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

    public void ClimbLadder()
    {
        isGrounded = true;

        transform.position = Vector3.MoveTowards(transform.position, ladderTopPosition, ladderClimbSpeed * Time.fixedDeltaTime);

        if (Vector3.Distance(transform.position, ladderTopPosition) < ladderSnapDistance)
        {
            StopClimbing();
        }
    }

    public void StartClimbing()
    {
        isStartClimbing = true;
    }

    public void StopClimbing()
    {
        isClimbing = false;
        GetComponent<CharacterInputController>().enabled = true;

        Vector3 stepForward = transform.forward * 0.5f;
        characterController.Move(stepForward);

        ladderBottomPosition = Vector3.zero;
        ladderTopPosition = Vector3.zero;
        interactPosition = Vector3.zero;
        interactTarget = Vector3.zero;
        IsInteractAction = false;
        isStartClimbing = false;
        targetLadder = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other != null)
        {
            Ladder ladder = other.GetComponent<Ladder>();
            if (ladder != null)
            {
                interactTarget = other.transform.position;

                targetLadder = ladder.GetComponent<Ladder>();
                ladderBottomPosition = interactPosition;
                ladderTopPosition = ladder.TopPosition.position;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Ladder ladder = other.GetComponent<Ladder>();
        if (ladder != null)
        {
            if (!isClimbing)
            {
                interactTarget = Vector3.zero;
                targetLadder = null;
                ladderBottomPosition = Vector3.zero;
                ladderTopPosition = Vector3.zero;
            }
        }
    }

    private bool UpdateGroundState()
    {
        if (currentDistanceToGround >= -0.001 && currentDistanceToGround <= 0.001)
        {
            lastDistanceToGround = transform.localPosition.y;
            isGrounded = true;
        }
        if (currentDistanceToGround < -0.001 || currentDistanceToGround > 0.001) isGrounded = false;

        return isGrounded;
    }

    private bool UpdateFallingState()
    {
        if (!isGrounded)
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
        isAiming = false;
        isStartClimbing = false;
    }

    private CharacterController UpdateCharacterControllerHeightState()
    {
        CharacterController temp = characterController;

        if (isCrouch && (isGrounded && movementDirection.y > -0.5f && movementDirection.y < 0.5f))
        {
            temp.height = crouchHeight;
            temp.center = new Vector3(0, baseCharacterHeightOffset / 2, 0);
        }

        if (!isCrouch || isGrounded && (movementDirection.y < -0.5f || movementDirection.y > 0.5f))
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

    // Public
    public bool CanStandUp()
    {
        Vector3 topPosition = transform.position + new Vector3(0, baseCharacterHeight, 0);
        Collider[] hitColliders = Physics.OverlapBox(topPosition, new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, obstacleLayer);
        return hitColliders.Length == 0;
    }

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
        if (!isGrounded) return;
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

    public void MoveAtInteractTarget()
    {
        if (interactPosition == Vector3.zero) return;
        if (interactTarget == Vector3.zero) return;

        transform.position = Vector3.MoveTowards(transform.position, interactPosition, rifleWalkSpeed * Time.fixedDeltaTime);
    }
}
