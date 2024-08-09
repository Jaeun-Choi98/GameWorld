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
  private Dictionary<int, OtherPlayerController> players = new Dictionary<int, OtherPlayerController>();

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
    tcpClient.SendPlayerTransform();
    GameObject player = Instantiate(playerFactory, spawnPosition, Quaternion.identity);
    players[playerId] = player.GetComponent<OtherPlayerController>();

  }

  public void RemovePlayer(int playerId)
  {
    if (players.ContainsKey(playerId))
    {
      Destroy(players[playerId].gameObject);
      players.Remove(playerId);
    }
  }

  public void UpdatePlayerTransform(PlayerTransformPayload transform)
  {
    if (players.ContainsKey(transform.playerId))
    {
      players[transform.playerId].UpdateTransform(transform);
    }
  }
}
