using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyRewardPanel : ShowHidable
{

    [SerializeField] private Button _collectBtn, _doubleBtn;
    [SerializeField] private TextMeshProUGUI _coinsTxt;

#if DAILY_REWARD
    private int _coins;

    public int Coins
    {
        get
        {
            return _coins;
        }
        set
        {
            _coinsTxt.text = value.ToString();
            _coins = value;
        }
    }


    private void Awake()
    {
        _collectBtn.onClick.AddListener(OnClickCollect);
        _doubleBtn.onClick.AddListener(OnClickDouble);
    }

    public override void Show(bool animate = true, Action completed = null)
    {
        if (!MyGame.GameManager.HasPendingDailyReward)
            throw new InvalidOperationException();

        Coins = MyGame.GameManager.PendingRewardValue;
        _doubleBtn.gameObject.SetActive(AdsManager.IsVideoAvailable());
        base.Show(animate, completed);

    }

    private void OnClickCollect()
    {
        MyGame.GameManager.GetReward(false);
        Hide();
    }

    private void OnClickDouble()
    {
        AdsManager.ShowVideoAds((success) =>
        {
            if (!success)
                return;

            MyGame.GameManager.GetReward(true);
            Hide();
        });
    }
#endif
}
