using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Sahneler arası geçiş için şart

public class HaritaKontrol : MonoBehaviour
{
    // Oyuncunun mini oyundan kazandığı ve telefona kaydedilen toplam puanı
    private int toplamPuan; 

    // Unity editöründen her binanın kilit görselini ve butonunu buraya sürükleyeceğiz
    [System.Serializable]
    public class AkademiKurulumu
    {
        public string akademiAdi;       // Hatırlatıcı isim (Örn: Simya)
        public int gerekenPuan;         // Kaç puanda açılsın?
        public GameObject kilitGorseli; // Üstteki dumanlı kilit resmi
        public Button butonComponent;  // Butonun kendisi
    }

    // Akademileri liste halinde tutacak kutu
    public AkademiKurulumu[] tumAkademiler;

    [Header("Arayüz Panelleri")]
    public GameObject dersSecmePaneli; // Sadece parşömen panelimiz kalıyor

    void Start()
    {
        PuanYukleVeGuncelle();
    }

    public void PuanYukleVeGuncelle()
    {
        toplamPuan = PlayerPrefs.GetInt("OyuncuPuani", 0);
        Debug.Log("Güncel Toplam Puan: " + toplamPuan); 

        KilitDurumlariniGuncelle();
    }

    void KilitDurumlariniGuncelle()
    {
        foreach (var akademi in tumAkademiler)
        {
            if (akademi == null || akademi.butonComponent == null) 
                continue;

            if (toplamPuan >= akademi.gerekenPuan)
            {
                if (akademi.kilitGorseli != null) 
                    akademi.kilitGorseli.SetActive(false); 

                akademi.butonComponent.interactable = true; 
            }
            else
            {
                if (akademi.kilitGorseli != null) 
                    akademi.kilitGorseli.SetActive(true);  

                akademi.butonComponent.interactable = false; 
            }
        }
    }

    // Haritadaki açık olan bir akademi binasına tıklanınca parşömeni açar
    public void PaneliAc()
    {
        if (dersSecmePaneli != null)
        {
            dersSecmePaneli.SetActive(true);
        }
    }

    // Parşömendeki Geri butonuna tıklanınca parşömeni kapatır (Haritayı gösterir)
    public void PaneliKapat()
    {
        Debug.Log("Parşömendeki Geri butonuna basıldı.");

        if (dersSecmePaneli != null)
        {
            dersSecmePaneli.SetActive(false);
            Debug.Log("Ders seçme paneli başarıyla kapatıldı.");
        }
        else
        {
            Debug.LogError("DİKKAT: 'Ders Secme Paneli' kutusu boş!");
        }
    }

    // Botanik dersinin butonuna tıklanınca direkt oyun sahnesini yükler
    public void BotanikOyununuAc()
    {
        Debug.Log("Botanik oyun sahnesi (GameScene) yükleniyor...");
        SceneManager.LoadScene("GameScene"); 
    }

    // TEST AMAÇLI HİLE BUTONLARI
    [ContextMenu("Puanı 1000 Yap ve Test Et")]
    public void TestPuanVer1000()
    {
        PlayerPrefs.SetInt("OyuncuPuani", 1000);
        PuanYukleVeGuncelle(); 
    }

    [ContextMenu("Puanı Sıfırla (0 Yap)")]
    public void TestPuanSifirla()
    {
        PlayerPrefs.SetInt("OyuncuPuani", 0);
        PuanYukleVeGuncelle(); 
    }
}