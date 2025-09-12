using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class Board : MonoBehaviour
{
    public event Action Completed;

    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private float _maxHeight = 6;
    [SerializeField] private float _minPaddingSide;
    [SerializeField] private float _spacing = 0.1f;
    [SerializeField] private TargetBoxController _targetBoxController;



    private readonly List<Tile> _tiles = new List<Tile>();

    public float TotalWidth { get; private set; }
    public float TileWidth { get; private set; }
    public int GridSize { get; private set; }
    public bool Dragging { get; private set; }

    private readonly List<Tile> _draggingTiles = new List<Tile>();
    public Quiz Quiz { get; private set; }
    public IEnumerable<Tile> DraggingTiles => _draggingTiles;
    public bool CanShowHint => _targetBoxController.TargetHintIndex < Quiz.letters.Length;

    public void SetUp(Quiz quiz)
    {
        var camera = Camera.main;

        Quiz = quiz;
        _tiles.ForEach(tile => Destroy(tile.gameObject));
        _tiles.Clear();
        // ReSharper disable once PossibleNullReferenceException
        var screenWidth = (camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height)) -
                           camera.ScreenToWorldPoint(Vector3.zero)).x;
        TotalWidth = Mathf.Min(_maxHeight, screenWidth - 2 * _minPaddingSide);

        GridSize = Mathf.RoundToInt(Mathf.Sqrt(quiz.letters.Length));
        TileWidth = (TotalWidth - (GridSize - 1) * _spacing) / GridSize;

        var topLeft = transform.position + Vector3.up * TotalWidth / 2 - Vector3.right * TotalWidth / 2;

        for (var i = 0; i < GridSize; i++)
        {
            for (var j = 0; j < GridSize; j++)
            {
                var tile = Instantiate(_tilePrefab,
                    topLeft + ((j + 0.5f) * TileWidth + j * _spacing) * Vector3.right -
                    Vector3.up * ((i + 0.5f) * TileWidth + i * _spacing), Quaternion.identity);
                tile.Size = TileWidth;
                tile.GridCoordinate = new Vector2Int(j, GridSize - i - 1);
                tile.Letter = quiz.letters[i * GridSize + j] + "";
                _tiles.Add(tile);
            }
        }

        _targetBoxController.SetUp(quiz.words);
    }

    private void UpdateBoard()
    {
        StopAllCoroutines();

        for (var i = 0; i < GridSize; i++)
        {
            var minY = 0;
            var tiles = _tiles.Where(tile => tile.CurrentState == Tile.State.None)
                .Where(tile => tile.GridCoordinate.x == i).OrderBy(tile => tile.GridCoordinate.y).ToList();
            foreach (var t in tiles)
            {
                Debug.Log("y:" + t.GridCoordinate.y + " minY:" + minY);
                if (t.GridCoordinate.y > minY)
                {
                    var startPoint = t.transform.position;
                    var targetPoint = GetPositionForGrid(new Vector2Int(t.GridCoordinate.x, minY));
                    var y = minY;
                    StartCoroutine(SimpleCoroutine.MoveTowardsEnumerator(0, 1,
                        n => { t.transform.position = Vector3.Lerp(startPoint, targetPoint, n); },
                        () => { t.GridCoordinate = new Vector2Int(t.GridCoordinate.x, y); }, 10));
                }

                minY++;
            }
        }
    }

    public Vector3 GetPositionForGrid(Vector2Int grid)
    {
        var bottomLeft = transform.position - Vector3.up * TotalWidth / 2 - Vector3.right * TotalWidth / 2;

        return bottomLeft + ((grid.x + 0.5f) * TileWidth + grid.x * _spacing) * Vector3.right +
               Vector3.up * ((grid.y + 0.5f) * TileWidth + grid.y * _spacing);
    }

    private void Update()
    {
        if (
#if UNITY_EDITOR

            EventSystem.current.IsPointerOverGameObject()
#else
            Input.touchCount>0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)
#endif
        )
        {
            if (Dragging)
            {
                _draggingTiles.ForEach(tile => tile.CurrentState = Tile.State.None);
                _draggingTiles.Clear();
                Dragging = false;
            }
            
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Dragging = true;
        }

        if (Dragging)
        {
            // ReSharper disable once PossibleNullReferenceException
            var col = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if (col != null && col.gameObject.layer == 9)
            {
                var tile = col.GetComponent<Tile>();
                if (tile.CurrentState == Tile.State.None &&
                    (_draggingTiles.Count == 0 ||
                     (Mathf.Abs(_draggingTiles.Last().GridCoordinate.x -
                                tile.GridCoordinate.x) <= 1 &&
                      Mathf.Abs(_draggingTiles.Last().GridCoordinate.y -
                                tile.GridCoordinate.y) <= 1)))
                {
                    tile.CurrentState = Tile.State.Touch;
                    _draggingTiles.Add(tile);
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (!_targetBoxController.TryCollect(_draggingTiles))
            {
                _draggingTiles.ForEach(tile => tile.CurrentState = Tile.State.None);
            }
            else
            {
                if (_targetBoxController.Completed)
                {
                    Completed?.Invoke();
                }
                else
                UpdateBoard();
            }

            _draggingTiles.Clear();
            Dragging = false;
        }
    }

    public void ShowHint()
    {
        _targetBoxController.ShowHint();
    }
}