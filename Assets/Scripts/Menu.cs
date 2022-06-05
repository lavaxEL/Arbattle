using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class Menu : MonoBehaviour
{
    public class ShopElem
    {
        private string Name;
        private bool Complete;
        private int Level;
        private int Prise;

        private Button Button;
        private Button ButtonComment;
        

        private Sprite Normal;
        private Sprite NormalRed;
        private Sprite Pressed;
        private Sprite PressedRed;

        private GameObject CommentBlock;
        public ShopElem(Button button, Sprite normal, Sprite pressed, Sprite normal_red, Sprite pressed_red, int level,
            bool complete, GameObject comment_block, Button button_comment, int prise, string name)
        {
            Button = button;
            ButtonComment = button_comment;
            Complete = complete;
            Level = level;
            Normal = normal;
            NormalRed = normal_red;
            Pressed = pressed;
            PressedRed = pressed_red;
            CommentBlock = comment_block;
            Prise = prise;
            Name = name;
        }
        public void UpdateElem(int lavaxium)
        {
            if (lavaxium>=Prise)
            {
                Button.GetComponent<Image>().sprite = Normal;
                SpriteState spriteState = new SpriteState();
                spriteState = Button.spriteState;
                spriteState.pressedSprite = Pressed;
                Button.spriteState = spriteState;
            }
            else
            {
                Button.GetComponent<Image>().sprite = NormalRed;
                SpriteState spriteState = new SpriteState();
                spriteState = Button.spriteState;
                spriteState.pressedSprite = PressedRed;
                Button.spriteState = spriteState;
            }
            if (Complete)
                Button.interactable = false;
        }
        public void CommentSetActive(int lavaxium)
        {
            if (lavaxium >= Prise)
                ButtonComment.interactable = true;
            else
                ButtonComment.interactable = false;
            CommentBlock.SetActive(true);
        }
        public void CommentSetNoActive()
        {
            CommentBlock.SetActive(false);
        }
        public int Buy(int lavaxium)
        {
            CommentSetNoActive();
            if ((Name == "health" || Name == "cross") && Level == 0) { Level++; return Prise; }
            else if (Name == "spell" && Level<3) { Level++;  return Prise; }
            Complete = true;
            Level++;
            UpdateElem(lavaxium);
            return Prise;
        }
        public int GetLevel() { return Level; }
        public bool GetComplete() { return Complete; }
    }
    /*Префабы игровые*/
    public Image ShopBanner;
    public GameObject Build;
    public GameObject Wall;
    public GameObject Tree1;
    public GameObject Tree2;

    /*Чиселки*/
    public List<Sprite> Sprites = new List<Sprite>();
    public List<Sprite> RedSprites = new List<Sprite>();
    public List<Image> ScoreImage = new List<Image>();
    public List<Image> BestScoreImage = new List<Image>();
    public List<Image> ComboImage = new List<Image>();
    public List<Image> BestComboImage = new List<Image>();
    public List<Image> LavaxiumImage = new List<Image>();
    public List<Image> ArrowImage = new List<Image>();
    public List<Image> SkeletonImage = new List<Image>();
    public List<Image> GhostImage = new List<Image>();
    public List<Image> KnightImage = new List<Image>();
    public List<Image> BossImage = new List<Image>();
    public List<Image> NecromantImage = new List<Image>();

    public List<Sprite> ShopInfoSprite = new List<Sprite>();

    public GameObject ScoreBlock;
    public GameObject ComboBlock;
    public GameObject BestScoreBlock;
    public GameObject BestComboBlock;
    public GameObject LavaxBlock;
    public GameObject ArrowBlock;
    public List<GameObject> MobBlocks;

    public GameObject ShopInfoBlock;
    public GameObject ShopInfoEffect;
    public Image ShopInfoImage;

    public Image BestScoreObj;
    public Image BestComboObj;

    public Image ComboX;
    public Image BestComboX;

    public GameObject LeftRecord;
    public GameObject RightRecord;

    public List<Button> ShopElts; //0-triple/1-slow/2-fire/3-bomb/4-health(1,2)/5-crossbow
    public List<Button> ShopCommentElts;
    public List<Button> ComboElts;
    public List<GameObject> ShopEltsCommentBlocks;
    public List<Sprite> TripleArr;
    public List<Sprite> SlowArr;
    public List<Sprite> FireArr;
    public List<Sprite> BombArr;
    public List<Sprite> Health;
    public List<Sprite> Spell;

    public GameObject MainMenu; // блок главного меню
    public GameObject Shop; // блок магазина
    public GameObject Combo; // блок комбо
    public GameObject Playing; // диспетчер игры
    public GameObject SoundBlock;

    public GameObject CommentBack;
    public GameObject CommentExit;

    public GameObject TripleInfo;

    public static GameClass.Numbers Score;
    Dictionary<string, ShopElem> ShopElements;
    public List<Sprite> SpellSprites;

    public static Vector2 CrossPos;
    public Button SpellButton;

    //public AudioListener AudioListener;
    /*Звуки*/
    public AudioSource MenuMusic;
    public AudioSource Tap;
    public AudioSource Complete;
    public Button SoundButton;
    public Sprite SoundOn;
    public Sprite SoundOff;
    public Button TargetButton;
    public Sprite TargetOn;
    public Sprite TargetOff;

    /*Сохраняемые переменные*/
    public static int BestScore=0;
    public static int BestCombo=0;
    public static int Lavaxium=500;
    public static bool Sound = true;
    public static bool Target = true;

    public static bool TripleComplete=false;
    public static int TripleLevel=0;

    public static bool SlowComplete = false;
    public static int SlowLevel = 0;

    public static bool FireComplete = false;
    public static int FireLevel = 0;

    public static bool BombComplete = false;
    public static int BombLevel = 0;

    public static bool HealthComplete = false;
    public static int HealthLevel = 0;

    public static bool SpellComplete = false;
    public static int SpellLevel = 0;

    public static int Hp = 3;
    public static int DeltaBest = Screen.width / 18;
    public static int DeltaGame = Screen.width / 14;
    public static bool UpdateShop = false;

    
    [Serializable]
    public class SaveData
    {
        public int Lavaxium;
        public int BestCombo;
        public int BestScore;
        public bool Sound;
        public bool Target;

        public bool TripleComlete;
        public int TripleLevel;
        public bool SlowComlete;
        public int SlowLevel;
        public bool FireComplete;
        public int FireLevel;
        public bool BombComplete;
        public int BombLevel;
        public bool HealthComplete;
        public int HealthLevel;
        public bool SpellComplete;
        public int SpellLevel;
    }
    /*загрузка данных игры*/
    void Load()
    {
        string json = PlayerPrefs.GetString("data", "empty");
        if (json=="empty")
        {
            return;
        }
        SaveData data = JsonUtility.FromJson<SaveData>(json);
        Lavaxium = data.Lavaxium;
        BestCombo = data.BestCombo;
        BestScore = data.BestScore;
        Sound = data.Sound;
        Target = data.Target;
        TripleComplete = data.TripleComlete;
        TripleLevel = data.TripleLevel;
        SlowComplete = data.SlowComlete;
        SlowLevel = data.SlowLevel;
        FireComplete = data.FireComplete;
        FireLevel = data.FireLevel;
        BombComplete = data.BombComplete;
        BombLevel = data.BombLevel;
        HealthComplete = data.HealthComplete;
        HealthLevel = data.HealthLevel;
        SpellComplete = data.SpellComplete;
        SpellLevel = data.SpellLevel;
        if (HealthLevel == 1)
            Hp = 4;
        else if (HealthLevel == 2)
            Hp = 5;
    }
    void Save()
    {
        SaveData data = new SaveData();
        data.Lavaxium = Lavaxium;
        data.BestCombo = BestCombo;
        data.BestScore = BestScore;
        data.Sound = Sound;
        data.Target = Target;
        data.TripleComlete = TripleComplete;
        data.TripleLevel = TripleLevel;
        data.SlowComlete = SlowComplete;
        data.SlowLevel = SlowLevel;
        data.FireComplete = FireComplete;
        data.FireLevel = FireLevel;
        data.BombComplete = BombComplete;
        data.BombLevel = BombLevel;
        data.HealthComplete = HealthComplete;
        data.HealthLevel = HealthLevel;
        data.SpellComplete = SpellComplete;
        data.SpellLevel = SpellLevel;
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("data", json);
    }
    public void Exit()
    {
        Tap.Play();
        Save();
        Application.Quit();
    }
    public void Music()
    {
        if (Sound)
        {
            AudioListener.pause = true;
            Sound = false;
            SoundButton.GetComponent<Image>().sprite = SoundOff;
        }
        else
        {
            Sound = true;
            AudioListener.pause = false;
            SoundButton.GetComponent<Image>().sprite = SoundOn;
        }
    }
    public void Targett()
    {
        Target = !Target;
        if (Target)
            TargetButton.GetComponent<Image>().sprite = TargetOn;
        else
            TargetButton.GetComponent<Image>().sprite = TargetOff;
    }
    /*Заполняем ShopElements*/
    void ShopIncluding()
    {
        ShopElements = new Dictionary<string, ShopElem>();
        ShopElements.Add("triple", new ShopElem(ShopElts[0], TripleArr[0], TripleArr[1], TripleArr[2], TripleArr[3], TripleLevel, TripleComplete,
            ShopEltsCommentBlocks[0], ShopCommentElts[0], 3000, "triple"));
        ShopElements.Add("slow", new ShopElem(ShopElts[1], SlowArr[0], SlowArr[1], SlowArr[2], SlowArr[3], SlowLevel, SlowComplete,
            ShopEltsCommentBlocks[1], ShopCommentElts[1], 1000, "slow"));
        ShopElements.Add("fire", new ShopElem(ShopElts[2], FireArr[0], FireArr[1], FireArr[2], FireArr[3], FireLevel, FireComplete,
            ShopEltsCommentBlocks[2], ShopCommentElts[2], 2000, "fire"));
        ShopElements.Add("bomb", new ShopElem(ShopElts[3], BombArr[0], BombArr[1], BombArr[2], BombArr[3], BombLevel, BombComplete,
            ShopEltsCommentBlocks[3], ShopCommentElts[3], 5000, "bomb"));
        if (HealthLevel==0)
            ShopElements.Add("health", new ShopElem(ShopElts[4], Health[0], Health[1], Health[2], Health[3], HealthLevel, HealthComplete,
                ShopEltsCommentBlocks[4], ShopCommentElts[4], 1000, "health"));
        else
            ShopElements.Add("health", new ShopElem(ShopElts[4], Health[0], Health[1], Health[2], Health[3], HealthLevel, HealthComplete,
                ShopEltsCommentBlocks[5], ShopCommentElts[5], 1000, "health"));
        if (SpellLevel==0)
            ShopElements.Add("spell", new ShopElem(ShopElts[5], Spell[0], Spell[1], Spell[2], Spell[3], SpellLevel, SpellComplete,
                ShopEltsCommentBlocks[6], ShopCommentElts[6], 4500, "spell"));
        else if(SpellLevel==1)
            ShopElements.Add("spell", new ShopElem(ShopElts[5], Spell[0], Spell[1], Spell[2], Spell[3], SpellLevel, SpellComplete,
                ShopEltsCommentBlocks[7], ShopCommentElts[7], 4500, "spell"));
        else if(SpellLevel==2)
            ShopElements.Add("spell", new ShopElem(ShopElts[5], Spell[0], Spell[1], Spell[2], Spell[3], SpellLevel, SpellComplete,
                ShopEltsCommentBlocks[8], ShopCommentElts[8], 4500, "spell"));
        else if(SpellLevel==3)
            ShopElements.Add("spell", new ShopElem(ShopElts[5], Spell[0], Spell[1], Spell[2], Spell[3], SpellLevel, SpellComplete,
                ShopEltsCommentBlocks[9], ShopCommentElts[9], 4500, "spell"));
        else if(SpellLevel>=4)
            ShopElements.Add("spell", new ShopElem(ShopElts[5], Spell[0], Spell[1], Spell[2], Spell[3], SpellLevel, true,
                ShopEltsCommentBlocks[9], ShopCommentElts[9], 4500, "spell"));
    }

    /*Точка входа в игру*/
    void Start() 
    {
        MenuMusic.Play();
        Score = new GameClass.Numbers(Sprites, RedSprites, ScoreImage, BestScoreImage, ComboImage, BestComboImage,
            LavaxiumImage, ScoreBlock, BestScoreBlock, ComboBlock, BestComboBlock, LavaxBlock, BestScoreObj, BestComboObj,
            LeftRecord, RightRecord, ComboX, BestComboX, ArrowImage, ArrowBlock, MobBlocks, SkeletonImage, GhostImage,
             KnightImage, BossImage, NecromantImage); // инициализируем объект класса чисел
        Vector2 wall_position = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width / 2, Screen.height/5));
        Instantiate(Wall, wall_position, Quaternion.identity);
        Instantiate(Wall, new Vector2(wall_position.x - 8.18f, wall_position.y), Quaternion.identity);
        Instantiate(Wall, new Vector2(wall_position.x + 8.18f, wall_position.y), Quaternion.identity);
        Vector2 position = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width / 2, Screen.height / 10));
        CrossPos = Instantiate(Build, position, Quaternion.identity).transform.Find("place").gameObject.transform.position; // разместили здание
        position= Camera.main.ScreenToWorldPoint(new Vector2(Screen.width/10, Screen.height / 9));
        Instantiate(Tree1, position, Quaternion.identity); // разместили дерево
        position = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width-Screen.width/6, Screen.height / 6));
        Instantiate(Tree2, position, Quaternion.identity); // разместили дерево
        position = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width - Screen.width / 12, Screen.height / 13));
        Instantiate(Tree1, position, Quaternion.identity); // разместили дерево
        position = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width/5, 0));
        Instantiate(Tree1, position, Quaternion.identity); // разместили дерево
        Load(); // загружаем данные из памяти
        if (!Sound)
        {
            AudioListener.pause = true;
            SoundButton.GetComponent<Image>().sprite = SoundOff;
        }
        if (!Target)
            TargetButton.GetComponent<Image>().sprite = TargetOff;
        ShopIncluding(); // заполняем данные магазина
        SpellButton.GetComponent<Image>().sprite = SpellSprites[SpellLevel];

        //Score.Lavaxium(Lavaxium);
    }
    private void OnApplicationPause(bool pause)
    {
        if (pause)
            Save();
    }
    void UpdateAllShop()
    {
        ShopElements["triple"].UpdateElem(Lavaxium);
        ShopElements["slow"].UpdateElem(Lavaxium);
        ShopElements["fire"].UpdateElem(Lavaxium);
        ShopElements["bomb"].UpdateElem(Lavaxium);
        ShopElements["health"].UpdateElem(Lavaxium);
        ShopElements["spell"].UpdateElem(Lavaxium);
    }
    /*Обработка кнопки "play"*/
    public void Play()
    {
        Tap.Play();
        MenuMusic.Stop();
        Score.Lavaxium(Lavaxium, false);
        MainMenu.SetActive(false);
        Playing.SetActive(true);
        SoundBlock.SetActive(false);
    }
    /*Обработка кнопки "shop"*/
    public void Market()
    {
        Tap.Play();
        UpdateAllShop();
        MainMenu.SetActive(false);
        Shop.SetActive(true);
       // Score.BestScore(BestScore, false);
       // Score.BestCombo(BestCombo, false);
        Score.Lavaxium(Lavaxium, false);
        //Score.Arrow(Arrows, false);
    }
    public void MarketExit()
    {
        Tap.Play();
        Shop.SetActive(false);
        MainMenu.SetActive(true);
    }
   
    void MarketInfoSetActive()
    {
        CommentBack.SetActive(true);
        CommentExit.SetActive(true);
    }
    void MarketInfoSetNoActive()
    {
        CommentBack.SetActive(false);
        CommentExit.SetActive(false);
    }
    /*Вход в комментыб выход и покупка*/
    public void MarketInfoExit()
    {
        Tap.Play();
        ShopElements["triple"].CommentSetNoActive();
        ShopElements["slow"].CommentSetNoActive();
        ShopElements["fire"].CommentSetNoActive();
        ShopElements["bomb"].CommentSetNoActive();
        ShopElements["health"].CommentSetNoActive();
        ShopElements["spell"].CommentSetNoActive();
        MarketInfoSetNoActive();
    }
    public void MarketInfoEnter(string name)
    {
        Tap.Play();
        ShopElements[name].CommentSetActive(Lavaxium);
        MarketInfoSetActive();
    }
    public void Buy(string name)
    {
        Tap.Play();
        Complete.Play();
        Lavaxium -=ShopElements[name].Buy(Lavaxium);
        Score.Lavaxium(Lavaxium, true);
        UpdateAllShop();
        MarketInfoExit();
        if (name == "triple")
        {
            TripleLevel = ShopElements["triple"].GetLevel();
            TripleComplete = ShopElements["triple"].GetComplete();
            ShopInfoImage.sprite = ShopInfoSprite[0];
        }
        else if(name == "slow")
        {
            SlowLevel = ShopElements["slow"].GetLevel();
            SlowComplete = ShopElements["slow"].GetComplete();
            ShopInfoImage.sprite = ShopInfoSprite[1];
        }
        else if (name == "fire")
        {
            FireLevel = ShopElements["fire"].GetLevel();
            FireComplete = ShopElements["fire"].GetComplete();
            ShopInfoImage.sprite = ShopInfoSprite[2];
        }
        else if (name == "bomb")
        {
            BombLevel = ShopElements["bomb"].GetLevel();
            BombComplete = ShopElements["bomb"].GetComplete();
            ShopInfoImage.sprite = ShopInfoSprite[3];
        }
        else if (name=="health")
        {
            HealthLevel = ShopElements["health"].GetLevel();
            Hp++;
            if (Hp==4)
            {
                ShopElements.Remove("health");
                ShopElements.Add("health", new ShopElem(ShopElts[4], Health[0], Health[1], Health[2], Health[3], HealthLevel, HealthComplete,
                    ShopEltsCommentBlocks[5], ShopCommentElts[5], 1000, "health"));
                UpdateAllShop();
            }
            else if(Hp==5)
            {
                HealthLevel = 2;
                HealthComplete = true;
            }

            ShopInfoImage.sprite = ShopInfoSprite[4];
        }
        else if (name == "spell")
        {
            SpellLevel = ShopElements["spell"].GetLevel();
            
            if (SpellLevel==1)// купили огненные
            {
                ShopElements.Remove("spell");
                ShopElements.Add("spell", new ShopElem(ShopElts[5], Spell[0], Spell[1], Spell[2], Spell[3], SpellLevel, SpellComplete,
                    ShopEltsCommentBlocks[7], ShopCommentElts[7], 4500, "spell"));
                ShopInfoImage.sprite = ShopInfoSprite[5];
            }
            else if(SpellLevel == 2)
            {
                ShopElements.Remove("spell");
                ShopElements.Add("spell", new ShopElem(ShopElts[5], Spell[0], Spell[1], Spell[2], Spell[3], SpellLevel, SpellComplete,
                    ShopEltsCommentBlocks[8], ShopCommentElts[8], 4500, "spell"));
                ShopInfoImage.sprite = ShopInfoSprite[6];
            }
            else if (SpellLevel == 3)
            {
                ShopElements.Remove("spell");
                ShopElements.Add("spell", new ShopElem(ShopElts[5], Spell[0], Spell[1], Spell[2], Spell[3], SpellLevel, SpellComplete,
                    ShopEltsCommentBlocks[9], ShopCommentElts[9], 4500, "spell"));
                ShopInfoImage.sprite = ShopInfoSprite[7];
            }
            else
                ShopInfoImage.sprite = ShopInfoSprite[8];
            SpellButton.GetComponent<Image>().sprite = SpellSprites[SpellLevel];
            UpdateAllShop();
            
        }

        ShopInfoBlock.SetActive(true);
        ShopInfoEffect.SetActive(false);
        ShopInfoEffect.SetActive(true);
    }
    public void Comboo()
    {
        Tap.Play();
        for (int i = 0; i < BestCombo / 10; i++)
            if (i<6)
                ComboElts[i].interactable = false;
        MainMenu.SetActive(false);
        Combo.SetActive(true);
        //Score.BestScore(BestScore, false);
        //Score.BestCombo(BestCombo, false);
        Score.Lavaxium(Lavaxium, false);
       // Score.Arrow(Arrows, false);
    }
    public void Taping()
    {
        Tap.Play();
    }
    public void CombooExit()
    {
        Tap.Play();
        Combo.SetActive(false);
        MainMenu.SetActive(true);
    }
    private void Update()
    {
        if (UpdateShop)
        {
            Complete.Play();
            UpdateShop = false;
            UpdateAllShop();
            ShopInfoImage.sprite = ShopInfoSprite[9];
            ShopInfoBlock.SetActive(true);
            ShopInfoEffect.SetActive(false);
            ShopInfoEffect.SetActive(true);
        }
    }
}
