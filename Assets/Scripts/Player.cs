﻿using Rewired;
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
    public bool hasVoted;
    private bool canDoMission;
    private bool hasStartMission;
    public bool canMove = true;
    private InteractableObjects mission;

    public GameObject canvas;

    // Start is called before the first frame update
    void Start()
    {
        mainPlayer = ReInput.players.GetPlayer(idPlayer);
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.HasToVote() && !hasVoted)
        {
            if (mainPlayer.GetButtonDown("VotedA"))
            {
                hasVoted = true;
                GameManager.instance.Vote(10);
                UIManager.instance.VotePlayer(idPlayer);
            }
            else if (mainPlayer.GetButtonDown("VotedB"))
            {
                hasVoted = true;
                GameManager.instance.Vote(5);
                UIManager.instance.VotePlayer(idPlayer);
            }
            else if (mainPlayer.GetButtonDown("VotedX"))
            {
                hasVoted = true;
                GameManager.instance.Vote(-5);
                UIManager.instance.VotePlayer(idPlayer);
            }
            else if (mainPlayer.GetButtonDown("VotedY"))
            {
                hasVoted = true;
                GameManager.instance.Vote(-10);
                UIManager.instance.VotePlayer(idPlayer);
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
        }
        else if (!canMove)
        {
            moveSpeed = 0;
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
                Debug.Log("Coucou");                
                hasStartMission = true;
                canMove = false;
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
                    mission.missionUI.transform.GetChild(2).GetComponent<Image>().fillAmount += Time.deltaTime;
                    if (timeAtStartHold >= maxTimeHold)
                    {
                        hasStartMission = false;
                        canDoMission = false;
                        timeAtStartHold = 0;
                        canMove = true;
                        GameManager.instance.MissionDone(true, isBoss, idPlayer,mission.gameObject);
                        Destroy(mission.gameObject);
                        return;
                        //Completed
                    }
                }
                else if (mainPlayer.GetButtonUp("Submit"))
                {
                    timeAtStartHold = 0;
                    mission.missionUI.transform.GetChild(2).GetComponent<Image>().fillAmount = 0;
                    hasStartMission = false;
                    canMove = true;
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
                        hasStartMission = false;
                        canDoMission = false;
                        startMash = 0;
                        canMove = true;
                        GameManager.instance.MissionDone(true, isBoss, idPlayer, mission.gameObject);
                        Destroy(mission.gameObject);
                        return;
                        //Completed
                    }
                }
                else if (startMash > 0)
                {
                    startMash -= Time.deltaTime * 0.5f;
                }
                else if (startMash <= 0)
                {
                    startMash = 0;
                }
                mission.missionUI.transform.GetChild(2).GetComponent<Image>().fillAmount = startMash / maxTimesToMash;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactable"))
        {
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
}
