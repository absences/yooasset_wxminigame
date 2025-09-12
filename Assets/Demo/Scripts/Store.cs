using System.Collections.Generic;
using UnityEngine;

public class Store : ScriptableObject
{
    public const string DEFAULT_NAME = nameof(Store);

    [SerializeField]private List<HintProduct> _hints = new List<HintProduct>();

    public static Store Default => Resources.Load<Store>(DEFAULT_NAME);
    public IEnumerable<HintProduct> Hints => _hints;


#if UNITY_EDITOR
    [UnityEditor.MenuItem("MyGames/Store")]
    public static void Open()
    {
        GamePlayEditorManager.OpenScriptableAtDefault<Store>();
    }
#endif
}