using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YG;

public class Player : MonoBehaviour
{
    public float cannonPower = 50f;
    public float cannonSize;
    public float cannonMass;
    public int floorLevel;
    public int maxAttempts;
    public float explosiveDamage;
    public int projectileAmount;
    public float armageddonLevel;

    public int totalCoins;
    public int totalDiamonds;

    public float speed = 10f;  // Скорость движения
    public float rotationSpeed = 700.0f;  // Скорость поворота
    public float leftBound = 0f;  // Левая граница по оси X
    public float rightBound = 100f;  // Правая граница по оси X

    public float minYRotation = -90f;  // Минимальный угол поворота по Y
    public float maxYRotation = 90f;   // Максимальный угол поворота по Y

    public Transform launchPoint;    // Позиция пушки или точки, откуда будет начинаться луч

    private float moveHorizontal = 0f;

    public bool IsLevelEnding = false;
    public bool IsArmageddonActivated = false;

    [Header("****Trajectory display****")]
    public LineRenderer lineRenderer; // Компонент LineRenderer
    public int linePoints = 175;
    public float timeIntervalPoints = 0.01f;

    public bool[] shopMaterialsPurchased;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Замораживаем ненужные оси, чтобы пушка не двигалась хаотично
        rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        int amountOfMaterials = SpawnManager.Instance.shopMaterials.Length;
        shopMaterialsPurchased = new bool[amountOfMaterials];
    }

    void Update()
    {
        if (!UIManager.Instance.IsUIActive())
        {
            moveHorizontal = 0f;

            if (Input.GetKey(KeyCode.A))
            {
                moveHorizontal = -1f;  // Движение влево
            }
            else if (Input.GetKey(KeyCode.D))
            {
                moveHorizontal = 1f;   // Движение вправо
            }

            // Рассчитываем движение только по оси X
            Vector3 movement = new Vector3(moveHorizontal, 0.0f, 0.0f);
            transform.position += movement * speed * Time.deltaTime;

            Vector3 clampedPosition = transform.position;
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, leftBound, rightBound);
            transform.position = clampedPosition;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                Vector3 targetPosition = hitInfo.point;

                // Проверка: курсор должен быть перед пушкой по оси Z
                if (targetPosition.z > transform.position.z)
                {
                    // Поворот пушки в сторону курсора
                    targetPosition.y = transform.position.y;
                    Vector3 lookDirection = (targetPosition - transform.position).normalized;
                    float targetAngle = Mathf.Atan2(lookDirection.x, lookDirection.z) * Mathf.Rad2Deg;
                    targetAngle = Mathf.Clamp(targetAngle, minYRotation, maxYRotation);
                    Quaternion lookRotation = Quaternion.Euler(0, targetAngle, 0);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);

                    // Логика стрельбы
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (LevelManager.Instance.attempts < LevelManager.Instance.maxAttempts)
                        {
                            LevelManager.Instance.AddAttempt();
                            StartCoroutine(ShootProjectiles());
                        }
                    }
                    // Логика отображения траектории
                    else if (Input.GetMouseButton(1))
                    {
                        DrawTrajectory();
                        lineRenderer.enabled = true;
                    }
                    else
                    {
                        lineRenderer.enabled = false;
                    }
                }
                else
                {
                    // Отключаем линию, если курсор за пушкой
                    lineRenderer.enabled = false;
                }
            }
        }
    }

    // Корутин для стрельбы снарядами с задержкой
    IEnumerator ShootProjectiles()
    {
        for (int i = 0; i < projectileAmount; i++)
        {
            // Рассчитываем направление для каждого снаряда непосредственно перед выстрелом
            Vector3 launchDirection = GetLaunchDirection();

            GameObject projectileObject = SpawnManager.Instance.SpawnObject(SpawnManager.Instance.projectilePrefab);

            projectileObject.transform.position = launchPoint.position;
            projectileObject.GetComponent<Rigidbody>().velocity = 50f * launchDirection;
            yield return new WaitForSeconds(0.6f); // Задержка между выстрелами (0.6 секунды)
        }

        if (!IsLevelEnding)
        {
            LevelManager.Instance.OnAllProjectilesShot();
        }
    }

    // Метод для получения нормализованного направления на основе позиции курсора
    Vector3 GetLaunchDirection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            // Получаем позицию, на которую указывает курсор
            Vector3 targetPosition = hitInfo.point;

            // Рассчитываем направление от launchPoint до этой позиции
            Vector3 direction = (targetPosition - launchPoint.position).normalized;

            return direction; // Возвращаем нормализованный вектор
        }

        // Если луч не попал в объект, возвращаем направление вперёд по умолчанию
        return launchPoint.forward;
    }

    public void DrawTrajectory()
    {
        Vector3 origin = launchPoint.position;
        Vector3 startVelocity = cannonPower * GetLaunchDirection(); // Используем вычисленное направление
        lineRenderer.positionCount = linePoints;
        float time = 0;

        for (int i = 0; i < linePoints; i++)
        {
            var x = (startVelocity.x * time) + (Physics.gravity.x / 2 * time * time);
            var y = (startVelocity.y * time) + (Physics.gravity.y / 2 * time * time);
            var z = (startVelocity.z * time) + (Physics.gravity.z / 2 * time * time); // Добавлен расчёт для оси Z

            Vector3 point = new Vector3(x, y, z);
            lineRenderer.SetPosition(i, origin + point);
            time += timeIntervalPoints;
        }
    }

    public void SaveData()
    {
        YandexGame.savesData.cannonPower = cannonPower;
        YandexGame.savesData.cannonSize = cannonSize;
        YandexGame.savesData.cannonMass = cannonMass;
        YandexGame.savesData.floorLevel = floorLevel;
        YandexGame.savesData.maxAttempts = maxAttempts;
        YandexGame.savesData.explosiveDamage = explosiveDamage;
        YandexGame.savesData.projectileAmount = projectileAmount;
        YandexGame.savesData.armageddonLevel = armageddonLevel;

        YandexGame.savesData.totalCoins = totalCoins;
        YandexGame.savesData.totalDiamonds = totalDiamonds;
        YandexGame.savesData.shopMaterialsPurchased = shopMaterialsPurchased;
    }

    public void LoadData()
    {
        cannonPower = YandexGame.savesData.cannonPower;
        cannonSize = YandexGame.savesData.cannonSize;
        cannonMass = YandexGame.savesData.cannonMass;
        floorLevel = YandexGame.savesData.floorLevel;
        maxAttempts = YandexGame.savesData.maxAttempts;
        explosiveDamage = YandexGame.savesData.explosiveDamage;
        projectileAmount = YandexGame.savesData.projectileAmount;
        armageddonLevel = YandexGame.savesData.armageddonLevel;

        totalCoins = YandexGame.savesData.totalCoins;
        totalDiamonds = YandexGame.savesData.totalDiamonds;

        shopMaterialsPurchased = YandexGame.savesData.shopMaterialsPurchased;
    }

    public void AddCoins(int coins)
    {
        totalCoins += coins;
    }
    public void AddDiamonds(int amount)
    {
        totalDiamonds += amount;
    }
}
