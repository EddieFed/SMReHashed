
using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;

namespace SuperMarioRehashed.Scripts;

public partial class WorldManager : Node2D
{

    private static WorldManager _worldManager = null;
    
    public const int NumChunks = 3;
    private static readonly Array<Node2D> Chunks = new Array<Node2D>();
    // A chunk is build by a WorldManager (Node2D) > TileMap > TileSet > ...
    
    public override void _Ready()
    {
        for (int i = 0; i < NumChunks; i++)
        {
            int chunkNum = Random.Shared.Next(1, 4);
            GD.Print($"Using chunk #{chunkNum}");
            Node2D chunk = (Node2D)GD.Load<PackedScene>($"res://Prefabs/Chunks/Chunk{chunkNum}.tscn").Instantiate();
            Chunks.Add(chunk);
        }
        
    }

    // We want a singleton world manager for each game!
    public static WorldManager GetWorldManager()
    {
        if (_worldManager == null)
        {
            _worldManager = new WorldManager();
        }

        return _worldManager;
    }

    public Node2D GetChunk(int i)
    {
        if (i >= NumChunks || i < 0 || _worldManager == null)
        {
            // GD.Print($"Error! i = {i}, WorldManager = {_worldManager}");
            return null;
        }

        Node2D ret = Chunks[i];
        // GD.Print($"Successfully grabbed chunk of index {i} with value {ret}");
        return (ret ?? GetChunk(++i));
    }

    // public float GetChunkWidth(int i)
    // {
    //     TileMap chunk = Chunks[i].GetNode<TileMap>("TileMap");
    //     return chunk.GetUsedRect().Size.X * chunk.TileSet.TileSize.X * 3;
    // }
    
    public float GetChunkWidth()
    {
        TileMap chunk = Chunks[0].GetNode<TileMap>("TileMap");
        return chunk.GetUsedRect().Size.X * chunk.TileSet.TileSize.X * 3;
    }
}
