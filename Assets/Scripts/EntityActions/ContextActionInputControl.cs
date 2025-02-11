using System.Collections.Generic;
using UnityEngine;

public class ContextActionInputControl : MonoBehaviour
{
    [SerializeField] private CharacterMovement targetCharacterMovement;
    [SerializeField] private EntityActionCollector TargetActionCollector;

    private TriggerInteractAction triggerInteractAction;
    public TriggerInteractAction TriggerInteractAction { get => triggerInteractAction; set => triggerInteractAction = value; }

    private void Update()
    { 
        // Interaction
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (triggerInteractAction == null) return;

            targetCharacterMovement.IsInteractAction = true;

            if (targetCharacterMovement.TargetLadder) targetCharacterMovement.StartClimbing();

            List<EntityContextAction> actionsList = TargetActionCollector.GetActionList<EntityContextAction>();

            for (int i = 0; i < actionsList.Count; i++)
            {
                actionsList[i].StartAction();
                actionsList[i].EndAction();
            }
        }
    }
}
