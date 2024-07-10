using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class DirectorAction : MonoBehaviour
{
  [SerializeField]
  private GameObject targetCam;
  [SerializeField]
  private PlayableDirector playableDirector;
  [SerializeField]
  private GameObject fadeInOut;

  void Start()
  {
    playableDirector = GetComponent<PlayableDirector>();
    playableDirector.Play();
    targetCam = GameObject.Find("Cinemachine Camera");
  }

  // Update is called once per frame
  void Update()
  {
    if(playableDirector.time >= playableDirector.duration)
    {
      targetCam.GetComponent<CinemachineBrain>().enabled = false;
      targetCam.SetActive(false);
      gameObject.SetActive(false);
      fadeInOut.SetActive(false);
    }
  }
}
