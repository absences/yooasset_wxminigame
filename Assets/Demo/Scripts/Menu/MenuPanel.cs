// /*
// Created by Darsan
// */

using UnityEngine;

namespace MainMenu
{
    public class MenuPanel : MonoBehaviour
    {
        public void OnClickPlay()
        {
            UIManager.Instance.CategoriesPanel.Show();
        }
    }
}