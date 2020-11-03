using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Component")]
    private Rigidbody2D _rigidbody;
    Vector2 movement;

    private Rewired.Player mainPlayer;

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
            }
        }
    }
}
