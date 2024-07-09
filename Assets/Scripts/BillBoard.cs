using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
  [SerializeField]
  private GameObject target;
  void Start()
  {
    target = GameObject.Find("Player");
  }

  // Update is called once per frame
  void Update()
  {
    transform.forward = target.transform.forward;
    //Debug.Log(transform.forward);
    //transform.LookAt(target.transform.position);
  }
}
