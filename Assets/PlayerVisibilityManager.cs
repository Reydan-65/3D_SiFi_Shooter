using System.Collections.Generic;
using UnityEngine;

public class PlayerVisibilityManager : MonoBehaviour
{
    public static PlayerVisibilityManager Instance;

    private HashSet<AIAlienSoldier> visibleAISoldier = new HashSet<AIAlienSoldier>();
    private HashSet<AIDrone> visibleAIDrone = new HashSet<AIDrone>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterVisibility(bool isVisible, AIAlienSoldier aiSoldier = null, AIDrone aIDrone = null)
    {
        if (aiSoldier != null)
        {
            if (isVisible)
                visibleAISoldier.Add(aiSoldier);
            else
                visibleAISoldier.Remove(aiSoldier);
        }

        if (aIDrone != null)
        {
            if (isVisible)
                visibleAIDrone.Add(aIDrone);
            else
                visibleAIDrone.Remove(aIDrone);
        }

        bool anyVisible = visibleAISoldier.Count > 0 || visibleAIDrone.Count > 0;

        DetectionIndicator.Instance.SetVisible(anyVisible);

        if (!anyVisible) DetectionIndicator.Instance.ResetVisibility();
    }
}
