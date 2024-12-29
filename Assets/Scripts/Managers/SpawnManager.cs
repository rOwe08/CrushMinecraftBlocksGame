using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using YG;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    public GameObject groundTileObject;
    public GameObject playerObject;
    public GameObject projectilePrefab;

    public GameObject camera;

    public List<GameObject> groundObjectsPrefabs;

    private int indexOfGround;

    public List<GameObject> groundBlocks;

    public int groundSizeX = 30;
    public int groundSizeY = 30;

    public float armageddonDuration = 10f; // Время действия Армагеддона
    public float armageddonInterval = 1f; // Интервал между падениями ядер
    public float projectileSpawnHeight = 30f; // Высота спавна ядер

    public GameObject blockPrefab;  // Префаб блока для пола магазина

    public GameObject shopPrefab;  // Массив префабов для магазина
    public Material[] shopMaterials;  // Массив префабов для магазина

    private int gridWidth = 10;  // Ширина сетки (например, 10 блоков)
    private int gridHeight = 10;  // Высота сетки (например, 10 блоков)
    private float blockSize = 1f;  // Размер блока (размер одного блока в юнитах)
    private Vector3 shopPosition = new Vector3(50f, 0f, -20f);  // Позиция магазина справа от основной локации

    private Dictionary<int, GameObject> shopBlocks = new Dictionary<int, GameObject>(); // Связь между индексом и блоком

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

        LoadData();

        SpawnGround();
        UpdateGroundMaterials();

        SpawnPlayer();
        SpawnCameraFocusObject();

        SpawnShop();
    }

    void SpawnPlayer()
    {
        GameObject player;

        player = SpawnObject(playerObject);
        player.transform.position = new Vector3((groundSizeX / 2) * 1, 10, 5 * 1);

        camera.GetComponent<CinemachineVirtualCamera>().Follow = player.transform;
        GameManager.Instance.player = player.GetComponent<Player>();
    }

    public void StartArmageddon()
    {
        if (!GameManager.Instance.player.IsArmageddonActivated && !UIManager.Instance.IsUIActive())
        {
            StartCoroutine(ArmageddonCoroutine());
            GameManager.Instance.player.IsArmageddonActivated = true;
            LevelManager.Instance.ArmageddonEnded = false;
        }

    }

    void SpawnShop()
    {
        int i = 0;
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                // Позиция для каждого блока в сетке
                Vector3 blockPosition = new Vector3(x * blockSize, 0f, z * blockSize) + shopPosition;

                // Спавним блок пола
                GameObject block = Instantiate(blockPrefab, blockPosition, Quaternion.identity);

                if (x % 3 == 0 && z % 3 == 0)
                {
                    // Спавним объект магазина на блоке (чуть выше, чтобы не пересекался с полом)
                    GameObject spawnedPrefab = Instantiate(shopPrefab, blockPosition + Vector3.up, Quaternion.identity);

                    // Находим "Projectile" в заспавненном префабе
                    Transform projectileTransform = spawnedPrefab.transform.Find("Projectile");
                    if (projectileTransform != null)
                    {
                        Renderer projectileRenderer = projectileTransform.GetComponent<Renderer>();
                        if (projectileRenderer != null && i < shopMaterials.Length)
                        {
                            // Устанавливаем новый материал для заспавненного объекта
                            projectileRenderer.material = shopMaterials[i];

                            Debug.Log("Adding block " + i + " to shopBlocks");
                            shopBlocks.Add(i, block);

                        }
                        i++;
                    }
                }
            }
        }
    }

    public void UpdateShopBlockColor(int materialIndex)
    {
        if (shopBlocks.ContainsKey(materialIndex) && GameManager.Instance.player.shopMaterialsPurchased[materialIndex])
        {
            // Меняем цвет/материал блока, чтобы показать, что объект куплен
            Renderer blockRenderer = shopBlocks[materialIndex].GetComponent<Renderer>();
            if (blockRenderer != null)
            {
                blockRenderer.material.color = Color.green;
                Debug.Log("Block color updated for block " + materialIndex);
            }
            else
            {
                Debug.Log("Renderer not found for block " + materialIndex);
            }

        }
    }
    private IEnumerator ArmageddonCoroutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < armageddonDuration)
        {
            SpawnArmageddonProjectile();
            elapsedTime += armageddonInterval;

            yield return new WaitForSeconds(armageddonInterval);
        }

        LevelManager.Instance.ArmageddonEnded = true;
    }

    private void SpawnArmageddonProjectile()
    {
        // Спавним ядро в случайной позиции над блоками
        Vector3 randomPosition = new Vector3(
            Random.Range(10, groundSizeX - 10), // случайная позиция по оси X
            projectileSpawnHeight,              // высота спавна
            Random.Range(10, groundSizeY - 10)  // случайная позиция по оси Z
        );

        GameObject projectileObject = Instantiate(projectilePrefab, randomPosition, Quaternion.identity);

        // Определяем центральную точку области, куда должны лететь снаряды
        Vector3 targetPosition = new Vector3(groundSizeX / 2, 0, groundSizeY / 2); // Центр области блоков

        // Вычисляем вектор направления к цели
        Vector3 directionToTarget = (targetPosition - randomPosition).normalized;

        // Добавляем небольшой случайный угол наклона для естественности
        float angleX = Random.Range(-5f, 5f); // Маленький наклон по оси X
        float angleZ = Random.Range(-5f, 5f); // Маленький наклон по оси Z

        // Применяем наклон к направлению
        Vector3 finalDirection = Quaternion.Euler(angleX, 0, angleZ) * directionToTarget;

        // Применяем скорость с учетом направления
        projectileObject.GetComponent<Rigidbody>().velocity = 50f * finalDirection;
        projectileObject.GetComponent<Rigidbody>().mass = GameManager.Instance.player.cannonMass;
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
                    GameObject spawnedTileBounds = SpawnObject(groundObjectsPrefabs[indexOfGround]);
                    spawnedTileBounds.transform.position = new Vector3(i * 1, groundTileObject.GetComponent<Collider>().bounds.size.y, j * 1);

                    // Добавляем тег "Ground" для объекта по краям
                    spawnedTileBounds.tag = "Ground";

                    groundBlocks.Add(spawnedTileBounds);
                }
                else
                {
                    spawnedTile = SpawnObject(groundTileObject);
                    spawnedTile.transform.position = new Vector3(i * 1, 0, j * 1);

                    // Можно добавить тег "Ground" и для центральных блоков, если это нужно
                    spawnedTile.tag = "Ground";

                    groundBlocks.Add(spawnedTile);
                }
            }
        }
    }


    public void SpawnWall(Vector3 startPosition)
    {
        int width = 5;
        int height = 3;

        startPosition -= new Vector3(width / 2, 0, width / 2);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                BlockType blockType;

                // Генерация случайного числа от 0 до 100
                float randomChance = Random.Range(0f, 100f);

                // Определение типа блока на основе случайного числа
                if (randomChance <= 2f)
                {
                    blockType = BlockManager.Instance.rewardBlockTypes[0]; // Наградной блок с шансом 5%
                }
                else if (randomChance <= 10f)
                {
                    blockType = BlockManager.Instance.rewardBlockTypes[1]; // Наградной блок с шансом 10%
                }
                else
                {
                    blockType = BlockManager.Instance.GetBlockType(LevelManager.Instance.level % 10); // Основной тип блока
                }

                GameObject block = Instantiate(blockType.prefab, new Vector3(startPosition.x + x, startPosition.y + y, startPosition.z), Quaternion.identity);
                BlockManager.Instance.SetupBlock(block, blockType);
                LevelManager.Instance.AddBlock(block);
            }
        }
    }

    public void SpawnHouse(Vector3 startPosition, int width = 0, int height = 0)
    {
        if (width == 0 || height == 0)
        {
            width = 4;
            height = 2;
        }

        startPosition -= new Vector3(width / 2, 0, width / 2);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < width; z++)
                {
                    BlockType blockType;

                    // Генерация случайного числа от 0 до 100
                    float randomChance = Random.Range(0f, 100f);

                    // Определение типа блока на основе случайного числа
                    if (randomChance <= 2f)
                    {
                        blockType = BlockManager.Instance.rewardBlockTypes[0]; // Наградной блок с шансом 5%
                    }
                    else if (randomChance <= 10f)
                    {
                        blockType = BlockManager.Instance.rewardBlockTypes[1]; // Наградной блок с шансом 10%
                    }
                    else
                    {
                        blockType = BlockManager.Instance.GetBlockType(LevelManager.Instance.level % 10); // Основной тип блока
                    }
                    GameObject block = Instantiate(blockType.prefab, new Vector3(startPosition.x + x, startPosition.y + y, startPosition.z + z), Quaternion.identity);
                    BlockManager.Instance.SetupBlock(block, blockType);
                    LevelManager.Instance.AddBlock(block);
                }
            }
        }
    }

    public Vector3 SpawnFortifiedBuilding(Vector3 startPosition, int width = 0, int height = 0)
    {
        if (width == 0 || height == 0) 
        {
            width = 4;
            height = 2;
        }

        // Спавним дом в центре
        SpawnHouse(startPosition, width, height);

        startPosition -= new Vector3(width / 2, 0, width / 2);

        SpawnWalls(startPosition, width + 3, height - 1);

        return startPosition;
    }

    public void SpawnWalls(Vector3 startPosition, int width, int height)
    {

        // Спавним стены вокруг дома
        for (int x = -1; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = -1; z < width; z++)
                {
                    // Проверяем, находятся ли текущие координаты на краях (стены)
                    if (x == -1 || x == width - 1 || z == -1 || z == width - 1)
                    {
                        BlockType blockType;

                        // Генерация случайного числа от 0 до 100
                        float randomChance = Random.Range(0f, 100f);

                        // Определение типа блока на основе случайного числа
                        if (randomChance <= 2f)
                        {
                            blockType = BlockManager.Instance.rewardBlockTypes[0]; // Наградной блок с шансом 5%
                        }
                        else if (randomChance <= 10f)
                        {
                            blockType = BlockManager.Instance.rewardBlockTypes[1]; // Наградной блок с шансом 10%
                        }
                        else
                        {
                            blockType = BlockManager.Instance.GetBlockType(LevelManager.Instance.level % 10); // Основной тип блока
                        }
                        GameObject block = Instantiate(blockType.prefab, new Vector3(startPosition.x + x - 1, startPosition.y + y, startPosition.z + z - 1), Quaternion.identity);
                        BlockManager.Instance.SetupBlock(block, blockType);
                        LevelManager.Instance.AddBlock(block);
                    }
                }
            }
        }
    }

    public Vector3 SpawnFortress(Vector3 startPosition, bool WithTowers, int width = 0, int height = 0)
    {

        if (width == 0 || height == 0)
        {
            width = 5;
            height = 3;
        }

        startPosition = SpawnFortifiedBuilding(startPosition, width, height);

        int widthOfWalls = 1;
        int heightOfWalls = 2;

        if (WithTowers)
        {
            startPosition += new Vector3(1, 0, 1);
            // Башни
            SpawnWalls(new Vector3(startPosition.x + 1, startPosition.y + height, startPosition.z + 1), widthOfWalls, heightOfWalls);
            SpawnWalls(new Vector3(startPosition.x + 1, startPosition.y + height, startPosition.z + width - widthOfWalls), widthOfWalls, heightOfWalls);

            SpawnWalls(new Vector3(startPosition.x + width - widthOfWalls, startPosition.y + height, startPosition.z + 1), widthOfWalls, heightOfWalls);

            SpawnWalls(new Vector3(startPosition.x + width - widthOfWalls, startPosition.y + height, startPosition.z + width - widthOfWalls), widthOfWalls, heightOfWalls); // +
        }

        return startPosition;
    }

    public void SpawnCastle(Vector3 startPosition, bool WithTowers)
    {
        int width = 6;
        int height = 4;
        startPosition = SpawnFortress(startPosition, true, width, height);

        if ( WithTowers)
        {
            // Башни
            SpawnWalls(new Vector3(startPosition.x - width, startPosition.y, startPosition.z + width + 4), 2, 4);
            SpawnWalls(new Vector3(startPosition.x + width + 4, startPosition.y, startPosition.z + width + 4), 2, 4);
            SpawnWalls(new Vector3(startPosition.x - width, startPosition.y, startPosition.z - width), 2, 4);
            SpawnWalls(new Vector3(startPosition.x + width + 4, startPosition.y, startPosition.z - width), 2, 4);
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

    public void ChangeGround()
    {
        indexOfGround++;

        UpdateGroundMaterials();

        SaveData();
    }

    private void UpdateGroundMaterials()
    {
        foreach (GameObject spawnedGroundObject in groundBlocks)
        {
            // Получаем текущий массив материалов
            Material[] materials = spawnedGroundObject.GetComponent<MeshRenderer>().materials;

            // Меняем только нужный материал
            materials[0] = groundObjectsPrefabs[indexOfGround].GetComponent<MeshRenderer>().sharedMaterials[0];

            // Присваиваем обратно изменённый массив материалов
            spawnedGroundObject.GetComponent<MeshRenderer>().materials = materials;
        }
    }

    public void ColorPurchasedBlocksOnStart()
    {
        int i = 0; // Индекс для материалов магазина

        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                // Если это магазинный блок (например, каждый третий блок по X и Z)
                if (x % 3 == 0 && z % 3 == 0 && i < shopMaterials.Length)
                {
                    // Если объект с этим материалом был куплен
                    if (GameManager.Instance.player.shopMaterialsPurchased[i])
                    {
                        // Найдите блок в магазине и его рендерер
                        Transform shopBlock = FindShopBlockAtPosition(x, z); // Нужно реализовать этот метод
                        if (shopBlock != null)
                        {
                            Renderer blockRenderer = shopBlock.GetComponent<Renderer>();
                            if (blockRenderer != null)
                            {
                                // Окрасьте блок в зеленый цвет (или любой другой цвет)
                                blockRenderer.material.color = Color.green;
                            }
                        }
                    }
                    i++;
                }
            }
        }
    }

    private Transform FindShopBlockAtPosition(int x, int z)
    {
        // Находим объект магазина в заданной позиции (x, z)
        Vector3 blockPosition = new Vector3(x * blockSize, 0f, z * blockSize) + shopPosition;
        Collider[] colliders = Physics.OverlapSphere(blockPosition, 0.1f); // Поиск объектов рядом с блоком

        foreach (Collider col in colliders)
        {
            if (col.CompareTag("ShopBlock")) // Убедитесь, что ваши блоки имеют тег "ShopBlock"
            {
                return col.transform;
            }
        }

        return null;
    }

    public int GetSpawnedGroundIndex()
    {
        return indexOfGround + 1;
    }

    public void LoadData()
    {
        indexOfGround = YandexGame.savesData.indexOfGround;
    }

    public void SaveData()
    {
        YandexGame.savesData.indexOfGround = indexOfGround;
    }
}
