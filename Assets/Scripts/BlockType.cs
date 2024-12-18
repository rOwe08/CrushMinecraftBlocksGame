using UnityEngine;

[CreateAssetMenu(fileName = "New Block Type", menuName = "BlockType", order = 51)]
public class BlockType : ScriptableObject
{
    public float health;  // Здоровье блока
    public float mass;  // Масса блока
    public Color blockColor;  // Цвет блока
    public GameObject prefab;  // Префаб блока, который будет использоваться для спавна
    public bool IsKinetic;
    public bool IsCollectable;
    public int reward;
}

