using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetEffect : MonoBehaviour
{
    public GameObject player;
    public float magnetDuration = 5f;
    public GameObject magnetPrefab;
    public Transform magnetSpawnPoint;
    public float targetDistance = 200f;


    private bool isMagnetActive = false;
    private bool isMagnetOn = false;
    private Vector3 startPosition;
    private bool magnetSpawned = false;

    private void Start()
    {
        startPosition = player.transform.position;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player && !isMagnetOn)
        {
            ActivateMagnet();
        }
        if (other.gameObject == player && !isMagnetActive)
        {
            StartMagnet();
        }

    }
    private void Update()
    {
        float distanceTravelled = Vector3.Distance(startPosition, player.transform.position);
        if(distanceTravelled>=targetDistance && !magnetSpawned) {
            SpawnMagnet();
        }
    }
    private void SpawnMagnet()
    {
        magnetSpawned = true;
        Debug.Log("Magnet has spawned");

        Instantiate(magnetPrefab, magnetSpawnPoint.position, magnetSpawnPoint.rotation);
    }
    private void ActivateMagnet()
    {
        isMagnetOn = true;
        Debug.Log("Magnet is activated");
    }
    private void StartMagnet()
    {
        isMagnetActive = true;
        Debug.Log("Magnet effect started: hut tien");
        //player.GetComponent<player>().ActivateMagnet();
        Invoke("StopMagnet", magnetDuration);
    }

    private void StopMagnet()
    {
        isMagnetActive = false;
    }
}
