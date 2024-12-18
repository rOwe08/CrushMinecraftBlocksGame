using UnityEngine;

public class BlockManager : MonoBehaviour
{
    public static BlockManager Instance { get; private set; }  // Синглтон

    public BlockType[] blockTypes;  // Массив типов блоков (с префабами, массой и здоровьем)

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
    public GameObject SetupBlock(GameObject block, BlockType blockType)
    {
        Rigidbody rb;

        // Добавляем компонент DestructibleBlock, если его нет
        if (block.GetComponent<DestructibleBlock>() == null)
        {
            block.AddComponent<DestructibleBlock>();
        }

        // Добавляем Rigidbody, если его нет
        if (block.GetComponent<Rigidbody>() == null)
        {
            rb = block.AddComponent<Rigidbody>();
        }
        else
        {
            rb = block.GetComponent<Rigidbody>();

        }
        rb.isKinematic = blockType.IsKinetic;  // Сначала отключаем физику для блоков
        rb.mass = blockType.mass;  // Устанавливаем массу из типа блока

        // Настроим здоровье блока через компонент DestructibleBlock
        if (block.GetComponent<DestructibleBlock>() != null)
        {
            DestructibleBlock destructibleBlock = block.GetComponent<DestructibleBlock>();
            destructibleBlock.hp = blockType.health;  // Устанавливаем здоровье блока из типа блока
            destructibleBlock.blockType = blockType;  // Устанавливаем BlockType для последующего использования
        }

        return block;  // Возвращаем настроенный блок
    }


    // Метод для получения типа блока по индексу
    public BlockType GetBlockType(int index)
    {
        if (index < 0 || index >= blockTypes.Length)
        {
            Debug.LogWarning("Invalid index for blockTypes array!");
            return null;
        }

        return blockTypes[index];  // Возвращаем тип блока по индексу
    }
}
