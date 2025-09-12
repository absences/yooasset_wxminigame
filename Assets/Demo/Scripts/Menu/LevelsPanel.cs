// /*
// Created by Darsan
// */

using System.Collections.Generic;
using System.Linq;
using MyGame;
using UnityEngine;

public class LevelsPanel : ShowHidable
{
    [SerializeField] private LevelTileUI _levelTilePrefab;
    [SerializeField] private RectTransform _content;

    public Category Category
    {
        get => _category;
        set
        {
            _category = value;
            _tiles.ForEach(ui => Destroy(ui.gameObject));
            _tiles.Clear();

            var quizzes = value.Quizzes.ToList();
            for (var i = 0; i < quizzes.Count; i++)
            {
                var levelTileUI = Instantiate(_levelTilePrefab,_content);
                levelTileUI.Quiz = quizzes[i];
                levelTileUI.Locked = ResourceManager.IsLevelLocked(value.ID, i + 1);
                levelTileUI.Solved = ResourceManager.GetCompletedLevelForCategory(value.ID) >= i + 1;
                levelTileUI.Clicked+=LevelTileUIOnClicked;
                _tiles.Add(levelTileUI);
            }
        }
    }

    private void LevelTileUIOnClicked(LevelTileUI tile)
    {
        if(tile.Locked)
            return;

        GameManager.LoadGame(new LoadGameDetails
        {
            Category = Category,
            Level = tile.Quiz.level
        });
    }

    private readonly List<LevelTileUI> _tiles = new List<LevelTileUI>();
    private Category _category;
}