using System;
using System.Linq;
using UnityEngine;


public abstract class LevelCreatorPrefabSelector : MonoBehaviour
{

  
    // ReSharper disable once MethodTooLong
    public abstract ICycleTile GetSelectedPrefab();

}