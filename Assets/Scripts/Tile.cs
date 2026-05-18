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

    [Header("Tohum Türü ve Çiçek Görselleri")]
    public CicekTuru uzerindekiCicek = CicekTuru.Hicbiri;
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

            // 2. Çiçek görselini hazırla
            GorseliAyarla(uzerindekiCicek);

            // 3. LevelManager ve UIManager'a haber ver, parşömendeki sayılar güncellensin
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

    private void GorseliAyarla(CicekTuru tur)
    {
        if (cicekGorseli == null) return;

        switch (tur)
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

    public void BuyumeyiGoster(bool fidanMi)
    {
        if (cicekGorseli != null) cicekGorseli.gameObject.SetActive(true);
    }

    public void EtkilesimeAc() { islemGordu = false; }
    public void EfektPatlat() { }
}