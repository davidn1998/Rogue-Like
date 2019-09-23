using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{

    [SerializeField] int playerFoodPoints = 100;
    [SerializeField] float turnDelay = 0.1f;
    [SerializeField] float levelStartDelay = 2f;

    public static GameManager instance = null;

    BoardManager boardScript;
    [SerializeField] bool playersTurn = true;
    int level = 0;
    List<Enemy> enemies;
    bool enemiesMoving;
    TextMeshProUGUI levelText;
    GameObject levelImage;
    bool doingSetup;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        enemies = new List<Enemy>();
        boardScript = GetComponent<BoardManager>();
    }

    //This is called each time a scene is loaded.
    void OnLevelFinishedLoading(Scene scene, LoadSceneMode
    mode)
    {
        //Add one to our level number.
        level++;
        //Call InitGame to initialize our level.
        InitGame();
    }

    void OnEnable()
    {
        //Tell our ‘OnLevelFinishedLoading’ function to 
        //start listening for a scene change event as soon as
        //this script is enabled.
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }


    void OnDisable()
    {
        //Tell our ‘OnLevelFinishedLoading’ function to stop
        //listening for a scene change event as soon as this
        //script is disabled.
        //Remember to always have an unsubscription for every
        //delegate you subscribe to!
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;

    }

    void InitGame()
    {
        doingSetup = true;

        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<TextMeshProUGUI>();
        levelText.text = "Day " + level;
        levelImage.SetActive(true);
        Invoke("HideLevelImage", levelStartDelay);

        enemies.Clear();
        boardScript.SetupScene(level);
    }

    void HideLevelImage()
    {
        levelImage.SetActive(false);
        doingSetup = false;
    }

    public BoardManager GetBoardScript()
    {
        return boardScript;
    }

    public int GetPlayerFoodPoints()
    {
        return playerFoodPoints;
    }

    public void SetPlayerFoodPoints(int points)
    {
        playerFoodPoints = points;
    }

    public bool GetPlayersTurn()
    {
        return playersTurn;
    }

    public void SetPlayersTurn(bool isPlayersTurn)
    {
        playersTurn = isPlayersTurn;
    }

    public void GameOver()
    {
        levelText.text = "After " + level + " days, you starved.";
        levelImage.SetActive(true);
        enabled = false;
    }

    private void Update()
    {
        if (playersTurn || enemiesMoving || doingSetup)
        {
            return;
        }

        StartCoroutine(MoveEnemies());
    }

    public void AddEnemyToList(Enemy enemy)
    {
        enemies.Add(enemy);
    }

    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;
        yield return new WaitForSeconds(turnDelay);
        if (enemies.Count == 0)
        {
            yield return new WaitForSeconds(turnDelay);
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].MoveEnemy();
            yield return new WaitForSeconds(enemies[i].GetMoveTime());
        }

        playersTurn = true;
        enemiesMoving = false;
    }
}
