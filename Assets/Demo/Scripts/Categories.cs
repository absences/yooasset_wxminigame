// /*
// Created by Darsan
// */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Categories : ScriptableObject,IEnumerable<Category>
{
    public const string DEFAULT_NAME = nameof(Categories);
    public static Categories Default => Resources.Load<Categories>(DEFAULT_NAME);



    [SerializeField]private List<Category> _category = new List<Category>();

    public IEnumerator<Category> GetEnumerator()
    {
        return _category.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

#if UNITY_EDITOR
    [UnityEditor.MenuItem("MyGames/Categories")]
    public static void Open()
    {
        GamePlayEditorManager.OpenScriptableAtDefault<Categories>();
    }
#endif
}