using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("Parşömen Görev Satırları (Kutular)")]
    public GameObject satirKirmizi; public GameObject satirMavi; public GameObject satirYesil;
    public GameObject satirMor; public GameObject satirSari; public GameObject satirPembe;

    [Header("Parşömen Sayaç Yazıları")]
    public TextMeshProUGUI txtKirmizi; public TextMeshProUGUI txtMavi; public TextMeshProUGUI txtYesil;
    public TextMeshProUGUI txtMor; public TextMeshProUGUI txtSari; public TextMeshProUGUI txtPembe;

    void Awake() 
    { 
        if (instance == null) instance = this; 
    }

    public void ParsomeniGuncelle()
    {
        var lm = LevelManager.instance;

        // 1. Yazıları "Ekilen / İstenen" şeklinde güncelle
        txtKirmizi.text = lm.ekilenler[CicekTuru.Kirmizi] + "/" + lm.recete[CicekTuru.Kirmizi];
        txtMavi.text = lm.ekilenler[CicekTuru.Mavi] + "/" + lm.recete[CicekTuru.Mavi];
        txtYesil.text = lm.ekilenler[CicekTuru.Yesil] + "/" + lm.recete[CicekTuru.Yesil];
        txtMor.text = lm.ekilenler[CicekTuru.Mor] + "/" + lm.recete[CicekTuru.Mor];
        txtSari.text = lm.ekilenler[CicekTuru.Sari] + "/" + lm.recete[CicekTuru.Sari];
        txtPembe.text = lm.ekilenler[CicekTuru.Pembe] + "/" + lm.recete[CicekTuru.Pembe];

        // 2. GÖRÜNMEZLİK KONTROLÜ: Eğer o çiçekten 0 tane isteniyorsa o satırı tamamen gizle
        satirKirmizi.SetActive(lm.recete[CicekTuru.Kirmizi] > 0);
        satirMavi.SetActive(lm.recete[CicekTuru.Mavi] > 0);
        satirYesil.SetActive(lm.recete[CicekTuru.Yesil] > 0);
        satirMor.SetActive(lm.recete[CicekTuru.Mor] > 0);
        satirSari.SetActive(lm.recete[CicekTuru.Sari] > 0);
        satirPembe.SetActive(lm.recete[CicekTuru.Pembe] > 0);
    }
}