public class RatingPopUp : ShowHidable
{
    public const int MIN_GAME_COUNT_AT_START = 5;
    public const int MIN_GAME_COUNT_AFTER_LATER = 50;

    private static int NEXT_MIN_COUNT
    {
        get { return PrefManager.GetInt("Rating_" + nameof(NEXT_MIN_COUNT), MIN_GAME_COUNT_AT_START);}
        set { PrefManager.SetInt("Rating_" + nameof(NEXT_MIN_COUNT),value); }
    }

    public static bool Available => !RatingButton.Rated && NEXT_MIN_COUNT < MyGame.GameManager.TOTAL_GAME_COUNT;


    public void OnClickRate()
    {
        RatingButton.OpenUrl();
        Hide();
    }

    public void OnClickLater()
    {
        NEXT_MIN_COUNT = MyGame.GameManager.TOTAL_GAME_COUNT + MIN_GAME_COUNT_AFTER_LATER;
        Hide();
    }
}
