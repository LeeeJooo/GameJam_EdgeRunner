using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private List<AudioSource> sfx;
    [SerializeField] private List<AudioSource> bgm;

    public bool CanPlaySound = true;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySFX(int sfxIndex)
    {
        // if (sfx[sfxIndex].isPlaying) return;

        if (!CanPlaySound) return;

        if (sfxIndex < sfx.Count)
        {
            sfx[sfxIndex].pitch = Random.Range(.8f, 1.1f);
            sfx[sfxIndex].Play();
        }
    }

    public void StopSFX(int sfxIndex) => sfx[sfxIndex].Stop();

    public void PlayBGM(int bgmIndex)
    {   
        StopAllBGM();

        if (!CanPlaySound) return;

        bgm[bgmIndex].Play();
    }

    public void StopAllBGM()
    {
        for (int i = 0; i < bgm.Count; i++)
        {
            bgm[i].Stop();
        }
    }
}
