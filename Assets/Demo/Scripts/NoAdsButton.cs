using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class NoAdsButton : MonoBehaviour, IPointerClickHandler
{

    private void Awake()
    {
        gameObject.SetActive(ResourceManager.EnableAds);
    }
#if IN_APP
    private void OnEnable()
    {
        ResourceManager.ProductPurchased+=ResourceManagerOnProductPurchased;
        ResourceManager.ProductRestored +=ResourceManagerOnProductRestored;
    }

    private void ResourceManagerOnProductRestored(bool success)
    {
        gameObject.SetActive(ResourceManager.EnableAds);
    }


    private void OnDisable()
    {
        ResourceManager.ProductPurchased-=ResourceManagerOnProductPurchased;
        ResourceManager.ProductRestored -= ResourceManagerOnProductRestored;

    }

    private void ResourceManagerOnProductPurchased(string productId)
    {
        gameObject.SetActive(ResourceManager.EnableAds);
    }
#endif
    public void OnPointerClick(PointerEventData eventData)
    {
#if IN_APP
        ResourceManager.PurchaseNoAds(success => { });
#endif
    }

}