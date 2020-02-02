using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour 
{
    public enum Music
    {
        MENU,
        AMBIANCE,
        WAVE,

        COUNT
    }

    public enum SoundEffect
    {
        EXPLOSION,
        MECH_FIRE,
        REPAIR_COMPLETE,
        REPAIRING,
        TURRET_FIRE,
        WAVE_END,
        WAVE_START,
        CORE,

        COUNT
    }


	static AudioManager                     _instance;


    Dictionary<Music, AudioClip>            _music;
    Dictionary<SoundEffect, AudioClip>      _soundEffects;

    AudioSource[]                           _soundEffectSources;
    AudioSource                             _loopingSoundEffectSource;
    AudioSource                             _musicSource;

    float                                   _volumeMusic;
    float                                   _volumeEffects;

    bool                                    _muteMusic;
    bool                                    _muteSoundEffects;

    const int                               AUDIO_SOURCES = 10;


    static public AudioManager Instance()
    {
        if (_instance == null)
        {
            _instance = new AudioManager();
            _instance.CreateAudioManager();
        }

        return _instance;
    }

    void CreateAudioManager()
    {//Create audio manager
        GameObject audioManager = new GameObject("AudioManager");
        
        //add AudioManager to the object
        _instance = audioManager.AddComponent<AudioManager>() as AudioManager;
        
        //stop it from destroying itself
        DontDestroyOnLoad(audioManager);
        
        //initialize that object
        _instance.Initialize();
    }

    void Initialize()
    {
        _muteMusic = false;
        _muteSoundEffects = false;

        _music = new Dictionary<Music, AudioClip>((int)Music.COUNT);
        _soundEffects = new Dictionary<SoundEffect, AudioClip>((int)SoundEffect.COUNT);

        LoadAudioSources();
        LoadContent();
        PrepareMusicVolume();
        PrepareEffectsVolume();
    }

    /// <summary>
    /// Create the neccessary audio sources.
    /// </summary>
    void LoadAudioSources()
    {
        _soundEffectSources = new AudioSource[AUDIO_SOURCES-2];

        for (int i = 0; i < AUDIO_SOURCES; i++)
        {//adds audio source componenets to the object
            gameObject.AddComponent<AudioSource>();
        }

        AudioSource[] sources = GetComponents<AudioSource>();

        int size = _soundEffectSources.Length;
        int index = 0;

        for (; index < size; index++)
        {//sets sound effect sources
            _soundEffectSources[index] = sources[index];
            _soundEffectSources[index].playOnAwake = false;
            _soundEffectSources[index].loop = false;
        }

        //sets a music source
        _musicSource = sources[index++];
        _musicSource.playOnAwake = true;
        _musicSource.loop = true;

        //sets a looping sound effect source
        _loopingSoundEffectSource = sources[index++];
        _loopingSoundEffectSource.playOnAwake = false;
        _loopingSoundEffectSource.loop = false;
    }

    /// <summary>
    /// Load all of the audio content
    /// </summary>
    void LoadContent()
    {
        // music
        _music.Add(Music.MENU, Resources.Load<AudioClip>("Audio/MenuTheme"));
        _music.Add(Music.AMBIANCE, Resources.Load<AudioClip>("Audio/Ambience"));
        _music.Add(Music.WAVE, Resources.Load<AudioClip>("Audio/UnderAttack"));

        // sound effects
        _soundEffects.Add(SoundEffect.EXPLOSION, Resources.Load<AudioClip>("Audio/ExplosionFX"));
        _soundEffects.Add(SoundEffect.MECH_FIRE, Resources.Load<AudioClip>("Audio/NPC_Fire"));
        _soundEffects.Add(SoundEffect.REPAIR_COMPLETE, Resources.Load<AudioClip>("Audio/RepairComplete"));
        _soundEffects.Add(SoundEffect.REPAIRING, Resources.Load<AudioClip>("Audio/RepairFX"));
        _soundEffects.Add(SoundEffect.TURRET_FIRE, Resources.Load<AudioClip>("Audio/TurretFire"));
        _soundEffects.Add(SoundEffect.WAVE_END, Resources.Load<AudioClip>("Audio/WaveEnd"));
        _soundEffects.Add(SoundEffect.WAVE_START, Resources.Load<AudioClip>("Audio/WaveStart"));
        _soundEffects.Add(SoundEffect.CORE, Resources.Load<AudioClip>("Audio/CoreFX"));
    }
    

    private void PrepareMusicVolume()
    {
        //string playerPref = EnumHolder.PlayerPref.VOLUME_MUSIC.ToString();
        float volume = 0.35f;

        //if (PlayerPrefs.HasKey(playerPref))
        //    volume = PlayerPrefs.GetFloat(playerPref);

        SetMusicVolume(volume);
    }

    private void PrepareEffectsVolume()
    {
        //string playerPref = EnumHolder.PlayerPref.VOLUME_EFFECTS.ToString();
        float volume = 1.0f;

       // if (PlayerPrefs.HasKey(playerPref))
        //    volume = PlayerPrefs.GetFloat(playerPref);

        SetEffectsVolume(volume);
    }


    /// <summary>
    /// Player specific music (song)
    /// </summary>
    /// <param name="music"></param>
    public void PlayMusic(Music music)
    {
        if (!_muteMusic)
        {//if music isn't muted
            if (_musicSource.clip != _music[music])
            {//if the music clip isn't already playing
                _musicSource.Stop();
                _musicSource.clip = _music[music];
                _musicSource.Play();
            }
        }
    }

    /// <summary>
    /// Play a specific sound effect.
    /// </summary>
    /// <param name="soundEffect"></param>
    public void PlaySoundEffect(SoundEffect soundEffect)
    {
        if (!_muteSoundEffects)
        {// if not muted
            for (int i = 0; i < _soundEffectSources.Length; i++)
            {//look for an avaialable channel
                if (!_soundEffectSources[i].isPlaying)
                {//if its free play the sound effect
                    _soundEffectSources[i].PlayOneShot(_soundEffects[soundEffect]);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Play a specific sound effect.
    /// </summary>
    /// <param name="soundEffect"></param>
    public void PlaySoundEffect(SoundEffect soundEffect, float volume)
    {
        if (!_muteSoundEffects)
        {// if not muted
            for (int i = 0; i < _soundEffectSources.Length; i++)
            {//look for an avaialable channel
                if (!_soundEffectSources[i].isPlaying)
                {//if its free play the sound effect
                    _soundEffectSources[i].PlayOneShot(_soundEffects[soundEffect], volume);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// mutes all of the auio
    /// </summary>
    /// <param name="mute"></param>
    public void Mute(bool mute)
    {
        _muteMusic = mute;
        _muteSoundEffects = mute;
        AudioListener.pause = mute;
    }

    public void SetMusicVolume(float value)
    {
        _volumeMusic = value;

        _musicSource.volume = _volumeMusic;

        if (_volumeMusic == 0)
            _muteMusic = true;
        else if (_muteMusic)
            _muteMusic = false;
    }

    public void SetEffectsVolume(float value)
    {
        _volumeEffects = value;

        for (int i = 0; i < _soundEffectSources.Length; i++)
        {
            _soundEffectSources[i].volume = _volumeEffects;
        }

        if (_volumeEffects == 0)
            _muteSoundEffects = true;
        else if (_muteSoundEffects)
            _muteSoundEffects = false;
    }


    public float VolumeMusic
    {
        get { return _volumeMusic; }
    }

    public float VolumeEffects
    {
        get { return _volumeEffects; }
    }
}
