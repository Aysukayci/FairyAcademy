using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("Bitiş Ekranı ve Puanlama")]
    public GameObject bitisPaneli;
    public TextMeshProUGUI txtBaslik; // "Tebrikler" veya "Başarısız" yazacak
    [Header("Yıldız Görselleri (Dolu Olanlar)")]
    public GameObject yildiz1; 
    public GameObject yildiz2; 
    public GameObject yildiz3;

    [Header("Oyun Bittiğinde Gizlenecek Paneller")]
    public GameObject gamePanel;
    public GameObject aracPaneli;

    [Header("Parşömen Görev Satırları (Kutular)")]
    public GameObject satirKirmizi; public GameObject satirMavi; public GameObject satirYesil;
    public GameObject satirMor; public GameObject satirSari; public GameObject satirPembe;

    [Header("Parşömen Sayaç Yazıları")]
    public TextMeshProUGUI txtKirmizi; public TextMeshProUGUI txtMavi; public TextMeshProUGUI txtYesil;
    public TextMeshProUGUI txtMor; public TextMeshProUGUI txtSari; public TextMeshProUGUI txtPembe;

    void Awake() { if (instance == null) instance = this; }

   public void BitisEkraniniGoster(int kazanilanPuan)
    {
        if (bitisPaneli != null) bitisPaneli.SetActive(true);
        if (gamePanel != null) gamePanel.SetActive(false);
        if (aracPaneli != null) aracPaneli.SetActive(false);

        // --- PUAN KAYDETME SİSTEMİ ---
        // Oyuncu 0'dan büyük bir puan aldıysa (bölümü başarıyla bitirdiyse) hafızaya ekle
        if (kazanilanPuan > 0) 
        {
            // 1. Önce hafızadaki eski toplam puanı çekiyoruz
            int eskiToplam = PlayerPrefs.GetInt("ToplamPuan", 0);
            
            // 2. Yeni kazanılan puanı eski puanın üzerine ekliyoruz
            int yeniToplam = eskiToplam + kazanilanPuan;
            
            // 3. Yeni toplamı kalıcı olarak hafızaya yazıp kaydediyoruz
            PlayerPrefs.SetInt("ToplamPuan", yeniToplam);
            PlayerPrefs.Save(); 
            
            Debug.Log("<color=cyan>FairyAcademy: Puan başarıyla hafızaya yazıldı! Yeni Toplam: </color>" + yeniToplam);
        }

        // Yıldızları önce temizle
        if (yildiz1 != null) yildiz1.SetActive(false); 
        if (yildiz2 != null) yildiz2.SetActive(false); 
        if (yildiz3 != null) yildiz3.SetActive(false);

        // KALMA VE YILDIZ MEKANİZMASI
        // Yıldızlar görünmeye başladığı an bu ses çalacak
        if (SoundManager.instance != null && kazanilanPuan >= 30) 
        {
            SoundManager.instance.YildizSesi();
        }
        if (kazanilanPuan >= 90) // 3 Yıldız
        {
            if (txtBaslik != null) txtBaslik.text = "MÜKEMMEL HASAT!";
            if (yildiz1 != null) yildiz1.SetActive(true); 
            if (yildiz2 != null) yildiz2.SetActive(true); 
            if (yildiz3 != null) yildiz3.SetActive(true);
        }
        else if (kazanilanPuan >= 60) // 2 Yıldız
        {
            if (txtBaslik != null) txtBaslik.text = "TEBRİKLER!";
            if (yildiz1 != null) yildiz1.SetActive(true); 
            if (yildiz2 != null) yildiz2.SetActive(true);
        }
        else if (kazanilanPuan >= 30) // 1 Yıldız
        {
            if (txtBaslik != null) txtBaslik.text = "BAŞARDIN!";
            if (yildiz1 != null) yildiz1.SetActive(true);
        }
        else // 30 Puan altı -> KALMA DURUMU
        {
            if (txtBaslik != null) txtBaslik.text = "BAŞARISIZ OLDUN!";
        }
    }
    public void ParsomeniGuncelle()
    {
        var lm = LevelManager.instance;
        txtKirmizi.text = lm.ekilenler[CicekTuru.Kirmizi] + "/" + lm.recete[CicekTuru.Kirmizi];
        txtMavi.text = lm.ekilenler[CicekTuru.Mavi] + "/" + lm.recete[CicekTuru.Mavi];
        txtYesil.text = lm.ekilenler[CicekTuru.Yesil] + "/" + lm.recete[CicekTuru.Yesil];
        txtMor.text = lm.ekilenler[CicekTuru.Mor] + "/" + lm.recete[CicekTuru.Mor];
        txtSari.text = lm.ekilenler[CicekTuru.Sari] + "/" + lm.recete[CicekTuru.Sari];
        txtPembe.text = lm.ekilenler[CicekTuru.Pembe] + "/" + lm.recete[CicekTuru.Pembe];

        satirKirmizi.SetActive(lm.recete[CicekTuru.Kirmizi] > 0);
        satirMavi.SetActive(lm.recete[CicekTuru.Mavi] > 0);
        satirYesil.SetActive(lm.recete[CicekTuru.Yesil] > 0);
        satirMor.SetActive(lm.recete[CicekTuru.Mor] > 0);
        satirSari.SetActive(lm.recete[CicekTuru.Sari] > 0);
        satirPembe.SetActive(lm.recete[CicekTuru.Pembe] > 0);
    }
}