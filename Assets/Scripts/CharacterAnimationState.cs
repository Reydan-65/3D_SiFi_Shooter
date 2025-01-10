using UnityEngine;

public class CharacterAnimationState : MonoBehaviour
{
    [SerializeField] private CharacterController targetCharacterController;
    [SerializeField] private CharacterMovement targetCharacterMovement;
    [SerializeField] private Animator targetAnimator;

    private void Update()
    {
        Vector3 movementSpeed = targetCharacterController.velocity;

        targetAnimator.SetFloat("Normalize Movement X", movementSpeed.x / targetCharacterMovement.GetCurrentSpeedByState());
        targetAnimator.SetFloat("Normalize Movement Z", movementSpeed.z / targetCharacterMovement.GetCurrentSpeedByState());
    }
}
