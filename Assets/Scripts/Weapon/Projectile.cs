using UnityEngine;

public class Projectile : ProjectileBase
{
    [SerializeField] protected ImpactEffect m_ImpactEffect;

    private ObstacleType.Type obstacleType;

    protected override void OnHit(Destructible destructible)
    {
        base.OnHit(destructible);

        OnTargetDestroyed(destructible);
    }

    /// <summary>
    /// Начисление очков за уничтожение объектов.
    /// Начисление количества убитых противников.
    /// Исключить корабль игрока.
    /// </summary>
    /// <param name="destructible"></param>
    public void OnTargetDestroyed(Destructible destructible)
    {
        if (destructible == null) return;

        if (destructible.HitPoints <= 0)
        {
            //if (m_Parent == Player.Instance.ActiveDrone)
            //{
            //    if (destructible != Player.Instance.ActiveDrone)
            //    {
            //        Player.Instance.AddScore(destructible.ScoreValue);

            //        if (destructible is Drone)
            //        {
            //            if (destructible.HitPoints <= 0)
            //                Player.Instance.AddKill();
            //        }
            //    }
            //}
        }
    }

    protected override void OnProjectileLifeEnd(Collider collider, Vector3 position, Vector3 normal)
    {
        if (m_ImpactEffect != null)
        {
            ImpactEffect impact = Instantiate(m_ImpactEffect, position, Quaternion.LookRotation(normal));
            impact.transform.SetParent(collider.transform);

            if (collider.GetComponent<ObstacleType>() != null)
                obstacleType = impact.SetObstacleType(collider.transform.GetComponent<ObstacleType>().type);

            if (obstacleType != ObstacleType.Type.Null)
            {
                float pitch = Random.Range(0.975f, 1.025f);
                impact.GetComponent<AudioSource>().pitch = pitch;

                if (obstacleType == ObstacleType.Type.Stone)
                {
                    impact.transform.GetChild(0).gameObject.SetActive(true);
                    impact.GetComponent<AudioSource>().clip = impact.AudioClips[0];
                }

                if (obstacleType == ObstacleType.Type.Metall)
                {
                    if (collider.transform.root.GetComponent<Drone>() == true)
                        impact.GetComponent<AudioSource>().volume /= 2;

                    impact.transform.GetChild(1).gameObject.SetActive(true);
                    impact.GetComponent<AudioSource>().clip = impact.AudioClips[1];
                }

                if (obstacleType == ObstacleType.Type.Flesh)
                {
                    //impact.transform.GetChild(2).gameObject.SetActive(true);
                    //AudioSource source = collider.transform.root.GetComponent<AudioSource>();
                    //source.clip = collider.transform.root.GetComponent<SpaceSoldier>().AudioClips[Random.Range(0, 3)];
                    //source.Play();
                }

                if (obstacleType != ObstacleType.Type.Flesh)
                    impact.GetComponent<AudioSource>().Play();
            }
        }

        Destroy(gameObject);
    }

    // Попадание в препятствие
    protected override RaycastHit OnHitObstacles(RaycastHit hit)
    {
        Collider collider = hit.collider.GetComponent<Collider>();

        //if (collider.GetComponent<Wall>() == true || collider.transform.root.GetComponent<GravityWell>() == true)
        if (collider && !collider.isTrigger)
            OnProjectileLifeEnd(hit.collider, hit.point, hit.normal);

        return base.OnHitObstacles(hit);
    }
}
