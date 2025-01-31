using UnityEngine;

public class PickupFirstAidKit : TriggerInteractAction
{
    protected override void OnEndAction(GameObject owner)
    {
        base.OnEndAction(owner);

        Destructible destructible = owner.transform.root.GetComponent<Destructible>();

        if (destructible != null)
        {
            destructible.RestoreHealth();
        }

        Destroy(gameObject);
    }
}
