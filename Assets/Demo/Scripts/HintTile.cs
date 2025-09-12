// /*
// Created by Darsan
// */

using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class HintTile : MonoBehaviour
{
    [SerializeField] private TextMeshPro _text;


    public float Size
    {
        get => transform.localScale.x;
        set => transform.localScale = Vector3.one * value;
    }

    public string Letter
    {
        get => _text.text;
        set => _text.text = value;
    }




}