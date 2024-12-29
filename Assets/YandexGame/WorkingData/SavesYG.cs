
using System.Collections.Generic;

namespace YG
{
    [System.Serializable]
    public class SavesYG
    {
        // "Технические сохранения" для работы плагина (Не удалять)
        public int idSave;
        public bool isFirstSession = true;
        public string language = "ru";
        public bool promptDone;

        public float cannonPower = 50f;
        public float cannonSize = 0.6f;
        public float cannonMass = 25f;
        public int floorLevel;
        public int maxAttempts;
        public float explosiveDamage;
        public int projectileAmount = 1;
        public float armageddonLevel = 0;

        public int totalCoins;
        public int totalDiamonds;

        public bool[] shopMaterialsPurchased;

        public int maxLevelCompleted = 0;
        public List<float> levelList = new List<float>();
        public int[] upgradeLevels; // Уровни апгрейда для всех кнопок
        public int[] upgradeCostsCoins; // Цены на апгрейды в монетах для всех кнопок
        public int[] upgradeCostsDiamonds; // Цены на апгрейды в кристаллах для всех кнопок

        public bool isExplosionPurchased;

        public int AmountOfRewardedHP;

        public int indexOfGround;

        // Вы можете выполнить какие то действия при загрузке сохранений
        public SavesYG()
        {
            shopMaterialsPurchased = new bool[16];
        }
    }
}
