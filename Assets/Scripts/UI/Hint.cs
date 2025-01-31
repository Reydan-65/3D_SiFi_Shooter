using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class Hint : MonoBehaviour
{
    [SerializeField] private GameObject m_Hint;
    [SerializeField] private float m_ActiveRadius;
    [SerializeField] private GameObject m_HintView;

    private Canvas m_Canvas;
    private Transform m_Target;
    private Transform m_LookTransform;

    private void Start()
    {
        m_Canvas = GetComponent<Canvas>();
        m_Canvas.worldCamera = Camera.main;
        m_LookTransform = Camera.main.transform;
        m_Target = GameObject.FindGameObjectWithTag("Player").transform;
        m_HintView.SetActive(false);
    }

    private void Update()
    {
        m_Hint.transform.LookAt(m_LookTransform);

        if (transform.parent.GetComponent<TriggerInteractAction>().InteractAmount == 0) return;
        
        if (Vector3.Distance(transform.position, m_Target.position) < m_ActiveRadius &&
            m_Target.transform.root.GetComponent<CharacterMovement>().IsGrounded == true &&
            m_Target.transform.root.GetComponent<CharacterMovement>().IsAiming == false)
        {
            m_HintView.SetActive(true);
            m_Hint.SetActive(true);
        }
        else
        {
            m_HintView.SetActive(false);
            m_Hint.SetActive(false);
        }
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, m_ActiveRadius);
    }

#endif
}
