using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputControl : MonoBehaviour, Input
{
    [SerializeField] private InputAction Movement;
    [SerializeField] private InputAction Mouse;
    [SerializeField] private InputAction Attack;
    [SerializeField] private InputAction Jump;
    [SerializeField] private InputActionMap WeaponSelection;
    [SerializeField] private InputAction Inventory;
    public bool GetInventory { get; set; }


    // Start is called before the first frame update

    private void OnEnable()
    {
        Movement.Enable();
        Mouse.Enable();
        Attack.Enable();
        Jump.Enable();
        WeaponSelection.Enable();
        Inventory.Enable();
        Inventory.started += Inventory_started;
    }

    private void OnDisable()
    {
        Movement.Disable();
        Mouse.Disable();
        Attack.Disable();
        Jump.Disable();
        WeaponSelection.Disable();
        Inventory.Disable();
        Inventory.started -= Inventory_started;
    }

    public float GetAttackHold()
    {
        return Attack.ReadValue<float>();
    }

    public Vector2 GetMousePos()
    {
        return Mouse.ReadValue<Vector2>();
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

    //~~~~~~~~~~~~~~~~~~ Callbacks ~~~~~~~~~~~~~~~~~~~

    private void Inventory_started(InputAction.CallbackContext obj)
    {
        GetInventory = true;
    }

}
