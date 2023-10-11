using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndergroundLayerHandler : VoxelsLayerHandler
{
    public VoxelsType undergroundVoxelType;

    protected override bool TryHandling(ChunkData chunkData, Vector3Int position, int surfaceHeight, Vector2Int mapSeedOffset)
    {
        if (position.y < surfaceHeight)
        {
            Chunk.SetVoxel(chunkData, position, undergroundVoxelType);
            return true;
        }
        return false;
    }
}
