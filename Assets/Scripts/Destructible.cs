using UnityEngine;

public class Destructible : DestructibleBase
{
    [SerializeField] private int m_ScoreValue;
    public int ScoreValue => m_ScoreValue;

    protected override void OnDeath()
    {
        // Вычитаем количество призванных объектов у Spawner'а, если
        // уничтоженный объект был создан этим Spawner'ом
        //if (transform.root.TryGetComponent(out AI_Controller controller) == true)
        //{
        //    if (controller.AISpawnedDrone == true)
        //    {
        //        EntitiesSpawner spawner = FindAnyObjectByType<EntitiesSpawner>();
        //        spawner.CountSpawned--;
        //    }
        //}

        //if (GetComponentInChildren<TrailRenderer>() != null)
        //    GetComponentInChildren<TrailRenderer>().enabled = false;
        //if (GetComponentInChildren<SpriteRenderer>() != null)
        //    GetComponentInChildren<SpriteRenderer>().enabled = false;
        //if (GetComponentInChildren<Collider2D>() != null)
        //    GetComponentInChildren<Collider2D>().enabled = false;

        //if (TryGetComponent(out Rigidbody2D rb) == true)
        //    rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        //if (gameObject.TryGetComponent(out Drone Drone) == true ||
        //    gameObject.TryGetComponent(out Debris debris) == true)
        //    Destroy(gameObject, 2);
        //else
            Destroy(gameObject);

        base.OnDeath();
    }
}
