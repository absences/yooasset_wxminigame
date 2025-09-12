using System;
using System.Linq;
using UnityEngine;

namespace Game
{
    public partial class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance { get; private set; }

        public static event Action LevelCompleted;
        public static event Action LevelLoaded;

        [SerializeField] private Board _board;

        private int _score;


        public Board Board => _board;
        public int Level { get; set; }

        public Quiz Quiz => Category.Quizzes.ElementAt(Level - 1);
        public Category Category { get; private set; }

        private void Awake()
        {
            Instance = this;
            var loadGameDetails = MyGame.GameManager.LoadGameDetails;

            Category = loadGameDetails.Category ?? ResourceManager.Categories.First();
            Level = loadGameDetails.Level >0 ?loadGameDetails.Level :Mathf.Min(ResourceManager.GetCompletedLevelForCategory(Category.ID)+1,Category.Quizzes.Count());

            _board.Completed+=BoardOnCompleted;
           
        }


        private void BoardOnCompleted()
        {
           
            LevelCompleted?.Invoke();    
        }



        public void LoadLevel(int lvl)
        {

            if(lvl>Category.Quizzes.Count())
                return;
            Level = lvl;

            _board.SetUp(Category.Quizzes.ElementAt(lvl-1));
            LevelLoaded?.Invoke();
        }



        private void OnEnable()
        {
        }


        private void OnDisable()
        {
        }


        private void Start()
        {
            LoadLevel(Level);
        }


        private void Update()
        {
           

            if (Input.GetKeyDown(KeyCode.R))
            {
                _board.SetUp(new Quiz
                {
                    letters = "HELDLAARS",
                    words = new[] { "HELL", "DARSA" }
                });
            }
        }


       
    }

}