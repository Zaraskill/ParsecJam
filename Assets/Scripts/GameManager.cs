﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Rewired;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GAME_STATE stateGame = GAME_STATE.dayTime;

    public int numberDays;
    public int day = 0;
    private List<int> playersBoss;
    private int[] playersScoreFinal;
    public int[] playersScore;
    private Player[] listPlayers;
    public int boss;

    public float timeDay = 120f;
    public float timeVote = 120f;
    public float timerSpawnMissions = 15f;
    public int numberMissionsByDay = 10;

    public float timeDayLeft;
    public float timeVoteLeft;
    public float timerSpawnMissionsLeft;
    public int numberMissionsDone = 0;

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
        timerSpawnMissionsLeft = timerSpawnMissions;
        timeDayLeft = timeDay;
        timeVoteLeft = timeVote;

        int random = Random.Range(0, numberDays);
        listPlayers[random].isBoss = true;
        playersBoss.Add(random);
        boss = random;
    }

    // Update is called once per frame
    void Update()
    {
        switch (stateGame)
        {
            case GAME_STATE.dayTime:
                timerSpawnMissionsLeft -= Time.deltaTime;
                if(timerSpawnMissionsLeft <= 0)
                {
                    SpawnMission();
                    timerSpawnMissionsLeft = timerSpawnMissions;
                }

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

    public void MissionDone(bool isSuccess, bool isBoss, int idPlayer)
    {
        if (isSuccess)
        {
            Debug.Log("Oh hé hein bon");
            numberMissionsDone++;
            if (isBoss)
            {
                playersScore[idPlayer]++;
            }
            else
            {
                playersScore[idPlayer] += 2;
            }
            if (numberMissionsDone == numberMissionsByDay)
            {
                SwitchStateGame(GAME_STATE.votingTime);
            }
        }
    }

    private void SwitchStateGame(GAME_STATE newState)
    {
        switch (newState)
        {
            case GAME_STATE.dayTime:
                timeDayLeft = timeDay;                
                int random;
                do
                {
                    random = Random.Range(0, numberDays);
                } while (playersBoss.Contains(random));
                listPlayers[random].isBoss = true;
                playersBoss.Add(random);
                boss = random;
                stateGame = newState;
                break;
            case GAME_STATE.votingTime:
                int score = 0;
                foreach(int value in playersScore)
                {
                    score += value;
                }
                playersScoreFinal[boss] = score;
                listPlayers[boss].isBoss = false;
                timeVoteLeft = timeVote;
                // Affichage UI résultats
                stateGame = newState;
                break;
            case GAME_STATE.results:
                stateGame = newState;
                break;
        }
    }

    private void SpawnMission()
    {

    }
}

public enum GAME_STATE
{
    dayTime,
    votingTime,
    results
}
