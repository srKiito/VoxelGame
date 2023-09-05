using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VoxelHelper
{
    private static Direction[] directions = {
        Direction.forward,
        Direction.backwards,
        Direction.up,
        Direction.down,
        Direction.right,
        Direction.left,
    };

    public static MeshData GetMeshData(ChunkData chunk, int x, int y, int z, MeshData meshData, VoxelsType voxeltype)
    {
        if (voxeltype == VoxelsType.Air || voxeltype == VoxelsType.Nothing)
            return meshData;

        foreach (Direction direction in directions)
        {
            var neighbourVoxelCoordinates = new Vector3Int(x, y, z) + direction.GetVector();
            var neighbourBlockType = Chunk.GetVoxelFromChunkCoordinates(chunk, neighbourVoxelCoordinates);

            if (neighbourBlockType != VoxelsType.Nothing && VoxelDataManager.voxelTextureDataDictionary[neighbourBlockType].isSolid == false)
            {

                if (voxeltype == VoxelsType.Water)
                {
                    if (neighbourBlockType == VoxelsType.Air)
                        meshData.waterMesh = GetFaceDataIn(direction, chunk, x, y, z, meshData.waterMesh, voxeltype);
                }
                else
                {
                    meshData = GetFaceDataIn(direction, chunk, x, y, z, meshData, voxeltype);
                }

            }
        }

        return meshData;
    }

    public static Vector2Int TexturePosition(Direction direction, VoxelsType voxelsType)
    {
        return direction switch
        {
            Direction.up => VoxelDataManager.voxelTextureDataDictionary[voxelsType].up,
            Direction.down => VoxelDataManager.voxelTextureDataDictionary[voxelsType].down,
            _ => VoxelDataManager.voxelTextureDataDictionary[voxelsType].side,
        };
    }
    public static MeshData GetFaceDataIn(Direction direction, ChunkData chunk, int x, int y, int z, MeshData meshData, VoxelsType voxelsType)
    {
        GetFaceVertices(direction, x, y, z, meshData, voxelsType);
        meshData.AddQuadTriangles(VoxelDataManager.voxelTextureDataDictionary[voxelsType].generatesCollider);
        meshData.UV.AddRange(FaceUVs(direction, voxelsType));

        return meshData;
    }

    public static void GetFaceVertices(Direction direction, int x, int y, int z, MeshData meshData, VoxelsType voxelsType)
    {
        var generatesCollider = VoxelDataManager.voxelTextureDataDictionary[voxelsType].generatesCollider;
        //order of vertices matters for the normals and how we render the mesh
        switch (direction)
        {
            case Direction.backwards:
                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                break;
            case Direction.forward:
                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                break;
            case Direction.left:
                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                break;

            case Direction.right:
                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                break;
            case Direction.down:
                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                break;
            case Direction.up:
                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                break;
            default:
                break;
        }
    }

    public static Vector2[] FaceUVs(Direction direction, VoxelsType voxelsType)
    {
        Vector2[] UVs = new Vector2[4];
        var tilePos = TexturePosition(direction, voxelsType);

        UVs[0] = new Vector2(VoxelDataManager.tileSizeX * tilePos.x + VoxelDataManager.tileSizeX - VoxelDataManager.textureOffset, VoxelDataManager.tileSizeY * tilePos.y + VoxelDataManager.textureOffset);
        UVs[1] = new Vector2(VoxelDataManager.tileSizeX * tilePos.x + VoxelDataManager.tileSizeX - VoxelDataManager.textureOffset, VoxelDataManager.tileSizeY * tilePos.y + VoxelDataManager.tileSizeY - VoxelDataManager.textureOffset);
        UVs[2] = new Vector2(VoxelDataManager.tileSizeX * tilePos.x + VoxelDataManager.textureOffset, VoxelDataManager.tileSizeY * tilePos.y + VoxelDataManager.tileSizeY - VoxelDataManager.textureOffset);
        UVs[3] = new Vector2(VoxelDataManager.tileSizeX * tilePos.x + VoxelDataManager.textureOffset, VoxelDataManager.tileSizeY * tilePos.y + VoxelDataManager.textureOffset);

        return UVs;
    }
}
