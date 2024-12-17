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
                spawnedTile = SpawnObject(groundTileObject);
                spawnedTile.transform.position = new Vector3(i * 1, 0, j * 1);

                if(i == 0 || j == 0 || i == groundSizeX - 1 || j == groundSizeY - 1)
                {
                    GameObject spawnedTileBounds = SpawnObject(groundTileObject);
                    spawnedTileBounds.transform.position = new Vector3(i * 1, groundTileObject.GetComponent<Collider>().bounds.size.y, j * 1);
                }

            }
        }
    }

    public void SpawnBuilding(Vector3 position, int width, int height)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject block = SpawnObject(groundTileObject);
                block.transform.position = new Vector3(position.x + x, position.y + y, position.z);
                block.AddComponent<DestructibleBlock>();
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
