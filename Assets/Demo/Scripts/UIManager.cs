using System.Collections;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private LevelCompletePanel _levelCompletePanel;
        [SerializeField] private GamePlayPanel _gamePlayPanel;
        [SerializeField] private StorePanel _storePanel;

        [SerializeField] private LevelPackCompletePanel _levelPackCompletePanel;
        //        public MenuPanel MenuPanel => _menuPanel;
        public StorePanel StorePanel => _storePanel;
        public GamePlayPanel GamePlayPanel => _gamePlayPanel;


        public static UIManager Instance { get; private set; }

    

        private void Awake()
        {
            Instance = this;
           
        }

        private void OnEnable()
        {
//            LevelManager.LevelLoaded += LevelManagerOnLevelLoaded;
            LevelManager.LevelCompleted += LevelManagerOnLevelResulted;
        }


        private void OnDisable()
        {
//            LevelManager.LevelLoaded -= LevelManagerOnLevelLoaded;
            LevelManager.LevelCompleted -= LevelManagerOnLevelResulted;
        }

        private void LevelManagerOnLevelResulted()
        {
            StartCoroutine(LevelResulted());
        }

        IEnumerator LevelResulted()
        {
            yield return new WaitForSeconds(0.6f);
            _levelCompletePanel.Show();

            yield return new WaitForSeconds(.6f);


            yield return new WaitForSeconds(.8f);
            _levelCompletePanel.Hide();

            if (LevelManager.Instance.Category.Quizzes.Count() <= LevelManager.Instance.Level)
            {
                _levelPackCompletePanel.Show();
            }
            else
            LevelManager.Instance.LoadLevel(LevelManager.Instance.Level+1);
        }
//
//
//
//        private void LevelManagerOnLevelLoaded(ILevel level1)
//        {
//            if (!GamePlayPanel.Showing)
//                GamePlayPanel.Show();
//
//        }
    }
}