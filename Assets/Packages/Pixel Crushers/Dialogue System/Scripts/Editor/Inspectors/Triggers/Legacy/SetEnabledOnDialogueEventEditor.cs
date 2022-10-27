// Copyright (c) Pixel Crushers. All rights reserved.

#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace PixelCrushers.DialogueSystem
{

    [CustomEditor(typeof(SetEnabledOnDialogueEvent), true)]
    public class SetEnabledOnDialogueEventEditor : ReferenceDatabaseDialogueEventEditor
    {
        protected override bool isDeprecated { get { return true; } }
    }

}

#endif
