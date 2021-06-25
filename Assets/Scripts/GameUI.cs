using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public GameObject GameOverUI;
    public GameObject GameWinUI;
    bool gameOver;
    // Start is called before the first frame update
    void Start()
    {
        Guard.OnGuardSpotted += ShowGameOverUI;
        FindObjectOfType<Player>().OnGameWon += ShowGameWinUI;
    }

    // Update is called once per frame
    void Update()
    {
        if(gameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(0);
            }
        }
    }
    void ShowGameOverUI()
    {
        OnGameOver(GameOverUI);
    }
    void ShowGameWinUI()
    {
        OnGameOver(GameWinUI);
    }
    void OnGameOver(GameObject gameOverUI)
    {
        gameOverUI.SetActive(true);
        gameOver = true;
        Guard.OnGuardSpotted -= ShowGameOverUI;
        FindObjectOfType<Player>().OnGameWon -= ShowGameWinUI;
    }
}
