using System;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class InputData
{
    public float rotationValue;
    public bool isWalking;
    bool isBack;
    public float moveValue;

    public bool GetBack()
    {
        bool back = isBack;
        isBack = false;
        return back;
    }

    public void SetBack()
    {
        isBack = true;
    }
}

public class InputHandler : MonoBehaviour
{
    private PlayerInputActions InputActions;

    public InputData inputData;

    private void Awake()
    {
        InputActions = new PlayerInputActions();

        inputData = new InputData();
    }

    private void OnEnable()
    {
        InputActions.Enable();

        InputActions.Player.Rotation.started += OnRotation;
        InputActions.Player.Rotation.canceled += OnRotation;

        InputActions.Player.Walk.started += OnWalk;
        InputActions.Player.Back.started += OnBack;

        InputActions.Player.Walk.canceled += OnWalk;

        InputActions.Player.Move.started += OnMove;
        InputActions.Player.Move.canceled += OnMove;
    }

    private void OnDisable()
    {
        InputActions.Player.Rotation.started -= OnRotation;
        InputActions.Player.Rotation.canceled -= OnRotation;

        InputActions.Player.Walk.started -= OnWalk;
        InputActions.Player.Back.started -= OnBack;

        InputActions.Player.Walk.canceled -= OnWalk;

        InputActions.Player.Move.started -= OnMove;
        InputActions.Player.Move.canceled -= OnMove;
    }

    public void OnRotation(InputAction.CallbackContext ctx)
    {
        inputData.rotationValue = ctx.ReadValue<float>();
    }

    public void OnWalk(InputAction.CallbackContext ctx)
    {
        inputData.isWalking = ctx.ReadValueAsButton();
    }

    public void OnBack(InputAction.CallbackContext ctx)
    {
        inputData.SetBack();
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
       inputData.moveValue = ctx.ReadValue<float>();
    }
}
