using UnityEngine;

public class WeaponAudio : MonoBehaviour
{
    //molts dels sons
    public AudioClip attackSound; 
    private AudioSource audioSource;
    public AudioClip DashSound;
    public AudioClip WalkSound;
    public AudioClip Item;
    public AudioClip Solution;
    public AudioClip Damage;
    public AudioClip Death;
    public AudioClip Explode;
    public AudioClip BulletShot;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayWeaponSound() //ho crida la animacio de l'arma
    {
        if (audioSource != null && attackSound != null)
        {
            audioSource.PlayOneShot(attackSound);
        }
    }

    public void StartSound() //aquest es per el flamethrower
    {
        if (audioSource != null && attackSound != null && !audioSource.isPlaying)
        {
            // Reproduce el sonido una vez mientras se mantenga presionado
            audioSource.loop = true;  // Hacemos que el sonido sea repetitivo mientras esté presionado
            audioSource.PlayOneShot(attackSound);
        }
    }

    public void StopSound() //aquest es per el flamethrower
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    public void PlayPlayerSound(AudioClip playerSound) //els del player
    {
        if (audioSource != null && playerSound != null)
        {
            audioSource.PlayOneShot(playerSound);
        }
    }

    public void StartPlayerSound(AudioClip playerSound) //per caminar
    {
        if (audioSource != null && playerSound != null)
        {
            audioSource.loop = true;
            audioSource.PlayOneShot(playerSound);
        }
    }

    public void StopPlayerSound()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    //Enemics
    public void PlayBombSound()
    {
        if (audioSource != null && Explode != null)
        {
            audioSource.PlayOneShot(Explode);
        }
    }

    public void PlayWhipSound()
    {
        if (audioSource != null && DashSound != null)
        {
            audioSource.PlayOneShot(DashSound);
        }
    }

    public void PlayTurretSound()
    {
        if (audioSource != null && BulletShot != null)
        {
            audioSource.PlayOneShot(BulletShot);
        }
    }

}
