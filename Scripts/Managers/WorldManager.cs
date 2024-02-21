using System;
using Godot;
using Godot.Collections;

namespace SuperMarioRehashed.Scripts.Managers;

public partial class WorldManager : Node2D
{

	private static WorldManager _worldManager = null;
	
	public const int NumChunks = 4;
	public static int ChunkSize { get; private set; }
	private static readonly Array<Node2D> Chunks = new Array<Node2D>();
	// A chunk is build by a WorldManager (Node2D) > TileMap > TileSet > ...
	
	public override void _Ready()
	{
		for (int i = 0; i < NumChunks; i++)
		{
			int chunkNum = Random.Shared.Next(1, 4);
			GD.Print($"Using chunk #{chunkNum}");
			Node2D chunk = (Node2D)GD.Load<PackedScene>($"res://Scenes/Chunks/Chunk{chunkNum}.tscn").Instantiate();
			Chunks.Add(chunk);
		}
		
		TileMap tileMap = Chunks[0].GetNode<TileMap>("TileMap");
		ChunkSize = tileMap.GetUsedRect().Size.X * tileMap.TileSet.TileSize.X * (int)tileMap.Scale.X;
		
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
	
}
