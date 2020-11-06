using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractableObjects : MonoBehaviour
{
    public int id;

    public bool isFailed = false;
    public bool hasStarted = false;
    public float timerMission = 3f;
    private float timerMissionLeft;
    public Player player;
    public SpriteRenderer needBoss;
    private List<int> playersNear;

    public GameObject holdButtonUI;
    public GameObject mashButtonUI;

    public GameObject missionUI = null;
    public GameObject canvas;

    // Start is called before the first frame update
    void Start()
    {
        //canvas = FindObjectOfType<CanvasScaler>().gameObject;
        id = Random.Range(1, 3);
        timerMissionLeft = timerMission;
        playersNear = new List<int>();
    }

    // Update is called once per frame
    void Update()
    {
        timerMissionLeft -= Time.deltaTime;
        if(timerMissionLeft <= 0 && !hasStarted)
        {
            hasStarted = false;
            player = null;
            if (isFailed)
            {
                GameManager.instance.RemoveMission(gameObject);
                AudioManager.instance.PlayMiss();
                Destroy(gameObject);
            }
            else
            {

                PlayerFailed();
            }
        }
    }

    public void PlayerFailed()
    {
        if (player != null && player.isBoss)
        {
            Destroy(gameObject);
        }
        needBoss.gameObject.SetActive(true);
        isFailed = true;
        timerMissionLeft = timerMission;
        hasStarted = false;
        player = null;
        Destroy(missionUI);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if (isFailed && !collision.GetComponent<Player>().isBoss)
            {
                return;
            }
            playersNear.Add(collision.GetComponent<Player>().GetId());
            if (hasStarted || missionUI != null)
            {
                return;
            }
            if(id == 1)
            {
                missionUI = Instantiate(holdButtonUI, transform.position, Quaternion.identity, collision.GetComponent<Player>().canvas.transform);
            }
            else if(id == 2)
            {
                missionUI = Instantiate(mashButtonUI, transform.position, Quaternion.identity, collision.GetComponent<Player>().canvas.transform);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && missionUI != null)
        {
            playersNear.Remove(collision.GetComponent<Player>().GetId());
            if(playersNear.Count == 0)
            {
                Destroy(missionUI);
            }            
        }
    }
}
