using UnityEngine;

public enum InteractType
{
    PickupItem,
    EnterCode,
    ClimbingLadder
}

[System.Serializable]
public class ActionInteractProperties : EntityActionProperties
{
    [SerializeField] private Transform m_InteractTransform;

    public Transform InteractTransform => m_InteractTransform;
}

public class ActionInteract : EntityContextAction
{
    [SerializeField] private Transform m_Owner;
    [SerializeField] private InteractType m_Type;
    public InteractType Type => m_Type;

    private new ActionInteractProperties Properties;

    public override void SetProperties(EntityActionProperties properties)
    {
        Properties = (ActionInteractProperties)properties;
    }

    public override void StartAction()
    {
        if (IsCanStart == false) return;

        base.StartAction();
    }
}
