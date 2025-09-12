using UnityEngine;

public interface ICycleTile
{
    int Id { get; }
    float Size { get; }
    Vector2 Position { get; }
    void SetPosition(Vector2 position);
}