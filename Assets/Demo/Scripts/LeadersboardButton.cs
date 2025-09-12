using UnityEngine;
using UnityEngine.EventSystems;

public class LeadersboardButton : MonoBehaviour,IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
#if GAME_SERVICE
        SocialService.Instance.ShowLeadersboard();
#endif
    }
}