using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
  public AudioClip[] audioClip;
  private AudioSource audioSource;
  void Start()
  {
    audioSource = GetComponent<AudioSource>();
    audioSource.clip = audioClip[0];
    audioSource.Play();
  }

  private void OnTriggerEnter(Collider other)
  {
    if (other.gameObject.name == "Enemy")
    {
      audioSource.clip = audioClip[1];
      audioSource.Play();
    }
  }

  private void OnTriggerExit(Collider other)
  {
    if (other.gameObject.name == "Enemy")
    {
      audioSource.clip = audioClip[0];
      audioSource.Play();
    }
  }
}
