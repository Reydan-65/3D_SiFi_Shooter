using UnityEngine;
using UnityEngine.UI;

public class UIWeaponEnergy : MonoBehaviour
{
    [SerializeField] private Weapon targetWeapon;
    [SerializeField] private Slider energySlider;
    [SerializeField] private Image[] energySliderImages;
    [SerializeField] private Animator animator;

    private void Start()
    {
        energySlider.maxValue = targetWeapon.PrimaryMaxEnergy;
        energySlider.value = energySlider.maxValue;
    }

    private void Update()
    {
        energySlider.value = targetWeapon.PrimaryEnergy;
        SetActiveImages(targetWeapon.PrimaryEnergy != targetWeapon.PrimaryMaxEnergy);
        animator.SetBool("Can Fire", targetWeapon.ReadyToFire);
    }

    private void SetActiveImages(bool active)
    {
        for (int i = 0; i < energySliderImages.Length; i++)
        {
            energySliderImages[i].enabled = active;
        }
    }
}
