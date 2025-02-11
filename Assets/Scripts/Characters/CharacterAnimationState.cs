using UnityEngine;

[System.Serializable]
public class CharacterAnimatorParametersName
{
    public string NormolizeMovementX;
    public string NormolizeMovementZ;
    public string Run;
    public string InteractAction;
    public string Crouch;
    public string Aiming;
    public string Ground;
    public string Falling;
    public string Jump;
    public string Climbing;
    public string JumpSpeed;
    public string DistanceToGround;
    public string GroundSpeed;
}

[System.Serializable]
public class AnimationCrossFadeParametrs
{
    public string Name;
    public float Duration;
}

public class CharacterAnimationState : MonoBehaviour
{
    private const float INPUT_CONTROL_LERP = 10.0f;

    [SerializeField] private CharacterController targetCharacterController;
    [SerializeField] private CharacterMovement targetCharacterMovement;
    [SerializeField] private Animator targetAnimator;

    [Space(10)]
    [SerializeField] private CharacterAnimatorParametersName AnimatorParametersName;
    
    [Space(10)]
    [Header("Fades")]
    [SerializeField] private AnimationCrossFadeParametrs fallFade;
    [SerializeField] private float minDistanceToGroundByFall;
    [SerializeField] private AnimationCrossFadeParametrs jumpIdleFade;
    [SerializeField] private AnimationCrossFadeParametrs jumpMoveFade;

    private Vector3 inputControl;

    private void LateUpdate()
    {
        Vector3 movementSpeed = transform.InverseTransformDirection(targetCharacterController.velocity);

        inputControl = Vector3.MoveTowards(inputControl, targetCharacterMovement.TargetDirectionControl, Time.deltaTime * INPUT_CONTROL_LERP);

        targetAnimator.SetFloat(AnimatorParametersName.NormolizeMovementX, inputControl.x);
        targetAnimator.SetFloat(AnimatorParametersName.NormolizeMovementZ, inputControl.z);

        targetAnimator.SetBool(AnimatorParametersName.Run, targetCharacterMovement.IsRun);
        targetAnimator.SetBool(AnimatorParametersName.Jump, targetCharacterMovement.IsJump);
        targetAnimator.SetBool(AnimatorParametersName.Crouch, targetCharacterMovement.IsCrouch);
        targetAnimator.SetBool(AnimatorParametersName.Aiming, targetCharacterMovement.IsAiming);
        targetAnimator.SetBool(AnimatorParametersName.Ground, targetCharacterMovement.IsGrounded);
        targetAnimator.SetBool(AnimatorParametersName.Falling, targetCharacterMovement.IsFalling);
        targetAnimator.SetBool(AnimatorParametersName.Climbing, targetCharacterMovement.IsClimbing);
        targetAnimator.SetBool(AnimatorParametersName.InteractAction, targetCharacterMovement.IsInteractAction);

        Vector3 groundSpeed = targetCharacterController.velocity;
        groundSpeed.y = 0;
        targetAnimator.SetFloat(AnimatorParametersName.GroundSpeed, groundSpeed.magnitude);

        if (targetCharacterMovement.IsJump == true)
        {
            if (groundSpeed.magnitude <= 0.01f)
            {
                CrossFade(jumpIdleFade);
            }

            if (groundSpeed.magnitude > 0.01f)
            {
                CrossFade(jumpMoveFade);
            }
        }

        if (targetCharacterMovement.IsGrounded)
        {
            if(Input.GetButtonDown("Jump")) targetAnimator.StopPlayback();
            targetAnimator.SetFloat(AnimatorParametersName.JumpSpeed, movementSpeed.y);

            if (movementSpeed.y < 0 && targetCharacterMovement.CurrentDistanceToGround > minDistanceToGroundByFall)
            {
                CrossFade(fallFade);
            }
        }

        targetAnimator.SetFloat(AnimatorParametersName.DistanceToGround, targetCharacterMovement.CurrentDistanceToGround);
    }

    private void CrossFade(AnimationCrossFadeParametrs parametrs)
    {
        targetAnimator.CrossFade(parametrs.Name, parametrs.Duration);
    }
}
