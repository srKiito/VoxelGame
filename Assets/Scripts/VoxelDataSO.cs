using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Voxel Data", menuName = "Data/Voxel Data")]
public class VoxelDataSO : ScriptableObject
{
    public float textureSizeX, textureSizeY;
    public List<TextureData> textureDataList;
}

[Serializable]
public class TextureData
{
    public VoxelsType voxelsType;
    public Vector2Int up, down, side;
    public bool isSolid = true;
    public bool generatesCollider = true;
}
