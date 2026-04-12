using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // Обязательно подключаем новую библиотеку

public class PlayerController : MonoBehaviour
{
    // ВАЖНО: Значение rotateSpeed пришлось сильно уменьшить (было 75).
    // Новая система мыши выдает другие значения (сырые пиксели), 
    // поэтому чувствительность нужно будет перенастроить в Инспекторе.
    [SerializeField] private float rotateSpeed = 5f;
    [SerializeField] private float walkSpeed = 7f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private float gravity = -9.81f;

    private CharacterController characterController;
    private Camera playerCamera;

    private Vector3 velocity;
    private Vector2 rotation;
    private Vector2 direction;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        // 1. Движение вниз (гравитация и применение скорости)
        characterController.Move(velocity * Time.deltaTime);

        // 2. Чтение WASD с клавиатуры (вместо Input.GetAxis)
        float moveX = 0f;
        float moveY = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.dKey.isPressed) moveX += 1f;
            if (Keyboard.current.aKey.isPressed) moveX -= 1f;
            if (Keyboard.current.wKey.isPressed) moveY += 1f;
            if (Keyboard.current.sKey.isPressed) moveY -= 1f;
        }

        // Нормализуем, чтобы игрок не бегал по диагонали быстрее, чем по прямой
        direction = new Vector2(moveX, moveY).normalized;

        // 3. Чтение мыши (вместо Input.GetAxis("Mouse X/Y"))
        Vector2 mouseDelta = Vector2.zero;
        if (Mouse.current != null)
        {
            mouseDelta = Mouse.current.delta.ReadValue();
        }

        // 4. Прыжок и гравитация (вместо Input.GetKeyDown)
        if (characterController.isGrounded)
        {
            // Проверяем, была ли нажата кнопка пробела в этом кадре
            bool isJumping = Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;
            velocity.y = isJumping ? jumpForce : -0.1f;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        // 5. Вращение камеры
        // В новой системе mouseDelta уже зависит от частоты кадров (это смещение за кадр), 
        // поэтому умножать на Time.deltaTime здесь не нужно, только на скорость.
        mouseDelta *= rotateSpeed * 0.01f;

        rotation.y += mouseDelta.x;
        rotation.x = Mathf.Clamp(rotation.x - mouseDelta.y, -90, 90);

        // Вращаем камеру по вертикали, а самого игрока по горизонтали
        playerCamera.transform.localEulerAngles = new Vector3(rotation.x, 0, 0);
        transform.rotation = Quaternion.Euler(0, rotation.y, 0);
    }

    private void FixedUpdate()
    {
        // 6. Логика бега (вместо Input.GetKey)
        bool isRunning = Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed;
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        // Преобразуем локальное направление в глобальное для движения
        Vector3 moveInput = new Vector3(direction.x, 0, direction.y) * currentSpeed;
        Vector3 move = transform.TransformDirection(moveInput);

        velocity.x = move.x;
        velocity.z = move.z;
    }
}