using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteRenderReSizer : MonoBehaviour
{
    [SerializeField] private Vector2 _worldRelSize;

    private SpriteRenderer _spriteRenderer;
    private Sprite _lastSprite;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _lastSprite = _spriteRenderer.sprite;
        RefreshSize();
    }

    private void Update()
    {
        if (_lastSprite != _spriteRenderer.sprite)
        {
            RefreshSize();
            _lastSprite = _spriteRenderer.sprite;
        }
    }

    private void RefreshSize()
    {
        if (_spriteRenderer.sprite == null)
        {
            return;
        }
        var extents = _spriteRenderer.sprite.bounds.extents;
        
        transform.localScale = new Vector3(_worldRelSize.x/ (extents.x * 2) , _worldRelSize.y/(extents.y*2),1);
    }
}