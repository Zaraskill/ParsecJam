﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractableObjects : MonoBehaviour
{
    public int id;
    public GameObject holdButtonUI;
    public GameObject mashButtonUI;

    public GameObject missionUI = null;
    public GameObject canvas;

    // Start is called before the first frame update
    void Start()
    {
        //canvas = FindObjectOfType<CanvasScaler>().gameObject;
        id = Random.Range(1, 3);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
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
            Destroy(missionUI);
        }
    }
}
