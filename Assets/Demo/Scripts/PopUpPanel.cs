using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUpPanel : ShowHidable
{

    [SerializeField] private TextMeshProUGUI _titleTxt, _messageTxt;
    [SerializeField]private List<Button> _buttons = new List<Button>();
    private ViewModel _mViewModel;

    public ViewModel MViewModel
    {
        get { return _mViewModel; }
        set
        {
            _titleTxt.text = value.Title;
            _messageTxt.text = value.Message;
            for (var i = 0; i < _buttons.Count; i++)
            {
                if (i < value.Buttons.Length)
                {
                    _buttons[i].gameObject.SetActive(true);
                    var j = i;
                    _buttons[i].onClick.RemoveAllListeners();
                    _buttons[i].onClick.AddListener(() => { OnClickButton(value.Buttons[j].action); });
                    _buttons[i].GetComponentInChildren<TextMeshProUGUI>().text = value.Buttons[i].title;
                }
                else
                {
                    _buttons[i].gameObject.SetActive(false);
                }
            }
            _mViewModel = value;
        }
    }

    private void OnClickButton(Action action)
    {
        Hide();
        action?.Invoke();
    }


    public void ShowAsConfirmation(string title,string message,Action<bool> onResulted=null)
    {
       MViewModel = new ViewModel
        {
            Title = title,
            Message = message,
            Buttons = new[]
            {
                new ViewModel.Button
                {
                    title = "No",
                    action = ()=> onResulted?.Invoke(false)
                },
                new ViewModel.Button
                {
                    title = "Yes",
                    action = () => onResulted?.Invoke(true)
                }
            }
        };
       Show();
    }

    public struct ViewModel
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public Button[] Buttons { get; set; }

        public struct Button
        {
            public string title;
            public Action action;
        }
    }
}
