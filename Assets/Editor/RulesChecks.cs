using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace PocketBlocks.Editor {
public static class RulesChecks {
 static void Check(bool ok,string name){if(!ok)throw new Exception("Failed: "+name);}
 [MenuItem("Pocket Blocks/Run Rules Checks")]
 public static void Run(){
    var e=new BlocksEngine(123);var bag=new HashSet<int>{e.Kind};for(int i=0;i<6;i++)bag.Add(e.Next[i]);Check(bag.Count==7,"seven bag");
    int first=e.Kind;e.SwapHold();Check(e.Hold==first&&e.HoldUsed,"hold");int current=e.Kind;e.SwapHold();Check(e.Kind==current,"hold once");
    e=new BlocksEngine(1);Check(e.GhostY<e.Y,"ghost descends");Check(e.Fits(e.Kind,e.Rotation,e.X,e.GhostY),"ghost valid");Check(!e.Fits(e.Kind,e.Rotation,e.X,e.GhostY-1),"ghost grounded");
    for(int i=0;i<40;i++)e.Move(-1,0);Check(e.Fits(e.Kind,e.Rotation,e.X,e.Y),"left wall");e.Rotate();Check(e.Fits(e.Kind,e.Rotation,e.X,e.Y),"wall rotation");
    e=new BlocksEngine(3);Array.Clear(e.Board,0,e.Board.Length);for(int y=0;y<4;y++)for(int x=0;x<10;x++)if(x!=4)e.Board[x,y]=2;
    e.Kind=0;e.Rotation=1;e.X=3;e.Y=0;Check(e.Fits(0,1,3,0),"I setup");int cleared=e.Drop();Check(cleared==4&&e.Lines==4&&e.Score>=800,"four line clear");
    e=new BlocksEngine(4);e.Move(-1,-1);e.SwapHold();var s=e.Save();var r=BlocksEngine.Restore(s);Check(r.Kind==e.Kind&&r.X==e.X&&r.Hold==e.Hold,"restore current");Check(string.Join(",",r.Next)==string.Join(",",e.Next),"restore queue");
    bool rejected=false;try{BlocksEngine.Restore(new BlocksEngine.Snapshot());}catch{rejected=true;}Check(rejected,"corrupt save");
    e=new BlocksEngine(5);int n=0;while(!e.Over&&n++<300)e.Drop();Check(e.Over,"top out");
    Debug.Log("Pocket Blocks: all rules checks passed.");
 }
}}
