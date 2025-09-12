using UnityEngine;

namespace MainMenu
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [SerializeField] private CategoriesPanel _categoriesPanel;
        [SerializeField] private LevelsPanel _levelsPanel;

        public CategoriesPanel CategoriesPanel => _categoriesPanel;

        public LevelsPanel LevelsPanel => _levelsPanel;

        private void Awake()
        {
            Instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}