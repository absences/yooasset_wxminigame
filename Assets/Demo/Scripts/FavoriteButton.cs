using UnityEngine;
using UnityEngine.EventSystems;

public class FavoriteButton : MonoBehaviour,IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        RatingButton.OpenUrl();
    }
}