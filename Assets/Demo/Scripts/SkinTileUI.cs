using System;
using UnityEngine;
using UnityEngine.UI;

public class SkinTileUI : MonoBehaviour
{
    public event Action<SkinTileUI> Clicked; 

    [SerializeField] private GameObject _selectedEffectGO;
    [SerializeField] private GameObject _lockedEffectGO;
    [SerializeField] private Image _img;

    private ViewModel _mViewModel;

    public ViewModel MViewModel
    {
        get { return _mViewModel; }
        set
        {
            _img.sprite = value.Skin.icon;
            _lockedEffectGO.SetActive(value.Locked);
            _mViewModel = value;
        }
    }

    public bool Selected
    {
        get { return _selectedEffectGO.activeSelf;}
        set { _selectedEffectGO.SetActive(value); }
    }


    public void OnClicked()
    {
        Clicked?.Invoke(this);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public struct ViewModel
    {
        public PlayerSkin Skin{ get; set; }
        public bool Locked { get; set; }
    }
}


[System.Serializable]
public struct PlayerSkin
{
    public string id;
    public Sprite icon;
    public bool preLocked;
    public LockDetails lockDetails;
    public Sprite image;
}


[System.Serializable]
public struct LockDetails
{
    public Type type;
    public int value;

    public enum Type
    {
        BestScore, TotalScore, PlayCount,Combo
    }
}