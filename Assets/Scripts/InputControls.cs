// GENERATED AUTOMATICALLY FROM 'Assets/Inputs/InputControls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @InputControls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputControls"",
    ""maps"": [
        {
            ""name"": ""Gameplay"",
            ""id"": ""d2952f49-76cb-4d01-8b8b-222d892e18ac"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""b3daf76e-8472-42ce-a7ec-6533b3393a50"",
                    ""expectedControlType"": ""Dpad"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Look"",
                    ""type"": ""Value"",
                    ""id"": ""82ea35de-efd4-4df4-9370-f310310f84ff"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""c7f9b41e-7237-473f-aed3-99335a44be4d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Sprint"",
                    ""type"": ""Button"",
                    ""id"": ""bb4b323b-e752-470c-a557-ade1bddf6201"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Inventory"",
                    ""type"": ""Button"",
                    ""id"": ""ad9a7b4a-93d4-401d-9071-f5b2070d02b4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Interact"",
                    ""type"": ""Button"",
                    ""id"": ""81ba77e7-e9d1-4a09-9cc4-7c94c8e04df1"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""35e01df3-ee03-40be-a4f9-97e62c808fd3"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""40a3b559-3237-4b2f-aa71-205969249f3c"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC Controls"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""fc0ff7aa-2697-4cf5-bdee-8254155604c4"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC Controls"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""7e6f3746-32ab-443a-a9b9-a26384371ab0"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC Controls"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""22451077-dfad-4388-84df-e97b0a94a66a"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC Controls"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Arrows"",
                    ""id"": ""e27fee09-cf5d-4588-b7b6-8de3c4de64eb"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""ff2244ad-d851-4fde-90c8-8a38eb45a66f"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC Controls"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""8b741e0c-60f5-41fa-8f5f-f6916f04dc04"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC Controls"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""e3f890fb-8f90-4902-bdc4-5cd6936979be"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC Controls"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""a17fce7f-15fa-482e-a42b-6f267742438f"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC Controls"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""27c464c4-6314-4b82-8ca0-8a15aa30f853"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC Controls"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fce2c0b5-b967-4841-807d-d2a8e2067a28"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC Controls"",
                    ""action"": ""Sprint"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1cf626da-c5fd-4b45-aac3-c87d5dd45a10"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC Controls"",
                    ""action"": ""Inventory"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5df62be0-811e-427d-8b19-92543fbb776b"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC Controls"",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4e2d443c-637a-430e-bee9-02153df95451"",
                    ""path"": ""<Pointer>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC Controls"",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Combat"",
            ""id"": ""c65d8edd-cd0b-4286-9448-6a63171e6b1f"",
            ""actions"": [
                {
                    ""name"": ""BasicAttack"",
                    ""type"": ""Button"",
                    ""id"": ""f5f80f5c-d1f8-4e70-838b-bd9968181561"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""RightSwing"",
                    ""type"": ""Button"",
                    ""id"": ""4fd2f3aa-13da-477e-b0f9-d63cad42d1a9"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""LeftSwing"",
                    ""type"": ""Button"",
                    ""id"": ""47fd227f-5183-42a2-8674-b0b5e2724cbd"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""UpSwing"",
                    ""type"": ""Button"",
                    ""id"": ""55fd1589-9b57-4ca5-b812-8db46694d82c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""DownSwing"",
                    ""type"": ""Button"",
                    ""id"": ""75e4eefd-db8d-4748-9e22-438c70b06f06"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""StabAttack"",
                    ""type"": ""Button"",
                    ""id"": ""19214906-a3a5-4161-b6f2-b7618ca09a27"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Block"",
                    ""type"": ""Button"",
                    ""id"": ""4180403f-9f27-47ed-ba28-eec093037d3a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""WeaponSpellSwap"",
                    ""type"": ""Button"",
                    ""id"": ""2392d178-6917-4cab-abf5-93d2b2301409"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""52e1aaf5-6092-4039-9425-4097f11c70a9"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC Controls"",
                    ""action"": ""BasicAttack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""RightSwingWASD"",
                    ""id"": ""499701e5-5fb6-494c-868a-531a3bf58c01"",
                    ""path"": ""ButtonWithOneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC Controls"",
                    ""action"": ""RightSwing"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""bb2d0325-21f1-48a4-80bc-a818ec24e4e8"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC Controls"",
                    ""action"": ""RightSwing"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""button"",
                    ""id"": ""a2620a84-f08e-4b13-8272-1b3cb1487274"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC Controls"",
                    ""action"": ""RightSwing"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""RightSwingArrows"",
                    ""id"": ""4cce3b94-4442-4698-a755-f5990ef2a7b5"",
                    ""path"": ""ButtonWithOneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC Controls"",
                    ""action"": ""RightSwing"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""c3ecbc6d-e23d-4a26-9b12-f63fc66baa28"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC Controls"",
                    ""action"": ""RightSwing"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""button"",
                    ""id"": ""fe003517-8ecf-4094-84a0-61c293229d9f"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC Controls"",
                    ""action"": ""RightSwing"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""LeftSwingWASD"",
                    ""id"": ""1f090125-444a-413d-b149-1ebbe33199b5"",
                    ""path"": ""ButtonWithOneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC Controls"",
                    ""action"": ""LeftSwing"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""64d160aa-5b4e-4d58-a7cf-b6bbf39c086d"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC Controls"",
                    ""action"": ""LeftSwing"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""button"",
                    ""id"": ""91128326-4c80-4e9b-857d-c81ae63fc008"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC Controls"",
                    ""action"": ""LeftSwing"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""LeftSwingArrows"",
                    ""id"": ""5d91e9c2-9e86-45ca-825a-1f923fbc58e9"",
                    ""path"": ""ButtonWithOneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC Controls"",
                    ""action"": ""LeftSwing"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""dabca4c5-16df-421d-9d1e-a1653b40f9a8"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC Controls"",
                    ""action"": ""LeftSwing"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""button"",
                    ""id"": ""7b41f0d9-5114-4877-8d0c-74915fed53c6"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC Controls"",
                    ""action"": ""LeftSwing"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""UpSwingWASD"",
                    ""id"": ""ff5e29ae-218a-473e-991f-90f83f183c50"",
                    ""path"": ""ButtonWithOneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC Controls"",
                    ""action"": ""UpSwing"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""32e1b262-8f7d-4fba-be84-f8f848c03392"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC Controls"",
                    ""action"": ""UpSwing"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""button"",
                    ""id"": ""e9bd1d07-c7c6-425a-97b7-585802ed3dc3"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC Controls"",
                    ""action"": ""UpSwing"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""UpSwingArrows"",
                    ""id"": ""7d46eaac-791a-4234-adcf-132338bb20d5"",
                    ""path"": ""ButtonWithOneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC Controls"",
                    ""action"": ""UpSwing"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""7d3c2b68-d5f0-47fd-b55e-bc70c4a4cfbe"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC Controls"",
                    ""action"": ""UpSwing"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""button"",
                    ""id"": ""b75bee4a-5d77-4161-b752-303992cb7b75"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC Controls"",
                    ""action"": ""UpSwing"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""DownSwingWASD"",
                    ""id"": ""a5833cfa-858f-47d6-9bde-0ae9b44f2d59"",
                    ""path"": ""ButtonWithOneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC Controls"",
                    ""action"": ""DownSwing"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""c4fb5f1f-bf80-4d85-8cb6-3650b45bd763"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC Controls"",
                    ""action"": ""DownSwing"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""button"",
                    ""id"": ""ddfb16b1-3288-41c3-84cb-c546517368d7"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC Controls"",
                    ""action"": ""DownSwing"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""DownSwingArrows"",
                    ""id"": ""f4923a04-84b9-49ed-8805-b86768543f72"",
                    ""path"": ""ButtonWithOneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC Controls"",
                    ""action"": ""DownSwing"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""fdeb0712-a771-4cdf-bced-97bd14fc1f0d"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC Controls"",
                    ""action"": ""DownSwing"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""button"",
                    ""id"": ""286d85fa-d6fa-41df-9968-3b5e0909f624"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC Controls"",
                    ""action"": ""DownSwing"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""ea3fac55-7f89-4e21-a6f7-3bb54f678eea"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC Controls"",
                    ""action"": ""StabAttack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f1af1678-08c7-47f8-a60f-531bae81356d"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC Controls"",
                    ""action"": ""Block"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fcfe93fe-40ad-4db8-9aa0-92d6a6d21a51"",
                    ""path"": ""<Keyboard>/tab"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC Controls"",
                    ""action"": ""WeaponSpellSwap"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""498e82b6-2c1e-4f25-bde0-ef2731123b3a"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC Controls"",
                    ""action"": ""WeaponSpellSwap"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f7963895-2269-43ad-9fc5-979bfba92e44"",
                    ""path"": ""<Keyboard>/2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC Controls"",
                    ""action"": ""WeaponSpellSwap"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c5147d40-2f8a-4cf2-9872-3a97c04bbeac"",
                    ""path"": ""<Keyboard>/3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC Controls"",
                    ""action"": ""WeaponSpellSwap"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e230d51c-6889-41f9-a2b2-99736b23b475"",
                    ""path"": ""<Keyboard>/4"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC Controls"",
                    ""action"": ""WeaponSpellSwap"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""PC Controls"",
            ""bindingGroup"": ""PC Controls"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Gameplay
        m_Gameplay = asset.FindActionMap("Gameplay", throwIfNotFound: true);
        m_Gameplay_Movement = m_Gameplay.FindAction("Movement", throwIfNotFound: true);
        m_Gameplay_Look = m_Gameplay.FindAction("Look", throwIfNotFound: true);
        m_Gameplay_Jump = m_Gameplay.FindAction("Jump", throwIfNotFound: true);
        m_Gameplay_Sprint = m_Gameplay.FindAction("Sprint", throwIfNotFound: true);
        m_Gameplay_Inventory = m_Gameplay.FindAction("Inventory", throwIfNotFound: true);
        m_Gameplay_Interact = m_Gameplay.FindAction("Interact", throwIfNotFound: true);
        // Combat
        m_Combat = asset.FindActionMap("Combat", throwIfNotFound: true);
        m_Combat_BasicAttack = m_Combat.FindAction("BasicAttack", throwIfNotFound: true);
        m_Combat_RightSwing = m_Combat.FindAction("RightSwing", throwIfNotFound: true);
        m_Combat_LeftSwing = m_Combat.FindAction("LeftSwing", throwIfNotFound: true);
        m_Combat_UpSwing = m_Combat.FindAction("UpSwing", throwIfNotFound: true);
        m_Combat_DownSwing = m_Combat.FindAction("DownSwing", throwIfNotFound: true);
        m_Combat_StabAttack = m_Combat.FindAction("StabAttack", throwIfNotFound: true);
        m_Combat_Block = m_Combat.FindAction("Block", throwIfNotFound: true);
        m_Combat_WeaponSpellSwap = m_Combat.FindAction("WeaponSpellSwap", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Gameplay
    private readonly InputActionMap m_Gameplay;
    private IGameplayActions m_GameplayActionsCallbackInterface;
    private readonly InputAction m_Gameplay_Movement;
    private readonly InputAction m_Gameplay_Look;
    private readonly InputAction m_Gameplay_Jump;
    private readonly InputAction m_Gameplay_Sprint;
    private readonly InputAction m_Gameplay_Inventory;
    private readonly InputAction m_Gameplay_Interact;
    public struct GameplayActions
    {
        private @InputControls m_Wrapper;
        public GameplayActions(@InputControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_Gameplay_Movement;
        public InputAction @Look => m_Wrapper.m_Gameplay_Look;
        public InputAction @Jump => m_Wrapper.m_Gameplay_Jump;
        public InputAction @Sprint => m_Wrapper.m_Gameplay_Sprint;
        public InputAction @Inventory => m_Wrapper.m_Gameplay_Inventory;
        public InputAction @Interact => m_Wrapper.m_Gameplay_Interact;
        public InputActionMap Get() { return m_Wrapper.m_Gameplay; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GameplayActions set) { return set.Get(); }
        public void SetCallbacks(IGameplayActions instance)
        {
            if (m_Wrapper.m_GameplayActionsCallbackInterface != null)
            {
                @Movement.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMovement;
                @Look.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnLook;
                @Look.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnLook;
                @Look.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnLook;
                @Jump.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnJump;
                @Sprint.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnSprint;
                @Sprint.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnSprint;
                @Sprint.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnSprint;
                @Inventory.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnInventory;
                @Inventory.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnInventory;
                @Inventory.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnInventory;
                @Interact.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnInteract;
                @Interact.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnInteract;
                @Interact.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnInteract;
            }
            m_Wrapper.m_GameplayActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @Look.started += instance.OnLook;
                @Look.performed += instance.OnLook;
                @Look.canceled += instance.OnLook;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Sprint.started += instance.OnSprint;
                @Sprint.performed += instance.OnSprint;
                @Sprint.canceled += instance.OnSprint;
                @Inventory.started += instance.OnInventory;
                @Inventory.performed += instance.OnInventory;
                @Inventory.canceled += instance.OnInventory;
                @Interact.started += instance.OnInteract;
                @Interact.performed += instance.OnInteract;
                @Interact.canceled += instance.OnInteract;
            }
        }
    }
    public GameplayActions @Gameplay => new GameplayActions(this);

    // Combat
    private readonly InputActionMap m_Combat;
    private ICombatActions m_CombatActionsCallbackInterface;
    private readonly InputAction m_Combat_BasicAttack;
    private readonly InputAction m_Combat_RightSwing;
    private readonly InputAction m_Combat_LeftSwing;
    private readonly InputAction m_Combat_UpSwing;
    private readonly InputAction m_Combat_DownSwing;
    private readonly InputAction m_Combat_StabAttack;
    private readonly InputAction m_Combat_Block;
    private readonly InputAction m_Combat_WeaponSpellSwap;
    public struct CombatActions
    {
        private @InputControls m_Wrapper;
        public CombatActions(@InputControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @BasicAttack => m_Wrapper.m_Combat_BasicAttack;
        public InputAction @RightSwing => m_Wrapper.m_Combat_RightSwing;
        public InputAction @LeftSwing => m_Wrapper.m_Combat_LeftSwing;
        public InputAction @UpSwing => m_Wrapper.m_Combat_UpSwing;
        public InputAction @DownSwing => m_Wrapper.m_Combat_DownSwing;
        public InputAction @StabAttack => m_Wrapper.m_Combat_StabAttack;
        public InputAction @Block => m_Wrapper.m_Combat_Block;
        public InputAction @WeaponSpellSwap => m_Wrapper.m_Combat_WeaponSpellSwap;
        public InputActionMap Get() { return m_Wrapper.m_Combat; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(CombatActions set) { return set.Get(); }
        public void SetCallbacks(ICombatActions instance)
        {
            if (m_Wrapper.m_CombatActionsCallbackInterface != null)
            {
                @BasicAttack.started -= m_Wrapper.m_CombatActionsCallbackInterface.OnBasicAttack;
                @BasicAttack.performed -= m_Wrapper.m_CombatActionsCallbackInterface.OnBasicAttack;
                @BasicAttack.canceled -= m_Wrapper.m_CombatActionsCallbackInterface.OnBasicAttack;
                @RightSwing.started -= m_Wrapper.m_CombatActionsCallbackInterface.OnRightSwing;
                @RightSwing.performed -= m_Wrapper.m_CombatActionsCallbackInterface.OnRightSwing;
                @RightSwing.canceled -= m_Wrapper.m_CombatActionsCallbackInterface.OnRightSwing;
                @LeftSwing.started -= m_Wrapper.m_CombatActionsCallbackInterface.OnLeftSwing;
                @LeftSwing.performed -= m_Wrapper.m_CombatActionsCallbackInterface.OnLeftSwing;
                @LeftSwing.canceled -= m_Wrapper.m_CombatActionsCallbackInterface.OnLeftSwing;
                @UpSwing.started -= m_Wrapper.m_CombatActionsCallbackInterface.OnUpSwing;
                @UpSwing.performed -= m_Wrapper.m_CombatActionsCallbackInterface.OnUpSwing;
                @UpSwing.canceled -= m_Wrapper.m_CombatActionsCallbackInterface.OnUpSwing;
                @DownSwing.started -= m_Wrapper.m_CombatActionsCallbackInterface.OnDownSwing;
                @DownSwing.performed -= m_Wrapper.m_CombatActionsCallbackInterface.OnDownSwing;
                @DownSwing.canceled -= m_Wrapper.m_CombatActionsCallbackInterface.OnDownSwing;
                @StabAttack.started -= m_Wrapper.m_CombatActionsCallbackInterface.OnStabAttack;
                @StabAttack.performed -= m_Wrapper.m_CombatActionsCallbackInterface.OnStabAttack;
                @StabAttack.canceled -= m_Wrapper.m_CombatActionsCallbackInterface.OnStabAttack;
                @Block.started -= m_Wrapper.m_CombatActionsCallbackInterface.OnBlock;
                @Block.performed -= m_Wrapper.m_CombatActionsCallbackInterface.OnBlock;
                @Block.canceled -= m_Wrapper.m_CombatActionsCallbackInterface.OnBlock;
                @WeaponSpellSwap.started -= m_Wrapper.m_CombatActionsCallbackInterface.OnWeaponSpellSwap;
                @WeaponSpellSwap.performed -= m_Wrapper.m_CombatActionsCallbackInterface.OnWeaponSpellSwap;
                @WeaponSpellSwap.canceled -= m_Wrapper.m_CombatActionsCallbackInterface.OnWeaponSpellSwap;
            }
            m_Wrapper.m_CombatActionsCallbackInterface = instance;
            if (instance != null)
            {
                @BasicAttack.started += instance.OnBasicAttack;
                @BasicAttack.performed += instance.OnBasicAttack;
                @BasicAttack.canceled += instance.OnBasicAttack;
                @RightSwing.started += instance.OnRightSwing;
                @RightSwing.performed += instance.OnRightSwing;
                @RightSwing.canceled += instance.OnRightSwing;
                @LeftSwing.started += instance.OnLeftSwing;
                @LeftSwing.performed += instance.OnLeftSwing;
                @LeftSwing.canceled += instance.OnLeftSwing;
                @UpSwing.started += instance.OnUpSwing;
                @UpSwing.performed += instance.OnUpSwing;
                @UpSwing.canceled += instance.OnUpSwing;
                @DownSwing.started += instance.OnDownSwing;
                @DownSwing.performed += instance.OnDownSwing;
                @DownSwing.canceled += instance.OnDownSwing;
                @StabAttack.started += instance.OnStabAttack;
                @StabAttack.performed += instance.OnStabAttack;
                @StabAttack.canceled += instance.OnStabAttack;
                @Block.started += instance.OnBlock;
                @Block.performed += instance.OnBlock;
                @Block.canceled += instance.OnBlock;
                @WeaponSpellSwap.started += instance.OnWeaponSpellSwap;
                @WeaponSpellSwap.performed += instance.OnWeaponSpellSwap;
                @WeaponSpellSwap.canceled += instance.OnWeaponSpellSwap;
            }
        }
    }
    public CombatActions @Combat => new CombatActions(this);
    private int m_PCControlsSchemeIndex = -1;
    public InputControlScheme PCControlsScheme
    {
        get
        {
            if (m_PCControlsSchemeIndex == -1) m_PCControlsSchemeIndex = asset.FindControlSchemeIndex("PC Controls");
            return asset.controlSchemes[m_PCControlsSchemeIndex];
        }
    }
    public interface IGameplayActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnLook(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnSprint(InputAction.CallbackContext context);
        void OnInventory(InputAction.CallbackContext context);
        void OnInteract(InputAction.CallbackContext context);
    }
    public interface ICombatActions
    {
        void OnBasicAttack(InputAction.CallbackContext context);
        void OnRightSwing(InputAction.CallbackContext context);
        void OnLeftSwing(InputAction.CallbackContext context);
        void OnUpSwing(InputAction.CallbackContext context);
        void OnDownSwing(InputAction.CallbackContext context);
        void OnStabAttack(InputAction.CallbackContext context);
        void OnBlock(InputAction.CallbackContext context);
        void OnWeaponSpellSwap(InputAction.CallbackContext context);
    }
}
