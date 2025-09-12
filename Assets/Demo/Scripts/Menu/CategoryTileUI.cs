// /*
// Created by Darsan
// */

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MainMenu
{
    public class CategoryTileUI : MonoBehaviour,IPointerClickHandler
    {
        public event Action<CategoryTileUI> Clicked;

        [SerializeField] private Image _img;
        [SerializeField] private TextMeshProUGUI _titleTxt;
        [SerializeField] private TextMeshProUGUI _sizeTxt;
        [SerializeField] private TextMeshProUGUI _brainTxt;

 

        [SerializeField]private List<GameObject> _lockEffects = new List<GameObject>();
        [SerializeField]private List<GameObject> _unLockEffects = new List<GameObject>();

        private Category _category;
        private bool _locked;

        public Category Category
        {
            get => _category;
            set
            {
                _img.sprite = value.Image;
                _titleTxt.text = value.Name;
                _sizeTxt.text = $"{value.Grid}x{value.Grid}";
                _brainTxt.text = $"{value.TotalBrain}";

                


                _category = value;
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
}