using System.Threading;
using Godot;
using SuperMarioRehashed.Scripts.Scenes;

namespace SuperMarioRehashed.Scripts.GameObjects;

public partial class FireFlowerObject : MyObject
{
    public override void OnCollide(Node2D other)
    {
        if (other is not Player p) return;
        
        // p.AddItem(ObjectType.FireFlower);
        p.GivePowerup(ObjectType.FireFlower);
        Rpc(nameof(DestroyObject));
        
        // Create timer for 7 seconds, remove after that time!
        GetTree().CreateTimer(7).Timeout += () =>
        {
            // p.RemoveItemType(ObjectType.FireFlower);
            p.RemovePowerUp(ObjectType.FireFlower);
        };
    }
}