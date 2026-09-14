using UnityEngine;

// Hace que una zona se sienta viva sin arte final: cada cierto tiempo (a un
// intervalo aleatorio) hace spawn de un prefab pequeño de un pool - un pajaro,
// luciernagas sobre el rio al atardecer, una hoja en el viento - en un punto al
// azar dentro de un radio. El prefab debe apagarse o autodestruirse solo (un
// ParticleSystem con "Stop Action: Destroy" lo hace sin código extra).
public class AmbientLifeSpawner : MonoBehaviour
{
    public GameObject[] prefabPool;
    public float radius = 5f;
    public float minInterval = 2f;
    public float maxInterval = 6f;

    private float timer;

    private void Start()
    {
        ScheduleNext();
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer > 0f) return;

        SpawnOne();
        ScheduleNext();
    }

    private void ScheduleNext()
    {
        timer = Random.Range(minInterval, maxInterval);
    }

    private void SpawnOne()
    {
        if (prefabPool == null || prefabPool.Length == 0) return;

        GameObject prefab = prefabPool[Random.Range(0, prefabPool.Length)];
        Vector2 offset = Random.insideUnitCircle * radius;
        Vector3 spawnPos = transform.position + new Vector3(offset.x, 0f, offset.y);

        Instantiate(prefab, spawnPos, Quaternion.identity);
    }
}
