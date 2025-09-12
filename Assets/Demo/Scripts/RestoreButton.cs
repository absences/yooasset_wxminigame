using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class RestoreButton : MonoBehaviour, IPointerClickHandler
{
    private void Awake()
    {
        gameObject.SetActive(Application.platform == RuntimePlatform.IPhonePlayer
#if IN_APP
                             && ResourceManager.AbleToRestore
#endif
        );
    }

#if IN_APP
    private void OnEnable()
    {
        ResourceManager.ProductRestored += ResourceManagerOnProductRestored;
        ResourceManager.ProductPurchased += ResourceManagerOnProductPurchased;
    }



    private void OnDisable()
    {
        ResourceManager.ProductRestored -= ResourceManagerOnProductRestored;
        ResourceManager.ProductPurchased -= ResourceManagerOnProductPurchased;
    }

    private void ResourceManagerOnProductRestored(bool b)
    {
        gameObject.SetActive(ResourceManager.AbleToRestore);
    }


    private void ResourceManagerOnProductPurchased(string s)
    {
        gameObject.SetActive(ResourceManager.AbleToRestore);
    }

#endif


    public void OnPointerClick(PointerEventData eventData)
    {
#if IN_APP
        ResourceManager.RestorePurchase();
#endif
    }
}