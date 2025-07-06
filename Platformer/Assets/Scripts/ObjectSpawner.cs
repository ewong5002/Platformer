using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ObjectSpawner : MonoBehaviour
{
    public enum ObjectType { Exit, Gem, Enemy, HealthItem }

    [Header("References")]
    public Tilemap tilemap;
    public GameObject[] objectPrefabs; // [0] Exit, [1] Gem, [2] Enemy

    [Header("Settings")]
    public int maxObjects = 5;
    public float spawnInterval = 3f;
    public float enemyLifetime = 20f;
    public float healthLifetime = 10f;
    public float healthProbability = 0.4f;

    List<Vector3> m_validSpawnPositions = new List<Vector3>();
    List<GameObject> m_spawnedObjects = new List<GameObject>();
    bool m_isSpawning;
    const int MAX_GEMS = 3;
    const float EXIT_HEIGHT = 0.4f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LevelChange();
        GameManager.OnReset += LevelChange;
    }

    void Update()
    {
        if (!m_isSpawning && ActiveObjectCount() < maxObjects)
        {
            StartCoroutine(SpawnObjects());
        }

        if (!tilemap.gameObject.activeInHierarchy)
        {
            LevelChange();
        }
    }

    void LevelChange()
    {
        tilemap = GameObject.Find("Ground").GetComponent<Tilemap>();
        GatherValidPositions();
        DestroySpawnedObjects();
        SpawnIntialObjects();
        StartCoroutine(SpawnObjects());
    }

    int ActiveObjectCount() =>
        m_spawnedObjects.Count(obj => obj != null && (obj.CompareTag("Enemy") || obj.CompareTag("Health")));

    void SpawnIntialObjects()
    {
        SpawnObject(ObjectType.Exit);

        for (int i = 0; i < MAX_GEMS; i++)
        {
            SpawnObject(ObjectType.Gem);
        }
    }

    ObjectType RandomObjectType()
    {
        float randomChoice = Random.value;

        if (randomChoice <= healthProbability)
        {
            return ObjectType.HealthItem;
        }

        return ObjectType.Enemy;
    }

    IEnumerator SpawnObjects()
    {
        m_isSpawning = true;
        while (ActiveObjectCount() < maxObjects)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnObject(RandomObjectType());
        }
        m_isSpawning = false;
    }

    void SpawnObject(ObjectType type)
    {
        if (m_validSpawnPositions.Count == 0) return;

        Vector3 spawnPosition = FindValidSpawnPosition(type);

        if (spawnPosition == Vector3.zero) return;

        GameObject newObject = Instantiate(objectPrefabs[(int)type],
            (type == ObjectType.Exit) ? GetPlatformPosition(spawnPosition) : spawnPosition, Quaternion.identity);
        m_spawnedObjects.Add(newObject);

        if (type == ObjectType.HealthItem)
        {
            StartCoroutine(DestroyObjectsAfterTime(newObject, healthLifetime));
        }
        else if (type == ObjectType.Enemy)
        {
            StartCoroutine(DestroyObjectsAfterTime(newObject, enemyLifetime));
        }
    }

    Vector3 FindValidSpawnPosition(ObjectType type)
    {
        for (int i = 0; i < Mathf.Min(50, m_validSpawnPositions.Count); i++)
        {
            int randomIndex = Random.Range(0, m_validSpawnPositions.Count);
            Vector3 testPosition = m_validSpawnPositions[randomIndex];

            if (type == ObjectType.Exit)
            {
                Vector3 exitPosition = GetPlatformPosition(testPosition);
                if (exitPosition != testPosition)
                {
                    m_validSpawnPositions.Remove(testPosition);
                    return exitPosition;
                }
            }
            else if (!Physics2D.OverlapCircle(testPosition, 0.5f))
            {
                m_validSpawnPositions.Remove(testPosition);
                return testPosition;
            }
        }
        
        if (m_validSpawnPositions.Count > 0)
        {
            Vector3 fallbackPosition = m_validSpawnPositions[0];
            m_validSpawnPositions.RemoveAt(0);
            return (type == ObjectType.Exit) ? GetPlatformPosition(fallbackPosition) : fallbackPosition;
        }

        return Vector3.zero;
    }

    IEnumerator DestroyObjectsAfterTime(GameObject gameObject, float time)
    {
        yield return new WaitForSeconds(time);

        if (gameObject)
        {
            m_spawnedObjects.Remove(gameObject);
            m_validSpawnPositions.Add(gameObject.transform.position);
            Destroy(gameObject);
        }
    }

    void DestroySpawnedObjects()
    {
        foreach (GameObject obj in m_spawnedObjects)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        m_spawnedObjects.Clear();
    }

    Vector3 GetPlatformPosition(Vector3 pos)
    {
        RaycastHit2D hit = Physics2D.Raycast(
            pos,
            Vector2.down,
            3f,
            LayerMask.GetMask("Ground")
        );

        if (hit.collider != null)
        {
            return hit.point + Vector2.up * EXIT_HEIGHT;
        }

        return pos;
    }

    void GatherValidPositions()
    {
        m_validSpawnPositions.Clear();
        BoundsInt boundsInt = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(boundsInt);
        Vector3 start = tilemap.CellToWorld(new Vector3Int(boundsInt.xMin, boundsInt.yMin, 0));

        for (int x = 0; x < boundsInt.size.x; x++)
        {
            for (int y = 0; y < boundsInt.size.y; y++)
            {
                TileBase tile = allTiles[x + y * boundsInt.size.x];
                if (tile != null)
                {
                    Vector3 place = start + new Vector3(x + 0.5f, y + 1.5f, 0);
                    m_validSpawnPositions.Add(place);
                }
            }
        }

        m_validSpawnPositions = m_validSpawnPositions.OrderBy(x => Random.value).ToList();
    }
}
