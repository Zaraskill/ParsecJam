using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public static AudioManager instance;

    public AudioSource source;
    public AudioClip taskFail;
    public AudioClip taskMissed;
    public AudioClip taskValid;
    public AudioClip vote;
    public AudioClip menuButton;
    public AudioClip results;
    public AudioClip musicMenu;
    public AudioClip musicGame;

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
        PlayMenu();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void PlayClip(AudioClip clip)
    {
        AudioSource.PlayClipAtPoint(clip, transform.position);
    }

    public void PlayFail()
    {
        PlayClip(taskFail);
    }

    public void PlayMiss()
    {
        PlayClip(taskMissed);
    }

    public void PlayValid()
    {
        PlayClip(taskValid);
    }

    public void PlayVote()
    {
        PlayClip(vote);
    }

    public void PlayButton()
    {
        PlayClip(menuButton);
    }

    public void PlayResults()
    {
        PlayClip(results);
    }

    public void PlayMenu()
    {
        source.Stop();
        source.clip = musicMenu;
        source.Play();
    }

    public void PlayGame()
    {
        source.Stop();
        source.clip = musicGame;
        source.Play();
    }
}
