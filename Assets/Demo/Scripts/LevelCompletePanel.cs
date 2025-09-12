using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class LevelCompletePanel : ShowHidable
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField]private List<string> _wishWords = new List<string>();
        [SerializeField] private Image _brainImg;
        [SerializeField] private TextMeshProUGUI _alreadyCollectedTxt;

        public override void Show(bool animate = true, Action completed = null)
        {
            base.Show(animate, completed);

            _text.text = _wishWords.GetRandom();
            _brainImg.transform.localScale = Vector3.one;
            var brainTxt = _brainImg.GetComponentInChildren<TextMeshProUGUI>();
            brainTxt.text = ResourceManager.Brains + "";

            if (ResourceManager.GetCompletedLevelForCategory(LevelManager.Instance.Category.ID) <
                LevelManager.Instance.Level)
            {
                StartCoroutine(CollectBrains());
                _alreadyCollectedTxt.gameObject.SetActive(false);
            }
            else
            {
                _alreadyCollectedTxt.gameObject.SetActive(true);
            }

        }

        IEnumerator CollectBrains()
        {
            var text = _brainImg.GetComponentInChildren<TextMeshProUGUI>();
          
            var collectCoins = LevelManager.Instance.Quiz.letters.Length;

            var startScale = 1f;
            var targetScale = _brainImg.transform.localScale.x*1.3f;

            yield return SimpleCoroutine.LerpNormalizedEnumerator(targetNormalized: 1.5f,
                onCallOnFrame: n =>
                {
                    _brainImg.transform.localScale = Mathf.Lerp(startScale, targetScale, n) * Vector3.one;
                    text.text = Mathf.RoundToInt(Mathf.Lerp(ResourceManager.Brains,
                                    ResourceManager.Brains + collectCoins, n)) + "";
                },lerpSpeed:7);

            ResourceManager.Brains += collectCoins;

            ResourceManager.CompleteLevel(LevelManager.Instance.Category.ID,LevelManager.Instance.Level);

            targetScale =(startScale+targetScale)/2;
            startScale = _brainImg.transform.localScale.x;

            yield return SimpleCoroutine.LerpNormalizedEnumerator(targetNormalized: 1.5f,
                onCallOnFrame: n =>
                {
                    _brainImg.transform.localScale = Mathf.Lerp(startScale, targetScale, n) * Vector3.one;
                },lerpSpeed:7);
        }
    }
}