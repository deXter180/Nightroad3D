using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MeleeAnimator : MonoBehaviour, InputControls.ICombatActions
{
    private int BAttackID;
    //private AnimatorStateInfo AnimState;
    private InputControls Controls;
    [HideInInspector]public WeaponBrain weaponSelector;
    [HideInInspector] public bool isBAPressed;
    [HideInInspector] public bool isRSPressed;
    [HideInInspector] public bool isLSPressed;
    
    //~~~~~~~~~~~~~~ Animation states ~~~~~~~~~~~~~~~~

    private int RSwing = Animator.StringToHash("RightSwing");
    private int LSwing = Animator.StringToHash("LeftSwing");

    private void Awake()
    {
        Controls = new InputControls();
        weaponSelector = GetComponentInParent<WeaponBrain>();
        Controls.Combat.SetCallbacks(this);
        //AnimState = weaponSelector.animator.GetCurrentAnimatorStateInfo(0);
    }

    private void OnEnable()
    {
        Controls.Enable();
    }

    private void OnDisable()
    {
        Controls.Disable();
    }

    //~~~~~~~~~~~~~~~~ Input Callbacks ~~~~~~~~~~~~~~~~~~

    public void OnBasicAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isBAPressed = context.ReadValueAsButton();
        }
        else isBAPressed = false;
    }

    public void OnRightSwing(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isRSPressed = context.ReadValueAsButton();
        }
        else isRSPressed = false;
    }

    public void OnLeftSwing(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isLSPressed = context.ReadValueAsButton();
        }
        else isLSPressed = false;
    }

    public void OnUpSwing(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnDownSwing(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnStabAttack(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnBlock(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnWeaponSpellSwap(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    //~~~~~~~~~~~~~~~~~~ Mechanics ~~~~~~~~~~~~~~~~~~

    private void FixedUpdate()
    {
        //PlayAnim();
    }

    //private void PlayAnim()
    //{
    //    if (gameObject.activeInHierarchy)
    //    {
    //        if (isBAPressed && AnimState.IsTag("Combat") == false)
    //        {
    //            if (BAttackID != 0)
    //            {
    //                weaponSelector.animator.PlayInFixedTime(BAttackID, 0, Time.deltaTime);
    //            }
    //            else
    //            {
    //                Debug.Log("Default");
    //            }
    //        }

    //        if (isRSPressed && AnimState.IsTag("Combat") == false)
    //        {
    //            weaponSelector.animator.PlayInFixedTime(RSwing, 0, Time.deltaTime);
    //            BAttackID = RSwing;
    //        }

    //        if (isLSPressed && AnimState.IsTag("Combat") == false)
    //        {
    //            weaponSelector.animator.PlayInFixedTime(LSwing, 0, Time.deltaTime);
    //            BAttackID = LSwing;
    //        }
    //    }
    //}
}