using UnityEngine;
using UnityEngine.UI;
namespace PocketBlocks {
public sealed class BoardGraphic : MaskableGraphic {
    public BlocksEngine Engine; public int Preview=-1; public bool IsPreview;
    public static readonly Color[] Colors={new Color32(57,217,239,255),new Color32(255,211,92,255),new Color32(177,133,255,255),new Color32(107,224,169,255),new Color32(255,113,145,255),new Color32(103,155,255,255),new Color32(255,168,100,255)};
    void Quad(VertexHelper vh,Rect r,Color c){int n=vh.currentVertCount;vh.AddVert(new Vector3(r.xMin,r.yMin),c,Vector2.zero);vh.AddVert(new Vector3(r.xMin,r.yMax),c,Vector2.zero);vh.AddVert(new Vector3(r.xMax,r.yMax),c,Vector2.zero);vh.AddVert(new Vector3(r.xMax,r.yMin),c,Vector2.zero);vh.AddTriangle(n,n+1,n+2);vh.AddTriangle(n,n+2,n+3);}
    void Block(VertexHelper vh,int x,int y,float cell,Vector2 o,Color c,bool ghost=false){Rect r=new Rect(o.x+x*cell+2,o.y+y*cell+2,cell-4,cell-4);if(ghost){c.a=.3f;Quad(vh,r,c);return;}Quad(vh,r,c);Quad(vh,new Rect(r.x+2,r.yMax-4,r.width-4,2),new Color(1,1,1,.4f));Quad(vh,new Rect(r.x+2,r.y+2,r.width-4,2),new Color(0,0,0,.15f));}
    protected override void OnPopulateMesh(VertexHelper vh){vh.Clear();Rect r=rectTransform.rect;
        if(IsPreview){if(Preview<0)return;float c=Mathf.Min(r.width/5,r.height/4);Vector2 o=new Vector2(r.center.x-2*c,r.center.y-1.5f*c);for(int i=0;i<4;i++){int x,y;BlocksEngine.Cell(Preview,0,i,out x,out y);Block(vh,x,y,c,o,Colors[Preview]);}return;}
        float size=Mathf.Min(r.width/10,r.height/20);Vector2 origin=new Vector2(r.center.x-5*size,r.center.y-10*size);
        Quad(vh,new Rect(origin.x-3,origin.y-3,size*10+6,size*20+6),new Color32(41,57,78,255));
        for(int y=0;y<20;y++)for(int x=0;x<10;x++){Quad(vh,new Rect(origin.x+x*size+1,origin.y+y*size+1,size-2,size-2),new Color32(15,25,42,255));if(Engine!=null&&Engine.Board[x,y]>0)Block(vh,x,y,size,origin,Colors[Engine.Board[x,y]-1]);}
        if(Engine==null||Engine.Over)return;int ghost=Engine.GhostY;
        for(int i=0;i<4;i++){int x,y;BlocksEngine.Cell(Engine.Kind,Engine.Rotation,i,out x,out y);if(ghost+y<20)Block(vh,Engine.X+x,ghost+y,size,origin,Colors[Engine.Kind],true);if(Engine.Y+y<20)Block(vh,Engine.X+x,Engine.Y+y,size,origin,Colors[Engine.Kind]);}
    }
}}
