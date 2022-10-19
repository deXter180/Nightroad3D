using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(InventoryItemSO))]
public class InventoryItemSOEditor : Editor
{
    #region SerializedProperties

    SerializedProperty itemIDProp;
    SerializedProperty itemNameProp;
    SerializedProperty itemDescriptionProp;
    SerializedProperty usableProp;
    SerializedProperty craftableProp;
    SerializedProperty stackableProp;
    SerializedProperty widthProp;
    SerializedProperty heightProp;
    SerializedProperty itemTypeProp;
    SerializedProperty weaponTypeProp;
    SerializedProperty armorTypeProp;
    SerializedProperty spellTypeProp;
    SerializedProperty spellCategoryProp;
    SerializedProperty rarityProp;
    SerializedProperty attributeAmountProp;
    SerializedProperty attributeIconProp;
    SerializedProperty inventoryPrefabProp;
    SerializedProperty worldPrefabProp;
    SerializedProperty requirementListProp;

    bool baseInfoGroup, typeGroup, referenceGroup = false;

    #endregion

    private void OnEnable()
    {
        itemIDProp = serializedObject.FindProperty("itemID");
        itemNameProp = serializedObject.FindProperty("itemName");
        itemDescriptionProp = serializedObject.FindProperty("itemDescription");
        usableProp = serializedObject.FindProperty("usable");
        craftableProp = serializedObject.FindProperty("craftable");
        stackableProp = serializedObject.FindProperty("stackable");
        widthProp = serializedObject.FindProperty("width");
        heightProp = serializedObject.FindProperty("height");
        itemTypeProp = serializedObject.FindProperty("itemType");
        weaponTypeProp = serializedObject.FindProperty("weaponType");
        armorTypeProp = serializedObject.FindProperty("armorType");
        spellTypeProp = serializedObject.FindProperty("spellType");
        spellCategoryProp = serializedObject.FindProperty("spellCategory");
        rarityProp = serializedObject.FindProperty("rarity");
        attributeAmountProp = serializedObject.FindProperty("attributeAmount");
        attributeIconProp = serializedObject.FindProperty("attributeIcon");
        inventoryPrefabProp = serializedObject.FindProperty("inventoryPrefab");
        worldPrefabProp = serializedObject.FindProperty("worldPrefab");
        requirementListProp = serializedObject.FindProperty("requirementList");
    }

    public override void OnInspectorGUI()
    {
        InventoryItemSO itemSO = (InventoryItemSO)target;
        serializedObject.Update();
        baseInfoGroup = EditorGUILayout.BeginFoldoutHeaderGroup(baseInfoGroup, "Base Info");
        if (baseInfoGroup)
        {
            EditorGUILayout.PropertyField(itemIDProp);
            EditorGUILayout.PropertyField(itemNameProp);
            EditorGUILayout.PropertyField(itemDescriptionProp);
            EditorGUILayout.PropertyField(usableProp);
            EditorGUILayout.PropertyField(craftableProp);
            EditorGUILayout.PropertyField(stackableProp);
            EditorGUILayout.PropertyField(widthProp);
            EditorGUILayout.PropertyField(heightProp);
            EditorGUILayout.PropertyField(attributeAmountProp);
            requirementListProp.arraySize = EditorGUILayout.IntField("Number of Requirements", requirementListProp.arraySize); 
            for (int i = 0; i < requirementListProp.arraySize; i++)
            {
                var requirement = requirementListProp.GetArrayElementAtIndex(i);
                EditorGUILayout.PropertyField(requirement, new GUIContent("Requirement" + i));
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUILayout.Space(10);
        typeGroup = EditorGUILayout.BeginFoldoutHeaderGroup(typeGroup, "Assign Type");
        if (typeGroup)
        {
            EditorGUILayout.PropertyField(rarityProp);
            EditorGUILayout.PropertyField(itemTypeProp);
            switch (itemSO.ItemType)
            {
                case ItemTypes.Weapon:
                    EditorGUILayout.PropertyField(weaponTypeProp);
                    break;
                case ItemTypes.Ammo:
                    EditorGUILayout.PropertyField(weaponTypeProp);
                    break;
                case ItemTypes.Spell:
                    EditorGUILayout.PropertyField(spellTypeProp);
                    EditorGUILayout.PropertyField(spellCategoryProp);
                    break;
                case ItemTypes.Armor:
                    EditorGUILayout.PropertyField(armorTypeProp);
                    break;
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUILayout.Space(10);
        referenceGroup = EditorGUILayout.BeginFoldoutHeaderGroup(referenceGroup, "Assign References");
        if (referenceGroup)
        {
            EditorGUILayout.PropertyField(attributeIconProp);
            EditorGUILayout.PropertyField(inventoryPrefabProp);
            EditorGUILayout.PropertyField(worldPrefabProp);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        serializedObject.ApplyModifiedProperties();
    }
}
#endif
