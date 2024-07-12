using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimingCameraManager : MonoBehaviour
{
  [SerializeField]
  private Transform player;
  private Vector3 offset;

  void Start()
  {
    player = GameObject.Find("Player").GetComponent<Transform>();
    offset = transform.position - player.position;
  }

  // Update is called once per frame
  void Update()
  {
    transform.position = player.position + player.TransformDirection(offset);
    transform.forward = player.forward;
  }
}
