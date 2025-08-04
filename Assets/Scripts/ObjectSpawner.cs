using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class ObjectSpawner : MonoBehaviour
{
    public enum ObjectType { Enemy, HealthItem, SpeedItem }

    [Header("References")]
    public Tilemap tilemap;
    public GameObject[] objectPrefabs;

    [Header("Settings")]
    public int maxObjects = 5;
    public float spawnInterval = 5f;
    public float enemyLifetime = 15f;
    public float consumableLifetime = 10f;
    public float healthProbability = 0.45f;
    public float speedProbability = 0.35f;

    List<Vector3> m_validSpawnPositions = new List<Vector3>();
    List<GameObject> m_spawnedObjects = new List<GameObject>();
    bool m_isSpawning = false;

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
        StartCoroutine(SpawnObjects());
    }

    int ActiveObjectCount()
    {
        m_spawnedObjects.RemoveAll(item => item == null);
        return m_spawnedObjects.Count;
    }


    ObjectType RandomObjectType()
    {
        float randomChoice = Random.value;

        if (randomChoice <= healthProbability)
        {
            return ObjectType.HealthItem;
        }
        else if (randomChoice <= (speedProbability + healthProbability))
        {
            return ObjectType.SpeedItem;
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

    bool PositionHasObject(Vector3 positionToCheck)
    {
        return m_spawnedObjects.Any(checkObj => checkObj && Vector3.Distance(checkObj.transform.position, positionToCheck) < 1.0f);
    }

    void SpawnObject(ObjectType type)
    {
        if (m_validSpawnPositions.Count == 0) return;

        Vector3 spawnPosition = Vector3.zero;
        bool validPositionFound = false;

        while (!validPositionFound && m_validSpawnPositions.Count > 0)
        {
            int randomIndex = Random.Range(0, m_validSpawnPositions.Count);
            Vector3 potentialPosition = m_validSpawnPositions[randomIndex];
            Vector3 leftPosition = potentialPosition + Vector3.left;
            Vector3 rightPosition = potentialPosition + Vector3.right;

            if (!PositionHasObject(leftPosition) && !PositionHasObject(rightPosition))
            {
                spawnPosition = potentialPosition;
                validPositionFound = true;
            }

            m_validSpawnPositions.RemoveAt(randomIndex);
        }

        if (validPositionFound)
        {
            ObjectType objectType = RandomObjectType();
            GameObject gameObject = Instantiate(objectPrefabs[(int)objectType], spawnPosition, Quaternion.identity);
            m_spawnedObjects.Add(gameObject);

            if (objectType != ObjectType.Enemy)
            {
                StartCoroutine(DestroyObjectsAfterTime(gameObject, consumableLifetime));
            }
            else
            {
                StartCoroutine(DestroyObjectsAfterTime(gameObject, enemyLifetime));
            }
        }
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
                    Vector3 place = start + new Vector3(x + 0.5f, y + 2f, 0);
                    m_validSpawnPositions.Add(place);
                }
            }
        }
    }
}
