using UnityEngine;

public abstract class Spawner : MonoBehaviour
{
    public enum SpawnMode
    {
        Start,
        Loop
    }

    [Range(0, 3)]
    [SerializeField] protected int m_TeamID;
    [SerializeField] protected CubeArea m_Area;
    [SerializeField] protected SpawnMode m_SpawnMode;
    [SerializeField] protected int m_NumSpawns;

    [Header("If SpawnMode = Loop")]
    [SerializeField] protected int m_MaxCountSpawned;
    [SerializeField] protected float m_RespawnTime;

    protected Camera m_Camera;

    protected int m_CountSpawned;
    protected float m_Timer;

    public int CountSpawned { get => m_CountSpawned; set => m_CountSpawned = value; }

    protected virtual void Start()
    {
        m_Camera = Camera.main;

        m_CountSpawned = 0;

        if (m_SpawnMode == SpawnMode.Start)
        {
            Spawn();
        }

        m_Timer = m_RespawnTime;
    }

    protected virtual void FixedUpdate()
    {
        if (m_Timer > 0)
            m_Timer -= Time.deltaTime;

        if (m_SpawnMode == SpawnMode.Loop && m_Timer < 0)
        {
            if (m_CountSpawned < m_MaxCountSpawned)
            {
                Spawn();

                m_CountSpawned++;

                m_Timer = m_RespawnTime;
            }
        }
    }

    protected virtual void Spawn() { }

    protected int SetTeamID(int teamID)
    {
        return teamID;
    }

    protected bool IsInCameraView(Vector3 position)
    {
        Vector3 screenPoint = m_Camera.GetComponent<Camera>().WorldToScreenPoint(position);
        return screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1 && screenPoint.z > 0 && screenPoint.z < 1;
    }
}
