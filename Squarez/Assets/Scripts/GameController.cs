using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject[] levels;
    private GameObject currentLevel;
    private int currentLevelNumber = 0;

    public GameObject[] guns;
    public List<GameObject> activeGuns = new List<GameObject>();

    [HideInInspector] public Transform[] playerSpawns;
    public Color[] playerColors;
    public int amountOfPlayers = 2;
    public ParticleSystem spawnEffect;

    public Camera mainCamera;
    public GameObject player;

    [HideInInspector] public GameObject[] players;

    public GameObject roundEndCanvas;
    public GameObject roundEndText;
    private string winText = "";
    private bool roundOver = false;

    public int scoreNeededToWin = 6;
    private int[] scores;

    public GameObject sceneChanger;

    // Start is called before the first frame update
    void Start()
    {
        amountOfPlayers = DataPasser.playerCount;
        scoreNeededToWin = DataPasser.roundsToWin;

        currentLevelNumber = Random.Range(0, levels.Length);
        currentLevel = Instantiate(levels[currentLevelNumber], transform.position, transform.rotation);

        SetupScores();
        StartCoroutine("StartRound");
    }

    public void SetupScores()
    {
        scores = new int[amountOfPlayers];
        for (var i = 0; i < scores.Length; i++)
        {
            scores[i] = 0;
        }
        roundEndCanvas.GetComponent<CanvasGroup>().alpha = 0;
    }

    // Update is called once per frame
    void Update()
    {
        HandleRoundOver();
    }

    IEnumerator HandleGunSpawns()
    {
        var droppedGunForPlayer = -1;
        for (var i = 0; i < players.Length;)
        {
            var playerToDropGunOn = Random.Range(0, players.Length);
            if (playerToDropGunOn != droppedGunForPlayer)
            {
                var gun = Instantiate(guns[Random.Range(0, guns.Length)], new Vector3(players[playerToDropGunOn].transform.position.x + Random.Range(-5, 5), 20, 0), transform.rotation);
                gun.GetComponent<GunController>().canFire = false;
                droppedGunForPlayer = playerToDropGunOn;
                activeGuns.Add(gun);
                i++;
            }

        }

        while (!roundOver)
        {
            yield return new WaitForSeconds(Random.Range(10, 20));
            var gun = Instantiate(guns[Random.Range(0, guns.Length)], new Vector3(players[Random.Range(0, players.Length)].transform.position.x + Random.Range(-3, 3), 20, 0), transform.rotation);
            gun.GetComponent<GunController>().canFire = false;
            activeGuns.Add(gun);
        }

        StopCoroutine("HandleGunSpawns");
    }

    IEnumerator StartRound()
    {
        roundOver = false;

        FindPlayerSpawns();
        SpawnPlayers();

        yield return new WaitForSeconds(0.5f);
        roundEndText.GetComponent<Text>().text = "3";
        StartCoroutine("FadeInRoundEndText");
        yield return new WaitForSeconds(1);
        roundEndText.GetComponent<Text>().text = "2";
        yield return new WaitForSeconds(1);
        roundEndText.GetComponent<Text>().text = "1";
        yield return new WaitForSeconds(1);
        roundEndText.GetComponent<Text>().text = "Go!";
        yield return new WaitForSeconds(0.5f);
        StartCoroutine("FadeOutRoundEndText");

        foreach (var player in players)
        {
            player.GetComponent<PlayerController>().disabled = false;
        }

        StartCoroutine("HandleGunSpawns");
    }

    public void FindPlayerSpawns()
    {
        var playerSpawnsList = new List<Transform>();
        foreach (Transform child in currentLevel.transform)
        {
            if (child.tag == "Spawn")
            {
                playerSpawnsList.Add(child);
            }
        }
        playerSpawns = playerSpawnsList.ToArray();

    }

    public void SpawnPlayers()
    {
        players = new GameObject[amountOfPlayers];
        for (int i = 0; i < amountOfPlayers; i++)
        {
            players[i] = Instantiate(player, playerSpawns[i].position, playerSpawns[i].rotation);
            var instantiatedSpawnEffect = Instantiate(spawnEffect, players[i].transform.position, players[i].transform.rotation);
            instantiatedSpawnEffect.startColor = playerColors[i];
            Destroy(instantiatedSpawnEffect.gameObject, 2f);

            var playerController = players[i].GetComponent<PlayerController>();
            playerController.playerNumber = i + 1;
            playerController.playerColor = playerColors[i];
            playerController.disabled = true;
        }

        Transform[] playerTargets = new Transform[amountOfPlayers];
        for (int i = 0; i < amountOfPlayers; i++)
        {
            playerTargets[i] = players[i].transform;
        }
        mainCamera.GetComponent<CameraController>().targets = playerTargets;
    }

    private void HandleRoundOver()
    {
        if (players.Length <= 1 && !(roundOver))
        {
            roundOver = true;

            if (players.Length == 1)
            {
                var winningPlayerNumber = players[0].GetComponent<PlayerController>().playerNumber;
                scores[winningPlayerNumber - 1] = scores[winningPlayerNumber - 1] + 1;
                winText = ("Player " + winningPlayerNumber + " Wins!\n");
            }
            else if (players.Length < 1)
            {
                winText = "Draw!\n";
            }

            for (var i = 0; i < scores.Length; i++)
            {
                if (i < scores.Length - 1)
                {
                    winText = winText + scores[i] + " - ";
                }
                else
                {
                    winText = winText + scores[i];
                }

            }

            var gameOver = false;
            for (var i = 0; i < scores.Length; i++)
            {
                if (scores[i] >= scoreNeededToWin)
                {
                    winText = ("Player " + (i + 1) + " Has Won The Game!");
                    gameOver = true;
                    break;
                }
                else
                {
                    gameOver = false;
                }
            }

            if (!gameOver)
            {
                StartCoroutine("RoundOver");
            }
            else
            {
                StartCoroutine("GameOver");
            }
        }
    }

    IEnumerator GameOver()
    {
        roundEndText.GetComponent<Text>().text = winText;
        StartCoroutine("FadeInRoundEndText");
        yield return new WaitForSeconds(3);
        StartCoroutine("FadeOutRoundEndText");

        sceneChanger.GetComponent<SceneChanger>().FadeToScene(0);
    }

    IEnumerator RoundOver()
    {
        currentLevelNumber = Random.Range(0, levels.Length);

        roundEndText.GetComponent<Text>().text = winText;
        StartCoroutine("FadeInRoundEndText");
        yield return new WaitForSeconds(3);
        StartCoroutine("FadeOutRoundEndText");

        var lastLevel = currentLevel;
        currentLevel = Instantiate(levels[currentLevelNumber], new Vector3((FindWidthOfLevel(lastLevel) / 2) + (FindWidthOfLevel(levels[currentLevelNumber]) / 2), 0, 0), transform.rotation);
        if (players.Length == 1)
        {
            players[0].GetComponent<PlayerController>().PlayerDeath();
        }

        foreach (var gun in activeGuns.ToArray())
        {
            Destroy(gun);
        }

        activeGuns = new List<GameObject>();

        for (float f = 0.0125f; f <= 1f; f += 0.0125f)
        {
            currentLevel.transform.position = new Vector3((currentLevel.transform.position.x - (FindWidthOfLevel(currentLevel) / 80)), 0, 0);
            lastLevel.transform.position = new Vector3((lastLevel.transform.position.x - (FindWidthOfLevel(currentLevel) / 80)), 0, 0);
            yield return new WaitForSeconds(0.0125f);
        }

        Destroy(lastLevel);

        StartCoroutine("StartRound");
    }

    public float FindWidthOfLevel(GameObject gameObjectToFindWidthOf)
    {
        float largestWidth = 0;

        foreach (Transform child in gameObjectToFindWidthOf.transform)
        {
            var childWidth = child.localScale.x;
            if (childWidth > largestWidth && child.tag == "WidthOfLevel")
            {
                largestWidth = childWidth;
            }
        }

        return largestWidth;
    }

    IEnumerator FadeInRoundEndText()
    {
        for (float f = 0.05f; f <= 1f; f += 0.05f)
        {
            roundEndCanvas.GetComponent<CanvasGroup>().alpha = f;
            yield return new WaitForSeconds(0.05f);
        }
    }

    IEnumerator FadeOutRoundEndText()
    {
        for (float f = 1f; f >= -0.5; f -= 0.05f)
        {
            roundEndCanvas.GetComponent<CanvasGroup>().alpha = f;
            yield return new WaitForSeconds(0.05f);
        }
    }
}
