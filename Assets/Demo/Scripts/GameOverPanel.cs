using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Game
{
    public class GameOverPanel : ShowHidable
    {
        [SerializeField] private TimeOutProgressBar _continueProgressBar;
        [SerializeField] private Button _restartBtn;
        [SerializeField] private TextMeshProUGUI _bestTxt;

        public void OnClickRestart()
        {
            MyGame.GameManager.LoadScene(SceneManager.GetActiveScene().name, false);
        }

        protected override void OnShowCompleted()
        {
            _continueProgressBar.gameObject.SetActive(true);
            Invoke(nameof(ShowRestart), 1);
            _bestTxt.text = $"BEST - {MyGame.GameManager.BEST_SCORE}";

            base.OnShowCompleted();
        }

        private void ShowRating()
        {
            SharedUIManager.RatingPopUp?.Show();
        }

        private void ShowRestart()
        {
            _restartBtn.gameObject.SetActive(true);
        }
    }
}

public enum Tag
{
}

public enum Layer
{
}

public static class Extensions
{
    public static Color WithAlpha(this Color color, float alpha)
    {
        color.a = alpha;
        return color;
    }

    public static int GetMask(this Layer layer) =>
        LayerMask.NameToLayer(layer.ToString());

    public static T GetRandom<T>(this IEnumerable<T> enumerable)
    {
        var list = enumerable.ToList();
        return list[Random.Range(0, list.Count)];
    }

    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
        foreach (var x1 in enumerable)
        {
            action?.Invoke(x1);
        }
    }

    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<int, T> action)
    {
        var i = 0;
        foreach (var x1 in enumerable)
        {
            action?.Invoke(i, x1);
            i++;
        }
    }


    public static bool Contains<T>(this IEnumerable<T> enumerable, IEnumerable<T> items)
    {
        var list = enumerable.ToList();

        foreach (var item in items)
        {
            if (!list.Contains(item))
            {
                return false;
            }
        }

        return true;
    }

    public static bool HasSameElementsSameNumber<T>(this IEnumerable<T> enumerable, IEnumerable<T> items)
    {
        var list = enumerable.ToList();
        var itemList = items.ToList();

        for (var i = 0; i < list.Count; i++)
        {
            var index = itemList.IndexOf(list[i]);

            if (index == -1)
                return false;

            itemList.RemoveAt(index);
            list.RemoveAt(i);
            --i;
        }

        return itemList.Count == 0;
    }
}