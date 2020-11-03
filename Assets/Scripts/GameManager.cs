using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Rewired;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private GAME_STATE stateGame = GAME_STATE.dayTime;

    private int numberDays;
    private int day = 0;
    private List<int> playersBoss;
    private int[] playersScoreFinal;
    private int[] playersScore;
    private Player[] listPlayers;
    private int boss;

    public float timeDay = 120f;
    public float timeVote = 120f;
    public int numberMissionsByDay = 10;

    private float timeDayLeft;
    private float timeVoteLeft;
    private int numberMissionsDone = 0;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;
        DontDestroyOnLoad(gameObject);


        numberDays = ReInput.controllers.joystickCount;
        day = 1;
        playersBoss = new List<int>();
        playersScoreFinal = new int[numberDays];
        playersScore = new int[numberDays];
        listPlayers = FindObjectsOfType<Player>();
    }

    // Start is called before the first frame update
    void Start()
    {
        numberMissionsDone = numberMissionsByDay;
        timeDayLeft = timeDay;
        timeVoteLeft = timeVote;

        int random = Random.Range(0, numberDays);
        //Code player dire qu'il est boss
        playersBoss.Add(random);
        boss = random;
    }

    // Update is called once per frame
    void Update()
    {
        switch (stateGame)
        {
            case GAME_STATE.dayTime:
                timeDayLeft -= Time.deltaTime;
                if (timeDayLeft <= 0)
                {
                    //bloque tout ce qui était en train d'être fait
                    SwitchStateGame(GAME_STATE.votingTime);
                }
                break;
            case GAME_STATE.votingTime:
                timeVoteLeft -= Time.deltaTime;
                if (timeVoteLeft <= 0)
                {
                    //Enlève l'UI
                    if (day == numberDays)
                    {
                        SwitchStateGame(GAME_STATE.results);
                    }
                    else
                    {
                        SwitchStateGame(GAME_STATE.dayTime);
                        day++;
                    }
                }
                break;
            case GAME_STATE.results:
                //Afficher meilleur boss
                break;
        }
    }

    public void MissionDone(bool isSuccess)
    {
        if (isSuccess)
        {
            ////missions + 1 
            //if(numberMissionsDone == numberMissionsByDay)
            //{
            //    //fin jour
            //}
        }
    }

    private void SwitchStateGame(GAME_STATE newState)
    {
        switch (stateGame)
        {
            case GAME_STATE.dayTime:
                timeDayLeft = Time.deltaTime;
                int random;
                do
                {
                    random = Random.Range(0, numberDays);
                } while (playersBoss.Contains(random));
                //Code player dire qu'il est boss
                playersBoss.Add(random);
                boss = random;
                break;
            case GAME_STATE.votingTime:
                int score = 0;
                foreach(int value in playersScore)
                {
                    score += value;
                }
                playersScoreFinal[boss] = score;

                timeVoteLeft = Time.deltaTime;
                // Affichage UI résultats
                break;
            case GAME_STATE.results:
                break;
        }
    }
}

public enum GAME_STATE
{
    dayTime,
    votingTime,
    results
}
