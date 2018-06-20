using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    public AudioSource Sound;
    public AudioSource BackgroundSound;
    public Object LobbyBackground;
    public Object GameBackground;
    public Object EatFood;
    public Object TouchBigEnemy;
    public Object TouchSmallEnemy;
    public Object TouchWall;
    public Object SeaOut;
    public Object SeaIn;

    public void Play(string MusicName)
    {
        if(Sound.isPlaying){
            return ;
        }
        Debug.Log("Play Music: " + MusicName);
        Sound.clip = (AudioClip)Resources.Load(MusicName, typeof(AudioClip));
        Sound.Play(); 
    }

    public void PlayLobbyBackground()
    {
        string MusicName = LobbyBackground.name;
        Debug.Log("Play Music: " + MusicName);
        BackgroundSound.clip = (AudioClip)Resources.Load(MusicName, typeof(AudioClip));
        BackgroundSound.Play();
    }

    public void PlayGameBackground()
    {
        string MusicName = GameBackground.name;
        Debug.Log("Play Music: " + MusicName);
        BackgroundSound.clip = (AudioClip)Resources.Load(MusicName, typeof(AudioClip));
        BackgroundSound.Play();
    }

    public void PlayEatFood()
    {
        Play(EatFood.name);
    }

    public void PlayTouchBigEnemy()
    {
        Play(TouchBigEnemy.name);
    }

    public void PlayTouchSmallEnemy()
    {
        Play(TouchSmallEnemy.name);
    }

    public void PlayTouchWall()
    {
        Play(TouchWall.name);
    }

    public void PlaySeaOut()
    {
        Play(SeaOut.name);
    }

    public void PlaySeaIn()
    {
        Play(SeaIn.name);
    }



}
