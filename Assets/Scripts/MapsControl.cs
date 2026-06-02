using UnityEngine;
using UnityEngine.UI;

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

    void Start()
    {
        PuanYukleVeGuncelle();
    }

    public void PuanYukleVeGuncelle()
    {
        // Oyuncunun puanını telefonun hafızasından çekiyoruz.
        toplamPuan = PlayerPrefs.GetInt("OyuncuPuani", 0);
        Debug.Log("Güncel Toplam Puan: " + toplamPuan); // Konsolda puanı görmek için

        KilitDurumlariniGuncelle();
    }

    void KilitDurumlariniGuncelle()
    {
        foreach (var akademi in tumAkademiler)
        {
            // GÜVENLİK DUVARI: Eğer Unity Inspector'da buton veya akademi boş bırakıldıysa pas geç, kodu dondurma!
            if (akademi == null || akademi.butonComponent == null) 
                continue;

            if (toplamPuan >= akademi.gerekenPuan)
            {
                // KİLİDİ AÇ:
                if (akademi.kilitGorseli != null) 
                    akademi.kilitGorseli.SetActive(false); // Dumanlı kilit görselini gizle

                akademi.butonComponent.interactable = true; // Butona tıklanabilir yap
            }
            else
            {
                // KİLİTLİ TUT:
                if (akademi.kilitGorseli != null) 
                    akademi.kilitGorseli.SetActive(true);  // Dumanlı kilit görselini göster

                akademi.butonComponent.interactable = false; // Butona tıklanamaz yap
            }
        }
    }

    // TEST AMAÇLI: Müfettiş (Inspector) panelinden puanı elinle değiştirip denemek istersen diye hile butonları
    [ContextMenu("Puanı 1000 Yap ve Test Et")]
    public void TestPuanVer1000()
    {
        PlayerPrefs.SetInt("OyuncuPuani", 1000);
        PuanYukleVeGuncelle(); // Hafızayı doldur ve ekranı anında tazele
    }

    [ContextMenu("Puanı Sıfırla (0 Yap)")]
    public void TestPuanSifirla()
    {
        PlayerPrefs.SetInt("OyuncuPuani", 0);
        PuanYukleVeGuncelle(); // Hafızayı sıfırla ve ekranı anında tazele
    }
}