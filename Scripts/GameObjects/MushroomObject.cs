using Godot;
using SuperMarioRehashed.Scripts.Scenes;

namespace SuperMarioRehashed.Scripts.GameObjects;

public partial class MushroomObject : MyObject
{
	public override void OnCollide(Node2D other)
	{
		if (other is not Player p) return;
		
		p.AddItem(ObjectType.Mushroom);
		Rpc(nameof(DestroyObject));
	}
}
