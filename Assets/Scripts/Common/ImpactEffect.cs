using UnityEngine;

public class ImpactEffect : ImpactEffectBase
{
    [Header("Destroyed Object Prefab")]
    [SerializeField] private GameObject m_ObjectPrefab;

    [SerializeField] private AudioClip[] audioClips;

    private ObstacleType.Type obstacleType;

    public AudioClip[] AudioClips => audioClips;

    public ObstacleType.Type SetObstacleType(ObstacleType.Type type)
    {
        obstacleType = type;
        return obstacleType;
    }
}
