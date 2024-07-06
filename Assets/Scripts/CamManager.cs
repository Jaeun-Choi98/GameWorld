using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamManager : MonoBehaviour
{
  [SerializeField]
  private GameObject target;

  private float r = 4f;  
  private float rOffset = 3f;    
  private float rotSpeed = 150f;
  private float yAngle = 0f;
  private float xAngle = 0f;
  private LayerMask collisionMask;

  void Start()
  {
    target = GameObject.Find("Player");
    collisionMask = LayerMask.GetMask("Envirionment", "Player");
  }

  void Update()
  {
    Rotate();
    CamPosition();
  }

  void Rotate()
  {
    float dy = Input.GetAxis("Mouse Y");
    yAngle -= dy * rotSpeed * Time.deltaTime;
    yAngle = Mathf.Clamp(yAngle, -90f, 90f);

    float dx = Input.GetAxis("Mouse X");
    xAngle += dx * rotSpeed * Time.deltaTime;
  }

  void CamPosition()
  {
    // ���� ��ǥ�踦 ����Ͽ� ī�޶��� ��ġ ���
    Vector3 offset = new Vector3(
     -r * Mathf.Cos(yAngle * Mathf.Deg2Rad) * Mathf.Sin(xAngle * Mathf.Deg2Rad),
      r * Mathf.Sin(yAngle * Mathf.Deg2Rad) + rOffset,
     -r * Mathf.Cos(yAngle * Mathf.Deg2Rad) * Mathf.Cos(xAngle * Mathf.Deg2Rad)
    );


    // ī�޶� �ֺ� ������Ʈ�� ����� ��, �հ� ���� ���� ->
    // ī�޶�� �÷��̾� ������ �浹�� �����ϰ� �浹�� ��� ī�޶� �ش� ������Ʈ�� ǥ������ �̵�
    Ray ray = new Ray(target.transform.position, offset.normalized);
    RaycastHit hit;

    if (Physics.Raycast(ray, out hit, offset.magnitude, collisionMask))
    {
      transform.position = target.transform.position + offset.normalized * hit.distance * 0.8f;
    }
    else
    {
      transform.position = target.transform.position + offset;
    }

    transform.LookAt(target.transform.position);
  }
}

