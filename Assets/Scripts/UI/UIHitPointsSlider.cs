using UnityEngine;
using UnityEngine.UI;

public class UIHitPointsSlider : MonoBehaviour
{
    [SerializeField] private Destructible m_Destructble;
    [SerializeField] private Slider m_Slider;
    [SerializeField] private Image m_FillImage;

    //[SerializeField] private Image[] energySliderImages;
    //[SerializeField] private Animator animator;
    //private Color baseSliderImagColor;

    private void Start()
    {
        m_Slider.maxValue = m_Destructble.MaxHitPoints;
        m_Slider.value = m_Slider.maxValue;
    }

    private void Update()
    {
        m_Slider.value = m_Destructble.HitPoints;

        if (m_Destructble.HitPoints <= m_Destructble.HitPoints / 2) m_FillImage.color = Color.yellow;
        if (m_Destructble.HitPoints <= m_Destructble.HitPoints / 4) m_FillImage.color = Color.red;
        if (m_Destructble.HitPoints == 0 ) m_FillImage.enabled = false;

        //SetActiveImages(targetWeapon.PrimaryEnergy != targetWeapon.PrimaryMaxEnergy);

            //animator.SetBool("Can Fire", targetWeapon.ReadyToFire);

            //energySliderImages[0].color = baseSliderImagColor;
    }
}