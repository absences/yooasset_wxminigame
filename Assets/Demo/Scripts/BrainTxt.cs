// /*
// Created by Darsan
// */

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TextMeshProUGUI))]
public class BrainTxt : MonoBehaviour
{
    private TextMeshProUGUI _text;

    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
        _text.text = ResourceManager.Brains.ToString();
    }

    private void Update()
    {
        _text.text = ResourceManager.Brains.ToString();
    }
}