using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AxisLineEffect : MonoBehaviour
{

    [SerializeField] private SpriteRenderer _tilePrefab;
    [SerializeField] private Vector2 _lineAndSpacing = new Vector2(0.3f,0.3f);
    [SerializeField] private float _lineLength = 15f;
    [SerializeField] private float _thickness = 0.2f;

    private readonly List<SpriteRenderer> _tiles = new List<SpriteRenderer>();
    private Line _line;
    private Color _color = Color.green;

    public Color Color
    {
        get => _color;
        set
        {
            _color = value;
            _tiles.ForEach(spriteRenderer => spriteRenderer.color = value);
        }
    }

    public Line Line
    {
        get => _line;
        set
        {
            _line = value;

            transform.position = !value.IsVertical ? Vector3.zero.WithY(value.GetY(0)) : Vector3.zero.WithX(value.XVertical);
            transform.rotation = Quaternion.AngleAxis(value.IsVertical?90:Mathf.Atan(value.M)*Mathf.Rad2Deg,Vector3.forward)*Quaternion.AngleAxis(-90,Vector3.forward);
        }
    }

    [ContextMenu("Show")]
    public void Show()
    {
        StopAllCoroutines();
        if (_tiles.Count == 0)
        {
            var count = Mathf.CeilToInt(_lineLength/(_lineAndSpacing.x+_lineAndSpacing.y));
            var lowerPoint = -(count * (_lineAndSpacing.x + _lineAndSpacing.y)) / 2f;

            for (var i = 0; i < count; i++)
            {
                var spriteRenderer = Instantiate(_tilePrefab,
                    transform);
                spriteRenderer.transform.localPosition =
                    Vector3.zero.WithY(lowerPoint + i * (_lineAndSpacing.x + _lineAndSpacing.y));
                spriteRenderer.transform.localRotation = Quaternion.identity;
                spriteRenderer.size = new Vector2(_thickness,_lineAndSpacing.x);
                _tiles.Add(spriteRenderer);
            }
        }
        gameObject.SetActive(true);
        StartCoroutine(ShowEnumerator());

    }

    [ContextMenu("SetLine")]
    private void SetLine()
    {
        Line = new Line(0,-2);
    }

    private IEnumerator ShowEnumerator()
    {
        
        _tiles.ForEach(renderer =>
        {
            renderer.color = Color.cyan.WithAlpha(0);
        });

        foreach (var spriteRenderer in _tiles)
        {
            StartCoroutine(
                SimpleCoroutine.MergeSequence(new[]
                    {ColorLerb(spriteRenderer, Color.cyan, 20), ColorLerb(spriteRenderer, Color, 15f)}));
            yield return new WaitForSeconds(0.01f);
        }
    }

    private IEnumerator ColorLerb(SpriteRenderer render,Color target,float speed=5f)
    {
        var startColor = render.color;
        yield return SimpleCoroutine.LerpNormalizedEnumerator(n => { render.color = Color.Lerp(startColor, target, n); },lerpSpeed:speed);
    }

    [ContextMenu("Hide")]
    public void Hide()
    {
        Hide(null);
    }

    public void Hide(Action completed)
    {
        StopAllCoroutines();
        StartCoroutine(SimpleCoroutine.CoroutineEnumerator(HideEnumerator(), () =>
        {
            gameObject.SetActive(false);
            completed?.Invoke();
        }));
    }

    private IEnumerator HideEnumerator()
    {
        var colors = _tiles.Select(sp => sp.color).ToList();

        yield return SimpleCoroutine.LerpNormalizedEnumerator(normalized =>
        {
            for (var i = 0; i < _tiles.Count; i++)
            {
                _tiles[i].color = Color.Lerp(colors[i], colors[i].WithAlpha(0), normalized);
            }
        },lerpSpeed:5f);
    }

}
