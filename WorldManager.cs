
using System;
using System.Collections.Generic;
using Godot;

namespace Test;

public partial class WorldManager : Node2D
{

    private const int NumChunks = 5;
    private readonly LinkedList<WorldChunk> _chunks = new LinkedList<WorldChunk>();
    
    public WorldManager()
    {
        for (int i = 0; i < NumChunks; i++)
        {
            // WorldChunk newChunk = new WorldChunk();
            int chunkNum = Random.Shared.Next(1, 4);
            GD.Print($"Using chunk #{chunkNum}");
            PackedScene chunkScene = GD.Load<PackedScene>($"res://Chunks/Chunk{chunkNum}.tscn");
            Node2D chunk = (Node2D)chunkScene.Instantiate();
            
            TileMap chunkTileMap = chunk.GetNode<TileMap>("TileMap");
            float chunkSize = chunkTileMap.GetQuadrantSize() * chunkTileMap.Transform.Scale.X * 24f;
            
            // chunk.Position += new Vector2(chunkSize * i, 0);
            chunk.MoveLocalX(chunkSize * i);
            
            this.AddChild(chunk);
            
            
            // _chunks.AddLast(Chunk);
            // var tilemap = newChunk.GetNode<TileMap>(".");
            // var xPos = tilemap.CellQuadrantSize * tilemap.Transform.Scale.X;
            // newChunk.MoveLocalX(xPos);
            // this.AddChild(newChunk);
            // GD.Print(_chunks.Count, xPos);
        }
    } 




}