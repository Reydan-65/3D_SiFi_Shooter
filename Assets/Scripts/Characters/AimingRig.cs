using UnityEngine;

public class AimingRig : MonoBehaviour
{
    [SerializeField] private CharacterMovement targetCharacterMovement;
    [SerializeField] private UnityEngine.Animations.Rigging.Rig[] rigs;

    [SerializeField] private float changeWeightLerpRate;

    private float targetWeight;

    private void Update()
    {
        if (targetCharacterMovement.IsAiming) targetWeight = 1;
        else targetWeight = 0;

        for (int i = 0; i < rigs.Length; i++)
        {
            rigs[i].weight = Mathf.MoveTowards(rigs[i].weight, targetWeight, Time.deltaTime * changeWeightLerpRate);
        }
    }
}
