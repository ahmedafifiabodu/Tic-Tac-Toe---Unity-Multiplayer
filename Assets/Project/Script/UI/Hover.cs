using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class Hover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData) => gameObject.transform.DOScale(1.1f, 0.3f).SetEase(Ease.OutBack);

    public void OnPointerExit(PointerEventData eventData) => gameObject.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
}