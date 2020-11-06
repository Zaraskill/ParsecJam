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
    private int[] playersScoreFinal;
    private int[] playersScore;
    public List<GameObject> missionsList;
    public List<GameObject> spawnList;
    public List<int> spawnUsed;
    private Player[] listPlayers;
    private List<int> playerFree;
    public int boss;

    public float timeDay = 120f;
    public float timeVote = 120f;
    public float timerSpawnMissions = 15f;
    public int numberMissionsByDay = 10;
    public float timeBeforeVote;

    private float timeDayLeft;
    private float timeVoteLeft;
    private float timerSpawnMissionsLeft;
    public float timeBeforeVoteLeft;
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
        playerFree = new List<int>();
        for (int index = 0; index < numberDays; index++)
        {
            playerFree.Add(index);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        timerSpawnMissionsLeft = timerSpawnMissions;
        timeDayLeft = timeDay;
        timeVoteLeft = timeVote;
        timeBeforeVoteLeft = timeBeforeVote;

        int random = Random.Range(0, numberDays);
        listPlayers[random].isBoss = true;
        playersBoss.Add(random);
        boss = random;
        listPlayers[random].bossAttitude.SetActive(true);
        playerFree.Remove(random);
        AudioManager.instance.PlayGame();
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
                    
                    SwitchStateGame(GAME_STATE.votingTime);
                }
                break;
            case GAME_STATE.votingTime:
                if(timeBeforeVoteLeft > 0)
                {
                    timeBeforeVoteLeft -= Time.deltaTime;
                }
                else
                {
                    timeVoteLeft -= Time.deltaTime;
                    if (timeVoteLeft <= 0 || CheckVote())
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
                }                
                break;
            case GAME_STATE.results:
                //Afficher meilleur boss
                break;
        }
    }

    public void MissionDone(bool isSuccess, bool isBoss, int idPlayer, GameObject mission)
    {
        if (isSuccess)
        {
            AudioManager.instance.PlayValid();
            numberMissionsDone++;

            RemoveMission(mission);

            if (isBoss)
            {
                if (mission.GetComponent<InteractableObjects>().isFailed)
                {
                    playersScore[idPlayer] += 2;
                }
                else
                {
                    playersScore[idPlayer]++;
                }
                
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
        else
        {
            AudioManager.instance.PlayFail();
            if (isBoss)
            {
                playersScore[idPlayer] -= 2;
            }
            else
            {
                playersScore[idPlayer]--;
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
                timeBeforeVoteLeft = timeBeforeVote;
                int random = 1;
                random = playerFree[Random.Range(0, playerFree.Count)];
                playerFree.Remove(random);
                listPlayers[random].isBoss = true;
                playersBoss.Add(random);
                boss = random;
                foreach(Player player in listPlayers)
                {
                    player.BossAttitude();
                }
                stateGame = newState;
                break;
            case GAME_STATE.votingTime:
                InteractableObjects[] missions = FindObjectsOfType<InteractableObjects>();
                foreach (InteractableObjects mission in missions)
                {
                    RemoveMission(mission.gameObject);
                    Destroy(mission.missionUI);
                    Destroy(mission.gameObject);
                }
                foreach(Player player in listPlayers)
                {
                    player.DynamicPlayer();
                }
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
                AudioManager.instance.PlayResults();
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
                List<int> leaderboard = GetRankPlayer();
                for(int index = 0; index < leaderboard.Count; ++index)
                {
                    UIManager.instance.ResultsPosition(leaderboard[index] + 1, playersScoreFinal[leaderboard[index]], index + 1);
                }
                UIManager.instance.DisplayResults();

                stateGame = newState;
                break;
        }
    }

    private List<int> GetRankPlayer()
    {
        List<int> leaderboard = new List<int>();
        leaderboard.Add(0);
        if(numberDays > 1)
        {
            if (playersScoreFinal[1] > playersScoreFinal[0])
            {
                leaderboard.Insert(0, 1);
            }
            if(numberDays > 2)
            {
                if (playersScoreFinal[2] > playersScoreFinal[leaderboard[0]])
                {
                    leaderboard.Insert(0, 2);
                }
                else if (playersScoreFinal[2] > playersScoreFinal[leaderboard[1]])
                {
                    leaderboard.Insert(1, 2);
                }
                else
                {
                    leaderboard.Insert(2, 2);
                }
            }
        }        
        return leaderboard;
    }

    private void SpawnMission()
    {
        if (spawnUsed.Capacity == spawnList.Count)
        {
            return;
        }
        int random;
        do
        {
            random = Random.Range(0, spawnList.Count);
        } while (spawnUsed.Contains(random));
        spawnUsed.Add(random);
        Instantiate(missionsList[Random.Range(0, missionsList.Count)], spawnList[random].transform.position, Quaternion.identity);
    }

    public void RemoveMission(GameObject mission)
    {
        for (int index = 0; index < spawnList.Capacity; ++index)
        {
            if (spawnList[index].transform.position == mission.transform.position)
            {
                spawnUsed.Remove(index);
            }
        }
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









/*
 *je COMMENTE QUAND JE LE VEUX SALE CHIEN
 * DE
 * SES
 * MORTS
 */