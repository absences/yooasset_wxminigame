// /*
// Created by Darsan
// */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace MainMenu
{
    public class CategoriesPanel : ShowHidable
    {
        [SerializeField] private CategoryTileUI _tilePrefab;
        [SerializeField] private RectTransform _content;

        private readonly List<CategoryTileUI> _tiles = new List<CategoryTileUI>();


        private void Awake()
        {
            foreach (var category in ResourceManager.Categories)
            {
                var tileUI = Instantiate(_tilePrefab,_content);
                tileUI.Category = category;
                tileUI.Locked = ResourceManager.IsCategoryLocked(category.ID);
                tileUI.Clicked +=TileUIOnClicked; 
                _tiles.Add(tileUI);
            }
        }

        private void TileUIOnClicked(CategoryTileUI tile)
        {
            if (tile.Locked)
                return;

            var levelsPanel = UIManager.Instance.LevelsPanel;
            levelsPanel.Show();
            levelsPanel.Category = tile.Category;
        }
    }
}