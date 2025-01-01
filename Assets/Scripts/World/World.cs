using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class World : MonoBehaviour
{
    public int mapSizeInChunks = 6;
    public int chunkSize = 16, chunkHeight = 124;
    public int chunkRenderRange = 6;
    public GameObject chunkPrefab;
    public TerrainGenerator terrainGenerator;
    public Vector2Int mapSeedOffset;

    public UnityEvent OnWorldCreated, OnNewChunksGenerated;

    public WorldData worldData { get; private set; }

    private void Awake()
    {
        worldData = new WorldData
        {
            chunkHeight = this.chunkHeight,
            chunkSize = this.chunkSize,
            chunkDataDictionary = new Dictionary<Vector3Int, ChunkData>(),
            chunkDictionary = new Dictionary<Vector3Int, ChunkRenderer>()
        };
    }

    public void GenerateWorld()
    {
        GenerateWorld(Vector3Int.zero);
    }

    private void GenerateWorld(Vector3Int playerPos)
    {
        WorldGenerationData worldGenerationData = GetPositionsSeable(playerPos);
        // Debug.Log(worldGenerationData.chunkPositionsToCreate.Count);

        foreach (var pos in worldGenerationData.chunkPositionsToRemove)
        {
            WorldDataHelper.RemoveChunk(this, pos);
        }

        foreach (var pos in worldGenerationData.chunkDataPositionsToRemove)
        {
            WorldDataHelper.RemoveData(this, pos);
        }

        foreach (var pos in worldGenerationData.chunkDataPositionsToCreate)
        {
            ChunkData data = new ChunkData(chunkSize, chunkHeight, this, pos);
            ChunkData newData = terrainGenerator.GenerateChunkData(data, mapSeedOffset);
            worldData.chunkDataDictionary.Add(pos, newData);
        }

        foreach (var pos in worldGenerationData.chunkPositionsToCreate)
        {
            ChunkData data = worldData.chunkDataDictionary[pos];
            MeshData meshData = Chunk.GetChunkMeshData(data);
            GameObject chunkObject = Instantiate(chunkPrefab, data.worldPosition, Quaternion.identity);
            ChunkRenderer chunkRenderer = chunkObject.GetComponent<ChunkRenderer>();
            worldData.chunkDictionary.Add(data.worldPosition, chunkRenderer);
            chunkRenderer.InitializeChunk(data);
            chunkRenderer.UpdateChunk(meshData);

        }

        OnWorldCreated?.Invoke();
    }

    private WorldGenerationData GetPositionsSeable(Vector3Int playerPosition)
    {
        List<Vector3Int> allChunkPositionsNeeded = WorldDataHelper.GetChunkPositionsAroundPlayer(this, playerPosition);
        List<Vector3Int> allChunkDataPositionsNeeded = WorldDataHelper.GetDataPositionsAroundPlayer(this, playerPosition);

        List<Vector3Int> chunkPositionToCreate = WorldDataHelper.SelectPositonsToCreate(worldData, allChunkPositionsNeeded, playerPosition);
        List<Vector3Int> chunkDataPositionToCreate = WorldDataHelper.SelectDataPositonsToCreate(worldData, allChunkDataPositionsNeeded, playerPosition);

        Debug.Log(allChunkPositionsNeeded.Count);
        List<Vector3Int> chunkPositionsToRemove = WorldDataHelper.GetUneededChunks(worldData, allChunkPositionsNeeded);
        List<Vector3Int> chunkDataPositionsToRemove = WorldDataHelper.GetUneededChunkDatas(worldData, allChunkDataPositionsNeeded);

        WorldGenerationData data = new WorldGenerationData
        {
            chunkPositionsToCreate = chunkPositionToCreate,
            chunkDataPositionsToCreate = chunkDataPositionToCreate,
            chunkPositionsToRemove = chunkPositionsToRemove,
            chunkDataPositionsToRemove = chunkDataPositionsToRemove

        };
        return data;
    }

    internal VoxelsType GetVoxelFromChunkCoordinates(ChunkData chunkData, int x, int y, int z)
    {
        Vector3Int pos = Chunk.ChunkPositionFromVoxelCoordinates(this, x, y, z);
        ChunkData containerChunk = null;

        worldData.chunkDataDictionary.TryGetValue(pos, out containerChunk);

        if (containerChunk == null) return VoxelsType.Nothing;

        Vector3Int voxelInChunkCoordinates = Chunk.GetVoxelInChunkCoordinate(containerChunk, new Vector3Int(x, y, z));

        return Chunk.GetVoxelFromChunkCoordinates(containerChunk, voxelInChunkCoordinates);
    }

    internal void LoadAdditionalChunksRequest(GameObject player)
    {
        GenerateWorld(Vector3Int.RoundToInt(player.transform.position));
        OnNewChunksGenerated?.Invoke();
    }

    internal void RemoveChunk(ChunkRenderer chunk)
    {
        DestroyImmediate(chunk.gameObject);
    }

    public struct WorldGenerationData
    {
        public List<Vector3Int> chunkPositionsToCreate;
        public List<Vector3Int> chunkDataPositionsToCreate;
        public List<Vector3Int> chunkPositionsToRemove;
        public List<Vector3Int> chunkDataPositionsToRemove;
    }

    public struct WorldData
    {
        public Dictionary<Vector3Int, ChunkData> chunkDataDictionary;
        public Dictionary<Vector3Int, ChunkRenderer> chunkDictionary;
        public int chunkSize;
        public int chunkHeight;
    }
}
