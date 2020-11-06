using Rewired;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private int idPlayer;

    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Component")]
    private Rigidbody2D _rigidbody;
    Vector2 movement;

    private Rewired.Player mainPlayer;

    public float maxTimeHold;
    public float timeAtStartHold = 0;

    public float maxTimesToMash;
    public float startMash = 0;

    public bool isBoss;
    public GameObject bossAttitude;
    public bool hasVoted;
    private bool canDoMission;
    private bool hasStartMission;
    public bool canMove = true;
    private InteractableObjects mission;

    private Animator _animator;
    public GameObject canvas;

    // Start is called before the first frame update
    void Start()
    {
        mainPlayer = ReInput.players.GetPlayer(idPlayer);
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.HasToVote() && !hasVoted && GameManager.instance.timeBeforeVoteLeft < 0)
        {
            if (GameManager.instance.ScorePlayer(idPlayer) < 4)
            {
                hasVoted = true;
                GameManager.instance.Vote(5);
                UIManager.instance.VotePlayer(idPlayer);
                AudioManager.instance.PlayVote();
            }
            if (mainPlayer.GetButtonDown("VotedA"))
            {
                if(GameManager.instance.ScorePlayer(idPlayer) >= 4)
                {
                    hasVoted = true;
                    GameManager.instance.Vote(10);
                    UIManager.instance.VotePlayer(idPlayer);
                    AudioManager.instance.PlayVote();
                }                
            }
            else if (mainPlayer.GetButtonDown("VotedB"))
            {
                if (GameManager.instance.ScorePlayer(idPlayer) >= 5)
                {
                    hasVoted = true;
                    GameManager.instance.Vote(5);
                    UIManager.instance.VotePlayer(idPlayer);
                    AudioManager.instance.PlayVote();
                }                    
            }
            else if (mainPlayer.GetButtonDown("VotedY"))
            {
                if (GameManager.instance.ScorePlayer(idPlayer) >= 10)
                {
                    hasVoted = true;
                    GameManager.instance.Vote(-5);
                    UIManager.instance.VotePlayer(idPlayer);
                    AudioManager.instance.PlayVote();
                }
            }
            else if (mainPlayer.GetButtonDown("VotedX"))
            {
                if (GameManager.instance.ScorePlayer(idPlayer) >= 15)
                {
                    hasVoted = true;
                    GameManager.instance.Vote(-10);
                    UIManager.instance.VotePlayer(idPlayer);
                    AudioManager.instance.PlayVote();
                }
            }
        }
        if (canDoMission)
        {
            CheckMission();
        }
        if (canMove)
        {
            moveSpeed = 5f;
            movement.x = mainPlayer.GetAxis("HorizontalMove");
            movement.y = mainPlayer.GetAxis("VerticalMove");
            _animator.SetFloat("Speed", Mathf.Abs(movement.x));
        }
        else if (!canMove)
        {
            moveSpeed = 0;
        }

        if (movement.x > 0)
        {
            transform.localScale = new Vector3(-0.2f, 0.2f, 1f);
        }
        else if(movement.x < 0)
        {
            transform.localScale = new Vector3(0.2f, 0.2f, 1f);
        }
    }

    private void FixedUpdate()
    {
        _rigidbody.MovePosition(_rigidbody.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    private void CheckMission()
    {
        if (!hasStartMission)
        {
            if (mainPlayer.GetButtonDown("Submit"))
            {
                if (mission.hasStarted)
                {
                    return;
                }
                mission.hasStarted = true;
                mission.player = this;
                Debug.Log("Coucou");
                _animator.SetBool("Tasking", true);
                hasStartMission = true;
                canMove = false;
                _rigidbody.bodyType = RigidbodyType2D.Kinematic;
            }
        }
        else
        {
            //Hold Button
            if (mission.id == 1)
            {
                //DisplayUI
                if (mainPlayer.GetButton("Submit"))
                {
                    timeAtStartHold += maxTimeHold * Time.deltaTime;
                    mission.missionUI.transform.GetChild(3).GetComponent<Image>().fillAmount += Time.deltaTime;
                    if (timeAtStartHold >= maxTimeHold)
                    {
                        _animator.SetBool("Tasking", false);
                        hasStartMission = false;
                        canDoMission = false;
                        timeAtStartHold = 0;
                        canMove = true;
                        _rigidbody.bodyType = RigidbodyType2D.Dynamic;
                        GameManager.instance.MissionDone(true, isBoss, idPlayer,mission.gameObject);
                        Destroy(mission.missionUI);
                        Destroy(mission.gameObject);
                        return;
                        //Completed
                    }
                }
                else if (mainPlayer.GetButtonUp("Submit"))
                {
                    GameManager.instance.MissionDone(false, isBoss, idPlayer, mission.gameObject);
                    timeAtStartHold = 0;
                    mission.missionUI.transform.GetChild(3).GetComponent<Image>().fillAmount = 0;
                    _animator.SetBool("Tasking", false);
                    mission.PlayerFailed();
                    mission = null;
                    hasStartMission = false;
                    canMove = true;
                    _rigidbody.bodyType = RigidbodyType2D.Dynamic;
                }
            }

            //Mash Button
            else if (mission.id == 2)
            {
                //DisplayUI
                if (mainPlayer.GetButtonDown("Submit"))
                {
                    startMash++;
                    if (startMash >= maxTimesToMash)
                    {
                        _animator.SetBool("Tasking", false);
                        hasStartMission = false;
                        canDoMission = false;
                        startMash = 0;
                        canMove = true;
                        _rigidbody.bodyType = RigidbodyType2D.Dynamic;
                        GameManager.instance.MissionDone(true, isBoss, idPlayer, mission.gameObject);
                        Destroy(mission.missionUI);
                        Destroy(mission.gameObject);                        
                        return;
                        //Completed
                    }
                }
                else if (startMash > 0)
                {
                    startMash -= Time.deltaTime * 0.8f;
                }
                else if (startMash < 0)
                {
                    GameManager.instance.MissionDone(false, isBoss, idPlayer, mission.gameObject);
                    startMash = 0;
                    _animator.SetBool("Tasking", false);
                    mission.PlayerFailed();
                    mission = null;
                    hasStartMission = false;
                    canMove = true;
                    _rigidbody.bodyType = RigidbodyType2D.Dynamic;
                }
                mission.missionUI.transform.GetChild(3).GetComponent<Image>().fillAmount = startMash / maxTimesToMash;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactable"))
        {
            if(collision.GetComponent<InteractableObjects>().isFailed && !isBoss)
            {
                return;
            }
            canDoMission = true;
            mission = collision.GetComponent<InteractableObjects>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactable"))
        {
            canDoMission = false;
            mission = null;
        }
    }

    public int GetId()
    {
        return idPlayer;
    }

    public void BossAttitude()
    {
        if (isBoss)
        {
            bossAttitude.SetActive(true);
        }
        else
        {
            bossAttitude.SetActive(false);
        }
    }

    public void DynamicPlayer()
    {
        _rigidbody.bodyType = RigidbodyType2D.Dynamic;
        _animator.SetBool("Tasking", false);
    }
}
