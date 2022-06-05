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

    private int Type = 0; // 0 - ������/1 - ������� �� ������/2-������� �� ����
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

    public void OnUnityAdsAdLoaded(string placementId) // ������� ��������� � ������ � ������
    {
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message) // ������� �� ������� �����������
    {
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message) //����� ��������� ������ ������ �������
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

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState) // ������� ����������
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
