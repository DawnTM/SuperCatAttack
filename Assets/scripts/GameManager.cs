using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Player refs")]
    public Color[] player_colors;
    public List<PlayerController> players = new List<PlayerController>();
    public Transform[] spawnpoints;

    [Header("prefab refs")]
    public GameObject playerContainerPrefab;
    public static GameManager instance;

    [Header("LavelVars")]
    public int startTime;
    public float curTime;
    List<PlayerController> winningPlayers;

    [Header("Components")]
    private AudioSource audio;
    public AudioClip[] game_fx;
    public Transform playerContainerParent;
    public TextMeshProUGUI time;

    private void Awake()
    {
        instance = this;
        audio = GetComponent<AudioSource>();
        startTime = PlayerPrefs.GetInt("roundTime", 100);
    }
    // Start is called before the first frame update
    void Start()
    {
        curTime = startTime;
        time.text = curTime.ToString();
        winningPlayers = new List<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(curTime<=0)
        {
            winningPlayers.Clear();
            int highscore = 0;
            int index= 0;
            foreach (PlayerController player in players)
            {
                if(player.score > highscore)
                {
                    winningPlayers.Clear();
                    highscore = player.score;
                    index = players.IndexOf(player);
                    winningPlayers.Add(player);
                }
                else if (player.score == highscore && player.score != 0)
                {
                    winningPlayers.Add(player);
                }
            }
            if (winningPlayers.Count > 1)
            {
                foreach (PlayerController player in players)
                {
                    if (!winningPlayers.Contains(player))
                    {
                        player.enabled = false;
                    }
                }
                curTime = 30;
                time.color = Color.red;
            }
            else
            {
                PlayerPrefs.SetInt("colorIndex", index);
                SceneManager.LoadScene("win screen");
            }
            
        }
    }

    private void FixedUpdate()
    {
        curTime -= Time.deltaTime;
        time.text = ((int)curTime).ToString();

    }

    public void OnPLayerJoined(PlayerInput player)
    {
        PlayerContainerScript containerUI = Instantiate(playerContainerPrefab, playerContainerParent).GetComponent<PlayerContainerScript>();
        containerUI.initialize(player_colors[players.Count]);
        player.GetComponentInChildren<SpriteRenderer>().color = player_colors[players.Count];
        players.Add(player.GetComponent<PlayerController>());
        player.GetComponent<PlayerController>().setUIContainer(containerUI);
        player.transform.position = spawnpoints[Random.Range(0, spawnpoints.Length)].position;
    }

    public void OnPlayerDeath(PlayerController player, PlayerController attacker)
    {
        if (attacker != null)
        {
            attacker.addScore();
        }
        player.die();
    }
}
