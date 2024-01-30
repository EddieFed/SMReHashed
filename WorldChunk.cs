using Godot;

namespace Test;

public partial class WorldChunk : Node2D
{
    
    public WorldChunk()
    {
        PackedScene chunkScene = GD.Load<PackedScene>("res://Chunks/Chunk1.tscn");
        GetNode<Node2D>("WorldManager").AddChild(chunkScene.Instantiate());
        
    }
}