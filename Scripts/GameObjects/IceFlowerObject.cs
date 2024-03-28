using Godot;
using SuperMarioRehashed.Scripts.Scenes;

namespace SuperMarioRehashed.Scripts.GameObjects;

public partial class IceFlowerObject : MyObject
{
    public override void OnCollide(Node2D other)
    {
        if (other is not Player p) return;
        
        // p.AddItem(ObjectType.FireFlower);
        p.GivePowerup(ObjectType.IceFlower);
        Rpc(nameof(DestroyObject));
        
        // Create timer for 7 seconds, remove after that time!
        GetTree().CreateTimer(10).Timeout += () =>
        {
            // p.RemoveItemType(ObjectType.IceFlower);
            p.RemovePowerUp(ObjectType.IceFlower);
        };
    }
}