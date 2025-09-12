using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCreator : MonoBehaviour
{

    public event Action<ICycleTile> CreatedTile;
    public event Action<ICycleTile> RemovedTile;


    private const float WORLD_WIDTH = Constants.WORLD_SIZE;
    private readonly List<ICycleTile> _tileList = new List<ICycleTile>();
    private readonly List<int> _levelMapList = new List<int>();

    [SerializeField] private float _coverage;
    [SerializeField] private Transform _startPointTrans;
    [SerializeField] private LevelCreatorPrefabSelector _prefabSelector;
    [SerializeField] private Transform _targetTransform;
    [SerializeField] private  bool _isVerticalDirection;

    public bool IsCreating { get; private set; }

    public IEnumerable<ICycleTile> Tiles => _tileList;

    public IEnumerable<int> LevelMap => _levelMapList;

    public bool IsVerticalDirection
    {
        get { return _isVerticalDirection; }
        private set { _isVerticalDirection = value; }
    }


    public Transform TargetTransform
    {
        get { return _targetTransform; }
        set { _targetTransform = value; }
    }

    public float Coverage
    {
        get { return _coverage; }
        set { _coverage = value; }
    }

    public void StartCreating()
    {
        IsCreating = true;
        Update();
    }

    public void StopCreating()
    {
        IsCreating = false;
    }






    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(_startPointTrans.position - Vector3.right * WORLD_WIDTH / 2, _startPointTrans.position + Vector3.right * WORLD_WIDTH / 2);
    }

    // ReSharper disable once MethodTooLong
    private void Update()
    {
        if (!IsCreating)
            return;
        while (_tileList.Count == 0 ||
            ((_tileList[_tileList.Count - 1].Position.y + _tileList[_tileList.Count - 1].Size - TargetTransform.position.y) <
            Coverage && IsVerticalDirection)|| (_tileList[_tileList.Count - 1].Position.x + _tileList[_tileList.Count - 1].Size - TargetTransform.position.x) <
               Coverage && !IsVerticalDirection)
        {
            Vector3 targetPos;
            if (IsVerticalDirection)
            {
              targetPos  = _tileList.Count > 0
                    ? _tileList[_tileList.Count - 1].Position + Vector2.up * (_tileList[_tileList.Count - 1].Size)
                    : (Vector2) _startPointTrans.position;
            }
            else
            {
                targetPos = _tileList.Count > 0
                    ? _tileList[_tileList.Count - 1].Position + Vector2.right * _tileList[_tileList.Count - 1].Size
                    : (Vector2)_startPointTrans.position;
            }

            targetPos.z = transform.position.z;

            var selectedPrefab = _prefabSelector.GetSelectedPrefab();
            if (selectedPrefab == null)
            {
                return;
            }
            var cycleTile = (ICycleTile)Instantiate((MonoBehaviour)selectedPrefab,transform);
            (cycleTile as MonoBehaviour)?.gameObject.SetActive(true);
            cycleTile.SetPosition(targetPos);
            AddTile(cycleTile);
        }


        if (_tileList.Count > 0 &&
            ((TargetTransform.position.y - (_tileList[0].Position.y + _tileList[0].Size) > Coverage&&IsVerticalDirection) 
             || (TargetTransform.position.x - (_tileList[0].Position.x + _tileList[0].Size) > Coverage)&&!IsVerticalDirection))
        {
            var cycleTile = _tileList[0];
            RemoveTile(cycleTile);
        }
    }

    private void RemoveTile(ICycleTile cycleTile)
    {
        _tileList.RemoveAt(0);
        RemovedTile?.Invoke(cycleTile);
        var monoBehavior = cycleTile as MonoBehaviour;
        if (monoBehavior != null)
        {
            Destroy(monoBehavior.gameObject);
        }
    }

    private void AddTile(ICycleTile cycleTile)
    {
        _tileList.Add(cycleTile);
        _levelMapList.Add(cycleTile.Id);
        CreatedTile?.Invoke(cycleTile);
    }

    public void ClearLevel()
    {
        _tileList.ForEach(tile => Destroy(((MonoBehaviour)tile).gameObject));
        _tileList.Clear();
    }
}
