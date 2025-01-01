using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class WorldDataHelper
{
    internal static Vector3Int ChunkPositionFromBlockCoords(World world, Vector3Int position)
    {
        return new Vector3Int
        {
            x = Mathf.FloorToInt(position.x / world.chunkSize) * world.chunkSize,
            y = Mathf.FloorToInt(position.y / world.chunkHeight) * world.chunkHeight,
            z = Mathf.FloorToInt(position.z / world.chunkSize) * world.chunkSize
        };
    }

    internal static List<Vector3Int> GetChunkPositionsAroundPlayer(World world, Vector3Int playerPosition)
    {
        int startX = playerPosition.x - world.chunkRenderRange * world.chunkSize;
        int startZ = playerPosition.z - world.chunkRenderRange * world.chunkSize;
        int endX = playerPosition.x + world.chunkRenderRange * world.chunkSize;
        int endZ = playerPosition.z + world.chunkRenderRange * world.chunkSize;

        List<Vector3Int> chunkPositionsToCreate = new List<Vector3Int>();
        for (int x = startX; x < endX; x += world.chunkSize)
        {
            for (int z = startZ; z < endZ; z += world.chunkSize)
            {
                Vector3Int chunkPos = ChunkPositionFromBlockCoords(world, new Vector3Int(x, 0, z));
                chunkPositionsToCreate.Add(chunkPos);
                // if (x >= playerPosition.x - world.chunkSize
                //     && x <= playerPosition.x + world.chunkSize
                //     && z >= playerPosition.z - world.chunkSize
                //     && z <= playerPosition.z + world.chunkSize)
                // {
                //     for (int y = -world.chunkHeight; y >= playerPosition.y - world.chunkHeight * 2; y -= world.chunkHeight)
                //     {
                //         chunkPos = ChunkPositionFromBlockCoords(world, new Vector3Int(x, y, z));
                //         chunkPositionsToCreate.Add(chunkPos);
                //     }
                // }
            }
        }
        return chunkPositionsToCreate.Distinct().ToList();
    }

    internal static List<Vector3Int> GetDataPositionsAroundPlayer(World world, Vector3Int playerPosition)
    {
        int startX = playerPosition.x - (world.chunkRenderRange + 1) * world.chunkSize;
        int startZ = playerPosition.z - (world.chunkRenderRange + 1) * world.chunkSize;
        int endX = playerPosition.x + (world.chunkRenderRange + 1) * world.chunkSize;
        int endZ = playerPosition.z + (world.chunkRenderRange + 1) * world.chunkSize;

        List<Vector3Int> chunkDataPositionsToCreate = new List<Vector3Int>();
        for (int x = startX; x < endX; x += world.chunkSize)
        {
            for (int z = startZ; z < endZ; z += world.chunkSize)
            {
                Vector3Int chunkPos = ChunkPositionFromBlockCoords(world, new Vector3Int(x, 0, z));
                chunkDataPositionsToCreate.Add(chunkPos);
                // if (x >= playerPosition.x - world.chunkSize
                //     && x <= playerPosition.x + world.chunkSize
                //     && z >= playerPosition.z - world.chunkSize
                //     && z <= playerPosition.z + world.chunkSize)
                // {
                //     for (int y = -world.chunkHeight; y >= playerPosition.y - world.chunkHeight * 2; y -= world.chunkHeight)
                //     {
                //         chunkPos = ChunkPositionFromBlockCoords(world, new Vector3Int(x, y, z));
                //         chunkDataPositionsToCreate.Add(chunkPos);
                //     }
                // }
            }
        }
        Debug.Log("GetDataPositionsAroundPlayer: " + chunkDataPositionsToCreate.Count);
        return chunkDataPositionsToCreate.Distinct().ToList();
    }

    internal static List<Vector3Int> GetUneededChunkDatas(World.WorldData worldData, List<Vector3Int> allChunkDataPositionsNeeded)
    {
        return worldData.chunkDataDictionary.Keys.Where(pos => allChunkDataPositionsNeeded.Contains(pos) == false).ToList();
    }

    internal static List<Vector3Int> GetUneededChunks(World.WorldData worldData, List<Vector3Int> allChunkPositionsNeeded)
    {
        return worldData.chunkDictionary.Keys.Where(pos => allChunkPositionsNeeded.Contains(pos) == false).ToList();

        // List<Vector3Int> positionToRemove = new List<Vector3Int>();
        // foreach (var pos in worldData.chunkDictionary.Keys
        //     .Where(pos => allChunkPositionsNeeded.Contains(pos) == false))
        // {
        //     if (worldData.chunkDictionary.ContainsKey(pos))
        //     {
        //         positionToRemove.Add(pos);

        //     }
        // }

        // return positionToRemove;
    }

    internal static void RemoveChunk(World world, Vector3Int pos)
    {
        ChunkRenderer chunk = null;
        if (world.worldData.chunkDictionary.TryGetValue(pos, out chunk))
        {
            world.RemoveChunk(chunk);
            world.worldData.chunkDictionary.Remove(pos);
        }
    }

    internal static void RemoveData(World world, Vector3Int pos)
    {
        world.worldData.chunkDataDictionary.Remove(pos);
    }

    internal static List<Vector3Int> SelectDataPositonsToCreate(World.WorldData worldData, List<Vector3Int> allChunkDataPositionsNeeded, Vector3Int playerPosition)
    {
        return allChunkDataPositionsNeeded
            .Where(pos => worldData.chunkDataDictionary.ContainsKey(pos) == false)
            .ToList();
    }

    internal static List<Vector3Int> SelectPositonsToCreate(World.WorldData worldData, List<Vector3Int> allChunkPositionsNeeded, Vector3Int playerPosition)
    {
        return allChunkPositionsNeeded
            .Where(pos => worldData.chunkDictionary.ContainsKey(pos) == false)
            .ToList();
    }
}
