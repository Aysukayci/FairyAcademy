using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("Bitiş Ekranı ve Puanlama")]
    public GameObject bitisPaneli;
    public TextMeshProUGUI txtBaslik; // "Tebrikler" veya "Başarısız" yazacak
    public TextMeshProUGUI txtFinalPuan;

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

        if (txtFinalPuan != null) txtFinalPuan.text = "Puan: " + kazanilanPuan;

        // Yıldızları önce temizle
        yildiz1.SetActive(false); yildiz2.SetActive(false); yildiz3.SetActive(false);

        // KALMA VE YILDIZ MEKANİZMASI
        if (kazanilanPuan >= 90) // 3 Yıldız
        {
            txtBaslik.text = "MÜKEMMEL HASAT!";
            yildiz1.SetActive(true); yildiz2.SetActive(true); yildiz3.SetActive(true);
        }
        else if (kazanilanPuan >= 60) // 2 Yıldız
        {
            txtBaslik.text = "TEBRİKLER!";
            yildiz1.SetActive(true); yildiz2.SetActive(true);
        }
        else if (kazanilanPuan >= 30) // 1 Yıldız
        {
            txtBaslik.text = "BAŞARDIN!";
            yildiz1.SetActive(true);
        }
        else // 30 Puan altı -> KALMA DURUMU
        {
            txtBaslik.text = "BAŞARISIZ OLDUN!";
            // Yıldızlar kapalı kalır
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