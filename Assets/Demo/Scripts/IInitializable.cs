public interface IInitializable
{
    bool Initialized { get; }
    void Init();
}

public interface IInitializable<in T>
{
    bool Initialized { get; }
    void Init(T data);
}