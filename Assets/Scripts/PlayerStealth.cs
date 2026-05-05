using UnityEngine;

/// <summary>
/// Компонент игрока для стелс-режима: приседание, генерация шума.
/// </summary>
public class PlayerStealth : MonoBehaviour
{
    [Header("Movement Speeds")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float crouchSpeed = 2.5f;

    [Header("Noise Levels")]
    [SerializeField] private float runNoise = 1f;
    [SerializeField] private float walkNoise = 0.5f;
    [SerializeField] private float crouchNoise = 0.1f;
    [SerializeField] private float idleNoise = 0f;

    [Header("Crouch Settings")]
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float standHeight = 2f;
    [SerializeField] private float crouchTransitionSpeed = 10f;

    private CharacterController controller;
    private Camera playerCamera;
    private float currentSpeed;
    private float currentNoise;
    private bool isCrouching;
    private Vector3 originalCameraPos;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        originalCameraPos = playerCamera.transform.localPosition;
    }

    private void Update()
    {
        HandleCrouch();
        HandleMovement();
        EmitNoise();
    }

    private void HandleCrouch()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            isCrouching = true;
            controller.height = Mathf.Lerp(controller.height, crouchHeight, crouchTransitionSpeed * Time.deltaTime);
            playerCamera.transform.localPosition = Vector3.Lerp(
                playerCamera.transform.localPosition,
                new Vector3(originalCameraPos.x, originalCameraPos.y - (standHeight - crouchHeight), originalCameraPos.z),
                crouchTransitionSpeed * Time.deltaTime);
        }
        else
        {
            isCrouching = false;
            controller.height = Mathf.Lerp(controller.height, standHeight, crouchTransitionSpeed * Time.deltaTime);
            playerCamera.transform.localPosition = Vector3.Lerp(
                playerCamera.transform.localPosition,
                originalCameraPos,
                crouchTransitionSpeed * Time.deltaTime);
        }
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 move = transform.right * horizontal + transform.forward * vertical;

        if (isCrouching)
        {
            currentSpeed = crouchSpeed;
            controller.Move(move * currentSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = runSpeed;
            controller.Move(move * currentSpeed * Time.deltaTime);
        }
        else if (move.magnitude > 0.1f)
        {
            currentSpeed = walkSpeed;
            controller.Move(move * currentSpeed * Time.deltaTime);
        }
        else
        {
            currentSpeed = 0f;
        }
    }

    private void EmitNoise()
    {
        if (currentSpeed > runSpeed * 0.8f)
            currentNoise = runNoise;
        else if (currentSpeed > walkSpeed * 0.8f)
            currentNoise = walkNoise;
        else if (currentSpeed > 0)
            currentNoise = crouchNoise;
        else
            currentNoise = idleNoise;

        if (currentNoise > 0)
        {
            // Вызываем событие шума, передаём позицию и интенсивность
            NoiseManager.EmitNoise(transform.position, currentNoise);
        }
    }
}