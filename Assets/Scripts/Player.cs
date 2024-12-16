using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 10f;  // Скорость движения
    public float rotationSpeed = 700.0f;  // Скорость поворота
    public float leftBound = 0f;  // Левая граница по оси X
    public float rightBound = 100f;  // Правая граница по оси X

    public float minYRotation = -90f;  // Минимальный угол поворота по Y
    public float maxYRotation = 90f;   // Максимальный угол поворота по Y

    public Transform launchPoint;    // Позиция пушки или точки, откуда будет начинаться луч

    private float moveHorizontal = 0f;
    public float launchSpeed = 10f;

    [Header("****Trajectory display****")]
    public LineRenderer lineRenderer; // Компонент LineRenderer
    public int linePoints = 175;
    public float timeIntervalPoints = 0.01f;

    void Update()
    {
        // Получаем входные данные от игрока
        moveHorizontal = 0f;

        if (Input.GetKey(KeyCode.A))
        {
            moveHorizontal = -1f;  // Движение влево
        }
        else if (Input.GetKey(KeyCode.D))
        {
            moveHorizontal = 1f;   // Движение вправо
        }
        else if (Input.GetMouseButtonDown(0))
        {
            GameObject projectileObject = SpawnManager.Instance.SpawnObject(SpawnManager.Instance.projectilePrefab);
            projectileObject.transform.position = launchPoint.position;
            projectileObject.GetComponent<Rigidbody>().velocity = launchSpeed * launchPoint.forward;
        }
        else if (Input.GetMouseButton(1)) 
        {
            DrawTrajectory();
            lineRenderer.enabled = true;
        }
        else
        {
            lineRenderer.enabled = false;
        }

        // Рассчитываем движение только по оси X
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, 0.0f);

        // Применяем движение, изменяя позицию объекта
        transform.position += movement * speed * Time.deltaTime;

        // Ограничиваем движение объекта в пределах границ по оси X
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, leftBound, rightBound);
        transform.position = clampedPosition;

        // Логика поворота к курсору мыши
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            // Получаем позицию точки столкновения луча
            Vector3 targetPosition = hitInfo.point;
            targetPosition.y = transform.position.y;  // Оставляем высоту игрока неизменной

            // Рассчитываем направление к курсору мыши
            Vector3 lookDirection = (targetPosition - transform.position).normalized;

            // Рассчитываем угол поворота по Y
            float targetAngle = Mathf.Atan2(lookDirection.x, lookDirection.z) * Mathf.Rad2Deg;

            // Ограничиваем угол поворота по оси Y
            targetAngle = Mathf.Clamp(targetAngle, minYRotation, maxYRotation);

            // Поворачиваем игрока в сторону курсора, но только по оси Y
            Quaternion lookRotation = Quaternion.Euler(0, targetAngle, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        }
    }

    public void DrawTrajectory()
    {
        Vector3 origin = launchPoint.position;
        Vector3 startVelocity = launchSpeed * launchPoint.forward;
        lineRenderer.positionCount = linePoints;
        float time = 0;

        for (int i = 0; i < linePoints; i++) 
        { 

            var x = (startVelocity.x * time) + (Physics.gravity.x / 2 * time * time);
            var y = (startVelocity.y * time) + (Physics.gravity.y / 2 * time * time);

            Vector3 point = new Vector3 (x, y, 0);
            lineRenderer.SetPosition(i, origin + point);
            time += timeIntervalPoints;

        }
    }
}
