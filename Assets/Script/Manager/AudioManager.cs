using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    public AudioSource Sound;
    public AudioSource BackgroundSound;
    public AudioSource SkillSound;

    public Object LobbyBackground;
    public Object GameBackground;
    public Object EatFood;
    public Object EatPoison;
    public Object TouchBigEnemy;
    public Object TouchSmallEnemy;
    public Object TouchWall;
    public Object SeaOut;
    public Object SeaIn;

    public Object SizeSkill;
    public Object SpeedSkill;
    public Object CopySkill;
    public Object HideSkill;

    public void Play(string MusicName)
    {

        MusicName = "Music/" + MusicName;
        if(Sound.isPlaying){
            return ;
        }
        Debug.Log("Play Music: " + MusicName);
        Sound.clip = (AudioClip)Resources.Load(MusicName, typeof(AudioClip));
        Sound.Play();
    }
    public void PlaySkill(string MusicName)
    {
        MusicName = "Music/" + MusicName;
        if (Sound.isPlaying)
        {
            return;
        }
        Debug.Log("Play Music: " + MusicName);
        SkillSound.clip = (AudioClip)Resources.Load(MusicName, typeof(AudioClip));
        SkillSound.Play();
    }

    public void PlayLobbyBackground()
    {
        string MusicName = "Music/" + LobbyBackground.name;
        Debug.Log("Play Music: " + MusicName);
        BackgroundSound.clip = (AudioClip)Resources.Load(MusicName, typeof(AudioClip));
        BackgroundSound.Play();
    }

    public void PlayGameBackground()
    {
        string MusicName = "Music/" + GameBackground.name;
        Debug.Log("Play Music: " + MusicName);
        BackgroundSound.clip = (AudioClip)Resources.Load(MusicName, typeof(AudioClip));
        BackgroundSound.Play();
    }


    public void PlayEatFood()
    {
        Play(EatFood.name);
    }

    public void PlayEatPoison()
    {
        Play(EatPoison.name);
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
    

    public void PlaySizeSkill()
    {
        PlaySkill(SizeSkill.name);
    }

    public void PlaySpeedSkill()
    {
        PlaySkill(SpeedSkill.name);
    }

    public void PlayCopySkill()
    {
        PlaySkill(CopySkill.name);
    }

    public void PlayHideSkill()
    {
        PlaySkill(HideSkill.name);
    }

    public void StopSpeedSkill()
    {
        SkillSound.Stop();
    }

}
