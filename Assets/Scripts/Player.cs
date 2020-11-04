using Rewired;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

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
    private bool canDoMission;
    private bool hasStartMission;
    private bool canMove = true;
    private InteractableObjects mission;

    // Start is called before the first frame update
    void Start()
    {
        mainPlayer = ReInput.players.GetPlayer(idPlayer);
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (canDoMission)
        {
            CheckMission();
        }
        if (canMove)
        {
            movement.x = mainPlayer.GetAxis("HorizontalMove");
            movement.y = mainPlayer.GetAxis("VerticalMove");
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
                    timeAtStartHold += Time.deltaTime;
                    if (timeAtStartHold >= maxTimeHold)
                    {
                        hasStartMission = false;
                        canDoMission = false;
                        timeAtStartHold = 0;
                        canMove = true;
                        GameManager.instance.MissionDone(true, isBoss, idPlayer);
                        //Completed
                    }
                }
                else if (mainPlayer.GetButtonUp("Submit"))
                {
                    timeAtStartHold = 0;
                }
            }

            //Mash Button
            if (mission.id == 2)
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
                        GameManager.instance.MissionDone(true, isBoss, idPlayer);
                        //Completed
                    }
                }
                else if (startMash > 0)
                {
                    startMash -= Time.deltaTime;
                }
                else if (startMash <= 0)
                {
                    startMash = 0;
                }
            }

            //Input Suite
            if (mission.id == 3)
            {

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
