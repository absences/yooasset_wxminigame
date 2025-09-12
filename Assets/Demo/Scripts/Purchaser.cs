using System;
using System.Collections.Generic;
using UnityEngine;
#if IN_APP
using UnityEngine.Purchasing;


public class Purchaser : IStoreListener
{
    /// <summary>
    /// Item Purchase Completed with id and success state
    /// </summary>
    public event Action<string, bool> OnItemPurchased;
    public event Action<bool> RestorePurchased;

    public bool Inilitized => _mStoreController != null && _mStoreExtensionProvider != null;

    public IEnumerable<string> ConsumableItems { get; }
    public IEnumerable<string> NonConsumableItems { get; }


    private static IStoreController _mStoreController;          // The Unity Purchasing system.
    private static IExtensionProvider _mStoreExtensionProvider; // The store-specific Purchasing subsystems.


    public Purchaser(IEnumerable<string> consumableItems, IEnumerable<string> nonConsumableItems)
    {
        this.NonConsumableItems = nonConsumableItems;
        this.ConsumableItems = consumableItems;
        Init();
    }


    private void Init()
    {
        //TODO:UnComment After InApp Plugin added
        if (Inilitized)
        {
            return;
        }
        if (_mStoreController == null)
        {
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            foreach (var premiumItem in ConsumableItems)
            {
                builder.AddProduct(premiumItem, ProductType.Consumable);
            }
            foreach (var nonConsumableItem in NonConsumableItems)
            {
                builder.AddProduct(nonConsumableItem, ProductType.NonConsumable);
            }
            UnityPurchasing.Initialize(this, builder);

        }
    }

    public string GetPrice(string productId) => GetProduct(productId)?.metadata.localizedPriceString;

    /// <summary>
    /// Buy the product with item
    /// </summary>
    /// <param name="productId">Product Id</param>
    /// <param name="callback">Buy product call back with success state</param>
    public void BuyProduct(string productId, Action<bool> callback)
    {
        Action<string, bool> onPurchase = null;
        onPurchase = (id, success) =>
        {
            if (id != productId)
                return;
            OnItemPurchased -= onPurchase;
            callback?.Invoke(success);
        };
        OnItemPurchased += onPurchase;

        BuyProductID(productId);
    }


    /// <summary>
    /// The Item Already Brought for Non consumable Products
    /// </summary>
    /// <param name="productId">product id</param>
    /// <returns></returns>
    public bool ItemAlreadyPurchased(string productId)
    {
        var product = GetProduct(productId);
        if (product != null && product.definition.type != ProductType.Consumable && product.hasReceipt)
        {
            return true;
        }

        return false;
    }

    void BuyProductID(string productId)
    {
        // If Purchasing has been initialized ...
        if (Inilitized)
        {
            Product product = _mStoreController.products.WithID(productId);

            if (product != null && product.availableToPurchase)
            {
                _mStoreController.InitiatePurchase(product);
            }
            else
            {
                OnItemPurchased?.Invoke(productId, false);
            }
        }
        else
        {
            OnItemPurchased?.Invoke(productId, false);
            Init();
        }
    }


    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        _mStoreController = controller;
        _mStoreExtensionProvider = extensions;
    }


    public void Restore()
    {
        _mStoreExtensionProvider.GetExtension<IAppleExtensions>().RestoreTransactions(success =>
        {
             RestorePurchased?.Invoke(success);
        });
    }


    public Product GetProduct(string id)
    {
        return _mStoreController?.products.WithID(id);
    }


    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("OnInilized Failed");
    }


    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        Debug.Log("Purchased Product:" + args.purchasedProduct.definition.id);
        OnItemPurchased?.Invoke(args.purchasedProduct.definition.id, true);
        return PurchaseProcessingResult.Complete;
    }


    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason) =>
        OnItemPurchased?.Invoke(product.definition.id, false);

}
#endif