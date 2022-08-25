using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(AttributeManager))]
public class AttributeManagerEditor : Editor
{
    #region SerializedProperties

    SerializedProperty strAttribute;
    SerializedProperty dexAttribute;
    SerializedProperty vitAttribute;
    SerializedProperty sprAttribute;
    SerializedProperty intAttribute;

    SerializedProperty strModType;
    SerializedProperty dexModType;
    SerializedProperty vitModType;
    SerializedProperty sprModType;
    SerializedProperty intModType;

    //Strength
    SerializedProperty str1stMultiplier;
    SerializedProperty str2ndMultiplier;
    SerializedProperty str3rdMultiplier;
    SerializedProperty str4thMultiplier;
    SerializedProperty str1stThreshold;
    SerializedProperty str2ndThreshold;
    SerializedProperty str3rdThreshold;
    SerializedProperty str4thThreshold;

    //Dexterity
    SerializedProperty dex1stMultiplier;
    SerializedProperty dex2ndMultiplier;
    SerializedProperty dex3rdMultiplier;
    SerializedProperty dex4thMultiplier;
    SerializedProperty dex1stThreshold;
    SerializedProperty dex2ndThreshold;
    SerializedProperty dex3rdThreshold;
    SerializedProperty dex4thThreshold;

    //Vitality
    SerializedProperty vit1stMultiplier;
    SerializedProperty vit2ndMultiplier;
    SerializedProperty vit3rdMultiplier;
    SerializedProperty vit4thMultiplier;
    SerializedProperty vit1stThreshold;
    SerializedProperty vit2ndThreshold;
    SerializedProperty vit3rdThreshold;
    SerializedProperty vit4thThreshold;

    //Spirit
    SerializedProperty spr1stMultiplier;
    SerializedProperty spr2ndMultiplier;
    SerializedProperty spr3rdMultiplier;
    SerializedProperty spr4thMultiplier;
    SerializedProperty spr1stThreshold;
    SerializedProperty spr2ndThreshold;
    SerializedProperty spr3rdThreshold;
    SerializedProperty spr4thThreshold;

    //Intelligence
    SerializedProperty int1stMultiplier;
    SerializedProperty int2ndMultiplier;
    SerializedProperty int3rdMultiplier;
    SerializedProperty int4thMultiplier;
    SerializedProperty int1stThreshold;
    SerializedProperty int2ndThreshold;
    SerializedProperty int3rdThreshold;
    SerializedProperty int4thThreshold;

    bool baseAttributeGroup, strGroup, dexGroup, vitGroup, sprGroup, intGroup = false;
    #endregion

    private void OnEnable()
    {
        strAttribute = serializedObject.FindProperty("StrengthInfo");
        dexAttribute = serializedObject.FindProperty("DexterityInfo");
        vitAttribute = serializedObject.FindProperty("VitalityInfo");
        sprAttribute = serializedObject.FindProperty("SpiritInfo");
        intAttribute = serializedObject.FindProperty("IntelligenceInfo");

        strModType = serializedObject.FindProperty("STRModType");
        dexModType = serializedObject.FindProperty("DEXModType");
        vitModType = serializedObject.FindProperty("VITModType");
        sprModType = serializedObject.FindProperty("SPRModType");
        intModType = serializedObject.FindProperty("INTModType");

        str1stMultiplier = serializedObject.FindProperty("STR1stMultiplier");
        str2ndMultiplier = serializedObject.FindProperty("STR2ndMultiplier");
        str3rdMultiplier = serializedObject.FindProperty("STR3rdMultiplier");
        str4thMultiplier = serializedObject.FindProperty("STR4thMultiplier");
        str1stThreshold = serializedObject.FindProperty("STR1stThreshold");
        str2ndThreshold = serializedObject.FindProperty("STR2ndThreshold");
        str3rdThreshold = serializedObject.FindProperty("STR3rdThreshold");
        str4thThreshold = serializedObject.FindProperty("STR4thThreshold");

        dex1stMultiplier = serializedObject.FindProperty("DEX1stMultiplier");
        dex2ndMultiplier = serializedObject.FindProperty("DEX2ndMultiplier");
        dex3rdMultiplier = serializedObject.FindProperty("DEX3rdMultiplier");
        dex4thMultiplier = serializedObject.FindProperty("DEX4thMultiplier");
        dex1stThreshold = serializedObject.FindProperty("DEX1stThreshold");
        dex2ndThreshold = serializedObject.FindProperty("DEX2ndThreshold");
        dex3rdThreshold = serializedObject.FindProperty("DEX3rdThreshold");
        dex4thThreshold = serializedObject.FindProperty("DEX4thThreshold");

        vit1stMultiplier = serializedObject.FindProperty("VIT1stMultiplier");
        vit2ndMultiplier = serializedObject.FindProperty("VIT2ndMultiplier");
        vit3rdMultiplier = serializedObject.FindProperty("VIT3rdMultiplier");
        vit4thMultiplier = serializedObject.FindProperty("VIT4thMultiplier");
        vit1stThreshold = serializedObject.FindProperty("VIT1stThreshold");
        vit2ndThreshold = serializedObject.FindProperty("VIT2ndThreshold");
        vit3rdThreshold = serializedObject.FindProperty("VIT3rdThreshold");
        vit4thThreshold = serializedObject.FindProperty("VIT4thThreshold");

        spr1stMultiplier = serializedObject.FindProperty("SPR1stMultiplier");
        spr2ndMultiplier = serializedObject.FindProperty("SPR2ndMultiplier");
        spr3rdMultiplier = serializedObject.FindProperty("SPR3rdMultiplier");
        spr4thMultiplier = serializedObject.FindProperty("SPR4thMultiplier");
        spr1stThreshold = serializedObject.FindProperty("SPR1stThreshold");
        spr2ndThreshold = serializedObject.FindProperty("SPR2ndThreshold");
        spr3rdThreshold = serializedObject.FindProperty("SPR3rdThreshold");
        spr4thThreshold = serializedObject.FindProperty("SPR4thThreshold");

        int1stMultiplier = serializedObject.FindProperty("INT1stMultiplier");
        int2ndMultiplier = serializedObject.FindProperty("INT2ndMultiplier");
        int3rdMultiplier = serializedObject.FindProperty("INT3rdMultiplier");
        int4thMultiplier = serializedObject.FindProperty("INT4thMultiplier");
        int1stThreshold = serializedObject.FindProperty("INT1stThreshold");
        int2ndThreshold = serializedObject.FindProperty("INT2ndThreshold");
        int3rdThreshold = serializedObject.FindProperty("INT3rdThreshold");
        int4thThreshold = serializedObject.FindProperty("INT4thThreshold");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        baseAttributeGroup = EditorGUILayout.BeginFoldoutHeaderGroup(baseAttributeGroup, "Base Attribute Group");
        if (baseAttributeGroup)
        {
            EditorGUILayout.PropertyField(strAttribute);
            EditorGUILayout.PropertyField(dexAttribute);
            EditorGUILayout.PropertyField(vitAttribute);
            EditorGUILayout.PropertyField(sprAttribute);
            EditorGUILayout.PropertyField(intAttribute);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUILayout.Space(5);
        strGroup = EditorGUILayout.BeginFoldoutHeaderGroup(strGroup, "Strength Group");
        if (strGroup)
        {
            EditorGUILayout.PropertyField(strModType);
            EditorGUILayout.PropertyField(str1stMultiplier);
            EditorGUILayout.PropertyField(str2ndMultiplier);
            EditorGUILayout.PropertyField(str3rdMultiplier);
            EditorGUILayout.PropertyField(str4thMultiplier);
            EditorGUILayout.PropertyField(str1stThreshold);
            EditorGUILayout.PropertyField(str2ndThreshold);
            EditorGUILayout.PropertyField(str3rdThreshold);
            EditorGUILayout.PropertyField(str4thThreshold);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUILayout.Space(5);
        dexGroup = EditorGUILayout.BeginFoldoutHeaderGroup(dexGroup, "Dexterity Group");
        if (dexGroup)
        {
            EditorGUILayout.PropertyField(dexModType);
            EditorGUILayout.PropertyField(dex1stMultiplier);
            EditorGUILayout.PropertyField(dex2ndMultiplier);
            EditorGUILayout.PropertyField(dex3rdMultiplier);
            EditorGUILayout.PropertyField(dex4thMultiplier);
            EditorGUILayout.PropertyField(dex1stThreshold);
            EditorGUILayout.PropertyField(dex2ndThreshold);
            EditorGUILayout.PropertyField(dex3rdThreshold);
            EditorGUILayout.PropertyField(dex4thThreshold);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUILayout.Space(5);
        vitGroup = EditorGUILayout.BeginFoldoutHeaderGroup(vitGroup, "Vitality Group");
        if (vitGroup)
        {
            EditorGUILayout.PropertyField(vitModType);
            EditorGUILayout.PropertyField(vit1stMultiplier);
            EditorGUILayout.PropertyField(vit2ndMultiplier);
            EditorGUILayout.PropertyField(vit3rdMultiplier);
            EditorGUILayout.PropertyField(vit4thMultiplier);
            EditorGUILayout.PropertyField(vit1stThreshold);
            EditorGUILayout.PropertyField(vit2ndThreshold);
            EditorGUILayout.PropertyField(vit3rdThreshold);
            EditorGUILayout.PropertyField(vit4thThreshold);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUILayout.Space(5);
        sprGroup = EditorGUILayout.BeginFoldoutHeaderGroup(sprGroup, "Spirit Group");
        if (sprGroup)
        {
            EditorGUILayout.PropertyField(sprModType);
            EditorGUILayout.PropertyField(spr1stMultiplier);
            EditorGUILayout.PropertyField(spr2ndMultiplier);
            EditorGUILayout.PropertyField(spr3rdMultiplier);
            EditorGUILayout.PropertyField(spr4thMultiplier);
            EditorGUILayout.PropertyField(spr1stThreshold);
            EditorGUILayout.PropertyField(spr2ndThreshold);
            EditorGUILayout.PropertyField(spr3rdThreshold);
            EditorGUILayout.PropertyField(spr4thThreshold);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUILayout.Space(5);
        intGroup = EditorGUILayout.BeginFoldoutHeaderGroup(intGroup, "Intelligence Group");
        if (intGroup)
        {
            EditorGUILayout.PropertyField(intModType);
            EditorGUILayout.PropertyField(int1stMultiplier);
            EditorGUILayout.PropertyField(int2ndMultiplier);
            EditorGUILayout.PropertyField(int3rdMultiplier);
            EditorGUILayout.PropertyField(int4thMultiplier);
            EditorGUILayout.PropertyField(int1stThreshold);
            EditorGUILayout.PropertyField(int2ndThreshold);
            EditorGUILayout.PropertyField(int3rdThreshold);
            EditorGUILayout.PropertyField(int4thThreshold);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        serializedObject.ApplyModifiedProperties();
    }
}
#endif