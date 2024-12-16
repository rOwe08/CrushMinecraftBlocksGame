using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject groundTileObject;
    public GameObject playerObject;
    public GameObject camera;

    public int groundSizeX = 30;
    public int groundSizeY = 30;

    // Start is called before the first frame update
    void Start()
    {
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

    GameObject SpawnObject(GameObject gameObjectToSpawn, GameObject parentObject = null)
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
