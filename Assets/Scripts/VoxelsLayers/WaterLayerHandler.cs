using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterLayerHandler : VoxelsLayerHandler
{
    public int waterLevel;

    protected override bool TryHandling(ChunkData chunkData, Vector3Int position, int surfaceHeight, Vector2Int mapSeedOffset)
    {
        if (position.y <= waterLevel && position.y > surfaceHeight)
        {
            Chunk.SetVoxel(chunkData, position, VoxelsType.Water);
            return true;
        }
        return false;
    }
}
