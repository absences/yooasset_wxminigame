using MyGame;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[ExecuteInEditMode]
public class GradientSpriteRenderer : MonoBehaviour
{
    [SerializeField] private Gradient _gradient;
    [SerializeField] private Vector2 _pivot = new Vector2(0.5f, 0.5f);

    public Gradient Gradient
    {
        get { return _gradient; }
        set
        {
            _gradient = value;
            RefreshGradient();
        }
    }

    // Use this for initialization
    void Awake()
    {
        RefreshGradient();

    }

    private void RefreshGradient()
    {
        var texture = Utils.CreateTexture(_gradient);
        GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), _pivot);
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        RefreshGradient();
#endif
    }
}