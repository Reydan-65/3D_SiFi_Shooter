using UnityEngine;

public class CharacterInputController : MonoBehaviour
{
    [SerializeField] private CharacterMovement targetCharacterMovement;
    [SerializeField] private ThirdPersonCamera targetCamera;
    [SerializeField] private PlayerShooter playerShooter;
    [SerializeField] private Vector3 aimingOffset;
    [SerializeField] private Vector3 crouchOffset;
    [SerializeField] private Vector3 aimingCrouchOffset;
    [SerializeField] private Vector3 cameraOffset;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        targetCharacterMovement.UpdatePosition = true;
    }

    private void Update()
    {
        targetCharacterMovement.TargetDirectionControl = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        targetCamera.RotationControl = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        if (targetCharacterMovement.TargetDirectionControl != Vector3.zero || targetCharacterMovement.IsAiming)
        {
            targetCamera.IsRotateTarget = true;
        }
        else
        {
            targetCamera.IsRotateTarget = false;
        }

        // Jump
        if (Input.GetButtonDown("Jump"))
        {
            targetCharacterMovement.Jump();
        }

        // Sprint
        if (Input.GetButtonDown("Hold Run"))
        {
            targetCharacterMovement.Run();
        }

        if (Input.GetButtonUp("Hold Run"))
        {
            targetCharacterMovement.UnRun();
        }

        // Crouch
        if (targetCharacterMovement.IsCrouch && targetCharacterMovement.CanStandUp() && !Input.GetButton("Hold Crouch"))
        {
            targetCharacterMovement.UnCrouch();
            if (targetCharacterMovement.IsAiming)
            {
                targetCamera.SetTargetOffset(aimingOffset);
            }
            else
            {
                targetCamera.SetDefaultTargetOffset();
            }
        }

        if (Input.GetButtonDown("Hold Crouch"))
        {
            targetCharacterMovement.Crouch();
            if (targetCharacterMovement.IsAiming)
            {
                targetCamera.SetTargetOffset(aimingCrouchOffset);
            }
            else
                targetCamera.SetTargetOffset(crouchOffset);
        }

        if (Input.GetButtonUp("Hold Crouch"))
        {
            if (targetCharacterMovement.CanStandUp())
            {
                targetCharacterMovement.UnCrouch();

                if (targetCharacterMovement.IsAiming)
                {
                    targetCamera.SetTargetOffset(aimingOffset);
                }
                else
                    targetCamera.SetDefaultTargetOffset();
            }
        }

        // Aiming
        if (Input.GetMouseButtonDown(1))
        {
            targetCharacterMovement.Aiming();

            if (targetCharacterMovement.IsCrouch)
            {
                targetCamera.SetTargetOffset(aimingCrouchOffset);
            }
            else
                targetCamera.SetTargetOffset(aimingOffset);
        }

        if (Input.GetMouseButtonUp(1))
        {
            targetCharacterMovement.UnAiming();

            if (targetCharacterMovement.IsCrouch)
            {
                targetCamera.SetDefaultTargetCrouchOffset();
            }
            else
                targetCamera.SetDefaultTargetOffset();
        }

        // Shoot
        if (targetCharacterMovement.IsAiming && Input.GetMouseButton(0))
        {
            playerShooter.Shoot();
        }
    }

    public void AssingCamera(ThirdPersonCamera camera)
    {
        targetCamera = camera;
        targetCamera.IsRotateTarget = false;
        targetCamera.SetTargetOffset(cameraOffset);
        targetCamera.SetTarget(targetCharacterMovement.transform);
    }
}
