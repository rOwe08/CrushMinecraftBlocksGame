using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int totalCoins = 0;

    public float speed = 10f;  // Скорость движения
    public float rotationSpeed = 700.0f;  // Скорость поворота
    public float leftBound = 0f;  // Левая граница по оси X
    public float rightBound = 100f;  // Правая граница по оси X

    public float minYRotation = -90f;  // Минимальный угол поворота по Y
    public float maxYRotation = 90f;   // Максимальный угол поворота по Y

    public Transform launchPoint;    // Позиция пушки или точки, откуда будет начинаться луч

    private float moveHorizontal = 0f;
    public float launchSpeed = 50f;

    [Header("****Trajectory display****")]
    public LineRenderer lineRenderer; // Компонент LineRenderer
    public int linePoints = 175;
    public float timeIntervalPoints = 0.01f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Замораживаем ненужные оси, чтобы пушка не двигалась хаотично
        rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
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
                        if(LevelManager.Instance.attempts < LevelManager.Instance.maxAttempts)
                        {
                            LevelManager.Instance.AddAttempt();

                            Vector3 launchDirection = GetLaunchDirection();
                            GameObject projectileObject = SpawnManager.Instance.SpawnObject(SpawnManager.Instance.projectilePrefab);
                            projectileObject.transform.position = launchPoint.position;
                            projectileObject.GetComponent<Rigidbody>().velocity = launchSpeed * launchDirection;

                            if (LevelManager.Instance.attempts >= LevelManager.Instance.maxAttempts)
                            {
                                UIManager.Instance.ActivateUI(); 
                                Invoke("EndLevelWithDelay", 1f); // Завершаем уровень через 1 секунду
                            }
                        }
                        else
                        {
                            GameManager.Instance.EndLevel();
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
        Vector3 startVelocity = launchSpeed * GetLaunchDirection(); // Используем вычисленное направление
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

    public void AddCoins(int coins)
    {
        totalCoins += coins;
    }

    void EndLevelWithDelay()
    {
        GameManager.Instance.EndLevel();
    }
}
