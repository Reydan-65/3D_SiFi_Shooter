using UnityEngine;

public class SpreadShootRig : MonoBehaviour
{
    [SerializeField] private UnityEngine.Animations.Rigging.Rig spreadRigs;

    [SerializeField] private float changeWeightLerpRate;

    private float targetWeight;

    private void Update()
    {
        spreadRigs.weight = Mathf.MoveTowards(spreadRigs.weight, targetWeight, Time.deltaTime * changeWeightLerpRate);

        if (spreadRigs.weight == 1)
            targetWeight = 0;
    }

    public void Spread()
    {
        targetWeight = 1;
    }
}
