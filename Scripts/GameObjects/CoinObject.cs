using Godot;
using SuperMarioRehashed.Scripts.Scenes;

namespace SuperMarioRehashed.Scripts.GameObjects;

public partial class CoinObject : MyObject
{
    public override void OnCollide(Node2D other)
    {
        if (other is not Player p) return;
        if (this.IsQueuedForDeletion()) return;
        
        p.AddItem(ObjectType.Coin);
        Rpc(nameof(DestroyObject));
    }
}