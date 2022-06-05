using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    /*Интерфейсы*/
    public GameObject GameInterface;
    public GameObject PauseInterface;
    public GameObject GameOvr;
    public GameObject MainMenu;
    public GameObject ArrowsStack;
    public GameObject GameInfoo;
    public GameObject Statistica;
    public GameObject SoundBlock;

    public Button Pausee;
    public GameObject Effect;
    

    /*Префабы игровые*/
    public GameObject Crossbow;
    public List<Image> HpImage;
    public Image LImage;
    public Image ArrImage;
    public Sprite CrossbowDefaultSprite;
    public List<Sprite> ArrowStackSprites;
    public List<Sprite> CrossbowSingle; // спрайты арбалета одиночной стрелы
    public List<Sprite> CrossbowMultiple; // спрайты арбалета тройной стрелы
    public List<Sprite> CrossbowFire; // спрайты арбалета огненной стрелы
    public List<Sprite> CrossbowSlow; // спрайты арбалета замедления стрелы
    public List<Sprite> CrossbowBomb; // спрайты арбалета взрывной стрелы
    public GameObject ArrowSingle;
    public GameObject ArrowFire;
    public GameObject ArrowSlow;
    public GameObject ArrowBomb;
    public GameObject Target;
    public GameObject Target1;

    public GameObject Mob1;
    public GameObject Mob2;
    public GameObject Mob3;
    public GameObject Mob4;
    public GameObject Mob5;
    public GameObject LCar;
    public GameObject LCarHp;
    public GameObject LCarArr;
    public GameObject Lvxm;
    public GameObject Arr;
    public GameObject Heart;
    public GameObject FireEffect;
    public GameObject SlowEffect;

    public Button SpellButton;
    public Image SpellCD;


    /*Игровые объекты*/
    GameClass.Crossbow CrossbowObj;
    GameClass.Spawn SpawnMob;
    List<List<Sprite>> TypesOfCrossbow;
    List<GameClass.Arrow> Arrows;
    List<GameClass.Enemy> Mobs;
    List<GameClass.Enemy> Skeletons;
    List<GameClass.LCar> LCars;
    List<GameClass.Basket> Basket;
    List<GameClass.Lavaxium> Lavaxis;

    /*Звуки*/
    public AudioSource MenuMusic;
    public AudioSource BattleMusic;
    public AudioSource CrossbowSound;
    public AudioSource DeathSound;
    public AudioSource HitSound;
    public AudioSource BombSound;
    public AudioSource LCarArrSound;
    public AudioSource LCarLavSound;
    public AudioSource LCarHealthSound;
    public AudioSource IncamSound;
    public AudioSource SpellSound;
    public AudioSource VapeSound;
    public AudioSource Necromant;
    public AudioSource Skeleton;
    //public AudioSource DeathSound;
    /*Игровые данные*/
    bool Start = false; // начата ли игра
    bool Spawn = true; // можно ли спавнить врагов
    public static bool Regen = false; // можно ли регенить
    public static bool ExitGO = false; // можно ли выходить
    public static bool AdsComplete = false; // реклама не просмотрена
    int Score = 0; // текущий счет
    Vector2 CrossbowStartTap = new Vector2(0, 0);
    bool CanFire = false; // можно ли натягивать арбалет
    int Combo = 0; // текущее число комбо
    int Arrow = 0;
    int Hp = 3; // текущее количество сердечек
    bool Resuming = true; // возможно ли продолжить игру за просмотр рекламного ролика
    bool Record = false; // новый ли рекорд сейчас
    int GameTime = 0; // игровое время
    bool NewBestCombo=false; // был ли отчет о новом рекорде комбо
    int SpellStatus = 2; // 0 - кд/1-кд закончилось/2-готово к использованию
    float LastSpell = 0;
    bool SecondSimple = false;
    bool SecondBomb = false;
    Dictionary<string, int> Kills=new Dictionary<string, int>();
    /*Заполняем те типы арбалеты, которые открыты*/
    void InsertTypesOfCrossbow()
    {
        TypesOfCrossbow.Add(CrossbowSingle);
        TypesOfCrossbow.Add(CrossbowMultiple);
        TypesOfCrossbow.Add(CrossbowFire);
        TypesOfCrossbow.Add(CrossbowSlow);
        TypesOfCrossbow.Add(CrossbowBomb);
    }
    /*Подготовка к игре*/
    private void OnEnable()
    {
        BattleMusic.Play();
        Kills.Clear();
        Kills.Add("skeleton", 0);
        Kills.Add("ghost", 0);
        Kills.Add("knight", 0);
        Kills.Add("boss", 0);
        Kills.Add("necromant", 0);
        SpellCD.fillAmount = 0;
        ClearObj();
        NewObj();
        InsertTypesOfCrossbow();
        GameInterface.SetActive(true);
        CrossbowObj = new GameClass.Crossbow(Crossbow, TypesOfCrossbow, CrossbowDefaultSprite, ArrowsStack, ArrowStackSprites, Target, Target1); // инициализируем объект арбалет
        Menu.Score.Score(Score, false, true, Record);
        Menu.Score.Arrow(Arrow, true);
        AdsComplete = false;
        Start = true;
        Hp = Menu.Hp;
        for (int i = 0; i < HpImage.Count; i++)
        {
            if (i < Hp)
                HpImage[i].enabled = true;
            else
                HpImage[i].enabled = false;
        }
        Pausee.interactable = true;
        SpellButton.GetComponent<RectTransform>().position = new Vector2(SpellButton.GetComponent<RectTransform>().position.x, Screen.height / 5 + 200);
        SpellButton.interactable = true;
    }
    /*Обработка паузы*/
    public void Pause()
    {
        Start = false;
        PauseInterface.SetActive(true);
        Pausee.interactable = false;
    }
    /*Продолжить*/
    public void Resume()
    {
        Time.timeScale = 1;
        Start = true;
        PauseInterface.SetActive(false);
        Pausee.interactable = true;
    }
    /*Декрементация сердечек*/
    void HpDec()
    {
        if (Hp==1) 
        {
            Spawn = false;
            Start = false;
            Hp--; 
            HpImage[Hp].enabled = false;
            for (int i = 0; i < Mobs.Count; i++)
            {
                Mobs[i].GetDamage(2, GameTime);
                Basket.Add(new GameClass.Basket(Mobs[i].Object, GameTime));
            }
            
            for (int i = 0; i < Skeletons.Count; i++)
                Skeletons[i].Remove();
            Skeletons.Clear();
            GameInfo.InfoStack.Clear();
            GameInfo.InfoStack.Add(10);
            GameInfoo.SetActive(false);
            GameInfoo.SetActive(true);
            Pausee.interactable = false;
            if (Resuming) { Invoke("SuggestToContinue", 1.5f); }
            else
                Invoke("Statistic", 1.5f);
            return; 
        }
        else if(Hp>0 && Hp<Menu.Hp+1) { Hp--; HpImage[Hp].enabled = false; }
    }
    /*Инкрементация сердечек*/
    void HpInc()
    {
        HpImage[Hp-1].enabled = true;
    }
    /*Меню для продолжения игры*/
    void SuggestToContinue() 
    {
        GameOvr.SetActive(true);
    }
    /*Продолжение игры */
    public void ContinueWithMoney()
    {
        /*Продолжение игры после просмотра рекламного ролика*/
        Resuming = false;
        GameOver.can = false;
        Pausee.interactable = true;
        GameOvr.SetActive(false);
        Hp = Menu.Hp;
        for (int i = 0; i < HpImage.Count; i++)
        {
            if (i < Hp)
                HpImage[i].enabled = true;
            else
                HpImage[i].enabled = false;
        }
        ClearBasket(true);
        ScoreControl(0, false, true);
        Mobs.Clear();
        Start = true;
        Spawn = true;
    }
    public void GameOverr()
    {
        Clear();
        PauseInterface.SetActive(false);
        GameInterface.SetActive(false);
        GameOvr.SetActive(false);
        MainMenu.SetActive(true);
        gameObject.SetActive(false);
        Statistica.SetActive(false);
    }
    void Statistic()
    {
        SpellSound.Play();
        Effect.SetActive(false);
        Effect.SetActive(true);
        ScoreControl(Combo, false, true);
        Statistica.SetActive(true);
        Menu.Score.Statistic(Kills);
        GameOvr.SetActive(false);
        Clear();
    }
    public void Exit()
    {
        Spawn = false;
        Time.timeScale = 1;
        GameOverr();
        ClearObj();
        Menu.Score.SetNotActiveScore();
        Menu.Score.SetNotActiveCombo();
        Menu.Score.SetNotActiveArr();
        BattleMusic.Stop();
        MenuMusic.Play();
        SoundBlock.SetActive(true);
    }
    /*Отчищаем всю старую игру*/
    void Clear()
    {
        Start = false;
        Spawn = true;
        Regen = false;
        Record = false;
        AdsComplete = false;
        NewBestCombo = false;
        GameTime = 0;
        Arrow = 0;
        SpellStatus = 2;
        LastSpell = 0;
        SecondBomb = false;
        SecondSimple = false;
        SpellCD.fillAmount = 0;
        SpellButton.interactable = true;
        SpellButton.gameObject.SetActive(false);
        SpellButton.gameObject.SetActive(true);
        SpellStatus = 2;
        SpellSound.Play();
        SpellCD.fillAmount = 0;
        Kills["skeleton"] = 0;
        Kills["ghost"] = 0;
        Kills["knight"] = 0;
        Kills["boss"] = 0;
        Kills["necromant"] = 0;
        Menu.Score.Arrow(Arrow, true);
        if (Menu.BestScore < Score)
            Menu.BestScore = Score;
        Score = 0;
        CrossbowStartTap = new Vector2(0, 0);
        CanFire = false;
        Combo = 0;
        Hp = Menu.Hp;
        ClearBasket(true);
        for (int i = 0; i < 5; i++)
            HpImage[i].enabled = true;
        Resuming = true;
        Menu.Score.SetNotActiveScore();
        Menu.Score.SetNotActiveCombo();
        Menu.Score.Score(Score, true, true, Record);
        CrossbowObj.Remove();
        for (int i=0; i<HpImage.Count; i++)
        {
            if (i < Hp)
                HpImage[i].enabled = true;
            else
                HpImage[i].enabled = false;
        }
    }
    void ClearObj()
    {
        if (Mobs != null && Mobs.Count > 0)
            for (int i = 0; i < Mobs.Count; i++)
                Mobs[i].Remove();
        if (Arrows != null && Arrows.Count > 0)
            for (int i = 0; i < Arrows.Count; i++)
                Arrows[i].Remove();
        if (Lavaxis != null && Lavaxis.Count > 0)
            for (int i = 0; i < Lavaxis.Count; i++)
                Lavaxis[i].Remove();
        if (LCars != null && LCars.Count > 0)
            for (int i = 0; i < LCars.Count; i++)
                LCars[i].Remove();
    }
    void NewObj()
    {
        TypesOfCrossbow = new List<List<Sprite>>();
        Arrows = new List<GameClass.Arrow>();
        Mobs = new List<GameClass.Enemy>();
        LCars = new List<GameClass.LCar>();
        Basket = new List<GameClass.Basket>();
        Lavaxis = new List<GameClass.Lavaxium>();
        Skeletons = new List<GameClass.Enemy>();
        SpawnMob = new GameClass.Spawn();
    }
    public void Restart()
    {
        Clear();
        GameOvr.SetActive(false);
        PauseInterface.SetActive(false);
        Statistica.SetActive(false);
        Time.timeScale = 1;
        /*Нужно отчистить все поле*/
        ClearObj();
        NewObj();
        InsertTypesOfCrossbow();
        GameInterface.SetActive(true);
        CrossbowObj = new GameClass.Crossbow(Crossbow, TypesOfCrossbow, CrossbowDefaultSprite, ArrowsStack, ArrowStackSprites, Target, Target1); // инициализируем объект арбалет
        Menu.Score.Score(Score, false, true, Record);
        Start = true;
        Pausee.interactable = true;
    }
    /*Логика спавна всех сущностей*/
    void SpawnMobs()
    {
        if (!Spawn)
            return;
        List<int> mobs = new List<int>();
        if (SpawnMob.GetCurrentSec() == 1)
            mobs = SpawnMob.Sec1;
        else if (SpawnMob.GetCurrentSec() == 2)
            mobs = SpawnMob.Sec2;
        else if (SpawnMob.GetCurrentSec() == 3)
            mobs = SpawnMob.Sec3;
        else if (SpawnMob.GetCurrentSec() == 4)
            mobs = SpawnMob.Sec4;
        else if (SpawnMob.GetCurrentSec() == 5)
            mobs = SpawnMob.Sec5;
        else if(SpawnMob.GetCurrentSec()>5)
        {
            SpawnMob.CalculateNewQuant(GameTime, Score, Mobs.Count);
            return;
        }
        SpawnMob.CurrentSecInc();
        for (int i=0; i<mobs.Count; i++)
        {
            int rand_x = Random.Range(Screen.width/25, Screen.width-Screen.width/25);
            Vector2 spawn_pos = Camera.main.ScreenToWorldPoint(new Vector2(rand_x, Screen.height));
            if (mobs[i]==1)
                Mobs.Add(new GameClass.Enemy(Mob1, new Vector2(spawn_pos.x, spawn_pos.y + Random.Range(3,6)), 2, 0.008f, 0.8f, "skeleton", FireEffect, SlowEffect));
            else if (mobs[i] == 2)
                Mobs.Add(new GameClass.Enemy(Mob2, new Vector2(spawn_pos.x, spawn_pos.y + Random.Range(3, 6)), 2, 0.03f, 0.85f, "ghost", FireEffect, SlowEffect));
            else if(mobs[i] == 3)
                Mobs.Add(new GameClass.Enemy(Mob3, new Vector2(spawn_pos.x, spawn_pos.y + Random.Range(3, 6)), 4, 0.009f, 0.9f, "knight", FireEffect, SlowEffect));
            else if(mobs[i] == 4)
                Mobs.Add(new GameClass.Enemy(Mob5, new Vector2(spawn_pos.x, spawn_pos.y + Random.Range(3, 6)), 10, 0.006f, 1.1f, "boss", FireEffect, SlowEffect));
            else if(mobs[i] == 5)
                Mobs.Add(new GameClass.Enemy(Mob4, new Vector2(spawn_pos.x, spawn_pos.y + Random.Range(3, 6)), 6, 0.007f, 0.9f, "necromant", FireEffect, SlowEffect));

        }
    }
    void SpawnLCars()
    {
        if (!Spawn)
            return;
        int random = Random.Range(0, 8);
        if (random != 5)
            return;
        int rand_y = Random.Range(Screen.height / 4, Screen.height - 100);
        int rand = Random.Range(0, 2);
        if (rand==0)
        {
            Vector2 spawn_pos = Camera.main.ScreenToWorldPoint(new Vector2(-50, rand_y));
            rand = Random.Range(0, 7);
            if (rand == 0 || rand==1 || rand==2)
                LCars.Add(new GameClass.LCar(LCar, true, spawn_pos, 1));
            else if (rand == 3)
                LCars.Add(new GameClass.LCar(LCarHp, true, spawn_pos, 2));
            else
                LCars.Add(new GameClass.LCar(LCarArr, true, spawn_pos, 3));

        }
        else
        {
            Vector2 spawn_pos = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width+50, rand_y));
            rand = Random.Range(0, 7);
            if (rand == 0 || rand == 1 || rand==2)
                LCars.Add(new GameClass.LCar(LCar, false, spawn_pos, 1));
            else if (rand == 3)
                LCars.Add(new GameClass.LCar(LCarHp, false, spawn_pos, 2));
            else
                LCars.Add(new GameClass.LCar(LCarArr, false, spawn_pos, 3));
        }

    }
    void ScoreControl(int delta_score, bool combo, bool change)
    {
        Score+=delta_score;
        if (combo) Combo++;
        else Combo = 0;
        if (Combo > Menu.BestCombo)
        {
            Menu.BestCombo = Combo;
            if (!NewBestCombo)
            {
                GameInfo.InfoStack.Add(8); // новый рекорд по комбо
                GameInfoo.SetActive(true);
                NewBestCombo = true;
            }
            if (Menu.BestCombo==10)
            {
                GameInfo.InfoStack.Add(0);
                GameInfoo.SetActive(true);
            }
            else if (Menu.BestCombo == 20)
            {
                GameInfo.InfoStack.Add(1);
                GameInfoo.SetActive(true);
            }
            else if (Menu.BestCombo == 30)
            {
                GameInfo.InfoStack.Add(2);
                GameInfoo.SetActive(true);
            }
            else if (Menu.BestCombo == 40)
            {
                GameInfo.InfoStack.Add(3);
                GameInfoo.SetActive(true);
            }
            else if (Menu.BestCombo == 50)
            {
                GameInfo.InfoStack.Add(4);
                GameInfoo.SetActive(true);
            }
            else if (Menu.BestCombo == 60)
            {
                GameInfo.InfoStack.Add(5);
                GameInfoo.SetActive(true);
            }
        }
        if (Combo%5==0 && Combo!=0)
        {
            GameInfo.InfoStack.Add(Random.Range(6,8)); // подбадривающие сообщения
            GameInfoo.SetActive(true);
        }
        if (!Record && Score>Menu.BestScore)
        {
            Record = true;
            GameInfo.InfoStack.Add(9); // новый рекорд по счету
            GameInfoo.SetActive(true);
        }
        if (Score > Menu.BestScore)
            Menu.BestScore = Score;
        bool temp = false;
        if (Combo >= 2) temp = true;
        if (temp && combo)
            Menu.Score.Combo(Combo);
        else if (!combo)
            Menu.Score.SetNotActiveCombo();
        Menu.Score.Score(Score, temp, change, Record);
        
    }
    void ArrowsAndEnemy(int ind)
    {
        float body = Mobs[ind].GetBody();
       for (int i=0; i< Arrows.Count; i++)
        {
            float dist = Arrows[i].Distance(new Vector2(Mobs[ind].GetTransform().position.x, Mobs[ind].GetTransform().position.y+0.5f));
            if (dist < body)
            {
                if (Mobs[ind].GetDamage(Arrows[i].Type, GameTime)) // если стрела убила
                {
                    if (Arrows[i].Type == 5)
                    {
                        Vector2 pos = Mobs[ind].GetTransform().position;
                        for (int j = 0; j < Mobs.Count; j++)
                        {
                            if (Vector3.Distance(pos, Mobs[j].GetTransform().position) <= 2 && j != ind)
                            {
                                Mobs[j].HpInc();
                                Mobs[j].GetDamage(3, GameTime);
                            }
                        }
                        BombSound.Play();
                    }
                    else
                        DeathSound.Play();
                    if (Mobs[ind].Type == "boss")
                        Lavaxis.Add(new GameClass.Lavaxium(Lvxm, Mobs[ind].GetTransform().position, 1, 0));
                    Basket.Add(new GameClass.Basket(Mobs[ind].Object, GameTime));
                    Mobs[ind].Object.GetComponent<SortOrder>().enabled = false;
                    Mobs[ind].SpriteRenderer.sortingOrder = 1;
                    if (Mobs[ind].Type == "skeleton" && Arrows[i].Type != 2)
                        Skeletons.Add(Mobs[ind]);
                    Kills[Mobs[ind].Type]++;
                    Mobs.RemoveAt(ind);
                    ScoreControl(1, true, true);
                    Arrows[i].Destruct();
                    Arrows.RemoveAt(i);
                    return;
                }
                else
                    HitSound.Play();
                Arrows[i].Destruct();
                Arrows.RemoveAt(i);
                break;
            }
        }
       if (Mobs[ind].Type == "necromant" && !Mobs[ind].Cast && Skeletons.Count>0 &&
            Camera.main.WorldToScreenPoint(Mobs[ind].GetTransform().position).y<=Screen.height/2+Screen.height/5)
        { Mobs[ind].Cast = true; Mobs[ind].StartCast(); Necromant.Play(); }
       else if((Mobs[ind].Type == "necromant" && Mobs[ind].Cast && Mobs[ind].GetCntrl()==0)
            || (Mobs[ind].Type == "necromant" && !Mobs[ind].Cast))
            Mobs[ind].Move();
       else if(Mobs[ind].Type != "necromant")
            Mobs[ind].Move();
        if ((Mobs[ind].GetStatus() == 1 || Mobs[ind].GetStatus() == 3) && GameTime - Mobs[ind].GetLastFire() >= 1)
        {
            if (Mobs[ind].GetFireDamage(GameTime))
            {
                if (Mobs[ind].Type=="boss")
                    Lavaxis.Add(new GameClass.Lavaxium(Lvxm, Mobs[ind].GetTransform().position, 1, 0));
                Basket.Add(new GameClass.Basket(Mobs[ind].Object, GameTime));
                Kills[Mobs[ind].Type]++;
                VapeSound.Play();
                Mobs.RemoveAt(ind);
                ScoreControl(1, true, true);
            }
        }
            

    }
    void WallAndEnemy(int ind)
    {
        if (ind < 0 || ind >= Mobs.Count)
            return;
        if (Camera.main.WorldToScreenPoint(Mobs[ind].GetTransform().position).y<=Screen.height/5+ Screen.height/25)
        {
            Mobs[ind].GetDamage(2, GameTime);
            HpDec();
            Basket.Add(new GameClass.Basket(Mobs[ind].Object, GameTime));
            VapeSound.Play();
            Mobs.RemoveAt(ind);
            Camera.main.transform.gameObject.GetComponent<Animator>().SetTrigger("cntrl");
        }
    }
    void ArrowsAndLCar(int ind)
    {
        for (int i = 0; i < Arrows.Count; i++)
        {
            float dist= Arrows[i].Distance(LCars[ind].GetTransform().position);
            if (dist<0.9f)
            {
                LCars[ind].Animator.SetInteger("cntrl", 2);
                if (LCars[ind].Type==1)
                {
                    DeathSound.Play();
                    LCarLavSound.Play();
                    int num = Random.Range(8, 12);
                    for (int j = 0; j < num; j++)
                        Lavaxis.Add(new GameClass.Lavaxium(Lvxm, LCars[ind].GetTransform().position, 1, 0));
                }
                else if(LCars[ind].Type == 2 && Hp < Menu.Hp)
                {
                    DeathSound.Play();
                    LCarHealthSound.Play();
                    Hp++;
                    Lavaxis.Add(new GameClass.Lavaxium(Heart, LCars[ind].GetTransform().position, 2, 2)); 
                }
                else if(LCars[ind].Type == 3)
                {
                    LCarArrSound.Play();
                    int num = Random.Range(8, 12);
                    for (int j = 0; j < num; j++)
                        Lavaxis.Add(new GameClass.Lavaxium(Arr, LCars[ind].GetTransform().position, 3, 0));
                }
                else if(LCars[ind].Type == 2)
                    DeathSound.Play();
                Basket.Add(new GameClass.Basket(LCars[ind].Object, GameTime));
                LCars[ind].SpriteRenderer.sortingOrder = 1;
                LCars.RemoveAt(ind);
                Arrows[i].Remove();
                Arrows.RemoveAt(i);
                return;
            }
        }
        if (LCars[ind].TurnRight) // направо едет
        {
            LCars[ind].Move(100);
            if (Camera.main.WorldToScreenPoint(LCars[ind].GetTransform().position).x >= Screen.width + 200)
            {
                LCars[ind].Remove();
                LCars.RemoveAt(ind);
            }
        }
            
        else // налево едет
        {
            LCars[ind].Move(-100);
            if (Camera.main.WorldToScreenPoint(LCars[ind].GetTransform().position).x <= -200)
            {
                LCars[ind].Remove();
                LCars.RemoveAt(ind);
            }
        }
            
    }
    void LavaxAndHp(int ind)
    {
        if (Lavaxis[ind].Status == 0)
            Lavaxis[ind].Move(Lavaxis[ind].Dest);
        else if (Lavaxis[ind].Status == 1 && Time.realtimeSinceStartup - Lavaxis[ind]._Time >= 1)
            Lavaxis[ind].Status = 2;
        else if (Lavaxis[ind].Status == 2 && Lavaxis[ind].Type==1)
            Lavaxis[ind].Move(Camera.main.ScreenToWorldPoint(LImage.rectTransform.position));
        else if (Lavaxis[ind].Status == 2 && Lavaxis[ind].Type == 3)
            Lavaxis[ind].Move(Camera.main.ScreenToWorldPoint(ArrImage.rectTransform.position));
        else if (Lavaxis[ind].Status == 2 && Lavaxis[ind].Type == 2)
            Lavaxis[ind].Move(Camera.main.ScreenToWorldPoint(HpImage[Hp-1].rectTransform.position));
        else if (Lavaxis[ind].Status == 3)
        {
            IncamSound.Play();
            if (Lavaxis[ind].Type == 1) { Menu.Lavaxium++; Menu.Score.Lavaxium(Menu.Lavaxium, true); }
            else if (Lavaxis[ind].Type == 2) { HpInc(); }
            else if(Lavaxis[ind].Type == 3) { Arrow++; Menu.Score.Arrow(Arrow, true); }
            Lavaxis[ind].Remove();
            Lavaxis.RemoveAt(ind);
        }
    }
    /*Управление мусоркой*/
    void ClearBasket(bool sos)
    {
       if (!sos)
        {
            for (int i = 0; i < Basket.Count; i++)
            {
                if (GameTime - Basket[i].Time >= 20) 
                { 
                    Basket[i].Remove();
                    for (int j = 0; j < Skeletons.Count; j++)
                        if (Skeletons[j].Object == Basket[i].GetGameObject()) { Skeletons.RemoveAt(j); break; }
                    Basket.RemoveAt(i); 
                }
            }      
        }     
       else
        {
            for (int i = 0; i < Basket.Count; i++) { Basket[i].Remove(); /*Basket.RemoveAt(i);*/ }
            Basket.Clear();
        }     
    }
    void Regenerations()
    {
        if (!Regen)
        {
            for (int i=0; i<Skeletons.Count; i++)
            {
                if (Skeletons[i].Cast && Skeletons[i].GetCntrl()==0)
                {
                    for (int j=0; j<Basket.Count; j++)
                    {
                        if (Basket[j].GetGameObject()== Skeletons[i].Object)
                        {
                            Basket.RemoveAt(j);
                            break;
                        }
                    }
                    Skeleton.Play();
                    Skeletons[i].SkeletonReLive();
                    Mobs.Add(Skeletons[i]);
                    Skeletons.RemoveAt(i);
                }
            }
            return;
        }
        Regen = false;
        for (int i=0; i<Skeletons.Count; i++)
        {
            Skeletons[i].Object.GetComponent<SortOrder>().enabled = true;
            Skeletons[i].StartCast();
            Skeletons[i].Cast = true;
        }

    }
    void CheckSpell()
    {
        if (SpellStatus==0)
        {
            if (SpellCD.fillAmount == 1)
            {
                SpellCD.fillAmount = 0;
                SpellButton.interactable = true;
                SpellButton.gameObject.SetActive(false);
                SpellButton.gameObject.SetActive(true);
                SpellStatus = 2;
                SpellSound.Play();
            }
            else
                SpellCD.fillAmount += 0.003f;
        }
        else if(SpellStatus==1 && Arrow>=20)
        {
            
            Arrow -= 20;
            Menu.Score.Arrow(Arrow, true);
            SpellStatus = 0;  
        }
    }
    public void Spell()
    {
        CrossbowSound.Play();
        SpellStatus = 1;
        SpellButton.interactable = false;
        if (Menu.SpellLevel==0)
        {
            for (int i = 1; i < 15; i++)
                Arrows.Add(new GameClass.Arrow(ArrowSingle, Camera.main.ScreenToWorldPoint(new Vector2(i * Screen.width / 15, -100)), Quaternion.identity, 0, 1));
        }
        else if(Menu.SpellLevel==1)
        {
            for (int i = 1; i < 15; i++)
                Arrows.Add(new GameClass.Arrow(ArrowFire, Camera.main.ScreenToWorldPoint(new Vector2(i * Screen.width / 15, -100)), Quaternion.identity, 0, 3));
        }
        else if(Menu.SpellLevel==2)
        {
            for (int i = 1; i < 15; i++)
                Arrows.Add(new GameClass.Arrow(ArrowSingle, Camera.main.ScreenToWorldPoint(new Vector2(i * Screen.width / 15, -100)), Quaternion.identity, 0, 1));
            LastSpell = Time.realtimeSinceStartup;
            SecondSimple = true;
        }
        else if(Menu.SpellLevel == 3)
        {
            for (int i = 1; i < 15; i++)
                Arrows.Add(new GameClass.Arrow(ArrowBomb, Camera.main.ScreenToWorldPoint(new Vector2(i * Screen.width / 15, -100)), Quaternion.identity, 0, 5));
        }
        else if(Menu.SpellLevel==4)
        {
            for (int i = 1; i < 15; i++)
                Arrows.Add(new GameClass.Arrow(ArrowBomb, Camera.main.ScreenToWorldPoint(new Vector2(i * Screen.width / 15, -100)), Quaternion.identity, 0, 5));
            LastSpell = Time.realtimeSinceStartup;
            SecondBomb = true;
        }
        
    }
    float last = 0;
    void Timer()
    {
        if(Time.realtimeSinceStartup-last>=1)
        {
            GameTime++;
            SpawnMobs();
            SpawnLCars();
            last = Time.realtimeSinceStartup;
        }
    }
    private void Update()
    {
        if (ExitGO)
        {
            ExitGO = false;
            Statistic();
        }
        if (AdsComplete)
        {
            AdsComplete = false;
            ContinueWithMoney();
        }
        if (!Start)
            return;
        
        Timer(); 
        if (SecondBomb && Time.realtimeSinceStartup-LastSpell>=0.2f)
        {
            SecondBomb = false;
            for (int i = 1; i < 15; i++)
                Arrows.Add(new GameClass.Arrow(ArrowBomb, Camera.main.ScreenToWorldPoint(new Vector2(i * Screen.width / 15, -100)), Quaternion.identity, 0, 5));
        }
        if (SecondSimple && Time.realtimeSinceStartup-LastSpell>=0.2f)
        {
            SecondSimple = false;
            for (int i = 1; i < 15; i++)
                Arrows.Add(new GameClass.Arrow(ArrowSingle, Camera.main.ScreenToWorldPoint(new Vector2(i * Screen.width / 15, -100)), Quaternion.identity, 0, 1));
        }
        /*Управление баллистой*/
        if (Input.GetMouseButtonDown(0) && !CrossbowObj.IsCoolDown)
        {
            CrossbowStartTap = Input.mousePosition;
            if (Vector2.Distance(CrossbowObj.GetTransform().position, Camera.main.ScreenToWorldPoint(CrossbowStartTap)) < 2)
            {
                CanFire = true;
            }
               
        }
        if (Input.GetMouseButton(0) && CanFire && !CrossbowObj.IsCoolDown)
        {
            CrossbowObj.Rotate(Input.mousePosition);
            CrossbowObj.FirePreparation(Vector2.Distance(Input.mousePosition, CrossbowStartTap));
        }
        if (Input.GetMouseButtonUp(0) && !CrossbowObj.IsCoolDown)
        {
            if (CanFire)
            {
                CrossbowSound.Play();
                CrossbowObj.Fire();
                int type = CrossbowObj.GetCurrentType();
                if (type==0)
                    Arrows.Add(new GameClass.Arrow(ArrowSingle, CrossbowObj.GetTransform().position, CrossbowObj.GetTransform().rotation, CrossbowObj.GetTransform().rotation.eulerAngles.z, 1));
                else if(type==1)
                {
                    Arrows.Add(new GameClass.Arrow(ArrowSingle, CrossbowObj.GetTransform().position, CrossbowObj.GetTransform().rotation, CrossbowObj.GetTransform().rotation.eulerAngles.z, 1));
                    Vector2 dest = Arrows[Arrows.Count - 1].Dest;
                    Arrows.Add(new GameClass.Arrow(ArrowSingle, CrossbowObj.GetTransform().position, CrossbowObj.GetTransform().rotation, new Vector2(dest.x+5, dest.y), 1));
                    Arrows.Add(new GameClass.Arrow(ArrowSingle, CrossbowObj.GetTransform().position, CrossbowObj.GetTransform().rotation, new Vector2(dest.x - 5, dest.y), 1));
                }
                else if(type==2)
                    Arrows.Add(new GameClass.Arrow(ArrowFire, CrossbowObj.GetTransform().position, CrossbowObj.GetTransform().rotation, CrossbowObj.GetTransform().rotation.eulerAngles.z, 3));
                else if(type==3)
                    Arrows.Add(new GameClass.Arrow(ArrowSlow, CrossbowObj.GetTransform().position, CrossbowObj.GetTransform().rotation, CrossbowObj.GetTransform().rotation.eulerAngles.z, 4));
                else if (type == 4)
                    Arrows.Add(new GameClass.Arrow(ArrowBomb, CrossbowObj.GetTransform().position, CrossbowObj.GetTransform().rotation, CrossbowObj.GetTransform().rotation.eulerAngles.z, 5));
                //Menu.Arrows--;
                //Menu.Score.Arrow(Menu.Arrows, false);
            }
                
            CanFire = false;
        }
        if (Menu.Target)
        {
            CrossbowObj.TargetTransform.position = new Vector2(Mathf.Lerp(CrossbowObj.TargetTransform.position.x, CrossbowObj.TargetDest.x, 15 * Time.deltaTime),
           Mathf.Lerp(CrossbowObj.TargetTransform.position.y, CrossbowObj.TargetDest.y, 15 * Time.deltaTime));
            CrossbowObj.Target1Transform.position = new Vector2(Mathf.Lerp(CrossbowObj.Target1Transform.position.x, CrossbowObj.Target1Dest.x, 15 * Time.deltaTime),
                Mathf.Lerp(CrossbowObj.Target1Transform.position.y, CrossbowObj.Target1Dest.y, 15 * Time.deltaTime));
        }
        
        CrossbowObj.CoolDown();
        /*Управление стрелами*/
        for (int i = 0; i < Arrows.Count; i++)
        {
            Arrows[i].Move();
            Vector2 temp = Camera.main.WorldToScreenPoint(Arrows[i].GetTransform().position);
            if (temp.x>=Screen.width+100 || temp.x<=-100 || temp.y>=Screen.height+50)
            {
                bool change=true;
                if (Combo == 1 || Combo==0)
                {
                    Combo = 0;
                    change = false;
                }
                    
                ScoreControl(Combo, false, change);
                Arrows[i].Remove();
                Arrows.RemoveAt(i);
            }
                
        }
        /*Убийство мобов стрелами, убийство стены мобами*/
        for (int i=0; i<Mobs.Count; i++)
        {
            ArrowsAndEnemy(i);
            WallAndEnemy(i);
        }
        /*Управление l-карами*/
        for (int i = 0; i < LCars.Count; i++)
            ArrowsAndLCar(i);
        /*Управление лутом (даваксий и сердечки)*/
        for (int i = 0; i < Lavaxis.Count; i++)
            LavaxAndHp(i);
        /*Управление регеном*/
        Regenerations();
        /*Отчистка мусора с карты*/
        ClearBasket(false);
        CheckSpell();
    }
}
