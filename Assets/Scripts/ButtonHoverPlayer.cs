using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverPlayer : MonoBehaviour, IPointerEnterHandler {
    public void OnPointerEnter(PointerEventData eventData) {
        SFXManager.Instance.PlayHoverSound();
    }
}
