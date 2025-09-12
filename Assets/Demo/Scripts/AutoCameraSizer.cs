using UnityEngine;

[RequireComponent(typeof(Camera))]
public class AutoCameraSizer : MonoBehaviour, IInilizable
{
    public const float WORLD_SIZE = Constants.WORLD_SIZE;


    private Camera _camera;


    public bool Initialized { get; private set; }

    public void Init()
    {
        if (Initialized)
        {
            return;
        }

        _camera = GetComponent<Camera>();


        var aspectRatio = Mathf.Max((Screen.height + 0f) / Screen.width);
        _camera.orthographicSize = aspectRatio * WORLD_SIZE * 0.5f;


        Initialized = true;
    }

    private void Awake()
    {
        Init();
    }

}


public interface IInilizable
{
    bool Initialized { get; }
    void Init();
}