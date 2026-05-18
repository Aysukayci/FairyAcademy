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
        tiklananTarlaSayisi++;
        if (tiklananTarlaSayisi >= tumTarlalar.Count) 
        {
            tumTarlalar.Sort((a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));
            StartCoroutine(GercekPeriTozuDalgasi());
        }
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

            if (!fidanOldu && ilerlemeHizi >= 0.4f)
            {
                fidanOldu = true;
                foreach (var t in tumTarlalar) t.BuyumeyiGoster(true); 
                Debug.Log("Bitkiler fidan oldu.");
            }

            yield return null; 
        }

        gunesSurecSlider.value = 0.8f; 
        foreach (var t in tumTarlalar) t.BuyumeyiGoster(false); 
        
        Debug.Log("<color=green>Güneş süreci bitti! Hasat hazır.</color>");

        yield return new WaitForSeconds(1.0f); 

        gunesSurecSlider.gameObject.SetActive(false); 
        mevcutAsama = BahceAsamasi.HasatAsamasi;
        foreach (var t in tumTarlalar) t.EtkilesimeAc(); 
    }

    public void OyunuYenidenBaslat()
    {
        if (bitisEkraniPaneli != null) bitisEkraniPaneli.SetActive(false);
        tiklananTarlaSayisi = 0;
        seciliArac = AracTipi.Hicbiri; 
        seciliTohum = CicekTuru.Hicbiri;
        mevcutAsama = BahceAsamasi.TirmikAsamasi; 
        foreach (var t in tumTarlalar) t.Sifirla(); 
        
        if (LevelManager.instance != null) LevelManager.instance.ReceteyiOlustur();
    }
}