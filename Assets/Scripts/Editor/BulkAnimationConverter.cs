using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class BulkAnimationConverter : EditorWindow
{
    private Object[] genericAnimations;
    private Avatar targetAvatar;

    [MenuItem("Tools/Bulk Animation Converter")]
    public static void ShowWindow()
    {
        GetWindow(typeof(BulkAnimationConverter));
    }

    void OnGUI()
    {
        GUILayout.Label("Bulk Animation Converter", EditorStyles.boldLabel);

        targetAvatar = EditorGUILayout.ObjectField("Target Humanoid Avatar", targetAvatar, typeof(Avatar), false) as Avatar;

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Generic Animations to Convert");
        EditorGUI.indentLevel++;
        ScriptableObject target = this;
        SerializedObject so = new SerializedObject(target);
        SerializedProperty genericAnimationsProperty = so.FindProperty("genericAnimations");
        EditorGUILayout.PropertyField(genericAnimationsProperty, true);
        so.ApplyModifiedProperties();
        EditorGUI.indentLevel--;

        if (GUILayout.Button("Convert Animations"))
        {
            ConvertAnimations();
        }
    }

    void ConvertAnimations()
    {
        if (targetAvatar == null)
        {
            EditorUtility.DisplayDialog("Error", "Please assign a target humanoid avatar.", "OK");
            return;
        }

        if (genericAnimations == null || genericAnimations.Length == 0)
        {
            EditorUtility.DisplayDialog("Error", "Please assign some generic animations to convert.", "OK");
            return;
        }

        foreach (Object obj in genericAnimations)
        {
            if (obj is AnimationClip)
            {
                AnimationClip clip = obj as AnimationClip;
                string path = AssetDatabase.GetAssetPath(clip);
                
                ModelImporter importer = AssetImporter.GetAtPath(path) as ModelImporter;
                if (importer != null)
                {
                    importer.animationType = ModelImporterAnimationType.Human;
                    importer.avatarSetup = ModelImporterAvatarSetup.CopyFromOther;
                    importer.sourceAvatar = targetAvatar;
                    
                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                    Debug.Log("Converted: " + clip.name);
                }
            }
        }

        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Conversion Complete", "All assigned animations have been converted to humanoid.", "OK");
    }
}