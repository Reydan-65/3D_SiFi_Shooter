using UnityEngine;
using UnityEngine.UI;

public class UISight : MonoBehaviour
{
    [SerializeField] private CharacterMovement characterMovement;
    [SerializeField] private Image sightImage;

    private void Update()
    {
        sightImage.enabled = characterMovement.IsAiming;
    }
}
