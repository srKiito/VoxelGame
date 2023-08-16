using System;
using UnityEngine;

public static class Chunk
{
    public static void LoopThroughTheVoxels(ChunkData chunkData, Action<int, int, int> actionToPerform)
    {
        for (int i = 0; i < chunkData.voxels.Length; i++)
        {
            var position = GetPositionFromIndex(chunkData, i);
            actionToPerform(position.x, position.y, position.z);
        }
    }

    private static Vector3Int GetPositionFromIndex(ChunkData chunkData, int i)
    {
        int x = i % chunkData.chunkSize;
        int y = (i / chunkData.chunkSize) % chunkData.chunkHeight;
        int z = i / (chunkData.chunkSize * chunkData.chunkHeight);
        return new Vector3Int(x, y, z);
    }

    private static bool InRange(ChunkData chunkData, int axisCoordinates)
    {
        if (axisCoordinates < 0 || axisCoordinates >= chunkData.chunkSize) return false;

        return true;
    }

    private static bool InRangeHeight(ChunkData chunkData, int yCoordinate)
    {
        if (yCoordinate < 0 || yCoordinate >= chunkData.chunkHeight) return false;

        return true;
    }

    public static void SetVoxel(ChunkData chunkData, Vector3Int localPosition, VoxelsType voxel)
    {
        if (InRange(chunkData, localPosition.x) && InRangeHeight(chunkData, localPosition.y) && InRange(chunkData, localPosition.z))
        {
            int index = GetIndexFromPosition(chunkData, localPosition.x, localPosition.y, localPosition.z);
            chunkData.voxels[index] = voxel;
            // Debug.Log(chunkData.voxels[index]);
        }
        else
        {
            throw new Exception("Need to ask for appropiate chunk");
        }
    }

    private static int GetIndexFromPosition(ChunkData chunkData, int x, int y, int z)
    {
        return x + chunkData.chunkSize * y + chunkData.chunkSize * chunkData.chunkHeight * z;
    }

    public static Vector3Int GetVoxelInChunkCoordinate(ChunkData chunkData, Vector3Int pos)
    {
        return new Vector3Int
        {
            x = pos.x - chunkData.worldPosition.x,
            y = pos.y - chunkData.worldPosition.y,
            z = pos.z - chunkData.worldPosition.z,
        };
    }

    public static VoxelsType GetVoxelFromChunkCoordinates(ChunkData chunkData, Vector3Int chunkCoordinates)
    {
        return GetVoxelFromChunkCoordinates(chunkData, chunkCoordinates.x, chunkCoordinates.y, chunkCoordinates.z);
    }
    public static VoxelsType GetVoxelFromChunkCoordinates(ChunkData chunkData, int x, int y, int z)
    {
        if (InRange(chunkData, x) && InRangeHeight(chunkData, y) && InRange(chunkData, z))
        {
            int index = GetIndexFromPosition(chunkData, x, y, z);
            return chunkData.voxels[index];
        }

        return chunkData.worldReference.GetVoxelFromChunkCoordinates(chunkData, chunkData.worldPosition.x + x, chunkData.worldPosition.y + y, chunkData.worldPosition.z + z);
    }

    internal static MeshData GetChunkMeshData(ChunkData chunkData)
    {
        MeshData meshData = new MeshData(true);

        LoopThroughTheVoxels(chunkData, (x, y, z) => meshData = VoxelHelper.GetMeshData(chunkData, x, y, z, meshData, chunkData.voxels[GetIndexFromPosition(chunkData, x, y, z)]));

        return meshData;
    }

    internal static Vector3Int ChunkPositionFromVoxelCoordinates(World world, int x, int y, int z)
    {
        Vector3Int pos = new Vector3Int
        {
            x = Mathf.FloorToInt(x / (float)world.chunkSize) * world.chunkSize,
            y = Mathf.FloorToInt(y / (float)world.chunkHeight) * world.chunkHeight,
            z = Mathf.FloorToInt(z / (float)world.chunkSize) * world.chunkSize
        };
        return pos;
    }
}