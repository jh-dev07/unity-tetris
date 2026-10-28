using System;
namespace UnityEditor { [AttributeUsage(AttributeTargets.Method)] public class MenuItem : Attribute {public MenuItem(string path){} } }
namespace UnityEngine { public static class Debug {public static void Log(object message){Console.WriteLine(message);} } }
public static class Runner { public static void Main(){ PocketBlocks.Editor.RulesChecks.Run();
    int actions=0;
    for(int seed=0;seed<100;seed++){
        var e=new PocketBlocks.BlocksEngine(seed);var rng=new Random(seed+42);
        for(int n=0;n<2000&&!e.Over;n++){
            switch(rng.Next(6)){case 0:e.Move(-1,0);break;case 1:e.Move(1,0);break;case 2:e.Rotate();break;case 3:e.SwapHold();break;case 4:if(!e.Move(0,-1))e.Lock();break;default:e.Drop();break;}
            if(!e.Over&&!e.Fits(e.Kind,e.Rotation,e.X,e.Y))throw new Exception("Active piece overlaps");
            if(!e.Over){var r=PocketBlocks.BlocksEngine.Restore(e.Save());if(r.GhostY!=e.GhostY||r.Kind!=e.Kind||r.Score!=e.Score)throw new Exception("Save mismatch");}
            actions++;
        }
        if(!e.Over)throw new Exception("Simulation did not end");
    }
    Console.WriteLine("100 seeded games; "+actions+" random actions passed collision and save invariants.");
} }
