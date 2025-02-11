using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DetectionIndicator : MonoBehaviour
{
    public static DetectionIndicator Instance;

    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI text;

    private bool isAnyVisible = false;

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

    public void SetVisible(bool isVisible)
    {
        isAnyVisible = isAnyVisible || isVisible;

        bool shouldDisplay = isAnyVisible;
        image.enabled = shouldDisplay;
        text.enabled = shouldDisplay;
    }

    public void ResetVisibility()
    {
        isAnyVisible = false;
        image.enabled = false;
        text.enabled = false;
    }
}
