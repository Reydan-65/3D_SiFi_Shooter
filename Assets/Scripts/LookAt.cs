using UnityEngine;

public class LookAt : MonoBehaviour
{
    [SerializeField] private Transform m_Target;

    private void Update()
    {
        transform.LookAt(m_Target);
    }
}
