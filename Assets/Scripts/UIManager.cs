using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public static UIManager instance;

    public List<TMP_Text> scores;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
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
    }
}
