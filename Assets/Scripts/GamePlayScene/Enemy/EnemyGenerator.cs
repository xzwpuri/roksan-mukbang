using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private List<GameObject> enemyPrefabs = new List<GameObject>();

    [Header("Spawn Settings")]
    [SerializeField] private float minSpawnDistance = 6f;
    [SerializeField] private float distancePadding = 3f;
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private int baseSpawnCount = 3;

    [Header("Difficulty Multipliers")]
    [SerializeField] private float enemyHpMultiplier = 1f;
    [SerializeField] private float enemyMoveSpeedMultiplier = 1f;

    private readonly List<System.Func<int, IEnumerator>> spawnPatterns = new();
    private Coroutine spawnRoutine;

    private void Awake()
    {
        spawnPatterns.Add(count => SpawnRandomScatter(count));
        spawnPatterns.Add(count => SpawnCardinalLine(count));
        spawnPatterns.Add(count => SpawnCircle(count));
    }

    public void ConfigureDifficulty(float interval, int spawnCount, float hpMultiplier, float moveSpeedMultiplier)
    {
        spawnInterval = interval;
        baseSpawnCount = spawnCount;
        enemyHpMultiplier = hpMultiplier;
        enemyMoveSpeedMultiplier = moveSpeedMultiplier;
    }

    public void BeginSpawn()
    {
        if (spawnRoutine != null)
            StopCoroutine(spawnRoutine);

        spawnRoutine = StartCoroutine(SpawnLoop());
    }

    public void StopSpawn()
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }
    }

    private IEnumerator SpawnLoop()
    {
        var wait = new WaitForSeconds(spawnInterval);
        while (true)
        {
            yield return wait;
            int spawnCount = Mathf.Max(1, baseSpawnCount + Random.Range(-1, 2));
            StartCoroutine(spawnPatterns[Random.Range(0, spawnPatterns.Count)](spawnCount));
        }
    }

    private IEnumerator SpawnRandomScatter(int count)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnEnemyAt(RandomPositionAroundPlayer());
            yield return null;
        }
    }

    private IEnumerator SpawnCardinalLine(int count)
    {
        Vector2 direction = GetRandomCardinal();
        Vector2 origin = (Vector2)player.position + direction * (minSpawnDistance + distancePadding);
        float spacing = 2f;
        for (int i = 0; i < count; i++)
        {
            Vector2 offset = Vector2.Perpendicular(direction) * (i - count / 2f) * spacing;
            SpawnEnemyAt(origin + offset);
            yield return null;
        }
    }

    private IEnumerator SpawnCircle(int count)
    {
        float radius = minSpawnDistance + distancePadding;
        for (int i = 0; i < count; i++)
        {
            float angle = (Mathf.PI * 2f / count) * i;
            Vector2 position = (Vector2)player.position + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            SpawnEnemyAt(position);
            yield return null;
        }
    }

    private Vector2 RandomPositionAroundPlayer()
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        float distance = Random.Range(minSpawnDistance, minSpawnDistance + distancePadding);
        return (Vector2)player.position + randomDirection * distance;
    }

    private Vector2 GetRandomCardinal()
    {
        return Random.Range(0, 4) switch
        {
            0 => Vector2.right,
            1 => Vector2.left,
            2 => Vector2.up,
            _ => Vector2.down,
        };
    }

    private void SpawnEnemyAt(Vector2 position)
    {
        if (enemyPrefabs.Count == 0)
        {
            Debug.LogWarning("Enemy prefabs are not assigned in EnemyGenerator.");
            return;
        }

        GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
        GameObject enemy = Instantiate(prefab, position, Quaternion.identity);

        if (enemy.TryGetComponent(out IUnit unit))
        {
            unit.Hp *= enemyHpMultiplier;
            unit.MoveSpeed *= enemyMoveSpeedMultiplier;
        }
    }
}