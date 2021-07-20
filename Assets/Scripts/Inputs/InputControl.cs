using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputControl : MonoBehaviour, Input
{
    [SerializeField] private InputAction Movement;
    [SerializeField] private InputAction MouseDelta;
    [SerializeField] private InputAction MousePosition;
    [SerializeField] private InputAction Attack;
    [SerializeField] private InputAction Jump;
    [SerializeField] private InputActionMap WeaponSelection;
    [SerializeField] private InputAction Inventory;
    [SerializeField] private InputAction RotateInInventory;
    [SerializeField] private InputAction MouseRightClick;
    [SerializeField] private InputAction PickItems;

    // Start is called before the first frame update

    private void OnEnable()
    {
        Movement.Enable();
        MouseDelta.Enable();
        Attack.Enable();
        Jump.Enable();
        WeaponSelection.Enable();
        Inventory.Enable();
        MousePosition.Enable();
        RotateInInventory.Enable();
        MouseRightClick.Enable();
        PickItems.Enable();
    }

    private void OnDisable()
    {
        Movement.Disable();
        MouseDelta.Disable();
        Attack.Disable();
        Jump.Disable();
        WeaponSelection.Disable();
        Inventory.Disable();
        MousePosition.Disable();
        RotateInInventory.Disable();
        MouseRightClick.Disable();
        PickItems.Disable();
    }

    public float GetAttackHold()
    {
        return Attack.ReadValue<float>();
    }

    public Vector2 GetMouseDelta()
    {
        return MouseDelta.ReadValue<Vector2>();
    }

    public Vector2 GetMousePosition()
    {
        return MousePosition.ReadValue<Vector2>();
    }

    public Vector2 GetMovement()
    {
        return Movement.ReadValue<Vector2>();
    }

    public float GetJump()
    {
        return Jump.ReadValue<float>();
    }

    public float GetWeapon1()
    {
        InputAction action = WeaponSelection.FindAction("Weapon 1", throwIfNotFound: true);
        return action.ReadValue<float>();
    }

    public float GetWeapon2()
    {
        InputAction action = WeaponSelection.FindAction("Weapon 2", throwIfNotFound: true);
        return action.ReadValue<float>();
    }

    public float GetWeapon3()
    {
        InputAction action = WeaponSelection.FindAction("Weapon 3", throwIfNotFound: true);
        return action.ReadValue<float>();
    }

    public bool GetInventory()
    {
        return Inventory.triggered;
    }

    public bool GetRotationItems()
    {
        return RotateInInventory.triggered;
    }

    public bool GetMouseRightClick()
    {
        return MouseRightClick.triggered;
    }

    public bool GetPickItems()
    {
        return PickItems.triggered;
    }
}
