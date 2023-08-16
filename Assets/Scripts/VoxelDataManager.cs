using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelDataManager : MonoBehaviour
{
    public static float textureOffset = 0.001f;
    public static float tileSizeX, tileSizeY;
    public static Dictionary<VoxelsType, TextureData> voxelTextureDataDictionary = new Dictionary<VoxelsType, TextureData>();
    public VoxelDataSO textureData;

    private void Awake()
    {
        foreach (var item in textureData.textureDataList)
        {
            if (voxelTextureDataDictionary.ContainsKey(item.voxelsType) == false)
            {
                voxelTextureDataDictionary.Add(item.voxelsType, item);
            }
        }
        tileSizeX = textureData.textureSizeX;
        tileSizeY = textureData.textureSizeY;
    }
}
