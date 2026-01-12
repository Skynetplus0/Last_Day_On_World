using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Background Music")]
    public AudioSource musicSource;
    public AudioClip backgroundMusic;
    public bool playMusicOnStart = true;
    [Range(0f, 1f)]
    public float musicVolume = 0.3f;

    [Header("Sound Effects")]
    public AudioSource sfxSource;
    
    [Header("Tower Clips")]
    public AudioClip shootSound;
    public AudioClip buildSound;
    
    [Header("Enemy Clips")]
    public AudioClip zombieSound;
    public AudioClip bossZombieSound;
    public AudioClip zombieDeathSound;
    public AudioClip bossDeathSound;
    
    [Header("Game Clips")]
    public AudioClip waveStartSound;
    public AudioClip waveCompleteSound;
    public AudioClip hitSound;
    
    [Header("Clip Settings")]
    public float shootSoundDuration = 0.5f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // SFX source yoksa olustur
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
        }
        
        // Music source yoksa olustur
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.playOnAwake = false;
            musicSource.loop = true;
        }
    }

    private void Start()
    {
        // Arka plan muzigini baslat
        if (playMusicOnStart && backgroundMusic != null)
        {
            PlayBackgroundMusic();
        }
    }
    
    // ============ BACKGROUND MUSIC ============
    
    public void PlayBackgroundMusic()
    {
        if (musicSource != null && backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.volume = musicVolume;
            musicSource.loop = true;
            musicSource.Play();
            Debug.Log("[SoundManager] Arka plan muzigi baslatildi");
        }
    }
    
    public void StopBackgroundMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }
    
    public void SetMusicVolume(float value)
    {
        musicVolume = value;
        if (musicSource != null)
            musicSource.volume = value;
    }

    // ============ SOUND EFFECTS ============

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip, volume);
        }
    }
    
    public void PlaySFXClipped(AudioClip clip, float duration, float volume = 1f)
    {
        if (sfxSource != null && clip != null)
        {
            StartCoroutine(PlayClippedSound(clip, duration, volume));
        }
    }
    
    private IEnumerator PlayClippedSound(AudioClip clip, float duration, float volume)
    {
        AudioSource tempSource = gameObject.AddComponent<AudioSource>();
        tempSource.clip = clip;
        tempSource.volume = volume;
        tempSource.time = 0f;
        tempSource.Play();
        
        yield return new WaitForSeconds(duration);
        
        tempSource.Stop();
        Destroy(tempSource);
    }

    // ============ SPECIFIC SOUNDS ============

    public void PlayShoot()
    {
        if (shootSound == null) return;
        
        if (shootSoundDuration > 0f && shootSoundDuration < shootSound.length)
        {
            PlaySFXClipped(shootSound, shootSoundDuration, 1.0f);
        }
        else
        {
            PlaySFX(shootSound, 1.0f);
        }
    }
    
    public void PlayZombieSpawn()
    {
        PlaySFX(zombieSound, 0.6f);
    }
    
    public void PlayBossZombieSpawn()
    {
        PlaySFX(bossZombieSound, 0.8f);
    }
    
    // Olum sesi cooldown (cok sik calmasin)
    private float lastZombieDeathTime = -10f;
    private float zombieDeathCooldown = 0.3f; // 0.3 saniyede 1 kez (daha sik)

    public void PlayZombieDeath()
    {
        // Cooldown kontrolu - 1 saniyede 1 kez
        if (Time.time - lastZombieDeathTime < zombieDeathCooldown) return;
        lastZombieDeathTime = Time.time;
        
        PlaySFX(zombieDeathSound, 0.7f);
    }
    
    public void PlayBossDeath()
    {
        PlaySFX(bossDeathSound, 0.9f);
    }

    public void PlayWaveStart()
    {
        PlaySFX(waveStartSound, 1f);
    }

    public void PlayWaveComplete()
    {
        PlaySFX(waveCompleteSound, 1f);
    }

    public void PlayBuild()
    {
        PlaySFX(buildSound, 0.8f);
    }

    public void PlayHit()
    {
        PlaySFX(hitSound, 0.6f);
    }
}
