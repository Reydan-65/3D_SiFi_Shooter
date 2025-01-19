using UnityEngine;

public class EntitiesSpawner : Spawner
{
    [Space(10)]
    [SerializeField] private Entity[] m_EntityPrefabs;

    //[SerializeField] private AI_PatrolArea m_PatrolArea;

    protected override void Spawn()
    {
        Vector3 spawmPosition = m_Area.GetRandomInsideZone();

        for (int i = 0; i < m_NumSpawns; ++i)
        {
            int index = Random.Range(0, m_EntityPrefabs.Length);

            //if (IsInCameraView(spawmPosition) == false)
            //{
                GameObject entity = Instantiate(m_EntityPrefabs[index].gameObject);

                //if (entity.transform.root.TryGetComponent(out Drone Drone) == true)
                //{
                //    SetTeamID(m_TeamID);

                //    if (m_TeamID == 0) { Drone.Nickname = "Neutral"; Drone.TeamId = 0; }
                //    if (m_TeamID == 1) { Drone.Nickname = "Freandly"; Drone.TeamId = 1; }
                //    if (m_TeamID == 2) { Drone.Nickname = "Enemy Drone"; Drone.TeamId = 2; }

                //    if (m_PatrolArea != null)
                //    {
                //        if (Drone.TryGetComponent(out AI_Controller controller) == true)
                //        {
                //            controller.AISpawnedDrone = true;

                //            if (controller._AIBehaviour == AI_Controller.AIBehaviour.PatrolArea)
                //                controller.PointPatrol = m_PatrolArea;
                //        }
                //    }
                //}

                entity.transform.position = spawmPosition;
            //}
        }
    }
}
