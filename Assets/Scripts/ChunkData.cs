using System;
using UnityEngine;

public class ChunkData
{
    public VoxelsType[] voxels;
    public int chunkSize = 16;
    public int chunkHeight = 124;
    public World worldReference;
    public Vector3Int worldPosition;

    public bool modifiedByPlayer = false;

    public ChunkData(int chunkSize, int chunkHeight, World world, Vector3Int worldPosition)
    {
        this.chunkSize = chunkSize;
        this.chunkHeight = chunkHeight;
        this.worldReference = world;
        this.worldPosition = worldPosition;
        voxels = new VoxelsType[chunkSize * chunkSize * chunkHeight];
    }
}
