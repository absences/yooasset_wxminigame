using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game;
using MyGame;
using UnityEngine;
using UnityEngine.UI;

public class LevelPackCompletePanel : ShowHidable
{
    [SerializeField] private Button _nextPackBtn;


    public override void Show(bool animate = true, Action completed = null)
    {
        base.Show(animate, completed);

        var categories = ResourceManager.Categories.ToList();
        var index = categories.FindIndex(category => category.ID == LevelManager.Instance.Category.ID);
        _nextPackBtn.gameObject.SetActive(index<categories.Count-1);
    }

    public void OnClickNextPack()
    {
        var categories = ResourceManager.Categories.ToList();
        var index = categories.FindIndex(category => category.ID == LevelManager.Instance.Category.ID);

        GameManager.LoadGame(new LoadGameDetails
        {
            Category = categories[index+1],
        });
    }

    public void OnClickHome()
    {
        GameManager.LoadScene("MainMenu");
    }
}
