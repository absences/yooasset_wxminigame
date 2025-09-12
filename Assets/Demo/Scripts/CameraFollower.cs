using System;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    [SerializeField] private Vector2 _offset;
    [SerializeField] private Transform _target;
    [SerializeField] private float _lerbSpeed=5;
    [SerializeField] private Direction _direction = Direction.Both;

    public Transform Target { get { return _target; } set { _target = value; } }
    public bool Active { get; set; } = true;

    private Vector2 Position
    {
        get { return transform.position + (Vector3)_offset;}
        set
        {
            var position = transform.position;
            position.x = value.x + _offset.x;
            position.y = value.y + _offset.y;
            transform.position = position;
        }
    }

    // Use this for initialization
    void Start()
    {
       
    }




    // Update is called once per frame
    void FixedUpdate()
    {
        if (!Active)
        {
            return;
        }

        if (_direction == Direction.Y || _direction == Direction.Both)
            Position = Vector2.Lerp(Position, Position*Vector2.right + (Vector2)_target.position*Vector2.up, _lerbSpeed * Time.fixedDeltaTime);
        if (_direction == Direction.X || _direction == Direction.Both)
            Position = Vector2.Lerp(Position, Position*Vector2.up+Vector2.right * _target.position.x, _lerbSpeed * Time.fixedDeltaTime);

       
    }

    public enum Direction
    {
        X, Y, Both
    }
}