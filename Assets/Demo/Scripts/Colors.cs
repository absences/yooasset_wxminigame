using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Colors : ScriptableObject, IEnumerable<Color>
{
    public static Colors Default => Resources.Load<Colors>(nameof(Colors));

    [SerializeField] private List<Color> _colors = new List<Color>();


    public Color this[int i] => _colors[i];

    public IEnumerator<Color> GetEnumerator()
    {
        return _colors.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}