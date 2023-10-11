using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VoxelsLayerHandler : MonoBehaviour
{
    [SerializeField]
    private VoxelsLayerHandler next;

    public bool Handle(ChunkData chunkData, Vector3Int position, int surfaceHeight, Vector2Int mapSeedOffset)
    {
        if (TryHandling(chunkData, position, surfaceHeight, mapSeedOffset))
        {
            return true;
        }
        if (next != null)
        {
            return next.Handle(chunkData, position, surfaceHeight, mapSeedOffset);
        }
        return false;
    }

    protected abstract bool TryHandling(ChunkData chunkData, Vector3Int position, int surfaceHeight, Vector2Int mapSeedOffset);
}
