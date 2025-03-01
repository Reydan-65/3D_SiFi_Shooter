using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitiesSpawner : Spawner
{
    [Space(10)]
    [SerializeField] private Entity[] m_EntityPrefabs;

    private List<Drone> m_SpawnedDronesList = new List<Drone>();
    public List<Drone> SpawnedDronesList => m_SpawnedDronesList;

    protected override void Spawn()
    {
        Vector3 spawmPosition = m_Area.GetRandomInsideZone();

        for (int i = 0; i < m_NumSpawns; ++i)
        {
            int index = Random.Range(0, m_EntityPrefabs.Length);

            GameObject entity = Instantiate(m_EntityPrefabs[index].gameObject);

            if (entity.transform.root.TryGetComponent(out Drone Drone) == true)
            {
                SetTeamID(m_TeamID);

                if (m_TeamID == 0) { Drone.Nickname = "Team 0"; Drone.TeamId = 0; }
                if (m_TeamID == 1) { Drone.Nickname = "Team 1"; Drone.TeamId = 1; }
                if (m_TeamID == 2) { Drone.Nickname = "Team 2"; Drone.TeamId = 2; }
                if (m_TeamID == 3) { Drone.Nickname = "Team 3"; Drone.TeamId = 3; }
            }

            entity.transform.position = spawmPosition;
            entity.GetComponent<AIDrone>().MovementArea = m_Area;
            entity.GetComponent<Drone>().EventOnDeath.AddListener(() => OnDroneDeath(entity));
            m_SpawnedDronesList.Add(entity.GetComponent<Drone>());
        }
    }

    private void OnDroneDeath(GameObject entity)
    {
        StartCoroutine(NextEntitySpawn(entity));

        entity.GetComponent<Drone>().EventOnDeath.RemoveListener(() => OnDroneDeath(entity));
    }

    private IEnumerator NextEntitySpawn(GameObject entity)
    {
        yield return new WaitForSeconds(m_RespawnTime);
        CountSpawned--;
        m_SpawnedDronesList.Remove(entity.GetComponent<Drone>());
    }
}
