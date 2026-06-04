using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("Audio Sources (Ses Çıkışları)")]
    public AudioSource bgmSource; // Müzik için
    public AudioSource sfxSource; // Efektler için

    [Header("Ses Dosyaları (Audio Clips)")]
    public AudioClip sesButon;
    public AudioClip sesTirmik;
    public AudioClip sesTohum;
    public AudioClip sesSu;
    public AudioClip sesGunes;
    public AudioClip sesHasat;
    public AudioClip sesYildiz;
    public AudioClip sesHata;

    void Awake()
    {
        // Sahne geçişlerinde sesin kesilmemesi ve tek bir merkez olması için Singleton yapısı
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void EfektCal(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    // Her yerden çağrılabilecek kolay tetikleyiciler
    public void ButonSesi() => EfektCal(sesButon);
    public void TirmikSesi() => EfektCal(sesTirmik);
    public void TohumSesi() => EfektCal(sesTohum);
    public void SuSesi()    => EfektCal(sesSu);
    public void GunesSesi()  => EfektCal(sesGunes);
    public void HasatSesi()  => EfektCal(sesHasat);
    public void YildizSesi() => EfektCal(sesYildiz);
    public void HataSesi()   => EfektCal(sesHata);
}