using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class ResourceManager : Singleton<ResourceManager>
{

#if IN_APP
    public static event Action<string> ProductPurchased;
    public static event Action<bool> ProductRestored;
#endif



    public static string NoAdsProductID => GameSettings.Default.InAppSetting.removeAdsId;

    public static bool EnableAds
    {
        get => PrefManager.GetBool(nameof(EnableAds), true);
        set => PrefManager.SetBool(nameof(EnableAds), value);
    }

    public static int Hints
    {
        get => PrefManager.GetInt(nameof(Hints), 3);
        set => PrefManager.SetInt(nameof(Hints), value);
    }

//    public static int Stars
//    {
//        get => PrefManager.GetInt(nameof(Stars), 0);
//        set => PrefManager.SetInt(nameof(Stars),value);
//    }

    public static IEnumerable<Category> Categories => global::Categories.Default; 

    public static bool UnlimitedHints
    {
        get => PrefManager.GetBool(nameof(UnlimitedHints), false);
        set => PrefManager.SetBool(nameof(UnlimitedHints), value);
    }

    public static int Brains
    {
        get => PrefManager.GetInt(nameof(Brains));
        set => PrefManager.SetInt(nameof(Brains), value);
    }

    public static Store Store => Store.Default;



    //    public static bool AbleToRestore => EnableAds;

   // public Purchaser Purchaser { get; private set; }

    private readonly List<Category> _categories = new List<Category>();


    protected override void OnInit()
    {
        base.OnInit();
      //  Purchaser = new Purchaser(
       //     Store.Hints.Where(product => product.priceDetails.type == PriceDetails.Type.InAppConsumable).Select(product => product.priceDetails.productId),
      //      Store.Hints.Where(product => product.priceDetails.type == PriceDetails.Type.InAppNonConsumable).Select(product => product.priceDetails.productId).Concat(new[] { NoAdsProductID }));
     //   Purchaser.RestorePurchased += PurchaserOnRestorePurchased;
    }

    //private void PurchaserOnRestorePurchased(bool restored)
    //{
    //    var unlimitedProductId = Store.Hints.FirstOrDefault(product => product.isUnlimited).priceDetails
    //        .productId;
    //    if (!UnlimitedHints &&
    //        Purchaser.ItemAlreadyPurchased(unlimitedProductId))
    //    {
    //        UnlimitedHints = true;
    //    }

    //    if (EnableAds && Purchaser.ItemAlreadyPurchased(NoAdsProductID))
    //    {
    //        EnableAds = false;
    //    }
    //    Debug.Log($"InAppPurchase Restored Unlimited:{Purchaser.ItemAlreadyPurchased(unlimitedProductId)}");
    //    ProductRestored?.Invoke(restored);
    //}


    //public static void RestorePurchase()
    //{
    //    Debug.Log("Restore InAppPurchase");
    //    Instance.Purchaser.Restore();
    //}

    //private static void PurchaseInApp(string productId, Action<bool> completed = null)
    //{
    //    Instance.Purchaser.BuyProduct(productId, success =>
    //    {
    //        completed?.Invoke(success);
    //        if (success)
    //            ProductPurchased?.Invoke(productId);
    //    });
    //}

    public static string GetPrice(string productId) => "free";// Instance?.Purchaser?.GetPrice(productId);

    public static HintProduct GetHint(string id) => Store.Hints.FirstOrDefault(product => product.id == id);

    //public static void PurchaseHint(string hintProductId, Action<bool> completed = null)
    //{
    //    var product = GetHint(hintProductId);
    //    if (product.priceDetails.type == PriceDetails.Type.InAppConsumable || product.priceDetails.type == PriceDetails.Type.InAppNonConsumable)
    //    {
    //        PurchaseInApp(product.priceDetails.productId, success =>
    //        {
    //            if (success)
    //            {
    //                if (product.isUnlimited)
    //                    UnlimitedHints = true;
    //                else
    //                    Hints += product.value;
    //            }
    //            completed?.Invoke(success);
    //        });
    //    }
    //}

    //public static void PurchaseNoAds(Action<bool> completed)
    //{
    //    PurchaseInApp(NoAdsProductID, (success) =>
    //    {
    //        if (success)
    //            EnableAds = false;

    //        completed?.Invoke(success);
    //    });
    //}
}


//Level Related
public partial class ResourceManager
{

   
    public static bool AbleToRestore => !UnlimitedHints;



    public static void CompleteLevel(string category,int lvl)
    {
        if (GetCompletedLevelForCategory(category)<lvl)
        {
            PrefManager.SetInt("Completed_"+category,lvl);

            var categories = Categories.ToList();
            var index = categories.FindIndex(c => c.ID == category);
            if (categories.Count -1>index && IsCategoryLocked(categories[index+1].ID) && categories[index].Quizzes.Count() == lvl)
            {
                UnLockCategory(categories[index+1].ID);
            }
        }

        
    }



    public static int GetCompletedLevelForCategory(string category)
    {
        return PrefManager.GetInt("Completed_" + category, 0);
    }

    public static bool IsLevelLocked(string category,int lvl)
    {
        return GetCompletedLevelForCategory(category) + 1 < lvl || Categories.First(cat => cat.ID == category).Quizzes.Count()< lvl;
    }

    public static bool IsCategoryLocked(string id)
    {
        return Categories.First().ID != id &&
               PrefManager.GetBool("CategoryLocked_" + id, true);
    }

    public static void UnLockCategory(string id)
    {
        PrefManager.SetBool("CategoryLocked_"+id,false);
    }

    
}