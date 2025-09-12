using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string _sceneName;

    private void Start()
    {
        MyGame.GameManager.LoadScene(_sceneName);
    }
}