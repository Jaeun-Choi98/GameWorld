using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class User
{
  public int userId;
  public string email;
  public string passwd;
}

[System.Serializable]
public class Player
{
  public int userId;
  public int playerId;
  public PlayerInfo playerInfo;
  public List<Inventory> inventory;
}

[System.Serializable]
public class PlayerInfo
{
  public int Speed;
  public int JumpPower;
  public int Money;
  public string Name;
}

[System.Serializable]
public class Inventory
{
  public int ItemId;
  public int Quantity;
  public string ItemName;
}

[System.Serializable]
public class Item
{
  public int itemId;
  public int price;
  public string itemName;
  public string itemType;
  public string description;
}

public class Server : MonoBehaviour
{
  private static Server instance = null;

  public User user;

  public Player player;

  public PlayerInfo playerInfo;

  public List<Inventory> inventory = new List<Inventory>();

  public List<Item> items = new List<Item>();

  public static Server Instance
  {
    get
    {
      if (instance == null)
      {
        return null;
      }
      return instance;
    }
  }

  private void Awake()
  {
    if (instance == null)
    {
      instance = this;
      DontDestroyOnLoad(gameObject);
    }
    else
    {
      Destroy(gameObject);
    }
  }

  private void Start()
  {
    // 실제 실행 시 삭제
    //InitDev();
    LoadPlayerData(1);

    LoadItemData();
  }

  private void InitDev()
  {
    player = new Player();
    playerInfo = new PlayerInfo();
    playerInfo.Speed = 10;
    playerInfo.JumpPower = 5;
    playerInfo.Name = "cjuTest";
    playerInfo.Money = 1111;
    player.userId = 1;
    player.playerId = 1;
    player.playerInfo = playerInfo;

    Inventory inventory1 = new Inventory();
    Inventory inventory2 = new Inventory();
    inventory1.ItemId = 1;
    inventory1.ItemName = "sword";
    inventory1.Quantity = 1;
    inventory2.ItemId = 2;
    inventory2.ItemName = "arrow";
    inventory2.Quantity = 1;
    inventory.Add(inventory1);
    inventory.Add(inventory2);

    // 이후에 로그인 씬에서 로그인 할 때 서버에서 모든 정보를 긁어와야 함. ( GetItemsInfo api 설계 )
    Item item1 = new Item();
    Item item2 = new Item();
    item1.itemId = 1;
    item1.itemName = "sword";
    item2.itemId = 2;
    item2.itemName = "arrow";
    items.Add(item1);
    items.Add(item2);
  }

  public void SaveBossGameData(int userId, int playerId, string name, int money, int speed, int jumpPower)
  {
    StartCoroutine(SaveBossGameDataProcess(userId, playerId, name, money, speed, jumpPower));
    //StartCoroutine(LoadPlayerDataProcess(userId));
  }

  IEnumerator SaveBossGameDataProcess(int userId, int playerId, string name, int money, int speed, int jumpPower)
  {
    PlayerInfo info = new PlayerInfo();
    info.Name = name;
    info.JumpPower = jumpPower;
    info.Money = money;
    info.Speed = speed;
    Player player = new Player();
    player.userId = userId;
    player.playerId = playerId;
    player.playerInfo = info;

    string url = "localhost:8080/players/save-playerinfo";
    UnityWebRequest request = new UnityWebRequest(url, "POST");
    string jsonStr = JsonConvert.SerializeObject(player);
    byte[] jsonStrToByte = new System.Text.UTF8Encoding().GetBytes(jsonStr);
    request.uploadHandler = new UploadHandlerRaw(jsonStrToByte);
    request.downloadHandler = new DownloadHandlerBuffer();

    yield return request.SendWebRequest();

    if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
    {
      Debug.LogError("Error: " + request.error);
    }
    else
    {
      string jsonResponse = request.downloadHandler.text;
      Player responseObj = JsonConvert.DeserializeObject<Player>(jsonResponse);
      Debug.Log("획득한 총 재화: " + responseObj.playerInfo.Money);
      yield return LoadPlayerDataProcess(userId);
    }
  }

  public void LoadPlayerData(int userId)
  {
    StartCoroutine(LoadPlayerDataProcess(userId));
  }

  IEnumerator LoadPlayerDataProcess(int userId)
  {
    string url = "localhost:8080/players/" + userId;
    UnityWebRequest request = new UnityWebRequest(url, "GET");
    request.downloadHandler = new DownloadHandlerBuffer();
    yield return request.SendWebRequest();

    if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
    {
      Debug.LogError("Error: " + request.error);
      yield break;
    }
    else
    {
      string jsonResponse = request.downloadHandler.text;
      //Debug.Log(jsonResponse);
      player = JsonConvert.DeserializeObject<Player>(jsonResponse);
      playerInfo = player.playerInfo;
      user.userId = userId;
      if (player.inventory == null)
      {
        player.inventory = new List<Inventory>();
      }
      inventory = player.inventory;
    }
  }

  public void LoadItemData()
  {
    StartCoroutine(LoadItemDataProcess());
  }

  IEnumerator LoadItemDataProcess()
  {
    string url = "localhost:8080/items/load-iteminfo";
    UnityWebRequest request = new UnityWebRequest(url, "GET");
    request.downloadHandler = new DownloadHandlerBuffer();
    yield return request.SendWebRequest();
    if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
    {
      Debug.LogError("Error: " + request.error);
      yield break;
    }
    else
    {
      string jsonResponse = request.downloadHandler.text;
      //Debug.Log(jsonResponse);
      items = JsonConvert.DeserializeObject<List<Item>>(jsonResponse);
    }
  }

  public void SavePlayerInfoAndInventory()
  {
    StartCoroutine(SavePlayerInfoAndInventoryProcess());
  }

  IEnumerator SavePlayerInfoAndInventoryProcess()
  {
    string url = "localhost:8080/players/save-playerinfo-playerinventory";
    UnityWebRequest request = new UnityWebRequest(url, "POST");
    string jsonStr = JsonConvert.SerializeObject(player);
    byte[] jsonStrToByte = new System.Text.UTF8Encoding().GetBytes(jsonStr);
    request.uploadHandler = new UploadHandlerRaw(jsonStrToByte);
    request.downloadHandler = new DownloadHandlerBuffer();

    yield return request.SendWebRequest();

    if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
    {
      Debug.LogError("Error: " + request.error);
    }
    else
    {
      string jsonResponse = request.downloadHandler.text;
      Player responseObj = JsonConvert.DeserializeObject<Player>(jsonResponse);
    }
  }
}
