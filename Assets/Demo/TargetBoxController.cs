// /*
// Created by Darsan
// */

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetBoxController : MonoBehaviour
{
    [SerializeField] private TargetBox _targetBoxPrefab;
    [SerializeField] private float _hSpacing, _vSpacing;
    [SerializeField] private HintTile _hintTile;
    [SerializeField] private ParticleSystem _tileSuccessParticle;
    [SerializeField] private AudioClip _collectClip, _completeClip;

    private readonly List<WordAndBoxes> _wordAndBoxes = new List<WordAndBoxes>();

    public float BoxSize { get; private set; }
    private readonly List<HintTile> _hints = new List<HintTile>();

    public bool Completed => _wordAndBoxes.All(boxes => boxes.Completed);
    public int TargetHintIndex { get; private set; } = 0;



    public void SetUp(params string[] words)
    {
        _wordAndBoxes.ForEach(boxes => boxes.boxes.ForEach(box => Destroy(box.gameObject)));
        _wordAndBoxes.Clear();

        _hints.ForEach(tile => Destroy(tile.gameObject));
        _hints.Clear();
        TargetHintIndex = 0;

        BoxSize = words.Length > 1 ? 0.35f : 0.45f;

        for (var i = 0; i < words.Length; i++)
        {
            var topLeft = transform.position +
                          ((words.Length - 1) * _vSpacing / 2 + words.Length * BoxSize * 0.5f - i*(BoxSize+_vSpacing)) * Vector3.up -
                          (words[i].Length * BoxSize / 2 + (words[i].Length - 1) * _hSpacing / 2) * Vector3.right;
            var wordAndBoxes = new WordAndBoxes
            {
                word = words[i],
                boxes = new TargetBox[words[i].Length]
            };

            for (var j = 0; j < words[i].Length; j++)
            {
                var targetBox = Instantiate(_targetBoxPrefab,topLeft + ((j+0.5f)*BoxSize +j* _hSpacing)*Vector3.right + Vector3.down*BoxSize/2,Quaternion.identity);
                targetBox.Size = BoxSize;
                wordAndBoxes.boxes[j] = targetBox;
            }
            _wordAndBoxes.Add(wordAndBoxes);
        }
    }

    public bool TryCollect(IEnumerable<Tile> tiles)
    {
        var list = tiles.ToList();
        var word = string.Join("",list.Select(tile => tile.Letter));

        var wordAndBoxes = _wordAndBoxes.FirstOrDefault(boxes => !boxes.Completed && boxes.word == word);
        if (wordAndBoxes==null)
        {
            return false;
        }

        StartCoroutine(TryCollect(list, wordAndBoxes));

        return true;
    }

    private IEnumerator TryCollect(List<Tile> tiles,WordAndBoxes wordAndBoxes)
    {
        for (var i = 0; i < tiles.Count; i++)
        {
            var startSize = tiles[i].Size;
            var startPosition = tiles[i].transform.position.WithZ(-1);
            var j = i;
            StartCoroutine(SimpleCoroutine.LerpNormalizedEnumerator(targetNormalized: 1.5f, onCallOnFrame: n =>
            {
                tiles[j].Size = Mathf.Lerp(startSize, BoxSize, n);
                tiles[j].transform.position = Vector3.Lerp(startPosition, wordAndBoxes.boxes[j].transform.position, n);
            }, onFinished: () => { Instantiate(_tileSuccessParticle, tiles[j].transform.position, Quaternion.identity); }, lerpSpeed: 8));
            PlayClipIfCan(_collectClip);
            tiles[i].CurrentState = Tile.State.Collected;
            wordAndBoxes.boxes[i].Tile = tiles[i];
            yield return new WaitForSeconds(0.05f);
        }

        if (Completed)
        {
            yield return new WaitForSeconds(0.3f);
            PlayClipIfCan(_completeClip);
        }
    }

    private void PlayClipIfCan(AudioClip clip,float volume=0.35f)
    {
        if(clip==null || !AudioManager.IsSoundEnable)
            return;
        AudioSource.PlayClipAtPoint(clip,Camera.main.transform.position,volume);
    }

    public void ShowHint()
    {
        if (TargetHintIndex > _wordAndBoxes.Sum(boxes => boxes.word.Length)-1)
        {
            return;
        }

        var elapsedLength = 0;

        foreach (var hint in _wordAndBoxes)
        {
            if (hint.Completed)
            {
                elapsedLength += hint.word.Length;
                continue;
            }

            if (TargetHintIndex < elapsedLength + hint.word.Length)
            {
                TargetHintIndex = Mathf.Max(TargetHintIndex, elapsedLength);
                break;
            }
        }

        var targetBox = _wordAndBoxes.SelectMany(boxes => boxes.boxes).ElementAt(TargetHintIndex);
        var targetLetter = _wordAndBoxes.SelectMany(boxes => boxes.word).ElementAt(TargetHintIndex)+"";

        var hintTile = Instantiate(_hintTile,targetBox.transform.position,Quaternion.identity);
        hintTile.Size = targetBox.Size;
        hintTile.Letter = targetLetter;
        _hints.Add(hintTile);
        TargetHintIndex++;
    }

    public class WordAndBoxes
    {
        public string word;
        public TargetBox[] boxes;
        public bool Completed => boxes != null && boxes.Any(box => box.Tile);
    }
}