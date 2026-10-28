using System;
using System.Collections.Generic;

namespace PocketBlocks {
// Pure C# rules: coordinates increase upward; two hidden spawn rows.
public sealed class BlocksEngine {
    public const int Width=10, Height=22;
    public readonly int[,] Board=new int[Width,Height];
    public int Kind, Rotation, X, Y, Hold=-1, Score, Lines, Combo=-1;
    public bool Over, HoldUsed;
    public int Level { get { return Lines/10+1; } }
    readonly Random random;
    readonly Queue<int> queue=new Queue<int>();
    readonly List<int> bag=new List<int>();
    public static readonly int[][] Shapes={
        new[]{0,1,1,1,2,1,3,1}, new[]{1,0,2,0,1,1,2,1},
        new[]{0,1,1,1,2,1,1,2}, new[]{1,1,2,1,0,2,1,2},
        new[]{0,1,1,1,1,2,2,2}, new[]{0,1,1,1,2,1,0,2},
        new[]{0,1,1,1,2,1,2,2}};
    public BlocksEngine(int seed) { random=new Random(seed); Fill(); Spawn(); }
    void Fill() { while(queue.Count<7) { if(bag.Count==0) { for(int i=0;i<7;i++) bag.Add(i); for(int i=6;i>0;i--) { int j=random.Next(i+1), t=bag[i]; bag[i]=bag[j]; bag[j]=t; } } queue.Enqueue(bag[0]);bag.RemoveAt(0); } }
    public int[] Next { get { return queue.ToArray(); } }
    public static void Cell(int kind,int rotation,int index,out int x,out int y) {
        x=Shapes[kind][index*2]; y=Shapes[kind][index*2+1];
        if(kind==1) return;
        int n=kind==0?3:2;
        for(int r=0;r<rotation;r++) { int old=x;x=y;y=n-old; }
    }
    public bool Fits(int kind,int rot,int px,int py) { for(int i=0;i<4;i++) { int x,y;Cell(kind,rot,i,out x,out y); x+=px;y+=py;if(x<0||x>=Width||y<0||y>=Height||Board[x,y]!=0)return false; }return true; }
    void Spawn() { Kind=queue.Dequeue(); Fill(); X=3;Y=19;Rotation=0;HoldUsed=false; if(!Fits(Kind,0,X,Y))Over=true; }
    public bool Move(int dx,int dy) { if(Over||!Fits(Kind,Rotation,X+dx,Y+dy))return false;X+=dx;Y+=dy;return true; }
    // Forgiving kick search, deliberately simpler than competition SRS.
    public bool Rotate(int direction=1) { if(Over)return false;int r=(Rotation+direction+4)%4;int[] kicks={0,-1,1,-2,2}; foreach(int dx in kicks)if(Fits(Kind,r,X+dx,Y)){X+=dx;Rotation=r;return true;}foreach(int dx in kicks)if(Fits(Kind,r,X+dx,Y+1)){X+=dx;Y++;Rotation=r;return true;}return false; }
    public int GhostY { get { int y=Y;while(Fits(Kind,Rotation,X,y-1))y--;return y; } }
    public int Drop() { if(Over)return 0;int d=Y-GhostY;Y-=d;Score+=d*2;return Lock(); }
    public void SwapHold() { if(Over||HoldUsed)return;int old=Kind;if(Hold<0)Spawn();else {Kind=Hold;X=3;Y=19;Rotation=0;if(!Fits(Kind,0,X,Y))Over=true;}Hold=old;HoldUsed=true; }
    public int Lock() {
        if(Over)return 0;
        for(int i=0;i<4;i++){int x,y;Cell(Kind,Rotation,i,out x,out y);Board[X+x,Y+y]=Kind+1;}
        int count=0;
        for(int y=0;y<Height;y++){bool full=true;for(int x=0;x<Width;x++)if(Board[x,y]==0)full=false;if(!full)continue;count++;for(int yy=y;yy<Height-1;yy++)for(int x=0;x<Width;x++)Board[x,yy]=Board[x,yy+1];for(int x=0;x<Width;x++)Board[x,Height-1]=0;y--;}
        if(count>0){Combo++;Score+=new[]{0,100,300,500,800}[count]*Level+Math.Max(0,Combo)*50*Level;Lines+=count;}else Combo=-1;
        for(int x=0;x<Width;x++)if(Board[x,20]!=0||Board[x,21]!=0)Over=true;
        if(!Over)Spawn();return count;
    }
    [Serializable] public class Snapshot { public int[] cells;public int kind,rotation,x,y,hold,score,lines,combo;public bool holdUsed;public int[] next, bag; }
    public Snapshot Save(){var s=new Snapshot{cells=new int[Width*Height],kind=Kind,rotation=Rotation,x=X,y=Y,hold=Hold,score=Score,lines=Lines,combo=Combo,holdUsed=HoldUsed,next=Next,bag=bag.ToArray()};for(int y=0;y<Height;y++)for(int x=0;x<Width;x++)s.cells[y*Width+x]=Board[x,y];return s;}
    public static BlocksEngine Restore(Snapshot s){if(s==null||s.cells==null||s.cells.Length!=220||s.next==null||s.next.Length!=7||s.bag==null||s.bag.Length>6||s.kind<0||s.kind>6||s.rotation<0||s.rotation>3||s.hold < -1||s.hold>6||s.score<0||s.lines<0)throw new ArgumentException("Invalid save");var e=new BlocksEngine(Environment.TickCount);for(int y=0;y<Height;y++)for(int x=0;x<Width;x++){int c=s.cells[y*Width+x];if(c<0||c>7)throw new ArgumentException();e.Board[x,y]=c;}e.bag.Clear();foreach(int n in s.bag){if(n<0||n>6||e.bag.Contains(n))throw new ArgumentException();e.bag.Add(n);}e.queue.Clear();foreach(int n in s.next){if(n<0||n>6)throw new ArgumentException();e.queue.Enqueue(n);}e.Kind=s.kind;e.Rotation=s.rotation;e.X=s.x;e.Y=s.y;e.Hold=s.hold;e.Score=s.score;e.Lines=s.lines;e.Combo=s.combo;e.HoldUsed=s.holdUsed;if(!e.Fits(e.Kind,e.Rotation,e.X,e.Y))throw new ArgumentException();return e;}
}}
