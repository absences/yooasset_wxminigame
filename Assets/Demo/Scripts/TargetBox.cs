// /*
// Created by Darsan
// */

using UnityEngine;

public class TargetBox : MonoBehaviour
{
    public Tile Tile { get; set; }
    public float Size
    {
        get => transform.localScale.x;
        set => transform.localScale = Vector3.one * value;
    }
}