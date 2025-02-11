using UnityEngine;

public class PatrolPathNode : MonoBehaviour
{
    [SerializeField] private float m_IdleTime;
    public float Idle_Time => m_IdleTime;

    private bool isOccupied;
    public bool IsOccupied { get => isOccupied; set => isOccupied = value; }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.5f);
    }

#endif

}
