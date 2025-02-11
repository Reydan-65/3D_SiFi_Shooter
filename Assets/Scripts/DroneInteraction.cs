using UnityEngine;

public class DroneInteraction : MonoBehaviour
{
    [SerializeField] private EntitiesSpawner m_Spawner;

    public void DestroyTheDrones()
    {
        if (m_Spawner != null)
        {
            foreach (Drone drone in m_Spawner.SpawnedDronesList)
            {
                drone.ApplyDamage(drone.MaxHitPoints, drone);
            }
        }
    }

    public void DisableDrones()
    {
        if (m_Spawner != null)
        {
            foreach (Drone drone in m_Spawner.SpawnedDronesList)
            {
                drone.GetComponent<AIDrone>().enabled = false;
            }
        }
    }
}
