using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

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
    public Tile sonEtkilesimdekiTarla; // Oyuncunun dokunduğu son tarla kutusu
    private int kullanilanGeriAlSayisi = 0; // Kaç kez geri al butonuna basıldı
    private int undoCezaPuani = 0; // Toplamda düşülecek ceza puanı
    private const int BEDAVA_GERI_AL_LIMITI = 1; // 1 kullanım hakkı bedava, sonrakiler ceza keser

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

    public void TirmikSec() { if (mevcutAsama == BahceAsamasi.TirmikAsamasi) seciliArac = AracTipi.Tirmik; }
    
    public void TohumSec(int tohumIndeksi) 
    { 
        if (mevcutAsama == BahceAsamasi.TohumAsamasi) 
        {
            seciliArac = AracTipi.Tohum; 
            seciliTohum = (CicekTuru)tohumIndeksi;
            Debug.Log("<color=yellow>Ekilmek üzere seçilen çiçek türü: </color>" + seciliTohum);
        }
    }
    
    public void SuSec() { if (mevcutAsama == BahceAsamasi.SuAsamasi) seciliArac = AracTipi.Su; }
    
    public void GunesSec() { if (mevcutAsama == BahceAsamasi.GunesAsamasi) StartCoroutine(GunesSurecAnimasoynu()); } 
    
    public void HasatSec() { if (mevcutAsama == BahceAsamasi.HasatAsamasi) seciliArac = AracTipi.Hasat; }

    public void TarlayaTiklandi()
    {
        // Eğer hasat aşamasındaysak, her tıklamada hasat edilen tarla sayısını artır
        if (mevcutAsama == BahceAsamasi.HasatAsamasi)
        {
            tarladakiIslemSayisi++;
        }

        tiklananTarlaSayisi++;
        if (tiklananTarlaSayisi >= tumTarlalar.Count) 
        {
            tumTarlalar.Sort((a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));
            StartCoroutine(GercekPeriTozuDalgasi());
        }

        // Hasat bittiğinde çalışacak kısım
        if (mevcutAsama == BahceAsamasi.HasatAsamasi && tarladakiIslemSayisi >= toplamTarlaSayisi)
        {
            OyunBitti();
        }
    }

    // Tile.cs içerisinden son tıklanan tarlayı bu fonksiyonla kaydedeceğiz
    public void SonHamleyiKaydet(Tile tarla)
    {
        sonEtkilesimdekiTarla = tarla;
    }

    // Sahnede oluşturacağın Geri Al butonuna bu fonksiyonu bağlayacaksın
    public void HamleyiGeriAl()
    {
        if (sonEtkilesimdekiTarla == null) return;

        // Güneş animasyonu sırasında veya Hasat bittikten sonra geri alma yapılamasın
        if (mevcutAsama == BahceAsamasi.GunesAsamasi || mevcutAsama == BahceAsamasi.HasatAsamasi) return;

        // Tarlanın kendi içindeki görsel ve veri durumunu bir önceki aşamaya sarıyoruz
        sonEtkilesimdekiTarla.HamleyiGeriSar(mevcutAsama);

        // Fazladan tıklama sayacını düşüyoruz ki peri tozu dalgası erken tetiklenmesin
        if (tiklananTarlaSayisi > 0) tiklananTarlaSayisi--;

        kullanilanGeriAlSayisi++;

        // Eğer bedava geri alma limiti (1) aşılırsa her geri al işleminde skordan 10 puan düşer
        if (kullanilanGeriAlSayisi > BEDAVA_GERI_AL_LIMITI)
        {
            undoCezaPuani += 10;
            Debug.Log("<color=red>Geri Al Cezası! Güncel Kesinti: </color>" + undoCezaPuani);
        }

        // Aynı hamlenin üst üste mükerrer geri alınmasını engellemek için hafızayı temizle
        sonEtkilesimdekiTarla = null;
    }

    public void OyunBitti()
    {
        int hamPuan = LevelManager.instance.ToplamPuaniHesapla();
        
        // Geri almalardan gelen cezayı nihai puandan düşüyoruz (0'ın altına inemez)
        int nihaiPuan = Mathf.Max(0, hamPuan - undoCezaPuani);
        
        // UIManager'a nihai puanı gönderiyoruz, yıldızları o hesaplayıp gösterecek
        UIManager.instance.BitisEkraniniGoster(nihaiPuan);
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

        Debug.Log("<color=yellow>Güneş süreci başladı!</color>");
        
        gunesSurecSlider.gameObject.SetActive(true);
        gunesSurecSlider.value = 0;

        float gecenSure = 0;
        bool fidanOldu = false;

        while (gecenSure < gunesSuresi)
        {
            gecenSure += Time.deltaTime;
            float ilerlemeHizi = gecenSure / gunesSuresi;
            gunesSurecSlider.value = ilerlemeHizi; 

            // Güneş barı %40 (0.4) seviyesine geldiğinde fidanları gösterelim
            if (!fidanOldu && ilerlemeHizi >= 0.4f)
            {
                fidanOldu = true;
                // DÜZELTME: Tüm tarlaları Fidan durumuna getiriyoruz
                foreach (var t in tumTarlalar) t.FidanDurumunaGetir(); 
                Debug.Log("<color=orange>Bitkiler fidan oldu.</color>");
            }

            yield return null; 
        }

        // --- GÜNEŞ SÜRECİ BİTTİ (Bar Doldu) ---

        // DÜZELTME 1: BuyumeyiGoster(false) yapan hatalı satırı sildik.
        // DÜZELTME 2: Tüm tarlaları artık fidan aşamasından renkli çiçek aşamasına geçiriyoruz.
        foreach (var t in tumTarlalar) t.CicekDurumunaGetir();
        
        Debug.Log("<color=green>Güneş süreci bitti! Hasat hazır.</color>");

        yield return new WaitForSeconds(1.0f); 

        gunesSurecSlider.gameObject.SetActive(false); 
        mevcutAsama = BahceAsamasi.HasatAsamasi;
        foreach (var t in tumTarlalar) t.EtkilesimeAc(); 
    }

    public void OyunuYenidenBaslat()
    {
        if (bitisEkraniPaneli != null) bitisEkraniPaneli.SetActive(false);
        
        if (UIManager.instance != null)
        {
            if (UIManager.instance.gamePanel != null) UIManager.instance.gamePanel.SetActive(true);
            if (UIManager.instance.aracPaneli != null) UIManager.instance.aracPaneli.SetActive(true);
        }

        // Yeni tura başlarken tüm durumları ve cezaları sıfırlıyoruz
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
}