using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StorePanel : ShowHidable
{

    [SerializeField] private StoreTileUI _storeTileUIPrefab;
    [SerializeField] private RectTransform _content;

    private readonly List<StoreTileUI> _tiles = new List<StoreTileUI>();

	// Use this for initialization
	void Start ()
	{
	    foreach (var hint in Store.Default.Hints)
	    {
	        var storeTileUI = Instantiate(_storeTileUIPrefab,_content);
            storeTileUI.Clicked +=StoreTileUIOnClicked;
	        storeTileUI.HintProduct = hint;
            _tiles.Add(storeTileUI);
	    }

    }

    private void StoreTileUIOnClicked(StoreTileUI tile)
    {
        if(ResourceManager.UnlimitedHints)
        {
            var popUpPanel = SharedUIManager.PopUpPanel;
            popUpPanel.MViewModel = new PopUpPanel.ViewModel
            {
                Title = "Unlimited Hints!",
                Message = "You already Purchased Unlimited Hints",
                Buttons = new []{new PopUpPanel.ViewModel.Button
                {
                    title = "Ok"
                }, }
            };
            popUpPanel.Show();
            return;
        }
        var hintProduct = tile.HintProduct;
        OnPurchasedHint(hintProduct);


        //if (hintProduct.priceDetails.type == PriceDetails.Type.InAppConsumable || hintProduct.priceDetails.type == PriceDetails.Type.InAppNonConsumable)
        //{
        //    ResourceManager.PurchaseHint(hintProduct.id, success =>
        //    {
        //        if (success)
        //        {
        //            OnPurchasedHint(hintProduct);
        //        }
        //    });
        //}
        //else
        //{
        //    if (AdsManager.IsVideoAvailable())
        //    {
        //        AdsManager.ShowVideoAds(true,success =>
        //        {
        //            if (success)
        //            {
        //                ResourceManager.Hints += hintProduct.value;
        //                OnPurchasedHint(hintProduct);
        //            }
        //        });

        //    }
        //}

    }

    private void OnPurchasedHint(HintProduct product)
    {
        var popUpPanel = SharedUIManager.PopUpPanel;
        popUpPanel.MViewModel = new PopUpPanel.ViewModel
        {
            Title = "Hints",
            Message = $"You have got " + (product.isUnlimited ? "Unlimited" : product.value.ToString()) + " hints",
            Buttons = new[]{new PopUpPanel.ViewModel.Button
            {
                title = "Ok"
            }, }
        };
        popUpPanel.Show();
    }

    public void OnClickClose()
    {
        Hide();
    }
}