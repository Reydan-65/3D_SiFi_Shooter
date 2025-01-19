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

            obstacleType = impact.SetObstacleType(collider.transform.GetComponent<ObstacleType>().type);

            if (obstacleType != ObstacleType.Type.Null)
            {
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
        if(collider)
            OnProjectileLifeEnd(hit.collider, hit.point, hit.normal);

        return base.OnHitObstacles(hit);
    }
}
