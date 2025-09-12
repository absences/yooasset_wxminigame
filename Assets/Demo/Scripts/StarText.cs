using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class StarText : MonoBehaviour
    {
        private TextMeshProUGUI _text;

        private void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();
        }

        private void Update()
        {
            _text.text = ResourceManager.Brains.ToString();
        }
    }
}