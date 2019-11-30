using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {
    
    WaitForSeconds Secs;
    public Transform[] locations;


    CameraManager shoot;
    CharacterManager Playerr;
    LevelUI levelUI;

    public int turn = 2;
    int turn1 = 1;

    
    public bool countdown;
    public int Time1 = 30;
    int Time2;
    float Time3;

	void Start () {
        
        Playerr = CharacterManager.GetInstance();
        levelUI = LevelUI.GetInstance();
        shoot = CameraManager.GetInstance();

        Secs = new WaitForSeconds(1);

        levelUI.Line1.gameObject.SetActive(false);
        levelUI.Line2.gameObject.SetActive(false);

        StartCoroutine("Start");
       
	}

    void FixedUpdate()
    {

        if(Playerr.players[0].playerStates.transform.position.x < 
            Playerr.players[1].playerStates.transform.position.x)
        {
            Playerr.players[0].playerStates.lookRight = true;
            Playerr.players[1].playerStates.lookRight = false;
        }
        else
        {
            Playerr.players[0].playerStates.lookRight = false;
            Playerr.players[1].playerStates.lookRight = true;
        }
    }

    void Update()
    {
        if (countdown)
        {
            HandleTurnTimer();
        }
    }

    void HandleTurnTimer()
    {
        levelUI.Time.text = Time2.ToString();

        Time3 += Time.deltaTime; 

        if (Time3 > 1)
        {
            Time2--; 
            Time3 = 0;
        }

        if (Time2 <= 0) 
        {
            EndTurnFunction(true);
            countdown = false;
        }
    }

    IEnumerator StartGame()
    {
        

        yield return CreatePlayers();

        yield return InitTurn();
    }
	
    IEnumerator InitTurn()
    {

        levelUI.Line1.gameObject.SetActive(false);
        levelUI.Line2.gameObject.SetActive(false);


        Time2 = Time1;
        countdown = false;

        yield return InitPlayers();

        yield return EnableControl();

    }

    IEnumerator CreatePlayers()
    {
        for (int i = 0; i < Playerr.players.Count; i++)
        {
            GameObject obj = Instantiate(Playerr.players[i].playerPrefab
            , locations[i].position, Quaternion.identity)
            as GameObject;

            Playerr.players[i].playerStates = obj.GetComponent<StateManager>();

            Playerr.players[i].playerStates.healthSlider = levelUI.health[i];

            shoot.characters.Add(obj.transform);
        }

        yield return null;
    }

    IEnumerator InitPlayers()
    {
        for (int i = 0; i < Playerr.players.Count; i++)
        {
            Playerr.players[i].playerStates.health = 100;
            Playerr.players[i].playerStates.handleAnim.anim.Play("Locomotion");
            Playerr.players[i].playerStates.transform.position = locations[i].position;
        }

        yield return null;
    }

	IEnumerator EnableControl()
    {

        levelUI.Line1.gameObject.SetActive(true);
        levelUI.Line1.text = "Turn " + turn1;
        levelUI.Line1.color = Color.white;
        yield return Secs;
        yield return Secs;

        levelUI.Line1.text = "3";
        levelUI.Line1.color = Color.green;
        yield return Secs;
        levelUI.Line1.text = "2";
        levelUI.Line1.color = Color.yellow;
        yield return Secs;
        levelUI.Line1.text = "1";
        levelUI.Line1.color = Color.red;
        yield return Secs;
        levelUI.Line1.color = Color.red;
        levelUI.Line1.text = "FIGHT!";

        for (int i = 0; i < Playerr.players.Count; i++)
        {
            if(Playerr.players[i].playerType == PlayerBase.PlayerType.user)
            {
                InputHandler ih = Playerr.players[i].playerStates.gameObject.GetComponent<InputHandler>();
                ih.playerInput = Playerr.players[i].inputId;
                ih.enabled = true;
            }

             if(Playerr.players[i].playerType == PlayerBase.PlayerType.ai)
             {
                 AICharacter ai = Playerr.players[i].playerStates.gameObject.GetComponent<AICharacter>();
                 ai.enabled = true;
                 
                 ai.enStates = Playerr.returnOppositePlater(Playerr.players[i]).playerStates;
             }
        }

        yield return Secs;
        levelUI.Line1.gameObject.SetActive(false);
        countdown = true;
    } 

    void DisableControl()
    {
        for (int i = 0; i < Playerr.players.Count; i++)
        {
            Playerr.players[i].playerStates.ResetStateInputs();

            if(Playerr.players[i].playerType == PlayerBase.PlayerType.user)
            {
                Playerr.players[i].playerStates.GetComponent<InputHandler>().enabled = false;
            }

            if(Playerr.players[i].playerType == PlayerBase.PlayerType.ai)
            {
                Playerr.players[i].playerStates.GetComponent<AICharacter>().enabled = false;
            }
        }
    }

    public void EndTurnFunction(bool timeOut = false)
    {
        
        countdown = false;
        levelUI.Time.text = Time1.ToString() ;

        if (timeOut)
        {
            levelUI.Line1.gameObject.SetActive(true);
            levelUI.Line1.text = "Time Out!";
            levelUI.Line1.color = Color.cyan;
        }
        else
        {
            levelUI.Line1.gameObject.SetActive(true);
            levelUI.Line1.text = "K.O.";
            levelUI.Line1.color = Color.red;
        }

        DisableControl();

        StartCoroutine("EndTurn");
    }

    IEnumerator EndTurn()
    {
        yield return Secs;
        yield return Secs;
        yield return Secs;

        PlayerBase vPlayer = FindWinningPlayer();

        if(vPlayer == null) 
        {
            levelUI.Line1.text = "Draw";
            levelUI.Line1.color = Color.blue;
        }
        else
        {
            levelUI.Line1.text = vPlayer.playerId + " Wins!";
            levelUI.Line1.color = Color.red;
        }

        yield return Secs;
        yield return Secs;
        yield return Secs;

        if (vPlayer != null)
        {
            if (vPlayer.playerStates.health == 100)
            {
                levelUI.Line2.gameObject.SetActive(true);
                levelUI.Line2.text = "Flawless Victory!";
            }
        }

        yield return Secs;
        yield return Secs;
        yield return Secs;

        turn1++;

        bool matchOver = isMatchOver();

        if (!matchOver)
        {
            StartCoroutine("InitTurn"); 
        }
        else
        {
            for (int i = 0; i < Playerr.players.Count; i++)
            {
                Playerr.players[i].score = 0;
                Playerr.players[i].hasCharacter = false;
            }

            if (Playerr.solo)
            {
                if(vPlayer == Playerr.players[0])
                    MySceneManager.GetInstance().LoadNextOnProgression();
                else
                    MySceneManager.GetInstance().RequestLevelLoad(SceneType.main, "game_over");
            }
            else
            {
                MySceneManager.GetInstance().RequestLevelLoad(SceneType.main, "select");
            }
        }
    }
  
    bool isMatchOver()
    {
        bool retVal = false;

        for (int i = 0; i < Playerr.players.Count; i++)
        {
            if(Playerr.players[i].score >= turn)
            {
                retVal = true;
                break;
            }
        }

        return retVal;
    }

    PlayerBase FindWinningPlayer()
    {
        PlayerBase retVal = null;

        StateManager targetPlayer = null;

        if(Playerr.players[0].playerStates.health != Playerr.players[1].playerStates.health)
        {
            if(Playerr.players[0].playerStates.health < Playerr.players[1].playerStates.health)
            {
                Playerr.players[1].score++;
                targetPlayer = Playerr.players[1].playerStates;
                levelUI.AddWinIndicator(1);
            }
            else
            {
                Playerr.players[0].score++;
                targetPlayer = Playerr.players[0].playerStates;
                levelUI.AddWinIndicator(0);
            }

            retVal = Playerr.returnPlayerFromStates(targetPlayer); 
        }

        return retVal;
    }

    public static LevelManager instance;
    public static LevelManager GetInstance()
    {
        return instance;
    }

    void Awake()
    {
        instance = this;
    }
   
}

