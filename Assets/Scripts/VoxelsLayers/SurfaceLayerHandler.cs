using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceLayerHandler : VoxelsLayerHandler
{
    public VoxelsType surfaceVoxelType;

    protected override bool TryHandling(ChunkData chunkData, Vector3Int position, int surfaceHeight, Vector2Int mapSeedOffset)
    {
        if (position.y == surfaceHeight)
        {
            Chunk.SetVoxel(chunkData, position, surfaceVoxelType);
            return true;
        }
        return false;
    }
}
