using UnityEngine;
using UnityEditor;
using System.IO;

public class AssignTexturesToMaterials
{
    [MenuItem("Tools/Assign Textures to Materials")]
    static void AssignTextures()
    {
        string materialsPath = "Assets/Imported/3D Blocks/Materials";
        string texturesPath = "Assets/Imported/3D Blocks/Textures";

        // Получение всех путей к файлам материалов и текстур
        string[] materialFiles = Directory.GetFiles(materialsPath, "*.mat", SearchOption.AllDirectories);
        string[] textureFiles = Directory.GetFiles(texturesPath, "*.png", SearchOption.AllDirectories);

        Material[] materials = new Material[materialFiles.Length];
        Texture2D[] textures = new Texture2D[textureFiles.Length];

        // Загрузка всех материалов и текстур
        for (int i = 0; i < materialFiles.Length; i++)
        {
            materials[i] = AssetDatabase.LoadAssetAtPath<Material>(materialFiles[i]);
            Debug.Log($"Loaded material: {materials[i].name}");
        }

        for (int i = 0; i < textureFiles.Length; i++)
        {
            textures[i] = AssetDatabase.LoadAssetAtPath<Texture2D>(textureFiles[i]);
            Debug.Log($"Loaded texture: {textures[i].name}");
        }

        Debug.Log($"Loaded {materials.Length} materials from {materialsPath}");
        Debug.Log($"Loaded {textures.Length} textures from {texturesPath}");

        foreach (var material in materials)
        {
            Debug.Log($"Processing material: {material.name}");

            // Формирование имени текстуры по шаблону "имя_материала_texture"
            string expectedTextureName = material.name + "_texture";
            Texture2D texture = System.Array.Find(textures, t => t.name.Equals(expectedTextureName, System.StringComparison.OrdinalIgnoreCase));

            if (texture != null)
            {
                Debug.Log($"Assigning texture {texture.name} to material {material.name}");
                material.SetTexture("_BaseMap", texture);
                EditorUtility.SetDirty(material); // Обновление состояния материала
            }
            else
            {
                Debug.LogWarning($"No matching texture found for material {material.name}");
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log("Textures assigned to materials successfully!");
    }
}
