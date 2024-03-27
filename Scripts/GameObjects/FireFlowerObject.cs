using System.Threading;
using Godot;
using SuperMarioRehashed.Scripts.Scenes;

namespace SuperMarioRehashed.Scripts.GameObjects;

public partial class FireFlowerObject : MyObject
{
    public override void OnCollide(Node2D other)
    {
        if (other is not Player p) return;
        
        p.AddItem(ObjectType.FireFlower);
        Rpc(nameof(DestroyObject));
        
        // Create timer for 7 seconds, remove after that time!
        Thread t = new Thread(() =>
        {
            Thread.Sleep(7000);
            p.RemoveItemType(ObjectType.FireFlower);
        });
        t.Start();
    }
}