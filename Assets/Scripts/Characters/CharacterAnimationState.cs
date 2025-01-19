using UnityEngine;

public class CharacterAnimationState : MonoBehaviour
{
    private const float INPUT_CONTROL_LERP = 10.0f;

    [SerializeField] private CharacterController targetCharacterController;
    [SerializeField] private CharacterMovement targetCharacterMovement;
    [SerializeField] private Animator targetAnimator;

    private Vector3 inputControl;

    private void LateUpdate()
    {
        Vector3 movementSpeed = transform.InverseTransformDirection(targetCharacterController.velocity);

        inputControl = Vector3.MoveTowards(inputControl, targetCharacterMovement.TargetDirectionControl, Time.deltaTime * INPUT_CONTROL_LERP);

        targetAnimator.SetFloat("Normalize Movement X", inputControl.x);
        targetAnimator.SetFloat("Normalize Movement Z", inputControl.z);

        targetAnimator.SetBool("Is Run", targetCharacterMovement.IsRun);
        targetAnimator.SetBool("Is Crouch", targetCharacterMovement.IsCrouch);
        targetAnimator.SetBool("Is Aiming", targetCharacterMovement.IsAiming);
        targetAnimator.SetBool("Is Ground", targetCharacterMovement.IsGround);
        targetAnimator.SetBool("Is Falling", targetCharacterMovement.IsFalling);

        if (targetCharacterMovement.IsGround)
        {
            if(Input.GetButtonDown("Jump")) targetAnimator.StopPlayback();
            targetAnimator.SetFloat("Jump", movementSpeed.y);
        }

        Vector3 groundSpeed = targetCharacterController.velocity;
        groundSpeed.y = 0;
        targetAnimator.SetFloat("Ground Speed", groundSpeed.magnitude);

        targetAnimator.SetFloat("Distance To Ground", targetCharacterMovement.CurrentDistanceToGround);
    }
}
