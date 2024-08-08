using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerManager : MonoBehaviour
{
  [SerializeField]
  private GameObject playerFactory;

  [SerializeField]
  private Dictionary<int, GameObject> players = new Dictionary<int, GameObject>();

  [SerializeField]
  private TCPClient tcpClient;

  public void SpawnPlayer(int playerId, Vector3 spawnPosition)
  {
    if (players.ContainsKey(playerId) || playerId == Server.Instance.player.playerId)
    {
      Debug.LogWarning(playerId + "�� �̹� �����մϴ�.");
      return;
    }
    // ���ο� Ŭ���̾�Ʈ�� �����ϸ�, ���� Ŭ���̾�Ʈ�鵵 ���ο� Ŭ���̾�Ʈ�� ������Ʈ ����ȭ
    tcpClient.SendMessageToServer(new Message
    {
      type = "CONNECT",
      payload = new { playerId = Server.Instance.player.playerId }
    });
    GameObject player = Instantiate(playerFactory, spawnPosition, Quaternion.identity);
    players[playerId] = player;

  }

  public void RemovePlayer(int playerId)
  {
    if (players.ContainsKey(playerId))
    {
      Destroy(players[playerId]);
      players.Remove(playerId);
    }
  }

  public void UpdatePlayerTransform(PlayerTransformPayload transform)
  {
    Vector3 position = new Vector3(transform.position[0], transform.position[1], transform.position[2]);
    Quaternion rotation = new Quaternion(transform.rotation[0], transform.rotation[1], transform.rotation[2], transform.rotation[3]);
    if (players.ContainsKey(transform.playerId))
    {
      players[transform.playerId].transform.position = position;
      players[transform.playerId].transform.rotation = rotation;
      /*players[transform.playerId].transform.position = Vector3.Lerp(players[transform.playerId].transform.position, position, 10f * Time.deltaTime);
      players[transform.playerId].transform.rotation = Quaternion.Lerp(players[transform.playerId].transform.rotation, rotation, 5f * Time.deltaTime);*/
    }
  }
}
