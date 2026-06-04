using UnityEngine;
using UnityEngine.UI;

public class ButonSesTetikleyici : MonoBehaviour
{
    void Start()
    {
        // Bulunduğu Canvas altındaki aktif/pasif TÜM butonları otomatik yakalar
        Button[] tumButonlar = GetComponentsInChildren<Button>(true);

        foreach (Button buton in tumButonlar)
        {
            // Butonlara tıklandığı an SoundManager'daki klik sesini tetikler
            buton.onClick.AddListener(() => 
            {
                if (SoundManager.instance != null)
                {
                    SoundManager.instance.ButonSesi();
                }
            });
        }
    }
}