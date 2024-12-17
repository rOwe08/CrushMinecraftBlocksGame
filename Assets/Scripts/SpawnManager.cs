using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    public GameObject groundTileObject;
    public GameObject playerObject;
    public GameObject projectilePrefab;

    public GameObject camera;

    public int groundSizeX = 30;
    public int groundSizeY = 30;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        SpawnGround();
        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        GameObject player;

        player = SpawnObject(playerObject);
        player.transform.position = new Vector3((groundSizeX / 2) * 1, 10, 5 * 1);

        // Camera settings
        camera.GetComponent<CinemachineVirtualCamera>().Follow = player.transform;
        camera.GetComponent<CinemachineVirtualCamera>().LookAt = player.transform;

        GameManager.Instance.player = player.GetComponent<Player>();
    }

    void SpawnGround()
    {
        GameObject spawnedTile;
     
        for (int i = 0; i < groundSizeX; i++)
        {
            for(int j = 0; j < groundSizeY; j++)
            {
                if (i == 0 || j == 0 || i == groundSizeX - 1 || j == groundSizeY - 1)
                {
                    GameObject spawnedTileBounds = SpawnObject(groundTileObject);
                    spawnedTileBounds.transform.position = new Vector3(i * 1, groundTileObject.GetComponent<Collider>().bounds.size.y, j * 1);
                }
                else
                {
                    spawnedTile = SpawnObject(groundTileObject);
                    spawnedTile.transform.position = new Vector3(i * 1, 0, j * 1);
                }
            }
        }
    }

    public void SpawnBuilding(int width, int height)
    {
        // Вычисляем центр карты
        float centerX = groundSizeX / 2f;
        float centerZ = groundSizeY / 2f;

        // Позиция основания здания по центру карты
        Vector3 startPosition = new Vector3(centerX - (width / 2f), 1f, centerZ - (width / 2f));

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject block = SpawnObject(groundTileObject);
                block.transform.position = new Vector3(startPosition.x + x, startPosition.y + y, startPosition.z);

                block.AddComponent<DestructibleBlock>();

                // Сообщаем LevelManager о добавлении нового блока
                LevelManager.Instance.AddBlock();
            }
        }
    }

    public GameObject SpawnObject(GameObject gameObjectToSpawn, GameObject parentObject = null)
    {
        GameObject gameObject;

        if (parentObject != null)
        {
            gameObject = Instantiate(gameObjectToSpawn, parentObject.transform);
        }
        else
        {
            gameObject = Instantiate(gameObjectToSpawn);
        }
        
        return gameObject;
    }
}
