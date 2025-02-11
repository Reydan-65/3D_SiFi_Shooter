using UnityEngine;
using UnityEngine.UI;

public class UISight : MonoBehaviour
{
    [SerializeField] private CharacterMovement characterMovement;
    [SerializeField] private Image sightImage;

    private void Update()
    {
        if (characterMovement != null)
            sightImage.enabled = characterMovement.IsAiming;
        else
            sightImage.enabled = true;
    }
}
