using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirLayerHandler : VoxelsLayerHandler
{
    protected override bool TryHandling(ChunkData chunkData, Vector3Int position, int surfaceHeight, Vector2Int mapSeedOffset)
    {
        if (position.y > surfaceHeight)
        {
            Chunk.SetVoxel(chunkData, position, VoxelsType.Air);
            return true;
        }
        return false;
    }
}
