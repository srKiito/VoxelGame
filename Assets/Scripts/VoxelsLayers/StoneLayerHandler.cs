using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneLayerHandler : VoxelsLayerHandler
{
    [Range(0, 1)] public float stoneThreshold = 0.5f;

    [SerializeField] private NoiseSettings stoneNoiseSettings;
    [SerializeField] private DomainWarping domainWarping;

    protected override bool TryHandling(ChunkData chunkData, Vector3Int position, int surfaceHeight, Vector2Int mapSeedOffset)
    {
        if (chunkData.worldPosition.y > surfaceHeight) return false;

        stoneNoiseSettings.WorldOffset = mapSeedOffset;
        float stoneNoise = domainWarping.GenerateDomainNoise(chunkData.worldPosition.x + position.x, chunkData.worldPosition.z + position.z, stoneNoiseSettings);

        int endPosition = surfaceHeight;
        if (chunkData.worldPosition.y < 0)
        {
            Debug.Log("teste");
            endPosition = chunkData.worldPosition.y + chunkData.chunkHeight;
        }

        if (stoneNoise > stoneThreshold)
        {
            for (int i = chunkData.worldPosition.y; i <= endPosition; i++)
            {
                Vector3Int pos = new Vector3Int(position.x, i, position.z);
                Chunk.SetVoxel(chunkData, pos, VoxelsType.Stone);
            }
            return true;
        }
        return false;
    }
}
