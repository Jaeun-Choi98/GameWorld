using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherPlayerController : MonoBehaviour
{
  Animator animator;
  private float h, v;

  [SerializeField]
  private bool isJump, isRoll;
  private void Start()
  {
    animator = GetComponentInChildren<Animator>();
  }

  public void UpdateTransform(PlayerTransformPayload playerTransform)
  {
    Vector3 position = new Vector3(playerTransform.position[0], playerTransform.position[1], playerTransform.position[2]);
    Quaternion rotation = new Quaternion(playerTransform.rotation[0], playerTransform.rotation[1], playerTransform.rotation[2], playerTransform.rotation[3]);
    transform.position = position;
    transform.rotation = rotation;
    h = playerTransform.h;
    v = playerTransform.v;

    animator.SetFloat("Horizontal", h);
    animator.SetFloat("Vertical", v);


    if (playerTransform.isJump && !isJump)
    {
      isJump = true;
      StartCoroutine(JumpProcess());
    }
    if (playerTransform.isRoll && !isRoll)
    {
      isRoll = true;
      StartCoroutine(RollProcess());
    }
    /*transform.position = Vector3.Lerp(players[transform.playerId].transform.position, position, 10f * Time.deltaTime);
    transform.rotation = Quaternion.Lerp(players[transform.playerId].transform.rotation, rotation, 5f * Time.deltaTime);*/
  }

  IEnumerator RollProcess()
  {
    animator.SetTrigger("RunToRoll");
    yield return new WaitForSeconds(1.5f);
    isRoll = false;
  }

  IEnumerator JumpProcess()
  {
    animator.SetTrigger("RunToJump");
    yield return new WaitForSeconds(1f);
    isJump = false;
  }
}
