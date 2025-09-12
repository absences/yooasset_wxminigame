using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class HintText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private string _prefix = "HINT(";
        [SerializeField] private string _postfix = ")";
        private void Update()
        {
            _text.text = _prefix+(ResourceManager.UnlimitedHints? ">":ResourceManager.Hints.ToString())+_postfix;
        }
    }
}