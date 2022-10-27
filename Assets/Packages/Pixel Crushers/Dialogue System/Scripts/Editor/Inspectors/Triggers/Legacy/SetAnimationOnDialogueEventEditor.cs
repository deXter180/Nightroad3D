// Copyright (c) Pixel Crushers. All rights reserved.

#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace PixelCrushers.DialogueSystem
{

    [CustomEditor(typeof(SetAnimationOnDialogueEvent), true)]
    public class SetAnimationOnDialogueEventEditor : ReferenceDatabaseDialogueEventEditor
    {
        protected override bool isDeprecated { get { return true; } }
    }

}

#endif
