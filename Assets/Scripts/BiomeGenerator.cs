using System.Collections.Generic;
using UnityEngine;

public class BiomeGenerator : MonoBehaviour
{
    public int waterThreshold = 50;
    public NoiseSettings biomeNoiseSettings;
    public DomainWarping domainWarping;
    public VoxelsLayerHandler startVoxelLayer;
    public List<VoxelsLayerHandler> additionalLayerHandlers;

    [SerializeField] private bool useWarping;

    public ChunkData ProcessChunkCreation(ChunkData data, int x, int z, Vector2Int mapSeedOffset)
    {
        biomeNoiseSettings.offset = mapSeedOffset;
        int groundPosition = GetSurfaceHeightNoise(data.worldPosition.x + x, data.worldPosition.z + z, data.chunkHeight) + 50;
        for (int y = 0; y < data.chunkHeight; y++)
        {
            startVoxelLayer.Handle(data, new Vector3Int(x, y, z), groundPosition, mapSeedOffset);
        }
        foreach (var layer in additionalLayerHandlers)
        {
            layer.Handle(data, new Vector3Int(x, data.worldPosition.y, z), groundPosition, mapSeedOffset);
        }
        return data;
    }

    private int GetSurfaceHeightNoise(int x, int z, int chunkHeight)
    {
        float terrainHeight;
        if (useWarping)
        {
            terrainHeight = Noise.OctavePerlin(x, z, biomeNoiseSettings);
        }
        else
        {
            terrainHeight = domainWarping.GenerateDomainNoise(x, z, biomeNoiseSettings);
        }
        terrainHeight = Noise.Redistribution(terrainHeight, biomeNoiseSettings);
        int surfaceHeight = (int)Noise.RemapValueZeroToOneInt(terrainHeight, 0, chunkHeight);
        return surfaceHeight;
    }
}
