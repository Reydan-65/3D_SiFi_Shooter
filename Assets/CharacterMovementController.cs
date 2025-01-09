using UnityEngine;

public class CharacterMovementController : MonoBehaviour
{
    [SerializeField] private CharacterMovement targetCharacterMovement;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        targetCharacterMovement.TargetDirectionControl = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        // Jump
        if (Input.GetButtonDown("Jump")) targetCharacterMovement.Jump();

        // Sprint
        if (Input.GetButtonDown("Run") || Input.GetButtonDown("Hold Run"))
        {
            targetCharacterMovement.Sprint();
        }

        if (Input.GetButtonDown("Run") || Input.GetButtonUp("Hold Run"))
        {
            targetCharacterMovement.UnSprint();
        }

        // Crouch
        if (Input.GetButtonDown("Crouch") || Input.GetButtonDown("Hold Crouch"))
        {
            targetCharacterMovement.Crouch();
        }

        if (Input.GetButtonDown("Crouch") || Input.GetButtonUp("Hold Crouch"))
        {
            targetCharacterMovement.UnCrouch();
        }

        // Aiming
        if (Input.GetMouseButtonDown(1))
        {
            targetCharacterMovement.Aiming();
        }

        if (Input.GetMouseButtonUp(1))
        {
            targetCharacterMovement.UnAiming();
        }
        // Aiming Crouch
        if (Input.GetMouseButtonDown(1) && Input.GetButtonDown("Crouch"))
        {
            targetCharacterMovement.Aiming();
        }

        if (Input.GetMouseButtonUp(1) && Input.GetButtonDown("Crouch"))
        {
            targetCharacterMovement.UnAiming();
        }
    }
}
