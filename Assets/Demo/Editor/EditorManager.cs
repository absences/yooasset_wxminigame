using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class EditorManager
{
    public const string RESOURCES_PATH = "Assets/Resources";

    [MenuItem("MyGames/Clear Prefs")]
    public static void ClearPrefs()
    {
        PrefManager.Clear();
    }

    private static readonly string GAME_SETTING_PATH = $"Assets/Resources/{GameSettings.DEFAULT_NAME}.asset";


    static EditorManager()
    {
        var defineSymbolsForGroup = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS);
        Debug.Log("Ios Symbols:" + defineSymbolsForGroup);
        var strings = defineSymbolsForGroup.Split(';').ToList();
        if (!strings.Contains("NO_GPGS"))
        {
            strings.Add("NO_GPGS");
        }
        var str = "";
        foreach (var s in strings)
        {
            str += s + ";";
        }
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, str);
    }


    [MenuItem("MyGames/Settings")]
    public static void Settings()
    {
        var groupScriptable = AssetDatabase.LoadAssetAtPath<GameSettings>(GAME_SETTING_PATH);

        if (groupScriptable == null)
        {
            groupScriptable = ScriptableObject.CreateInstance<GameSettings>();
            AssetDatabase.CreateAsset(groupScriptable, GAME_SETTING_PATH);
            AssetDatabase.SaveAssets();
        }

        Selection.activeObject = groupScriptable;
    }
}