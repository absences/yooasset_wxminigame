using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SoundButton:MonoBehaviour,IPointerClickHandler
{
    [SerializeField] private Sprite[] _soundEnableAndDisableSprites;
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        AudioManagerOnSoundStateChanged(AudioManager.IsSoundEnable);
    }

    private void OnEnable()
    {
        AudioManager.SoundStateChanged += AudioManagerOnSoundStateChanged;
    }

    private void OnDisable()
    {
        AudioManager.SoundStateChanged -= AudioManagerOnSoundStateChanged;
    }

    private void AudioManagerOnSoundStateChanged(bool b)
    {
        _button.image.sprite = _soundEnableAndDisableSprites[b ? 0 : 1];
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        AudioManager.IsSoundEnable = !AudioManager.IsSoundEnable;
    }
}