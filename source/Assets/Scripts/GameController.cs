using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using System.Threading;

[System.Serializable]
public class Player
{
    public Image panel;
    public Text text;
    public Button button;
}

[System.Serializable]
public class PlayerColor
{
    public Color panelColor;
    public Color textColor;
}

public class GameController : MonoBehaviour {

    public Text[] buttonList;
    public Text gameOverText;
    public Text bestText;
    public Text countText;
    public Button buttonVolume;

    public GameObject gameOverPanel;
    public GameObject restartButton;
    public GameObject startInfo;
    public GameObject[] grid;

    public AudioSource soundSource1;
    public AudioSource soundSource2;
    public AudioSource musicSource;

    public AudioClip clickSound;
    public AudioClip winSound;
    public AudioClip wrongSound;
    public AudioClip loseSound;
    public AudioClip music1;

    public Player playerX;
    public Player playerO;
    public PlayerColor activePlayerColor;
    public PlayerColor inactivePlayerColor;

    [HideInInspector]
    public List<Color32> lastColors;

    private string playerSide;
    private int moveCount = 0;
    private int winCount = 0;

    [SerializeField]
    private int best = 0;

    [SerializeField]
    public bool musicEnabled = true;

    void Awake()
    {
        //images

        LoadGame();

        UpdateUI();

        SetGameControllerReferenceOnButtons();

        RestartGame();

        PlayMusic();

        //disabled
        //startInfo.SetActive(false);
        //playerX.panel.SetActive(false);
        //playerX.text.SetActive(false);
        //playerX.button.SetActive(false);

        Vector2 far = new Vector2(-100000, -100000);

        startInfo.transform.position = far;
        playerX.panel.transform.position = far;
        playerO.panel.transform.position = far;
        gameOverPanel.transform.position = far;
        //grid
        foreach(GameObject obj in grid)
        {
            obj.transform.position = far;
        }

    }

    void SetGameControllerReferenceOnButtons()
    {
        for (int i = 0; i < buttonList.Length; i++)
        {
            
            buttonList[i].GetComponentInParent<GridSpace>().SetGameControllerReference(this);
        }
    }

    public void SetStartingSide(string startingSide)
    {
        //PlayClickSound();

        playerSide = startingSide;

        if (playerSide == "X")
        {
            SetPlayerColors(playerX, playerO);
        }
        else
        {
            SetPlayerColors(playerO, playerX);
        }

        StartGame();
    }

    void StartGame()
    {
        SetBoardInteractable(true);

        SetPlayerButtons(false);

        //startInfo.SetActive(false);
    }

    public void ActionVolume()
    {
        PlayClickSound();

        musicEnabled = !musicEnabled;
        PlayMusic();

        SaveGame();

        UpdateUI();
    }

    public void ActionHome()
    {
        //PlayClickSound();
        PlayWrongSound();
    }

    public void ActionRate()
    {
        //PlayClickSound();
        PlayWrongSound();
    }



    public void ActionNoAds()
    {
        //PlayClickSound();
        PlayWrongSound();
    }


    public void ActionRestartGame()
    {
        winCount = 0;
        moveCount = 0;
        UpdateUI();

        PlayClickSound();
        RestartGame();
    }

    public void UpdateUI()
    {
        if (countText)
        {
            if (moveCount > 0)
            {
                countText.text = "" + winCount;
                countText.fontSize = 100; //140;
            }
            else
            {
                //countText.text = "Tic Tac Toe!";
                //countText.text = "Tic Toe!";
                //countText.text = "Tac Toe!";
                countText.text = "Tic Tac Toe?";
                countText.fontSize = 64;
            }
        }
        

        if(best < winCount)
        {
            best = winCount;
        }

        if (bestText)
            bestText.text = "Best " + best;


        //volume
        //buttonVolume.alpha = musicEnabled ? 1.0f : 0.6f; 

        buttonVolume.image.color = musicEnabled ? new Color(255f, 255f, 255f, 1.0f)  : new Color(255f, 255f, 255f, 0.4f);


    }

    public void RestartGame()
    {
        //playerSide = "X";

        //reset colors
        lastColors = new List<Color32>();


        if (gameOverPanel)
            gameOverPanel.SetActive(false);

        moveCount = 0;
        //winCount = 0;

        SetBoardInteractable(false);

        for (int i = 0; i < buttonList.Length; i++)
        {
            buttonList[i].text = "";
        }

        for (int i = 0; i < buttonList.Length; i++)
        {
             //image
            buttonList[i].GetComponentInParent<GridSpace>().ResetSpace();
        }

        if (restartButton != null)
            restartButton.SetActive(false);

        SetPlayerButtons(true);
        SetPlayerColorsInactive();

        if(startInfo != null)
         startInfo.SetActive(true);

        //force x
        SetStartingSide("X");       
    }

    void SetPlayerColorsInactive()
    {
        if (playerX != null && playerX.button != null)
        { 
            playerX.panel.color = inactivePlayerColor.panelColor;
            playerX.text.color = inactivePlayerColor.textColor;
            playerO.panel.color = inactivePlayerColor.panelColor;
            playerO.text.color = inactivePlayerColor.textColor;
        }
    }



    void SetPlayerColors(Player newPlayer, Player oldPlayer)
    {
        newPlayer.panel.color = activePlayerColor.panelColor;
        newPlayer.text.color = activePlayerColor.textColor;
        oldPlayer.panel.color = inactivePlayerColor.panelColor;
        oldPlayer.text.color = inactivePlayerColor.textColor;
    }

    void SetPlayerButtons(bool toggle)
    {
        if (playerX != null && playerX.button != null)
        {
            playerX.button.interactable = toggle;
            playerO.button.interactable = toggle;
        }
    }

    public string GetPlayerSide()
    {
        return playerSide;
    }


    public void EndTurn()
    {
        moveCount++;
        UpdateUI();

        if (buttonList[0].text == playerSide && buttonList[1].text == playerSide && buttonList[2].text == playerSide)
        {
            StartCoroutine(GameOver(playerSide));

            if (playerSide == "X")
            {
                buttonList[0].GetComponentInParent<GridSpace>().SetWin();
                buttonList[1].GetComponentInParent<GridSpace>().SetWin();
                buttonList[2].GetComponentInParent<GridSpace>().SetWin();
            }
            else
            {
                buttonList[0].GetComponentInParent<GridSpace>().SetLose();
                buttonList[1].GetComponentInParent<GridSpace>().SetLose();
                buttonList[2].GetComponentInParent<GridSpace>().SetLose();
            }
        }
        else if (buttonList[3].text == playerSide && buttonList[4].text == playerSide && buttonList[5].text == playerSide)
        {
            StartCoroutine(GameOver(playerSide));


            if (playerSide == "X")
            {
                buttonList[3].GetComponentInParent<GridSpace>().SetWin();
                buttonList[4].GetComponentInParent<GridSpace>().SetWin();
                buttonList[5].GetComponentInParent<GridSpace>().SetWin();
            }
            else
            {
                buttonList[3].GetComponentInParent<GridSpace>().SetLose();
                buttonList[4].GetComponentInParent<GridSpace>().SetLose();
                buttonList[5].GetComponentInParent<GridSpace>().SetLose();
            }

        }
        else if (buttonList[6].text == playerSide && buttonList[7].text == playerSide && buttonList[8].text == playerSide)
        {
            StartCoroutine(GameOver(playerSide));

            if (playerSide == "X")
            {
                buttonList[6].GetComponentInParent<GridSpace>().SetWin();
                buttonList[7].GetComponentInParent<GridSpace>().SetWin();
                buttonList[8].GetComponentInParent<GridSpace>().SetWin();
            }
            else
            {
                buttonList[6].GetComponentInParent<GridSpace>().SetLose();
                buttonList[7].GetComponentInParent<GridSpace>().SetLose();
                buttonList[8].GetComponentInParent<GridSpace>().SetLose();
            }

        }
        else if (buttonList[0].text == playerSide && buttonList[3].text == playerSide && buttonList[6].text == playerSide)
        {
            StartCoroutine(GameOver(playerSide));

            if (playerSide == "X")
            {
                buttonList[0].GetComponentInParent<GridSpace>().SetWin();
                buttonList[3].GetComponentInParent<GridSpace>().SetWin();
                buttonList[6].GetComponentInParent<GridSpace>().SetWin();
            }
            else
            {
                buttonList[0].GetComponentInParent<GridSpace>().SetLose();
                buttonList[3].GetComponentInParent<GridSpace>().SetLose();
                buttonList[6].GetComponentInParent<GridSpace>().SetLose();
            }

        }
        else if (buttonList[1].text == playerSide && buttonList[4].text == playerSide && buttonList[7].text == playerSide)
        {
            StartCoroutine(GameOver(playerSide));

            if (playerSide == "X")
            {
                buttonList[1].GetComponentInParent<GridSpace>().SetWin();
                buttonList[4].GetComponentInParent<GridSpace>().SetWin();
                buttonList[7].GetComponentInParent<GridSpace>().SetWin();
            }
            else
            {
                buttonList[1].GetComponentInParent<GridSpace>().SetLose();
                buttonList[4].GetComponentInParent<GridSpace>().SetLose();
                buttonList[7].GetComponentInParent<GridSpace>().SetLose();
            }

        }
        else if (buttonList[2].text == playerSide && buttonList[5].text == playerSide && buttonList[8].text == playerSide)
        {
            StartCoroutine(GameOver(playerSide));

            if (playerSide == "X")
            {
                buttonList[2].GetComponentInParent<GridSpace>().SetWin();
                buttonList[5].GetComponentInParent<GridSpace>().SetWin();
                buttonList[8].GetComponentInParent<GridSpace>().SetWin();
            }
            else
            {
                buttonList[2].GetComponentInParent<GridSpace>().SetLose();
                buttonList[5].GetComponentInParent<GridSpace>().SetLose();
                buttonList[8].GetComponentInParent<GridSpace>().SetLose();
            }

        }
        else if (buttonList[0].text == playerSide && buttonList[4].text == playerSide && buttonList[8].text == playerSide)
        {
            StartCoroutine(GameOver(playerSide));

            if (playerSide == "X")
            {
                buttonList[0].GetComponentInParent<GridSpace>().SetWin();
                buttonList[4].GetComponentInParent<GridSpace>().SetWin();
                buttonList[8].GetComponentInParent<GridSpace>().SetWin();
            }
            else
            {
                buttonList[0].GetComponentInParent<GridSpace>().SetLose();
                buttonList[4].GetComponentInParent<GridSpace>().SetLose();
                buttonList[8].GetComponentInParent<GridSpace>().SetLose();
            }

        }
        else if (buttonList[2].text == playerSide && buttonList[4].text == playerSide && buttonList[6].text == playerSide)
        {
            StartCoroutine(GameOver(playerSide));

            if (playerSide == "X")
            {
                buttonList[2].GetComponentInParent<GridSpace>().SetWin();
                buttonList[4].GetComponentInParent<GridSpace>().SetWin();
                buttonList[6].GetComponentInParent<GridSpace>().SetWin();
            }
            else
            {
                buttonList[2].GetComponentInParent<GridSpace>().SetLose();
                buttonList[4].GetComponentInParent<GridSpace>().SetLose();
                buttonList[6].GetComponentInParent<GridSpace>().SetLose();
            }

        }
        else if (moveCount >= 9)
        {
            //draw
            StartCoroutine(GameOver("draw"));

        }
        else
            ChangeSides();


        SaveGame();

    }

    void SetBoardInteractable(bool toggle)
    {
        for (int i = 0; i < buttonList.Length; i++)
        {
            buttonList[i].GetComponentInParent<Button>().interactable = toggle;
        }

    }

    void SetGameOverText(string value)
    {
        if (gameOverPanel)
        {
            gameOverPanel.SetActive(true);
            gameOverText.text = value;
        }
    }

    //void GameOver(string winningPlayer)
    IEnumerator GameOver(string winningPlayer)
    {
        SetBoardInteractable(false);

        if (winningPlayer == "draw")
        {
            PlayLoseSound();
            SetGameOverText("Draw!");

            //count = 0;
            //UpdateUI();

            SetPlayerColorsInactive();

            if (restartButton)
                restartButton.SetActive(true);

        }
        else
        {
            if (winningPlayer == "X")
            {
                PlayWinSound();

                winCount++;
                UpdateUI();

                //reset

                //after delay

                //StartCoroutine(RestartGame());

                yield return new WaitForSeconds(0.4f);

                RestartGame();
            }
            else
            {
                PlayLoseSound();

                SetGameOverText(winningPlayer + " Wins !");

                if (restartButton)
                    restartButton.SetActive(true);


            }
        }

        yield break;
    }

    void ChangeSides()
    {

        //switch
        playerSide = (playerSide == "X") ? "O" : "X";    // Note: Capital Letters for "X" and "O"
        //Debug.Log("ChangeSides: " + playerSide);

        if (playerSide == "X")
        {
            if (playerX != null && playerO != null)
                SetPlayerColors(playerX, playerO);

            SetBoardInteractable(true);

        }
        else
        {
            if (playerX != null && playerO != null)
                SetPlayerColors(playerO, playerX);



            SetBoardInteractable(false);

            StartCoroutine(ComputerPlay());


            //coin random
            int index = Random.Range(0, 100);
            if(index <= 25)
                StartCoroutine(AddCoin());
        }




    }



    public IEnumerator ComputerPlay()
    {
        //Debug.Log("ComputerPlay");
        yield return new WaitForSeconds(0.4f);

        bool found = false;

        //random

        Text[] buttonList2 = (Text[])buttonList.Clone();

        Utils.Shuffle(buttonList2);

        //find 1st one
        for (int i = 0; i < buttonList2.Length; i++)
        {
            if(buttonList2[i].text == "")
            {
                if (!found)
                {
                    //Debug.Log("ComputerPlay: found");

                    found = true;

                    buttonList2[i].GetComponentInParent<GridSpace>().SetSpace();

                    //buttonList[i].text = playerSide; 

                    //ChangeSides();

                    //  yield return nil;
                }
            }
        }


        yield break;

    }

    public IEnumerator AddCoin()
    {
        yield return new WaitForSeconds(0.6f);

        bool found = false;

        //random

        Text[] buttonList2 = (Text[])buttonList.Clone();

        Utils.Shuffle(buttonList2);

        //find 1st one
        for (int i = 0; i < buttonList2.Length; i++)
        {
            if (buttonList2[i].text == "")
            {
                if (!found)
                {

                    found = true;

                    buttonList2[i].GetComponentInParent<GridSpace>().SetCoin();

                    PlayCoinSound();

                }
            }
        }


        yield break;

    }
    public void PlayClickSound()
    {
        //SoundManager.instance.PlaySingle (clickSound);
        soundSource1.clip = clickSound;
        soundSource1.Play();
    }

    public void PlayWinSound()
    {
        soundSource2.clip = winSound;
        soundSource2.Play();
    }

    public void PlayLoseSound()
    {
        soundSource2.clip = loseSound;
        soundSource2.Play();
    }

    public void PlayCoinSound()
    {
        soundSource2.clip = clickSound;
        soundSource2.Play();
    }


    public void PlayWrongSound()
    {
        soundSource2.clip = wrongSound;
        soundSource2.Play();
    }

    public void PlayMusic()
    {
        Debug.Log("PlayMusic");

        musicSource.Stop();

        if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
        {
            //musicEnabled = false; //disabled on mac
            //return;
        }


        if (musicEnabled)
        {
            Debug.Log("PlayMusic good");
            musicSource.clip = music1;
            musicSource.Play();
        }
    }


    private Save CreateSaveGameObject()
    {
        //https://www.raywenderlich.com/160815/save-load-game-unity

        Save save = new Save();
        //int i = 0;
        //foreach (GameObject targetGameObject in targets)
        //{
        //    Target target = targetGameObject.GetComponent<Target>();
        //    if (target.activeRobot != null)
        //    {
        //        save.livingTargetPositions.Add(target.position);
        //        save.livingTargetsTypes.Add((int)target.activeRobot.GetComponent<Robot>().type);
        //        i++;
        //    }
        //}

        save.best = best;
        save.musicEnabled = musicEnabled;

        return save;
    }

    public void LoadGame()
    {
        // 1
        if (File.Exists(Application.persistentDataPath + "/gamesave.save"))
        {
            // 2
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
            Save save = (Save)bf.Deserialize(file);
            file.Close();

            // 3
            //for (int i = 0; i < save.livingTargetPositions.Count; i++)
            //{
            //    int position = save.livingTargetPositions[i];
            //    Target target = targets[position].GetComponent<Target>();
            //    target.ActivateRobot((RobotTypes)save.livingTargetsTypes[i]);
            //    target.GetComponent<Target>().ResetDeathTimer();
            //}

            musicEnabled = save.musicEnabled;
            best = save.best;

            Debug.Log("Game Loaded");

        }
        else
        {
            //defaults
            musicEnabled = true;
            best = 0;

            Debug.Log("No game saved!");
        }
    }


    public void SaveGame()
    {
        // 1
        Save save = CreateSaveGameObject();

        // 2
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/gamesave.save");
        bf.Serialize(file, save);
        file.Close();


        //debug
        //SaveAsJSON();

    }

    public void SaveAsJSON()
    {
        Save save = CreateSaveGameObject();
        string json = JsonUtility.ToJson(save);

        Debug.Log("Saving as JSON: " + json);

    }

}
