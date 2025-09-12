using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_IOS
using UnityEngine.iOS;
#endif
using UnityEngine.SceneManagement;
using YooAsset;
using Random = UnityEngine.Random;

namespace MyGame
{

    public partial class GameManager : Singleton<GameManager>
    {

        public static event Action<int> TopScoreChanged;


        public static int BEST_SCORE
        {
            get { return PlayerPrefs.GetInt(nameof(BEST_SCORE)); }
            set
            {
                if (BEST_SCORE >= value)
                    return;
                PlayerPrefs.SetInt(nameof(BEST_SCORE), value);
                TopScoreChanged?.Invoke(value);
            }
        }

        public static int TOTAL_GAME_COUNT
        {
            get { return PrefManager.GetInt(nameof(TOTAL_GAME_COUNT)); }
            set
            {
                PrefManager.SetInt(nameof(TOTAL_GAME_COUNT), value);
            }
        }

        protected override void OnInit()
        {
            base.OnInit();
            Application.targetFrameRate = 60;

#if NOTIFICATION && UNITY_IOS
            OnInitNotification();
#endif
        }
    }

    public partial class GameManager
    {
        public static LoadGameDetails LoadGameDetails { get; private set; }

        // ReSharper disable once FlagArgument
        public static void LoadScene(string sceneName, bool showLoading = true, float loadingScreenSpeed = 1f)
        {
            var loadingPanel = SharedUIManager.LoadingPanel;
            if (showLoading && loadingPanel != null)
            {
                loadingPanel.Speed = loadingScreenSpeed;
                loadingPanel.Show(completed: () =>
                {
                    LoadSceneAsync(sceneName).Forget();
                    loadingPanel.Hide();
                });
            }
            else
            {
                LoadSceneAsync(sceneName).Forget();
            }
        }
        static async UniTask LoadSceneAsync(string sceneName)
        {
            await YooAssets.LoadSceneAsync(sceneName);
        }

        public static void LoadGame(LoadGameDetails loadGameDetails, bool showLoading = true,
            float loadingScreenSpeed = 1f)
        {
            LoadGameDetails = loadGameDetails;
            LoadScene("Main", showLoading, loadingScreenSpeed);
        }
    }

    public partial class GameManager
    {

#if NOTIFICATION && UNITY_IOS
#if DAILY_REWARD
        private UnityEngine.iOS.LocalNotification _rewardNotification;
#endif
        private void OnInitNotification()
        {
            NotificationService.CancelAll();

            //Stop Notification If All Level Finished
            if(ResourceManager.TargetLevel>= ResourceManager.Levels.Max(level => level.LevelNo))
                return;

            var notificationSetting = GameSettings.Default.NotificationSetting;
            foreach (var time in notificationSetting.comebackDelayTimes)
            {
                NotificationService.SendNotification(TimeSpan.FromSeconds(time), Application.productName, notificationSetting.comebackMessages.GetRandom());
            }
        }


        private void OnApplicationFocus(bool hasFocus)
        {
#if DAILY_REWARD
            if (hasFocus)
            {
                if (_rewardNotification != null)
                    NotificationService.CanCelLocalNotification(_rewardNotification);

                return;
            }

            //Sent for DailyReward
            _rewardNotification = NotificationService.SendNotification(LastRewardTime.AddDays(1).Subtract(DateTime.Now), "Math Race",
            "Daily reward awaits you!");
#endif
        }
#endif


    }

#if DAILY_REWARD

    public partial class GameManager
    {

        private static DateTime LastRewardTime
        {
            get
            {
                var time = long.Parse(PlayerPrefs.GetString(nameof(LastRewardTime), "0"));
                return new DateTime(time);
            }
            set { PlayerPrefs.SetString(nameof(LastRewardTime), value.Ticks.ToString()); }
        }

        

        public static int PendingRewardValue
        {
            get
            {
                if (!HasPendingDailyReward)
                    return -1;

                var val = PlayerPrefs.GetInt(nameof(PendingRewardValue), -1);

                if (val <= 0)
                {
                    PendingRewardValue = GameSettings.Default.DailyRewardSetting.rewards[UnityEngine.Random.Range(0
                        , GameSettings.Default.DailyRewardSetting.rewards.Length)];
                    return PendingRewardValue;
                }

                return val;
            }
            set { PlayerPrefs.SetInt(nameof(PendingRewardValue), value); }
        }

        public static bool HasPendingDailyReward => DateTime.Now.Subtract(LastRewardTime).Days > 0;

        public static bool GetReward(bool doubled)
        {
            if (!HasPendingDailyReward)
                return false;

            ResourceManager.Coins += PendingRewardValue * (doubled ? 2 : 1);
            PendingRewardValue = -1;
            LastRewardTime = DateTime.Now;
            return true;
        }
    }
#endif

}

public struct LoadGameDetails
{
    public GameType GameType { get; set; }
    public Category Category { get; set; }
    public int Level { get; set; }
}

public enum GameType
{
    Normal, Editor
}