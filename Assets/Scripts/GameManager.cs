using System.Collections;
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
    public int[] playersScoreFinal;
    private int[] playersScore;
    public List<GameObject> missionsList;
    public List<GameObject> spawnList;
    private List<int> spawnUsed;
    private Player[] listPlayers;
    public int boss;

    public float timeDay = 120f;
    public float timeVote = 120f;
    public float timerSpawnMissions = 15f;
    public int numberMissionsByDay = 10;

    private float timeDayLeft;
    private float timeVoteLeft;
    public float timerSpawnMissionsLeft;
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
        spawnUsed = new List<int>();
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
        listPlayers[boss].GetComponent<SpriteRenderer>().color = Color.red;
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
                if (timeVoteLeft <= 0 || CheckVote() )
                {
                    AddMissingVotes();
                    UIManager.instance.UndisplayVote();

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
                numberMissionsDone = 0;
                SwitchStateGame(GAME_STATE.votingTime);
            }
        }
    }

    private void SwitchStateGame(GAME_STATE newState)
    {
        switch (newState)
        {
            case GAME_STATE.dayTime:
                for(int index = 0; index < playersScore.Length; ++index)
                {
                    playersScore[index] = 0;
                }
                Unvote();
                timeDayLeft = timeDay;                
                int random;
                do
                {
                    random = Random.Range(0, numberDays);
                } while (playersBoss.Contains(random));
                listPlayers[random].isBoss = true;
                playersBoss.Add(random);
                boss = random;
                listPlayers[boss].GetComponent<SpriteRenderer>().color = Color.blue;
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
                UIManager.instance.DisplayVote();
                stateGame = newState;

                foreach (Player player in listPlayers)
                {
                    player.hasVoted = false;
                    player.canMove = false;
                }

                break;
            case GAME_STATE.results:

                int bestBoss = -1;
                int bestScore = 0;
                for(int index = 0; index < playersScoreFinal.Length; ++index)
                {
                    if(playersScoreFinal[index] > bestScore)
                    {
                        bestScore = playersScoreFinal[index];
                        bestBoss = index;
                    }
                }
                UIManager.instance.DisplayResults(bestBoss, bestScore);

                stateGame = newState;
                break;
        }
    }

    private void SpawnMission()
    {
        if (spawnUsed.Capacity == spawnList.Capacity)
        {
            return;
        }
        int random;
        do
        {
            random = Random.Range(0, spawnList.Capacity);
        } while (spawnUsed.Contains(random));
        spawnUsed.Add(random);
        Instantiate(missionsList[Random.Range(0, missionsList.Capacity)], spawnList[random].transform.position, Quaternion.identity);
    }

    public int ScorePlayer(int index)
    {
        return playersScore[index];
    }

    public float TimerLeft()
    {
        return timeDayLeft;
    }

    public float TimerVoteLeft()
    {
        return timeVoteLeft;
    }

    public bool HasToVote()
    {
        return stateGame == GAME_STATE.votingTime;
    }

    private bool CheckVote()
    {
        foreach(Player player in listPlayers)
        {
            if (!player.hasVoted)
            {
                return false;
            }
        }
        return true;
    }

    private void AddMissingVotes()
    {
        foreach (Player player in listPlayers)
        {
            if (!player.hasVoted)
            {
                playersScoreFinal[boss] += 5;
            }
        }
    }

    private void Unvote()
    {
        foreach(Player player in listPlayers)
        {
            player.hasVoted = false;
            player.canMove = true;
        }
    }

    public void Vote(int voteValue)
    {
        playersScoreFinal[boss] += voteValue;
    }

}

public enum GAME_STATE
{
    dayTime,
    votingTime,
    results
}
