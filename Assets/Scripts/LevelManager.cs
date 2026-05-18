using UnityEngine;
using System.Collections.Generic;

public enum CicekTuru { Kirmizi, Mavi, Yesil, Mor, Sari, Pembe, Hicbiri }

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [Header("Mira'nın Reçetesi")]
    public Dictionary<CicekTuru, int> recete = new Dictionary<CicekTuru, int>();
    public Dictionary<CicekTuru, int> ekilenler = new Dictionary<CicekTuru, int>();
    public int undoCezaPuani = 0;

    void Awake() 
    { 
        instance = this; 
    }

    void Start()
    {
        ReceteyiOlustur();
    }

    public void ReceteyiOlustur()
    {
        // Eski hafızayı tamamen temizle
        recete.Clear();
        ekilenler.Clear();

        // 6 Rengi de sisteme sıfır (0) olarak tanıt (Burası şart, yoksa kod patlar)
        recete[CicekTuru.Kirmizi] = 0; recete[CicekTuru.Mavi] = 0; recete[CicekTuru.Yesil] = 0;
        recete[CicekTuru.Mor] = 0; recete[CicekTuru.Sari] = 0; recete[CicekTuru.Pembe] = 0;

        ekilenler[CicekTuru.Kirmizi] = 0; ekilenler[CicekTuru.Mavi] = 0; ekilenler[CicekTuru.Yesil] = 0;
        ekilenler[CicekTuru.Mor] = 0; ekilenler[CicekTuru.Sari] = 0; ekilenler[CicekTuru.Pembe] = 0;

        // SENİN ORİJİNAL RASTGELE DAĞITIM KODUN (Geri getirdik!)
        int toplamTarla = 9;
        for (int i = 0; i < toplamTarla; i++)
        {
            CicekTuru rastgeleTur = (CicekTuru)Random.Range(0, 6); 
            recete[rastgeleTur]++;
        }

        // Ekranı Güncelle
        if (UIManager.instance != null)
        {
            UIManager.instance.ParsomeniGuncelle();
        }
    }
  public int ToplamPuaniHesapla()
{
    int temelPuan = 0;
    foreach (var tur in recete.Keys)
    {
        int kazanilan = Mathf.Min(ekilenler[tur], recete[tur]);
        temelPuan += kazanilan * 10;
    }

    // Ceza puanını düşüyoruz ama puanın 0'ın altına inmesini engelliyoruz
    int finalPuan = temelPuan - undoCezaPuani;
    return Mathf.Max(0, finalPuan);
}
}