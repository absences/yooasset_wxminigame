using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private TextMeshPro _text;
    [SerializeField] private SpriteRenderer _bg;
    [SerializeField]private List<StateAndColors> _stateAndColors = new List<StateAndColors>();

    private State _currentState;

    public float Size
    {
        get => transform.localScale.x;
        set => transform.localScale = Vector3.one*value;
    }

    public string Letter
    {
        get => _text.text;
        set => _text.text = value;
    }

    public Vector2Int GridCoordinate { get; set; }

    public State CurrentState
    {
        get => _currentState;
        set
        {
            var stateAndColors = _stateAndColors.First(colors => colors.state == value);
            _bg.color = stateAndColors.bgColor;
            _text.color = stateAndColors.textColor;
            _currentState = value;
        }
    }

    public int Order => 2;




    public enum State
    {
        None,Touch,Collected
    }

    [Serializable]
    private struct StateAndColors
    {
        [SerializeField] public State state;
        [SerializeField] public Color bgColor;
        [SerializeField] public Color textColor;
    }
}