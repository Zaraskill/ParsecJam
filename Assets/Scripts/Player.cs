using Rewired;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Component")]
    private Rigidbody2D _rigidbody;
    Vector2 movement;

    private Rewired.Player mainPlayer;

    public float maxTimeHold;
    private float timeAtStartHold = 0;

    public float maxTimesToMash;
    private float startMash = 0;

    // Start is called before the first frame update
    void Start()
    {
        mainPlayer = ReInput.players.GetPlayer("Player0");
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        movement.x = mainPlayer.GetAxis("HorizontalMove");
        movement.y = mainPlayer.GetAxis("VerticalMove");
    }

    private void FixedUpdate()
    {
        _rigidbody.MovePosition(_rigidbody.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactable"))
        {
            if(mainPlayer.GetButtonDown("Submit"))
            {
                // Lancement mission
                Debug.Log("Coucou");

                //Hold Button
                if(collision.GetComponent<InteractableObjects>().id == 1)
                {
                    //DisplayUI
                    if (mainPlayer.GetButton("Submit"))
                    {
                        timeAtStartHold += Time.deltaTime;
                        if(timeAtStartHold >= maxTimeHold)
                        {
                            //Completed
                        }
                    }
                    else if (mainPlayer.GetButtonUp("Submit"))
                    {
                        timeAtStartHold = 0;
                    }
                }

                //Mash Button
                if(collision.GetComponent<InteractableObjects>().id == 2)
                {
                    //DisplayUI
                    if (mainPlayer.GetButtonDown("Submit"))
                    {
                        startMash++;
                        if(startMash >= maxTimesToMash)
                        {
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
                if(collision.GetComponent<InteractableObjects>().id == 3)
                {

                }
            }
        }
    }
}
