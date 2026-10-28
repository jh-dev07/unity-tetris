using System;
using UnityEngine;
using UnityEngine.EventSystems;
namespace PocketBlocks {
public sealed class RepeatButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler {
    public Action Press;public bool Repeat;bool held;float next;
    public void OnPointerDown(PointerEventData e){held=true;transform.localScale=Vector3.one*.95f;next=Time.unscaledTime+.18f;Press?.Invoke();}
    public void OnPointerUp(PointerEventData e){held=false;transform.localScale=Vector3.one;} public void OnPointerExit(PointerEventData e){held=false;transform.localScale=Vector3.one;}
    void OnDisable(){held=false;transform.localScale=Vector3.one;}void Update(){if(held&&Repeat&&Time.unscaledTime>=next){next=Time.unscaledTime+.065f;Press?.Invoke();}}
}
}
