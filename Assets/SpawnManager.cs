using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject groundTileObject;
    
    // Start is called before the first frame update
    void Start()
    {
        SpawnGround();
    }

    void SpawnGround()
    {
        GameObject spawnedTile;

        for (int i = 0; i < 100; i++)
        {
            for(int j = 0; j < 100; j++)
            {
                spawnedTile = SpawnObject(groundTileObject);
                spawnedTile.transform.position = new Vector3(i * 1, 0, j * 1);
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
