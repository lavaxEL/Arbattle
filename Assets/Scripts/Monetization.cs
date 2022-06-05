using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class Monetization : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    private string GameID = "4525367";
    private string InterstitialVideo = "Rewarded_Android";
    private bool Test = false;
    private bool Init = false;
    public GameObject ShopInfoBlock;
    public Image ShopInfoImage;
    public Sprite Sprite;

    private int Type = 0; // 0 - ничего/1 - реклама из магаза/2-реклама из игры
    //public static RewardedAds S;
    private void Awake()
    {
        InitializeAds();
        LoadAd();
    }
    public void LoadAd()
    {
        Advertisement.Load(InterstitialVideo, this);
    }
    public void ShowAd(int type)
    {
        Type = type;
        if (Type==2)
            GameOver.can = false;
        Advertisement.Show(InterstitialVideo, this);
    }
    public void InitializeAds()
    {
        Advertisement.Initialize(GameID, Test);
        Init = true;
    }
    public void OnInitializationComplete()
    {
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
    }

    public void OnUnityAdsAdLoaded(string placementId) // реклама загружена и готова к показу
    {
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message) // рекламе не удалось загрузиться
    {
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message) //метод обработки ошибки показа рекламы
    {
        ShopInfoImage.sprite = Sprite;
        ShopInfoBlock.SetActive(false);
        ShopInfoBlock.SetActive(true);
        if (Type == 2)
            GameOver.can = true;
        if (!Init)
        {
            Advertisement.Initialize(GameID, Test);
            Init = true;
        }
            
        LoadAd();
    }

    public void OnUnityAdsShowStart(string placementId)
    {
    }

    public void OnUnityAdsShowClick(string placementId)
    {
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState) // рекламу досмотрели
    {
        if(Type==1)
        {
            Type = 0;
            Menu.Lavaxium += 100;
            Menu.Score.Lavaxium(Menu.Lavaxium, true);
            Menu.UpdateShop = true;
        }
        else if(Type==2)
        {
            Type = 0;
            Main.AdsComplete = true;
        }
        LoadAd();
    }
}
