using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public static UIManager instance;

    public TMP_Text timer;
    public TMP_Text timerVote;
    public TMP_Text bestBossText;
    public TMP_Text resultsText;
    public GameObject voteCanvas;
    public GameObject resultCanvas;
    public List<TMP_Text> scores;
    public List<Image> voted;

    private bool isVoting = false;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        scores[0].text = GameManager.instance.ScorePlayer(0).ToString();
        scores[1].text = GameManager.instance.ScorePlayer(1).ToString();
        scores[2].text = GameManager.instance.ScorePlayer(2).ToString();

        if (!isVoting)
        {
            float time = GameManager.instance.TimerLeft();
            timer.text = string.Format("{0} : {1:F0}", ((int)time / 60), time % 60);
        }
        else
        {
            float time = GameManager.instance.TimerVoteLeft();
            timerVote.text = string.Format("{0} : {1:F0}", ((int)time / 60), time % 60);
        }
        


    }

    public void DisplayVote()
    {
        isVoting = true;
        voteCanvas.SetActive(true);
        timer.gameObject.SetActive(false);
    }

    public void UndisplayVote()
    {
        isVoting = false;
        foreach(Image voteToken in voted)
        {
            voteToken.gameObject.SetActive(false);
        }
        voteCanvas.SetActive(false);
        timer.gameObject.SetActive(true);
    }

    public void VotePlayer(int idPlayer)
    {
        voted[idPlayer].gameObject.SetActive(true);
    }

    public void DisplayResults()
    {
        resultCanvas.SetActive(true);
        voteCanvas.SetActive(false);        
    }

    public void ResultsPosition(int boss, int score, int position)
    {
        string replace;
        resultCanvas.SetActive(true);
        string text = bestBossText.text;
        string textValue = resultsText.text;
        if (position == 1)
        {
            replace = "X";
        }
        else if (position == 2)
        {
            replace = "Y";
        }
        else
        {
            replace = "Z";
        }
        bestBossText.text = text.Replace(replace, boss.ToString());
        resultsText.text = textValue.Replace(replace, score.ToString());
    }

}
