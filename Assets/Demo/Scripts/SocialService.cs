#if GAME_SERVICE
using GooglePlayGames;
using UnityEngine;

public class SocialService : Singleton<SocialService>
{
    private static string LEADERS_BOARD_ID => Application.platform == RuntimePlatform.Android
        ? GameSettings.Default.AndroidLeadersboardSetting.leadersboardId
        : GameSettings.Default.IosLeadersboardSetting.leadersboardId;


    public bool IsInit { get; private set; }
    public bool IsLogIn { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        
        Init();
    }

    private void OnEnable()
    {
        MyGame.GameManager.TopScoreChanged += OnTopScoreChanged;
    }

    private void OnDisable()
    {
        MyGame.GameManager.TopScoreChanged -= OnTopScoreChanged;
    }

    private void OnTopScoreChanged(int score)
    {
        SubmitTopScoreOnLeadersBoard(score);
    }

    void Start()
    {
        SignIn();
    }

    void Init()
    {
        if (IsInit)
            return;
#if UNITY_ANDROID
        PlayGamesPlatform.Activate();
#endif
        IsInit = true;
    }

    public void SignIn()
    {
        if (!IsInit)
        {
            Init();
            return;
        }
        Social.localUser.Authenticate(success =>
        {
            // handle success or failure
            if (success)
            {
                IsLogIn = true;
            }

        });
    }

    public void ShowLeadersboard()
    {
        Debug.Log("Show Leaders Board");

        if (!IsLogIn)
        {
            SignIn();
            return;
        }
        Social.ShowLeaderboardUI();
    }

    public void SubmitTopScoreOnLeadersBoard(int score)
    {
        if (!IsLogIn)
        {
            return;
        }
        Social.ReportScore(score,LEADERS_BOARD_ID, success =>
          {
          });

    }
}
#endif