using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private GAME_STATE stateGame = GAME_STATE.dayTime;

    public float timeDay = 120f;
    public int numberMissionsByDay = 10;

    private float timeDayLeft;
    private int numberMissionsDone = 0;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        numberMissionsDone = numberMissionsByDay;
    }

    // Update is called once per frame
    void Update()
    {
        switch (stateGame)
        {
            case GAME_STATE.dayTime:
                break;
            case GAME_STATE.votingTime:
                break;
            case GAME_STATE.results:
                break;
        }
    }

    public void MissionDone(bool isSuccess)
    {

    }


}

public enum GAME_STATE
{
    dayTime,
    votingTime,
    results
}
