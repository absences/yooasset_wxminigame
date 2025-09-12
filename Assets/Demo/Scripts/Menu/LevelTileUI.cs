// /*
// Created by Darsan
// */

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelTileUI : MonoBehaviour,IPointerClickHandler
{
    public event Action<LevelTileUI> Clicked;
    
    [SerializeField] private RectTransform _letterTilesContent;
    [SerializeField] private LetterTileUI _letterTilePrefab;
    [SerializeField] private List<GameObject> _lockEffects = new List<GameObject>();
    [SerializeField] private List<GameObject> _unLockEffects = new List<GameObject>();
    [SerializeField] private TextMeshProUGUI _statusTxt;
    [SerializeField] private TextMeshProUGUI _titleTxt;


    private bool _locked;
    private bool _solved;
    private Quiz _quiz;

    public Quiz Quiz
    {
        get => _quiz;
        set
        {
            _titleTxt.text = "Level " + value.level;

            var totalSize
                = _letterTilesContent.rect.width;
            var grid = Mathf.RoundToInt(Mathf.Sqrt(value.letters.Length));
            var spacing = 2f;
            var tileSize = (totalSize - (grid - 1) * spacing) / grid;
            var topLeftAnchorPos = new Vector2(-totalSize/2 + tileSize/2 ,totalSize/2 - tileSize/2);

            for (var i = 0; i < grid; i++)
            {
                for (var j = 0; j < grid; j++)
                {
                    var letterTileUI = Instantiate(_letterTilePrefab,_letterTilesContent);
                    letterTileUI.Letter = value.letters[i * grid + j] + "";
                    var rectTransform = (RectTransform)letterTileUI.transform;
                    rectTransform.anchoredPosition = topLeftAnchorPos +
                                                     new Vector2((j) * tileSize + j * spacing,
                                                         -i * tileSize - i * spacing);
                    rectTransform.sizeDelta = new Vector2(tileSize,tileSize);
                }
            }


            _quiz = value;
        }
    }

    public bool Solved
    {
        get => _solved;
        set
        {
            _statusTxt.text = value ? "Solved!" : "Not Solved";
            _solved = value;
        }
    }

    public bool Locked
    {
        get => _locked;
        set
        {
            _lockEffects.ForEach(go => go.SetActive(value));
            _unLockEffects.ForEach(go => go.SetActive(!value));
            _locked = value;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Clicked?.Invoke(this);
    }
}