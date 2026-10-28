using System;
using UnityEngine;
using UnityEngine.EventSystems;
namespace PocketBlocks {
public sealed class BoardGesture : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler {
    public Action<int> Move;public Action Turn,Drop,Down; Vector2 start,last;bool moved;
    public void OnPointerDown(PointerEventData e){start=last=e.position;moved=false;}
    public void OnDrag(PointerEventData e){float threshold=Screen.width/14f;if(Mathf.Abs(e.position.x-last.x)>threshold){Move?.Invoke(e.position.x>last.x?1:-1);last=e.position;moved=true;}else if(last.y-e.position.y>threshold){Down?.Invoke();last=e.position;moved=true;}}
    public void OnPointerUp(PointerEventData e){if(e.position.y-start.y < -Screen.height*.16f)Drop?.Invoke();else if(!moved&&(e.position-start).sqrMagnitude<400)Turn?.Invoke();}
}
}
