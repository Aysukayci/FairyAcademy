using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections; 

public class Tile : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler
{
    [Header("Tarla Görselleri")]
    private Image tarlaResmi;
    public Image cicekGorseli; 
    private bool islemGordu = false;

    public Sprite tirmiklanmamisResim;
    public Sprite tirmiklanmisResim;
    public Sprite tohumluResim;
    public Sprite sulanmisResim;

    [Header("Büyüme Görselleri")]
    public Sprite fidanSprite; // Kilitli: Buraya genel fidan resmini sürükle
    public CicekTuru uzerindekiCicek = CicekTuru.Hicbiri;

    [Header("Renkli Çiçek Görselleri")]
    public Sprite kirmiziCicekSprite;
    public Sprite maviCicekSprite;
    public Sprite yesilCicekSprite;
    public Sprite morCicekSprite;
    public Sprite sariCicekSprite;
    public Sprite pembeCicekSprite;

    void Start()
    {
        tarlaResmi = GetComponent<Image>();
        if (tirmiklanmamisResim != null) tarlaResmi.sprite = tirmiklanmamisResim;
        tarlaResmi.color = Color.white; 
        
        if (cicekGorseli != null) 
        {
            cicekGorseli.gameObject.SetActive(false);
            cicekGorseli.transform.localScale = Vector3.one; 
            cicekGorseli.color = Color.white; 
        }
        if (GameManager.instance != null) GameManager.instance.TarlayiListeyeEkle(this);
    }

    public void OnPointerDown(PointerEventData eventData) { Etkilesim(); }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.eligibleForClick || eventData.dragging) { Etkilesim(); }
    }

    void Etkilesim()
    {
        if (islemGordu || GameManager.instance == null) return;

        // Geri al butonunun çalışabilmesi için son etkileşime girilen tarlayı GameManager'a bildiriyoruz
        GameManager.instance.SonHamleyiKaydet(this);

        var arac = GameManager.instance.seciliArac;
        var asama = GameManager.instance.mevcutAsama;

        bool dogruKullanim = false;

        if (asama == GameManager.BahceAsamasi.TirmikAsamasi && arac == GameManager.AracTipi.Tirmik) dogruKullanim = true;
        else if (asama == GameManager.BahceAsamasi.TohumAsamasi && arac == GameManager.AracTipi.Tohum) 
        {
            // Eğer oyuncu parşömenden bir renk seçmişse işlemi kabul et!
            if (GameManager.instance.seciliTohum != CicekTuru.Hicbiri)
            {
                dogruKullanim = true;
                
                // 1. Tarlanın hafızasına ektiğimiz çiçeği yazalım
                uzerindekiCicek = GameManager.instance.seciliTohum;

                // NOT: Artık burada çiçek görselini ayarlamıyoruz, fidan olması için güneş sürecini bekliyoruz!

                // 2. LevelManager ve UIManager'a haber ver, parşömendeki sayılar güncellensin
                if (LevelManager.instance != null && UIManager.instance != null)
                {
                    LevelManager.instance.ekilenler[uzerindekiCicek]++;
                    UIManager.instance.ParsomeniGuncelle();
                }
            }
        }
        else if (asama == GameManager.BahceAsamasi.SuAsamasi && arac == GameManager.AracTipi.Su) dogruKullanim = true;
        else if (asama == GameManager.BahceAsamasi.HasatAsamasi && arac == GameManager.AracTipi.Hasat) dogruKullanim = true;

        if (dogruKullanim)
        {
            islemGordu = true; 
            tarlaResmi.color = new Color(0.8f, 0.8f, 0.8f); 

            if (asama == GameManager.BahceAsamasi.HasatAsamasi && cicekGorseli != null) 
            {
                cicekGorseli.color = new Color(0.8f, 0.8f, 0.8f);
            }

            GameManager.instance.TarlayaTiklandi(); 
        }
    }

    // GameManager içerisinden "Geri Al" basıldığında bu tarlanın durumunu bir önceki aşamaya döndüren fonksiyon
    public void HamleyiGeriSar(GameManager.BahceAsamasi asama)
    {
        // Tarlanın işlem görme kilidini açıyoruz ve rengini normale döndürüyoruz
        islemGordu = false;
        tarlaResmi.color = Color.white;

        if (asama == GameManager.BahceAsamasi.TirmikAsamasi)
        {
            // Tırmıklamayı geri alıyorsak ham tırpılanmamış toprağa geri döner
            tarlaResmi.sprite = tirmiklanmamisResim;
        }
        else if (asama == GameManager.BahceAsamasi.TohumAsamasi)
        {
            // Tohum ekmeyi geri alıyorsak tırmıklanmış boş toprağa geri döner
            tarlaResmi.sprite = tirmiklanmisResim;

            // Yanlış ekilen tohumu listelerden ve UI parşömenden düşüyoruz
            if (uzerindekiCicek != CicekTuru.Hicbiri)
            {
                if (LevelManager.instance != null && LevelManager.instance.ekilenler.ContainsKey(uzerindekiCicek))
                {
                    if (LevelManager.instance.ekilenler[uzerindekiCicek] > 0)
                    {
                        LevelManager.instance.ekilenler[uzerindekiCicek]--;
                    }
                }
                
                if (UIManager.instance != null)
                {
                    UIManager.instance.ParsomeniGuncelle();
                }
            }

            uzerindekiCicek = CicekTuru.Hicbiri;
            if (cicekGorseli != null) cicekGorseli.gameObject.SetActive(false);
        }
        else if (asama == GameManager.BahceAsamasi.SuAsamasi)
        {
            // Sulamayı geri alıyorsak sadece tohumlu kuru toprak görseline geri döner
            tarlaResmi.sprite = tohumluResim;
        }
    }

    public void DalgaUlasti()
    {
        tarlaResmi.color = Color.white; 
        var asama = GameManager.instance.mevcutAsama;

        if (asama == GameManager.BahceAsamasi.TirmikAsamasi) tarlaResmi.sprite = tirmiklanmisResim;
        else if (asama == GameManager.BahceAsamasi.TohumAsamasi) tarlaResmi.sprite = tohumluResim;
        else if (asama == GameManager.BahceAsamasi.SuAsamasi) tarlaResmi.sprite = sulanmisResim;
        else if (asama == GameManager.BahceAsamasi.HasatAsamasi) 
        {
            tarlaResmi.sprite = tirmiklanmamisResim;
            if (cicekGorseli != null) 
            {
                cicekGorseli.gameObject.SetActive(false); 
                cicekGorseli.color = Color.white; 
            }
        }
        EfektPatlat(); 
    }

    // YENİ: Güneş barı %40'a gelince sadece fidan görselini açar
    public void FidanDurumunaGetir()
    {
        if (cicekGorseli == null || uzerindekiCicek == CicekTuru.Hicbiri) return;
        
        cicekGorseli.sprite = fidanSprite; // Genel fidan resmini koy
        cicekGorseli.gameObject.SetActive(true); // Görseli aç
    }

    // YENİ: Güneş barı dolunca fidanı asıl renkli çiçeğe dönüştürür
    public void CicekDurumunaGetir()
    {
        if (cicekGorseli == null || uzerindekiCicek == CicekTuru.Hicbiri) return;

        // Hafızadaki çiçek türüne göre asıl görseli ayarla
        switch (uzerindekiCicek)
        {
            case CicekTuru.Kirmizi: cicekGorseli.sprite = kirmiziCicekSprite; break;
            case CicekTuru.Mavi: cicekGorseli.sprite = maviCicekSprite; break;
            case CicekTuru.Yesil: cicekGorseli.sprite = yesilCicekSprite; break;
            case CicekTuru.Mor: cicekGorseli.sprite = morCicekSprite; break;
            case CicekTuru.Sari: cicekGorseli.sprite = sariCicekSprite; break;
            case CicekTuru.Pembe: cicekGorseli.sprite = pembeCicekSprite; break;
        }
    }

    public void Sifirla()
    {
        tarlaResmi.sprite = tirmiklanmamisResim;
        tarlaResmi.color = Color.white;
        islemGordu = false;
        uzerindekiCicek = CicekTuru.Hicbiri; 

        if (cicekGorseli != null)
        {
            cicekGorseli.gameObject.SetActive(false);
            cicekGorseli.color = Color.white; 
        }
    }

    // Eski sistemden kalan bu fonksiyonu içi boş bırakıyoruz ki Unity diğer kodlarda hata vermesin
    public void BuyumeyiGoster(bool fidanMi) { }

    public void EtkilesimeAc() { islemGordu = false; }
    public void EfektPatlat() { }
}