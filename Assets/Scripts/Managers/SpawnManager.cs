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
        SpawnCameraFocusObject();
    }

    void SpawnPlayer()
    {
        GameObject player;

        player = SpawnObject(playerObject);
        player.transform.position = new Vector3((groundSizeX / 2) * 1, 10, 5 * 1);

        camera.GetComponent<CinemachineVirtualCamera>().Follow = player.transform;
        GameManager.Instance.player = player.GetComponent<Player>();
    }

    // Метод для создания пустого объекта для фокуса камеры
    void SpawnCameraFocusObject()
    {
        // Создаем пустой объект в центре карты
        GameObject cameraFocusObject = new GameObject("CameraFocusObject");
        cameraFocusObject.transform.position = new Vector3((groundSizeX / 2), 0, (groundSizeY / 2));

        // Настраиваем камеру, чтобы она смотрела на этот объект
        camera.GetComponent<CinemachineVirtualCamera>().LookAt = cameraFocusObject.transform;
    }

    void SpawnGround()
    {
        GameObject spawnedTile;

        for (int i = 0; i < groundSizeX; i++)
        {
            for (int j = 0; j < groundSizeY; j++)
            {
                if (i == 0 || j == 0 || i == groundSizeX - 1 || j == groundSizeY - 1)
                {
                    GameObject spawnedTileBounds = SpawnObject(groundTileObject);
                    spawnedTileBounds.transform.position = new Vector3(i * 1, groundTileObject.GetComponent<Collider>().bounds.size.y, j * 1);

                    // Добавляем тег "Ground" для объекта по краям
                    spawnedTileBounds.tag = "Ground";
                }
                else
                {
                    spawnedTile = SpawnObject(groundTileObject);
                    spawnedTile.transform.position = new Vector3(i * 1, 0, j * 1);

                    // Можно добавить тег "Ground" и для центральных блоков, если это нужно
                    spawnedTile.tag = "Ground";
                }
            }
        }
    }


    public void SpawnWall(Vector3 startPosition)
    {
        int width = 5;
        int height = 3;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                BlockType blockType = BlockManager.Instance.GetBlockType(LevelManager.Instance.level % 10);  // Листовой блок для начальных уровней
                GameObject block = Instantiate(blockType.prefab, new Vector3(startPosition.x + x, startPosition.y + y, startPosition.z), Quaternion.identity);
                BlockManager.Instance.SetupBlock(block, blockType);
                LevelManager.Instance.AddBlock(block);
            }
        }
    }

    public void SpawnMediumHouse(Vector3 startPosition)
    {
        int width = 7;
        int height = 4;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < width; z++)
                {
                    BlockType blockType = BlockManager.Instance.GetBlockType(1);  // Трава, деревянные блоки
                    GameObject block = Instantiate(blockType.prefab, new Vector3(startPosition.x + x, startPosition.y + y, startPosition.z + z), Quaternion.identity);
                    BlockManager.Instance.SetupBlock(block, blockType);
                    LevelManager.Instance.AddBlock(block);
                }
            }
        }
    }

    public void SpawnTwoStoryBuilding(Vector3 startPosition)
    {
        int width = 7;
        int height = 6; // Два этажа по 3 блока

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < width; z++)
                {
                    BlockType blockType = BlockManager.Instance.GetBlockType(2);  // Деревянные и каменные блоки
                    GameObject block = Instantiate(blockType.prefab, new Vector3(startPosition.x + x, startPosition.y + y, startPosition.z + z), Quaternion.identity);
                    BlockManager.Instance.SetupBlock(block, blockType);
                    LevelManager.Instance.AddBlock(block);
                }
            }
        }
    }

    public void SpawnFortress(Vector3 startPosition)
    {
        int width = 10;
        int height = 8;

        // Основание
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < width; z++)
                {
                    BlockType blockType = BlockManager.Instance.GetBlockType(3);  // Каменные или кирпичные блоки
                    GameObject block = Instantiate(blockType.prefab, new Vector3(startPosition.x + x, startPosition.y + y, startPosition.z + z), Quaternion.identity);
                    BlockManager.Instance.SetupBlock(block, blockType);
                    LevelManager.Instance.AddBlock(block);
                }
            }
        }

        // Башни
        SpawnTower(new Vector3(startPosition.x, startPosition.y, startPosition.z));
        SpawnTower(new Vector3(startPosition.x + width - 1, startPosition.y, startPosition.z + width - 1));
    }

    public void SpawnCastle(Vector3 startPosition)
    {
        int width = 15;
        int height = 10;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < width; z++)
                {
                    BlockType blockType = BlockManager.Instance.GetBlockType(4);  // Более прочные блоки, кирпичи и железо
                    GameObject block = Instantiate(blockType.prefab, new Vector3(startPosition.x + x, startPosition.y + y, startPosition.z + z), Quaternion.identity);
                    BlockManager.Instance.SetupBlock(block, blockType);
                    LevelManager.Instance.AddBlock(block);
                }
            }
        }

        // Дополнительные элементы (башни, ворота)
        SpawnTower(new Vector3(startPosition.x, startPosition.y, startPosition.z));
        SpawnTower(new Vector3(startPosition.x + width - 1, startPosition.y, startPosition.z + width - 1));
        // Добавьте ворота или другие декоративные элементы
    }

    public void SpawnTower(Vector3 startPosition)
    {
        int towerHeight = 10;  // Высота башни
        int towerWidth = 3;    // Ширина башни

        for (int x = 0; x < towerWidth; x++)
        {
            for (int y = 0; y < towerHeight; y++)
            {
                for (int z = 0; z < towerWidth; z++)
                {
                    // Получаем тип блока для башни
                    BlockType blockType = BlockManager.Instance.GetBlockType(3);  // Например, каменный блок для башни
                    GameObject block = Instantiate(blockType.prefab, new Vector3(startPosition.x + x, startPosition.y + y, startPosition.z + z), Quaternion.identity);

                    // Настраиваем блок
                    BlockManager.Instance.SetupBlock(block, blockType);
                    LevelManager.Instance.AddBlock(block);
                }
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
