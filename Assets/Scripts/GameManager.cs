using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public enum AracTipi { Hicbiri, Tirmik, Tohum, Su, Gunes, Hasat }
    public AracTipi seciliArac = AracTipi.Hicbiri;

    public enum BahceAsamasi { TirmikAsamasi, TohumAsamasi, SuAsamasi, GunesAsamasi, HasatAsamasi }
    public BahceAsamasi mevcutAsama = BahceAsamasi.TirmikAsamasi;

    public int tiklananTarlaSayisi = 0; 
    public List<Tile> tumTarlalar = new List<Tile>();

    [Header("UI Elemanları")]
    public GameObject bitisEkraniPaneli; 

    [Header("Oyun Bitiş ve Kalma Ayarları")]
    private int toplamTarlaSayisi = 9; 
    private int tarladakiIslemSayisi = 0;

    [Header("Geri Al (Undo) Mekanizması")]
    public Tile sonEtkilesimdekiTarla; 
    private int kullanilanGeriAlSayisi = 0; 
    private int undoCezaPuani = 0; 
    private const int BEDAVA_GERI_AL_LIMITI = 1; 

    [Header("Büyülü Dalga Prefab (Fairy Trail)")]
    public ParticleSystem periTozuTrailPrefab; 
   
    [Header("Güneş Aşaması UI")]
    public Slider gunesSurecSlider; 
    public float gunesSuresi = 5.0f;

    [Header("Tohum Seçim Hafızası (Renkler)")]
    public CicekTuru seciliTohum = CicekTuru.Hicbiri;

    private void Awake() 
    { 
        if (instance == null) instance = this; 
        Debug.Log("<color=cyan>Oyun Başladı!</color>");
        
        if (bitisEkraniPaneli != null) bitisEkraniPaneli.SetActive(false); 
        if (gunesSurecSlider != null) gunesSurecSlider.gameObject.SetActive(false); 
    }

    public void TarlayiListeyeEkle(Tile tarla) { if (!tumTarlalar.Contains(tarla)) tumTarlalar.Add(tarla); }

    // --- BUTON SESLERİ DOĞRUDAN FONKSİYONLARIN İÇİNE EKLENDİ ---

    public void TirmikSec() 
    { 
        if (SoundManager.instance != null) SoundManager.instance.ButonSesi();
        if (mevcutAsama == BahceAsamasi.TirmikAsamasi) seciliArac = AracTipi.Tirmik; 
    }
    
    public void TohumSec(int tohumIndeksi) 
    { 
        if (SoundManager.instance != null) SoundManager.instance.ButonSesi();
        if (mevcutAsama == BahceAsamasi.TohumAsamasi) 
        {
            seciliArac = AracTipi.Tohum; 
            seciliTohum = (CicekTuru)tohumIndeksi;
            Debug.Log("<color=yellow>Ekilmek üzere seçilen çiçek türü: </color>" + seciliTohum);
        }
    }
    
    public void SuSec() 
    { 
        if (SoundManager.instance != null) SoundManager.instance.ButonSesi();
        if (mevcutAsama == BahceAsamasi.SuAsamasi) seciliArac = AracTipi.Su; 
    }
    
    public void GunesSec() 
    { 
        if (mevcutAsama == BahceAsamasi.GunesAsamasi) 
        {
            StartCoroutine(GunesSurecAnimasoynu()); 
            if (SoundManager.instance != null) SoundManager.instance.GunesSesi();
        } 
    } 
    
    public void HasatSec() 
    { 
        if (SoundManager.instance != null) SoundManager.instance.ButonSesi();
        if (mevcutAsama == BahceAsamasi.HasatAsamasi) seciliArac = AracTipi.Hasat; 
    }

    public void TarlayaTiklandi()
    {
        // Tarlaya işlem uygulandığında çıkacak oyun içi efekt sesleri
        if (SoundManager.instance != null)
        {
            if (mevcutAsama == BahceAsamasi.TirmikAsamasi) SoundManager.instance.TirmikSesi();
            else if (mevcutAsama == BahceAsamasi.TohumAsamasi) SoundManager.instance.TohumSesi();
            else if (mevcutAsama == BahceAsamasi.SuAsamasi) SoundManager.instance.SuSesi(); 
            else if (mevcutAsama == BahceAsamasi.HasatAsamasi) SoundManager.instance.HasatSesi();
        }

        if (mevcutAsama == BahceAsamasi.HasatAsamasi) tarladakiIslemSayisi++;

        tiklananTarlaSayisi++;
        if (tiklananTarlaSayisi >= tumTarlalar.Count) 
        {
            tumTarlalar.Sort((a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));
            StartCoroutine(GercekPeriTozuDalgasi());
        }

        if (mevcutAsama == BahceAsamasi.HasatAsamasi && tarladakiIslemSayisi >= toplamTarlaSayisi)
        {
            OyunBitti();
        }
    }

    public void SonHamleyiKaydet(Tile tarla) { sonEtkilesimdekiTarla = tarla; }

    public void HamleyiGeriAl()
    {
        if (sonEtkilesimdekiTarla == null) return;
        if (mevcutAsama == BahceAsamasi.GunesAsamasi || mevcutAsama == BahceAsamasi.HasatAsamasi) return;

        sonEtkilesimdekiTarla.HamleyiGeriSar(mevcutAsama);
        
        if (SoundManager.instance != null) SoundManager.instance.ButonSesi();

        if (tiklananTarlaSayisi > 0) tiklananTarlaSayisi--;
        kullanilanGeriAlSayisi++;

        if (kullanilanGeriAlSayisi > BEDAVA_GERI_AL_LIMITI)
        {
            undoCezaPuani += 10;
            if (SoundManager.instance != null) SoundManager.instance.HataSesi();
        }

        sonEtkilesimdekiTarla = null;
    }

    public void OyunBitti()
    {
        int hamPuan = LevelManager.instance.ToplamPuaniHesapla();
        int nihaiPuan = Mathf.Max(0, hamPuan - undoCezaPuani);
        UIManager.instance.BitisEkraniniGoster(nihaiPuan);
        
        // Bölüm başarıyla bittiğinde tebrik/yıldız sesi
        if (SoundManager.instance != null) SoundManager.instance.YildizSesi();
    }

    private IEnumerator GercekPeriTozuDalgasi()
    {
        if (periTozuTrailPrefab == null || tumTarlalar.Count == 0) yield break;

        ParticleSystem periTozu = Instantiate(periTozuTrailPrefab, tumTarlalar[0].transform.position, Quaternion.identity);
        periTozu.Play();
        float hareketHizi = 15f; 

        for (int i = 0; i < tumTarlalar.Count; i++)
        {
            Tile hedefTarla = tumTarlalar[i];
            Vector3 hedefKonum = hedefTarla.transform.position;

            while (Vector3.Distance(periTozu.transform.position, hedefKonum) > 0.1f)
            {
                periTozu.transform.position = Vector3.MoveTowards(periTozu.transform.position, hedefKonum, hareketHizi * Time.deltaTime);
                yield return null; 
            }
            hedefTarla.DalgaUlasti(); 
        }

        periTozu.Stop();
        Destroy(periTozu.gameObject, 2f); 
        tiklananTarlaSayisi = 0; 
        AsamaGecisiniYap();
    }

    void AsamaGecisiniYap()
    {
        seciliArac = AracTipi.Hicbiri;
        seciliTohum = CicekTuru.Hicbiri; 
        foreach (var t in tumTarlalar) t.EtkilesimeAc(); 

        if (mevcutAsama == BahceAsamasi.TirmikAsamasi) mevcutAsama = BahceAsamasi.TohumAsamasi;
        else if (mevcutAsama == BahceAsamasi.TohumAsamasi) mevcutAsama = BahceAsamasi.SuAsamasi;
        else if (mevcutAsama == BahceAsamasi.SuAsamasi) mevcutAsama = BahceAsamasi.GunesAsamasi;
        else if (mevcutAsama == BahceAsamasi.HasatAsamasi)
        {
            if (bitisEkraniPaneli != null) bitisEkraniPaneli.SetActive(true);
        }
    }

    IEnumerator GunesSurecAnimasoynu()
    {
        seciliArac = AracTipi.Hicbiri; 
        if (gunesSurecSlider == null) yield break;

        gunesSurecSlider.gameObject.SetActive(true);
        gunesSurecSlider.value = 0;
        float gecenSure = 0;
        bool fidanOldu = false;

        while (gecenSure < gunesSuresi)
        {
            gecenSure += Time.deltaTime;
            float ilerlemeHizi = gecenSure / gunesSuresi;
            gunesSurecSlider.value = ilerlemeHizi; 

            if (!fidanOldu && ilerlemeHizi >= 0.4f)
            {
                fidanOldu = true;
                foreach (var t in tumTarlalar) t.FidanDurumunaGetir(); 
            }
            yield return null; 
        }

        foreach (var t in tumTarlalar) t.CicekDurumunaGetir();
        yield return new WaitForSeconds(1.0f); 

        gunesSurecSlider.gameObject.SetActive(false); 
        mevcutAsama = BahceAsamasi.HasatAsamasi;
        foreach (var t in tumTarlalar) t.EtkilesimeAc(); 
    }

    public void OyunuYenidenBaslat()
    {
        if (SoundManager.instance != null) SoundManager.instance.ButonSesi();

        if (bitisEkraniPaneli != null) bitisEkraniPaneli.SetActive(false);
        if (UIManager.instance != null)
        {
            if (UIManager.instance.gamePanel != null) UIManager.instance.gamePanel.SetActive(true);
            if (UIManager.instance.aracPaneli != null) UIManager.instance.aracPaneli.SetActive(true);
        }

        tiklananTarlaSayisi = 0;
        tarladakiIslemSayisi = 0; 
        undoCezaPuani = 0;
        kullanilanGeriAlSayisi = 0;
        sonEtkilesimdekiTarla = null;
        seciliArac = AracTipi.Hicbiri; 
        seciliTohum = CicekTuru.Hicbiri;
        mevcutAsama = BahceAsamasi.TirmikAsamasi; 
        foreach (var t in tumTarlalar) t.Sifirla(); 
        
        if (LevelManager.instance != null) LevelManager.instance.ReceteyiOlustur();
    }

    public void DerslereDon()
    {
        if (SoundManager.instance != null) SoundManager.instance.ButonSesi();

        PlayerPrefs.SetInt("DersPaneliniAc", 1);
        PlayerPrefs.Save(); 
        SceneManager.LoadScene("MapScene");
    }
}