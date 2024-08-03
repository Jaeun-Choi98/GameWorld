using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
  [SerializeField]
  private Transform player;
  [SerializeField]
  private float cameraSpeed = 3f;

  private Vector3 threshold;
  private Vector3 cameraTargetPos;

  private void Start()
  {
    player = GameObject.Find("Player").GetComponent<Transform>();
    threshold = CalculateThreshold();
  }

  private void LateUpdate()
  {
    Vector3 dif = player.position - transform.position;
    cameraTargetPos.y = transform.position.y;
    // 만약 좌우 오브젝트(벽)가 있을 경우, 오브젝트와 카메라의 x좌표의 차이가 threshold보다 작거나 같은 경우는 움직x
    // 이후에 구현 시 적용
    if (Mathf.Abs(dif.x) > threshold.x)
    {
      cameraTargetPos.x = player.position.x + (dif.x > 0 ? -threshold.x : threshold.x);
    }

    if (dif.y > threshold.y * (2f / 3f))
    {
      cameraTargetPos.y = player.position.y + -threshold.y * (2f / 3f);
    }
    else if (dif.y < -threshold.y + 1)
    {
      cameraTargetPos.y = player.position.y + threshold.y - 1;
    }
    
    cameraTargetPos.z = transform.position.z;
    transform.position = Vector3.Lerp(transform.position, cameraTargetPos, cameraSpeed * Time.deltaTime);
  }

  Vector3 CalculateThreshold()
  {
    Rect aspect = Camera.main.pixelRect;
    Vector3 threshold = new Vector3(Camera.main.orthographicSize * aspect.width / aspect.height,
      Camera.main.orthographicSize, 0);
    threshold.x *= 2f / 3f;
    return threshold;
  }

}
