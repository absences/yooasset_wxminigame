using System;
using System.Collections;
using MyGame;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class GamePlayPanel : ShowHidable
    {
        

        [SerializeField] private Button _previousBtn, _nxtBtn;
        [SerializeField] private TextMeshProUGUI _lvlTxt;
        [SerializeField] private TextMeshProUGUI _categoryNameTxt;
        [SerializeField] private RectTransform _moreContent;
        [SerializeField] private float _moreContentExpandHeight;


        private LevelManager LevelManager => LevelManager.Instance;



        private int CurrentLevel
        {
            set
            {
                _lvlTxt.text = $"Level {value}";
                RefreshBtns();
            }
        }


        private bool _showingMoreContent;

        public bool DisableButtons { get; set; } = false;



        private bool ShowingMoreContent
        {
            get => _showingMoreContent;
            set
            {
                StopCoroutine(nameof(MoreContentSizeAnim));
                StartCoroutine(MoreContentSizeAnim(value ? _moreContentExpandHeight : 0));
                _showingMoreContent = value;
            }
        }



        private void Awake()
        {
            _previousBtn.onClick.AddListener(OnClickPrevious);
            _nxtBtn.onClick.AddListener(OnClickNxt);
        }


        protected override void OnEnable()
        {
            LevelManager.LevelLoaded += LevelManagerOnLevelLoaded;
//            LevelManager.LevelCompleted += LevelManagerOnLevelCompleted;
            base.OnEnable();
        }


        protected override void OnDisable()
        {
            LevelManager.LevelLoaded -= LevelManagerOnLevelLoaded;
//            LevelManager.LevelCompleted -= LevelManagerOnLevelCompleted;
            base.OnDisable();
        }

        private void LevelManagerOnLevelLoaded()
        {
            CurrentLevel = LevelManager.Level;
            _categoryNameTxt.text = LevelManager.Category.Name;
        }

        private IEnumerator Start()
        {
            LevelManagerOnLevelLoaded();
            yield return new WaitForEndOfFrame();
        }

        private void Update()
        {
            RefreshBtns();
        }


        private void OnClickNxt()
        {
            LevelManager.LoadLevel(LevelManager.Level + 1);
        }

        private void OnClickPrevious()
        {
            LevelManager.LoadLevel(LevelManager.Level-1);
        }

        public void OnClickRestart()
        {
            LevelManager.LoadLevel(LevelManager.Level);
        }


        public void OnClickMore()
        {
            ShowingMoreContent = !ShowingMoreContent;
        }

        private IEnumerator MoreContentSizeAnim(float height)
        {
            while (Mathf.Abs(_moreContent.rect.height - height) > 1)
            {
                var h = Mathf.MoveTowards(_moreContent.rect.height, height, Time.deltaTime * 2500);
                _moreContent.sizeDelta = new Vector2(_moreContent.rect.width, h);
                yield return null;
            }

            _moreContent.sizeDelta = new Vector2(_moreContent.rect.width, height);
        }


        public void OnClickMenu()
        {
            var popUpPanel = SharedUIManager.PopUpPanel;
            popUpPanel.ShowAsConfirmation("Quit?","Are you sure want to Quiz the Game", success =>
            {
                if (!success)
                {
                    return;
                }
                GameManager.LoadScene("MainMenu");
            });
        }

        public void OnClickStore()
        {
            var storePanel = UIManager.Instance.StorePanel;
            storePanel.Show();
        }


        public void OnClickHint()
        {
            if (ResourceManager.Hints <= 0 && !ResourceManager.UnlimitedHints)
            {
                UIManager.Instance.StorePanel.Show();
                return;
            }

            if(!LevelManager.Instance.Board.CanShowHint)
                return;

            if(!ResourceManager.UnlimitedHints)
            {
                ResourceManager.Hints--;
            }

            LevelManager.Board.ShowHint();

        }






        private void RefreshBtns()
        {
            _nxtBtn.interactable = !ResourceManager.IsLevelLocked(LevelManager.Category.ID, LevelManager.Level + 1);
            _previousBtn.interactable = LevelManager.Level > 1;
        }

    }
}