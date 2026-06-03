using UnityEngine;
using UnityEngine.SceneManagement; // Sahne geçişleri için bu kütüphane şart!

public class MenuKontrol : MonoBehaviour
{
    // Butona basıldığında çalışacak fonksiyon
    public void OyunaBasla()
    {
        // Geçmek istediğimiz sahnenin adını tam olarak buraya yazıyoruz
        SceneManager.LoadScene("MapScene"); 
    }
}