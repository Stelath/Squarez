using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public Transform[] playerSpawns;
    public Color[] playerColors;
    public int amountOfPlayers = 2;

    public Camera mainCamera;
    public GameObject player;

    private GameObject[] players;

    // Start is called before the first frame update
    void Start()
    {
        players = new GameObject[amountOfPlayers];
        for (int i = 0; i < amountOfPlayers; i++)
        {
            players[i] = Instantiate(player, playerSpawns[i].position, playerSpawns[i].rotation);
            players[i].GetComponent<PlayerController>().playerNumber = i + 1;
            players[i].GetComponent<PlayerController>().playerColor = playerColors[i];
        }

        Transform[] playerTargets = new Transform[amountOfPlayers];
        for (int i = 0; i < amountOfPlayers; i++)
        {
            playerTargets[i] = players[i].transform;
        }
        mainCamera.GetComponent<CameraController>().targets = playerTargets;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
