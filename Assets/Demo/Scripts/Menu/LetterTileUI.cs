// /*
// Created by Darsan
// */

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LetterTileUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;


    public string Letter
    {
        get => _text.text;
        set => _text.text = value;
    }
}