using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class CombatController : MonoBehaviour, InputControls.ICombatActions
{
    
    private Animator ModelAnim;
    private AnimatorStateInfo AnimState;
    private InputControls Controls;
    [HideInInspector] public bool isBAPressed;
    [HideInInspector] public bool isRSPressed;
    [HideInInspector] public bool isLSPressed;
    private int BAttackID;
    
    //~~~~~~~~~~~~~~ Animation states ~~~~~~~~~~~~~~~~

    private int RSwing = Animator.StringToHash("RightSwing");
    private int LSwing = Animator.StringToHash("LeftSwing");
    
    private void Awake()
    {
        Controls = new InputControls();
        ModelAnim = GetComponentInChildren<Animator>();
        Controls.Combat.SetCallbacks(this);
        AnimState = ModelAnim.GetCurrentAnimatorStateInfo(0);
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
        PlayAnim();
    }

    private void PlayAnim()
    {

        if (gameObject.activeInHierarchy)
        {
            if (isBAPressed && AnimState.IsTag("Combat") == false)
            {
                if (BAttackID != 0)
                {
                    ModelAnim.PlayInFixedTime(BAttackID, 0, Time.deltaTime);
                }
                else
                {
                    Debug.Log("Default");
                }

            }

            if (isRSPressed && AnimState.IsTag("Combat") == false)
            {

                ModelAnim.PlayInFixedTime(RSwing, 0, Time.deltaTime);
                BAttackID = RSwing;
            }

            if (isLSPressed && AnimState.IsTag("Combat") == false)
            {
                ModelAnim.PlayInFixedTime(LSwing, 0, Time.deltaTime);
                BAttackID = LSwing;
            }
        }
        
    }

}


