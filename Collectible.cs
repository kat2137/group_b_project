
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using C2 = UnityEngine.Plugins.SieConfigurableControllerUnity;

/*
namespace GameLogic.TreasureHunt
{
    public class Collectible : MonoBehaviour
    {
        public GameObject scoreDisplayPrefab;
        public SoundPlayer soundPlayer;
        public UnityEvent onCollected;
        private void OnTriggerEnter(Collider other)
        {
            
            if (!other.gameObject.CompareTag("Player")) return;
            {
                Debug.Log("Treasure collected!");
            }
                

            if (scoreDisplayPrefab != null)
            {   
                Instantiate(scoreDisplayPrefab, other.transform.position + Vector3.up * 2f, Quaternion.identity);

                soundPlayer?.SelectSound(C2.Cube.SpeakerA, 5, 2.0f);
                onCollected?.Invoke();
                Destroy(gameObject);
            }
                
        }


    }
}
*/