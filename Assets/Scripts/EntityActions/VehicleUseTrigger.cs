using UnityEngine;

public class VehicleUseTrigger : TriggerInteractAction
{
    [SerializeField] private ActionUseVehicleProperties m_UseProperties;

    protected override void InitActionProperties()
    {
        m_Action.SetProperties(m_UseProperties);
    }
}
