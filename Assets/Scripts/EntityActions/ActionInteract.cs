using UnityEngine;

public enum InteractType
{
    PickupItem,
    EnterCode,
    UseLadder,
    UseVehicle
}

[System.Serializable]
public class ActionInteractProperties : EntityActionProperties
{
    [SerializeField] private Transform m_InteractTransform;

    public Transform InteractTransform => m_InteractTransform;
}

public class ActionInteract : EntityContextAction
{
    [SerializeField] protected Transform m_Owner;
    [SerializeField] private InteractType m_Type;
    public InteractType Type => m_Type;

    protected new ActionInteractProperties Properties;

    public override void SetProperties(EntityActionProperties properties)
    {
        Properties = (ActionInteractProperties)properties;
    }
}
