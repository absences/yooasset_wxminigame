#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

public class GamePlayEditorManager
{
    public static void OpenScriptableAtDefault<T>() where T : ScriptableObject
    {
        var type = typeof(T);
        var field = type.GetField("DEFAULT_NAME");
        if (field == null)
        {
            throw new Exception("The Type you open dont have 'DEFAULT_NAME' constant");
        }
        var path = $"Assets/Resources/{field.GetRawConstantValue()}.asset";
        var asset = AssetDatabase.LoadAssetAtPath<T>(path);

        if (asset == null)
        {
            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
        }

        Selection.activeObject = asset;
    }
}

#endif