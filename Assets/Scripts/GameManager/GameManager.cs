using System;
using System.Collections;
using Cinemachine;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;
    private GameObject player;
    public Vector3Int currentPlayerChunkPosition;
    private Vector3Int currentChunkCenter = Vector3Int.zero;

    public World world;

    public float detectionTime = 1;
    public CinemachineVirtualCamera camera_VM;

    public void SpawnPlayer()
    {
        if (player != null) return;

        Vector3Int raycastStartPosition = new Vector3Int(world.chunkSize / 2, 100, world.chunkSize / 2);
        RaycastHit hit;
        if (Physics.Raycast(raycastStartPosition, Vector3.down, out hit, 120))
        {
            player = Instantiate(playerPrefab, hit.point + Vector3.up * 100, Quaternion.identity);
            camera_VM.Follow = player.transform.GetChild(0);
            StartMapChecking();
        }
    }

    public void StartMapChecking()
    {
        SetCurrentChunkCoordinates();
        StopAllCoroutines();
        StartCoroutine(NeedToLoadNewChunkCheck());
    }

    private void SetCurrentChunkCoordinates()
    {
        currentPlayerChunkPosition = WorldDataHelper.ChunkPositionFromBlockCoords(world, Vector3Int.RoundToInt(player.transform.position));
        currentChunkCenter.x = currentPlayerChunkPosition.x + world.chunkSize / 2;
        currentChunkCenter.y = currentPlayerChunkPosition.y + world.chunkSize / 2;
    }

    IEnumerator NeedToLoadNewChunkCheck()
    {
        yield return new WaitForSeconds(detectionTime);
        if (
            Mathf.Abs(currentChunkCenter.x - player.transform.position.x) > world.chunkSize ||
            Mathf.Abs(currentChunkCenter.z - player.transform.position.z) > world.chunkSize ||
            (Mathf.Abs(currentPlayerChunkPosition.y - player.transform.position.y) > world.chunkHeight)
            )
        {
            world.LoadAdditionalChunksRequest(player);
        }
        else
        {
            StartCoroutine(NeedToLoadNewChunkCheck());
        }
    }
}
