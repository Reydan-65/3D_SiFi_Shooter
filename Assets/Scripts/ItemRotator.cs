using UnityEngine;

public class ItemRotator : MonoBehaviour
{
    [SerializeField] private Vector3 m_RotationRate;

    private void Update()
    {
        transform.Rotate(m_RotationRate * Time.deltaTime);
    }
}
