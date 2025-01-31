using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class TriggerInteractAction : MonoBehaviour
{
    [SerializeField] private InteractType m_InteractType;

    [SerializeField] private int m_InteractAmount;
    public int InteractAmount => m_InteractAmount;

    [SerializeField] private ActionInteractProperties m_ActionProperties;
    public ActionInteractProperties ActionProperties => m_ActionProperties;

    [Space(10)]
    [SerializeField] private UnityEvent m_EventOnInteract;
    public UnityEvent EventOnInteract => m_EventOnInteract;

    protected ActionInteract m_Action;

    private GameObject m_Owner;

    protected virtual void InitActionProperties()
    {
        m_Action.SetProperties(m_ActionProperties);
    }

    protected virtual void OnStartAction(GameObject owner) { }
    protected virtual void OnEndAction(GameObject owner) { }

    private void OnTriggerEnter(Collider other)
    {
        if (m_InteractAmount == 0) return;
        if (other.transform.root.GetComponent<CharacterMovement>() == true)
        if (other.transform.root.GetComponent<CharacterMovement>().IsJump == true) return;

        EntityActionCollector actionCollector = other.GetComponent<EntityActionCollector>();

        if (actionCollector != null)
        {
            m_Action = GetActionInteract(actionCollector);

            if (m_Action != null)
            {
                InitActionProperties();

                m_Action.IsCanStart = true;

                m_Action.EventOnStart.AddListener(ActionStarted);
                m_Action.EventOnEnd.AddListener(ActionEnded);

                m_Owner = other.gameObject;

                m_Owner.transform.root.GetComponent<CharacterInputController>().TriggerInteractAction = this;
                m_Owner.transform.root.GetComponent<CharacterMovement>().InteractPosition = ActionProperties.InteractTransform.position;
                m_Owner.GetComponent<CharacterMovement>().interactTarget = new Vector3(transform.position.x, 0, transform.position.z);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (m_InteractAmount == 0) return;
        if (other.transform.root.GetComponent<CharacterMovement>().IsJump == true) return;

        other.transform.root.GetComponent<CharacterInputController>().TriggerInteractAction = null;
        other.transform.root.GetComponent<CharacterMovement>().InteractPosition = Vector3.zero;
        other.GetComponent<CharacterMovement>().interactTarget = Vector3.zero;
        other.GetComponent<CharacterMovement>().IsInteractAction = false;

        EntityActionCollector actionCollector = other.GetComponent<EntityActionCollector>();

        if (actionCollector != null)
        {
            m_Action = GetActionInteract(actionCollector);

            if (m_Action != null)
            {
                m_Action.IsCanStart = false;
                m_Action.EventOnStart.RemoveListener(ActionStarted);
                m_Action.EventOnEnd.RemoveListener(ActionEnded);
            }
        }
    }

    private void ActionStarted()
    {
        Vector3 pos = new Vector3(transform.position.x, 0, transform.position.z);

        if (Vector3.Distance(m_Owner.transform.position, pos) < 0.1f)
        {
            OnStartAction(m_Owner);
        }
    }

    private void ActionEnded()
    {
        m_InteractAmount--;

        if (m_InteractAmount == 0)
        {
            m_Action.IsCanStart = false;
            m_Owner.transform.root.GetComponent<CharacterInputController>().TriggerInteractAction = null;
            m_Owner.transform.root.GetComponent<CharacterMovement>().InteractPosition = Vector3.zero;
            m_Owner.GetComponent<CharacterMovement>().interactTarget = Vector3.zero;
            m_Action.EventOnStart.RemoveListener(ActionStarted);
            m_Action.EventOnEnd.RemoveListener(ActionEnded);
        }
        else
            m_Action.IsCanStart = true;

        m_EventOnInteract?.Invoke();

        m_Owner.GetComponent<CharacterMovement>().IsInteractAction = false;

        OnEndAction(m_Owner);
    }

    private ActionInteract GetActionInteract(EntityActionCollector entityActionCollector)
    {
        List<ActionInteract> actions = entityActionCollector.GetActionList<ActionInteract>();

        for (int i = 0; i < actions.Count; i++)
        {
            if (actions[i] != null)
            {
                if (actions[i].Type == m_InteractType)
                {
                    return actions[i];
                }
            }
        }

        return null;
    }
}
