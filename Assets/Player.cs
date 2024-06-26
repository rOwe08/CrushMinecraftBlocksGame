using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5.0f;  // Скорость движения
    public float rotationSpeed = 700.0f;  // Скорость поворота

    // Update is called once per frame
    void Update()
    {
        // Получаем входные данные от игрока
        float horizontal = Input.GetAxis("Horizontal");  // Движение влево и вправо
        float vertical = Input.GetAxis("Vertical");  // Движение вперед и назад

        // Рассчитываем направление движения
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        // Движение игрока
        if (direction.magnitude >= 0.1f)
        {
            // Рассчитываем угол для поворота
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationSpeed, 0.1f);

            // Поворачиваем игрока
            transform.rotation = Quaternion.Euler(0, angle, 0);

            // Перемещаем игрока
            transform.Translate(direction * speed * Time.deltaTime, Space.World);
        }
    }
}
