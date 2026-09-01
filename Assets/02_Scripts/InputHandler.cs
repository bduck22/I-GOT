using System;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class InputData
{
    public float rotationValue;
    public bool isWalking;
    bool isBack;
    bool isInteraction;
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

    public bool GetInteraction()
    {
        bool interaction = isInteraction;
        isInteraction = false;
        return interaction;
    }

    public void SetInteraction()
    {
        isInteraction = true;
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
        InputActions.Player.Interaction.started += OnInteraction;

        InputActions.Player.Walk.canceled += OnWalk;
        InputActions.Player.Interaction.canceled += OnInteraction;

        InputActions.Player.Move.started += OnMove;
        InputActions.Player.Move.canceled += OnMove;
    }

    private void OnDisable()
    {
        InputActions.Player.Rotation.started -= OnRotation;
        InputActions.Player.Rotation.canceled -= OnRotation;

        InputActions.Player.Walk.started -= OnWalk;
        InputActions.Player.Back.started -= OnBack;
        InputActions.Player.Interaction.started -= OnInteraction;

        InputActions.Player.Walk.canceled -= OnWalk;
        InputActions.Player.Interaction.canceled -= OnInteraction;

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

    public void OnInteraction(InputAction.CallbackContext ctx)
    {
        inputData.SetInteraction();
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
       inputData.moveValue = ctx.ReadValue<float>();
    }
}
