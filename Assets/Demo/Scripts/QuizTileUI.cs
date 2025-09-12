using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuizTileUI : MonoBehaviour,IPointerClickHandler
{
    public event Action<QuizTileUI> Clicked; 

    [SerializeField] private TextMeshProUGUI _lvlTxt;
    [SerializeField] private GameObject _lockEffect;


    private Quiz _level;
    private bool _locked;

    public Quiz Level
    {
        get => _level;
        set
        {
            _level = value;
        }
    }

    public bool Locked
    {
        get => _locked;
        set
        {
            _lockEffect.gameObject.SetActive(value);
            _locked = value;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Clicked?.Invoke(this);
    }
}