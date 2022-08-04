using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(AudioSO))]
public class AudioSOEditor : Editor
{

    #region SerializedProperties

    SerializedProperty audioType;
    SerializedProperty musicAudioList;
    SerializedProperty environmentAudioList;
    SerializedProperty weaponAudioList;
    SerializedProperty spellAudioList;
    SerializedProperty enemyAudioList;
    #endregion

    private void OnEnable()
    {
        audioType = serializedObject.FindProperty("AudioType");
        musicAudioList = serializedObject.FindProperty("musicAudioList");
        environmentAudioList = serializedObject.FindProperty("environmentAudioList");
        weaponAudioList = serializedObject.FindProperty("weaponAudioList");
        spellAudioList = serializedObject.FindProperty("spellAudioList");
        enemyAudioList = serializedObject.FindProperty("enemyAudioList");
    }

    public override void OnInspectorGUI()
    {
        AudioSO audioSO = (AudioSO)target;
        serializedObject.Update();
        EditorGUILayout.PropertyField(audioType);
        switch (audioSO.AudioType)
        {
            case AudioTypes.Music:
                {
                    EditorGUILayout.PropertyField(musicAudioList);
                    break;
                }

            case AudioTypes.Environment:
                {
                    EditorGUILayout.PropertyField(environmentAudioList);
                    break;
                }

            case AudioTypes.Weapon:
                {
                    EditorGUILayout.PropertyField(weaponAudioList);
                    break;
                }

            case AudioTypes.Spell:
                {
                    EditorGUILayout.PropertyField(spellAudioList);
                    break;
                }

            case AudioTypes.Enemy:
                {
                    EditorGUILayout.PropertyField(enemyAudioList);
                    break;
                }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif