using UnityEngine;

public class BlockManager : MonoBehaviour
{
    public static BlockManager Instance { get; private set; }  // Синглтон

    public GameObject[] blockPrefabs;  // Префабы для разных типов блоков

    private void Awake()
    {
        // Проверка, существует ли уже экземпляр синглтона
        if (Instance == null)
        {
            Instance = this;  // Назначаем текущий экземпляр
        }
        else
        {
            Destroy(gameObject);  // Уничтожаем дубликат
        }
    }

    // Метод для настройки блока
    public GameObject SetupBlock(GameObject block)
    {
        // Добавляем компонент DestructibleBlock, если его нет
        if (block.GetComponent<DestructibleBlock>() == null)
        {
            block.AddComponent<DestructibleBlock>();
        }

        // Добавляем Rigidbody, если его нет
        if (block.GetComponent<Rigidbody>() == null)
        {
            Rigidbody rb = block.AddComponent<Rigidbody>();
            rb.isKinematic = true;  // Сначала отключаем физику для блоков
            rb.mass = 1000f;  // Устанавливаем массу, например, 1. Можно настроить в зависимости от нужд
        }

        return block;  // Возвращаем настроенный блок
    }


    // Метод для выбора префаба блока
    public GameObject GetBlockPrefab(int x, int y)
    {
        // Пример: каждые 5 блоков другого типа
        if ((x + y) % 5 == 0)
        {
            return blockPrefabs.Length > 1 ? blockPrefabs[1] : blockPrefabs[0];
        }

        return blockPrefabs[0];
    }
}
