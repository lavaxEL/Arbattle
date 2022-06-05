using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameClass : MonoBehaviour
{
    /*Класс баллисты*/
    public class Crossbow
    {
        public bool IsCoolDown;
        public int CurrentType; // 0 - simple/1 - triple/2 - fire/3 - slow/4 - bomb
        public int NextType;

        private GameObject Object;
        private Transform Transform;
        private Sprite DefaultSprite;
        private float Angle;
        private float LastFireTime;

        private List<List<Sprite>> AllCrossbows;
        private List<Sprite> CurrentsPics; // текущие спрайты арбалета
        private List<Sprite> NextPics; // ледующие спрайты арбалета
        private List<Sprite> ArrowsPics; // спрайты из очереди стрел
        private GameObject ArrowsStack; // очередь стрел
        private Image NextArrowImg; // само изображение стрелы следующей
        private Image CurArrowImg; // само изображение стрелы текущей

        private GameObject Target;
        public Transform TargetTransform;
        private GameObject Target1;
        public Transform Target1Transform;
        public Vector2 TargetDest;
        public Vector2 Target1Dest;
        public Crossbow(GameObject obj, List<List<Sprite>> sprites, Sprite default_sprite, GameObject arrows_stack, List<Sprite> arrows, GameObject target, GameObject target1)
        {
            Angle = 90;
            LastFireTime = 0;
            IsCoolDown = false;
            CurrentType = 0;
            NextType = 0;
            AllCrossbows = sprites;
            //Vector2 position = Camera.main.ScreenToWorldPoint(new Vector2((Screen.width / 2) - 22, 275));
            Object = Instantiate(obj, Menu.CrossPos, Quaternion.identity);
            Transform = Object.transform;
            DefaultSprite = default_sprite;
            Object.GetComponent<SpriteRenderer>().sprite = sprites[0][0];
            CurrentsPics = sprites[0];
            NextPics = sprites[0];
            ArrowsStack = arrows_stack;
            ArrowsPics = arrows;
            ArrowsStack.SetActive(true);
            NextArrowImg = ArrowsStack.transform.Find("NextArr").gameObject.GetComponent<Image>();
            CurArrowImg= ArrowsStack.transform.Find("CurArr").gameObject.GetComponent<Image>();
            NextArrowImg.sprite = ArrowsPics[0];
            CurArrowImg.sprite = ArrowsPics[0];
            ArrowStackSetActive();
            Target = Instantiate(target, Camera.main.ScreenToWorldPoint(new Vector2(Screen.width / 2, Screen.height)), Quaternion.identity);
            TargetTransform = Target.transform;
            Target1 = Instantiate(target1, Camera.main.ScreenToWorldPoint(new Vector2(Screen.width / 2, Screen.height)), Quaternion.identity);
            Target1Transform = Target1.transform;
            TargetDest = Target.transform.position;
            Target1Dest = Target1.transform.position;
            if (!Menu.Target)
            {
                Target.SetActive(false);
                Target1.SetActive(false);
            }
        }
        public void Rotate(Vector2 pos)
        {
            if (pos.y > Camera.main.WorldToScreenPoint(Transform.position).y - 50 && pos.x > Screen.width / 2 - 50 && pos.x < Screen.width / 2 + 50)
                return;
            pos = Camera.main.ScreenToWorldPoint(pos);
            var angle = Vector2.Angle(Vector2.right, pos - new Vector2(Transform.position.x, Transform.position.y));
            if (pos.y >= Transform.position.y && pos.x > Transform.position.x)
                angle = 0;
            else if (pos.y >= Transform.position.y && pos.x < Transform.position.x)
                angle = 180;
            if (Mathf.Abs(Angle - angle) < 2.5f)
                return;
            else if (Angle > angle)
                angle = Angle - 1.8f;
            else
                angle = Angle + 1.8f;
            Angle = angle;
            Transform.eulerAngles = new Vector3(0f, 0f, -angle + 90);
            MoveTarget(Transform.position, Transform.rotation.eulerAngles.z);
        }
        private void MoveTarget(Vector2 source, float angle)
        {
            Vector2 dest;
            float alpha;
            if (angle > 90)
            {
                alpha = (angle - 360) * Mathf.PI / 180;
                float y = source.y + 50;
                float b = 50 / Mathf.Cos(alpha) * (50 / Mathf.Cos(alpha));
                float c= Mathf.Sqrt(b - 2500);
                float x = source.x + c;
                dest = new Vector2(x, y);
                float y1 = 0;
                float x1 = 0;
                

                x1 = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x - source.x;
                y1 = 50 * x1 / c;
                TargetDest = new Vector2(x1 + source.x, Mathf.Clamp(y1 + source.y, -100, Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height)).y) );
            }
            else
            {
                alpha = angle * Mathf.PI / 180;
                float y = source.y + 50;
                float b = 50 / Mathf.Cos(alpha) * (50 / Mathf.Cos(alpha));
                float c = Mathf.Sqrt(b - 2500);
                float x = source.x - c;
                dest = new Vector2(x, y);
                float y1 = 0;
                float x1 = 0;
                x1 = source.x-Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).x;
                y1 = 50 * x1 / c;
                if (y1 > 100)
                    return;
                TargetDest = new Vector2(source.x-x1, Mathf.Clamp(y1 + source.y, -100, Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height)).y));
            }
            float _y = Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height-Screen.height/22)).y;
            float _b = (_y-source.y )/ Mathf.Cos(alpha) * ((_y - source.y) / Mathf.Cos(alpha));
            float _c = Mathf.Sqrt(_b - (_y - source.y)*((_y - source.y)));
            if (100 < Mathf.Abs(_c))
                return;
            float _x;
            if (angle>90)
                _x= source.x + _c;
            else
                _x = source.x - _c;
            Target1Dest = new Vector2(_x, _y);
            if (Camera.main.WorldToScreenPoint(Target1Dest).x < 0 || Camera.main.WorldToScreenPoint(Target1Dest).x > Screen.width)
                Target1.SetActive(false);
            else if(Menu.Target)
                Target1.SetActive(true);
        }
        public void FirePreparation(float dist)
        {
            if (dist >= 10 && dist < 20)
                Object.GetComponent<SpriteRenderer>().sprite = CurrentsPics[1];
            else if (dist >= 20 && dist < 30)
                Object.GetComponent<SpriteRenderer>().sprite = CurrentsPics[2];
            else if (dist >= 30 && dist < 40)
                Object.GetComponent<SpriteRenderer>().sprite = CurrentsPics[3];
            else if (dist >= 40 && dist < 50)
                Object.GetComponent<SpriteRenderer>().sprite = CurrentsPics[4];
            else if (dist >= 50 && dist < 60)
                Object.GetComponent<SpriteRenderer>().sprite = CurrentsPics[5];
            else if (dist >= 60)
                Object.GetComponent<SpriteRenderer>().sprite = CurrentsPics[6];
        }
        public void Fire()
        {
            Object.GetComponent<SpriteRenderer>().sprite = DefaultSprite;
            LastFireTime = Time.realtimeSinceStartup;
            IsCoolDown = true;
        }
        private void SetNextArrowPic(int ind, int ind1)
        {
            ArrowsStack.SetActive(false);
            NextArrowImg.sprite = ArrowsPics[ind];
            CurArrowImg.sprite = ArrowsPics[ind1];
            ArrowsStack.SetActive(true);
        }
        private List<Sprite> ChooseNextPics()
        {
            int rand = Random.Range(0, 100);
            if (rand >= 0 && rand < 40)
                NextType = 0;
            else if(rand>=40 && rand<55)
            {
                if (Menu.TripleComplete)
                    NextType = 1;
                else
                    NextType = 0;
            }
            else if (rand >= 55 && rand < 70)
            {
                if (Menu.FireComplete)
                    NextType = 2;
                else
                    NextType = 0;
            }
            else if (rand >= 70 && rand < 85)
            {
                if (Menu.SlowComplete)
                    NextType = 3;
                else
                    NextType = 0;
            }
            else if (rand >= 85)
            {
                if (Menu.BombComplete)
                    NextType = 4;
                else
                    NextType = 0;
            }
            return AllCrossbows[NextType];
        }
        public void CoolDown()
        {
            if (!IsCoolDown || Time.realtimeSinceStartup - LastFireTime < 0.15f)
                return;
            CurrentsPics = NextPics;
            CurrentType = NextType;
            Object.GetComponent<SpriteRenderer>().sprite = CurrentsPics[0];
            NextPics = ChooseNextPics();
            SetNextArrowPic(NextType, CurrentType);
            IsCoolDown = false;
        }
        public Transform GetTransform(){ return Transform; }
        public void Remove() { Destroy(Object); NextArrowImg.enabled = false; CurArrowImg.enabled = false; Destroy(Target); Destroy(Target1); }
        public void ArrowStackSetActive() { NextArrowImg.enabled = true; CurArrowImg.enabled = true; }
        public int GetCurrentType() { return CurrentType; }
    }
    /*Класс стрел*/
    public class Arrow
    {
        public short Type; // 1 - обычная/3-огненная/4 - отравленная стрела/5 - взрывная
        public Vector2 Dest;

        private GameObject Object;
        private Transform Transform;
        private Animator Animator;
        public Arrow(GameObject obj, Vector2 source, Quaternion rotation, float angle, short type)
        {
            Object = Instantiate(obj, source, rotation);
            Animator = Object.GetComponent<Animator>();
            Type = type;
            Transform = Object.transform;
            if (angle > 90)
            {
                float alpha = (angle - 360) * Mathf.PI / 180;
                float y = source.y + 50;
                float b = 50 / Mathf.Cos(alpha) * (50 / Mathf.Cos(alpha));
                float x = source.x + Mathf.Sqrt(b - 2500);
                Dest = new Vector2(x, y);
            }
            else
            {
                float alpha = angle * Mathf.PI / 180;
                float y = source.y + 50;
                float b = 50 / Mathf.Cos(alpha) * (50 / Mathf.Cos(alpha));
                float x = source.x - Mathf.Sqrt(b - 2500);
                Dest = new Vector2(x, y);
            }
        }
        public Arrow(GameObject obj, Vector2 source, Quaternion rotation, Vector2 dest, short type)
        {
            Object = Instantiate(obj, source, rotation);
            Animator = Object.GetComponent<Animator>();
            Type = type;
            Transform = Object.transform;
            Dest = dest;
        }
        public void Move()
        {
            Transform.position = new Vector3(Mathf.Lerp(Transform.position.x, Dest.x, 1 * Time.deltaTime),
               Mathf.Lerp(Transform.position.y, Dest.y, 1 * Time.deltaTime), Transform.position.z);
        }
        public Transform GetTransform() { return Transform; }
        public float Distance(Vector2 dist){ return Vector2.Distance(Transform.position, dist); }
        public void Remove(){ Destroy(Object); }
        public void Destruct() { Animator.SetInteger("cntrl", 1); }
    }
    /*Класс мобов*/
    public class Enemy
    {

        public GameObject Object;
        public SpriteRenderer SpriteRenderer;
        public string Type;
        public bool Cast;

        private Transform Transform;
        private Animator Animator;
        private int HP;
        private float Speed;
        private float Body;
        private int Status; // 0 - вс ок/1 - горит/2 - отравлен/3- и горит и отравлен
        private int LastFire;

        private GameObject Fire;
        private GameObject Slow;
        private GameObject FireEffect;
        private GameObject SlowEffect;
        public Enemy(GameObject obj, Vector2 pos, int hp, float speed, float body, string type, GameObject fire, GameObject slow)
        {
            Object = Instantiate(obj, pos, Quaternion.identity);
            Transform = Object.transform;
            Animator = Object.GetComponent<Animator>();
            HP = hp;
            Speed = speed;
            SpriteRenderer = Object.GetComponent<SpriteRenderer>();
            Body = body;
            Type = type;
            Cast = false;
            Status = 0;
            Fire = fire;
            Slow = slow;
        }
        public Transform GetTransform(){ return Transform; }
        public void Move()
        {
            Transform.position = new Vector3(Transform.position.x,
               Mathf.Lerp(Transform.position.y, -100, Speed * Time.deltaTime), Transform.position.z);
            if (Status == 1)
                FireEffect.transform.position = new Vector2(Transform.position.x + 0.5f, Transform.position.y);
            else if (Status==2)
                SlowEffect.transform.position = new Vector2(Transform.position.x, Transform.position.y+0.3f);
            else if (Status==3)
            {
                FireEffect.transform.position = new Vector2(Transform.position.x + 0.5f, Transform.position.y);
                SlowEffect.transform.position = new Vector2(Transform.position.x, Transform.position.y+0.3f);
            }
        }
        public bool GetDamage(short arr_type, int time)
        {
            bool type_of_dead = false;
            if (arr_type == 1) // обычная стрела
                HP -= 2;
            else if(arr_type==2) // стрела испарения
            {
                type_of_dead = true;
                HP -= 100;
            }
            else if(arr_type==3) //стрела огня
            {
                HP -= 1;
                if (Status==0 || Status==2)
                {
                    LastFire = time;
                    FireEffect = Instantiate(Fire, new Vector2(Transform.position.x + 0.5f, Transform.position.y), Quaternion.Euler(new Vector3(-90, 0, 0)));
                    if (Status == 2)
                        Status = 3;
                    else
                        Status = 1;
                } 
            }
            else if (arr_type==4)
            {
                HP -= 1;
                if (Status == 0 || Status==1)
                {
                    SlowEffect = Instantiate(Slow, new Vector2(Transform.position.x, Transform.position.y+ 0.3f), Quaternion.Euler(new Vector3(-90, 0, 0)));
                    if (Status == 1)
                        Status = 3;
                    else
                        Status = 2;
                    Speed = 0.002f;
                }
            }
            else if(arr_type==5)
            {
                /*Эффект взрыва*/
                HP = 0;
                type_of_dead = true;
            }
            if (HP <= 0 && !type_of_dead)
            {
                Animator.SetInteger("cntrl", Random.Range(1,3));
                Status = 0;
                Destroy(FireEffect);
                Destroy(SlowEffect);
                return true;
            }
            else if (HP<=0 && type_of_dead)
            {
                Animator.SetInteger("cntrl", 3);
                Status = 0;
                Destroy(FireEffect);
                Destroy(SlowEffect);
                return true;
            }
            else
                return false;
        }
        public bool GetFireDamage(int time)
        {
            HP -= 1;
            LastFire = time;
            if (HP<=0)
            {
                Animator.SetInteger("cntrl", 3);
                Status = 0;
                Destroy(FireEffect);
                Destroy(SlowEffect);
                return true;
            }
            return false;
        }
        public void Remove() { Destroy(Object); Destroy(FireEffect); Destroy(SlowEffect); }
        public float GetBody() { return Body; }
        public void StartCast() { Animator.SetInteger("cntrl", 4); }
        public int GetCntrl() { return Animator.GetInteger("cntrl"); }
        public void SkeletonReLive() { HP = 2; Speed = 0.01f; }
        public void HpInc() { HP++; }
        public int GetStatus() { return Status; }
        public int GetLastFire() { return LastFire; }
    }
    /*Класс телег*/
    public class LCar
    {
        public GameObject Object;
        public Animator Animator;
        public bool TurnRight; // true - едет на право/false - едет налево
        public short Type; // 1- лаваксий/2 - хп/3 - стрелы
        public SpriteRenderer SpriteRenderer;

        private Transform Transform;
        public LCar(GameObject obj, bool turn_right, Vector2 spawn_pos, short type)
        {
            Object = Instantiate(obj, spawn_pos, Quaternion.identity);
            Animator = Object.GetComponent<Animator>();
            if (!turn_right)
                Animator.SetInteger("cntrl", 1);
            Transform = Object.GetComponent<Transform>();
            TurnRight = turn_right;
            Type = type;
            SpriteRenderer = Object.GetComponent<SpriteRenderer>();
            if (Transform.position.y >= 0)
                SpriteRenderer.sortingOrder = 30000 - (int)(Transform.position.y * 100);
            else
                SpriteRenderer.sortingOrder = 30000 + (int)(Mathf.Abs(Transform.position.y) * 100);
        }
        public void Move(int dest)
        {
            Transform.position = new Vector3(Mathf.Lerp(Transform.position.x, dest, 0.01f * Time.deltaTime),
                Transform.position.y, Transform.position.z);

        }
        public Transform GetTransform() { return Transform; }
        public void Remove() { Destroy(Object); }
    }
    /*Класс чисел*/
    public class Numbers
    {
        private List<Sprite> Sprites=new List<Sprite>();
        private List<Sprite> RedSprites=new List<Sprite>();
        private List<Image> ScoreImage=new List<Image>();
        private List<Image> BestScoreImage=new List<Image>();
        private List<Image> ComboImage = new List<Image>();
        private List<Image> BestComboImage = new List<Image>();
        private List<Image> LavaxiumImage=new List<Image>();
        private List<Image> ArrowImage=new List<Image>();

        private List<Image> SkeletonImage = new List<Image>();
        private List<Image> GhostImage = new List<Image>();
        private List<Image> KnightImage = new List<Image>();
        private List<Image> BossImage = new List<Image>();
        private List<Image> NecromantImage = new List<Image>();

        private GameObject ScoreBlock;
        private GameObject ComboBlock;
        private GameObject BestScoreBlock;
        private GameObject BestComboBlock;
        private GameObject LavaxBlock;
        private GameObject ArrowBlock;

        private GameObject SkeletonBlock;
        private GameObject GhostBlock;
        private GameObject KnightBlock;
        private GameObject BossBlock;
        private GameObject NecromantBlock;

        private Image BestScoreObj;
        private Image BestComboObj;

        private Image ComboX;
        private Image BestComboX;

        private GameObject LeftRecord;
        private GameObject RightRecord;
        public Numbers(List<Sprite> sprites, List<Sprite> red_sprites,
            List<Image> score_image, List<Image> best_score_image, List<Image> combo_image, List<Image> best_combo_image, List<Image> lavaxium_image,
            GameObject score_block, GameObject best_score_block, GameObject combo_block, GameObject best_combo_block, GameObject lavax_block,
            Image best_score_obj, Image best_combo_obj, GameObject left_record, GameObject right_record, Image combo_x, Image best_combo_x,
            List<Image> arrow_image, GameObject arrow_block, List<GameObject> mob_blocks, List<Image> skeleton_image, List<Image> ghost_image, List<Image> knight_image,
            List<Image> boss_image, List<Image> necromant_image) 
        {
            Sprites = sprites;
            RedSprites = red_sprites;
            ScoreImage = score_image;
            BestScoreImage = best_score_image;
            ComboImage = combo_image;
            BestComboImage = best_combo_image;
            LavaxiumImage = lavaxium_image;
            ScoreBlock = score_block;
            ComboBlock = combo_block;
            BestScoreBlock = best_score_block;
            BestComboBlock = best_combo_block;
            LavaxBlock = lavax_block;
            BestScoreObj = best_score_obj;
            BestComboObj = best_combo_obj;
            LeftRecord = left_record;
            RightRecord = right_record;
            ComboX = combo_x;
            BestComboX = best_combo_x;
            ArrowImage = arrow_image;
            ArrowBlock = arrow_block;
            SkeletonImage = skeleton_image;
            GhostImage = ghost_image;
            KnightImage = knight_image;
            BossImage = boss_image;
            NecromantImage = necromant_image;
            SkeletonBlock = mob_blocks[0];
            GhostBlock = mob_blocks[1];
            KnightBlock = mob_blocks[2];
            BossBlock = mob_blocks[3];
            NecromantBlock = mob_blocks[4];
        }
        /*Проверка числе на валидность*/
        private bool CheckNum(int num)
        {
            if (num > 999999 || num < 0)
                return false;
            else
                return true;
        }
        /*Вывод счета на экран*/
        public void Score(int score, bool combo, bool change, bool record)
        {
            if (!CheckNum(score))
                score=999999;
            List<int> dig = new List<int>();
            int delta = Menu.DeltaGame;
            int start = Screen.width/2;
            int digits = 0;
            int height = (int)ScoreImage[0].GetComponent<RectTransform>().position.y;
            while (true)
            {
                dig.Add(score % 10);
                digits++;
                score = score / 10;
                if (score == 0)
                    break;
            }
            if (digits % 2 != 0)
                start -= (digits / 2) * delta;
            else
                start -= delta / 2 + delta * (digits / 2 - 1);
            for (int i = 0; i < ScoreImage.Count; i++)
                ScoreImage[i].enabled = false;
            if (record)
            {
                LeftRecord.SetActive(true);
                LeftRecord.GetComponent<RectTransform>().position = new Vector2(start-delta, height);
            }
            for (int i=dig.Count-1, j=0; i>-1; i--, j++)
            {
                if (combo)
                    ScoreImage[j].sprite = RedSprites[dig[i]];
                else
                    ScoreImage[j].sprite = Sprites[dig[i]];
                ScoreImage[j].GetComponent<RectTransform>().position = new Vector2(start, height);
                ScoreImage[j].enabled = true;
                start += delta;
            }
            if (record)
            {
                RightRecord.SetActive(true);
                RightRecord.GetComponent<RectTransform>().position = new Vector2(start, height);
            }
            if (change)
            {
                ScoreBlock.SetActive(false);
                ScoreBlock.SetActive(true);
            }
        }
        public void Combo(int Combo)
        {
            if (!CheckNum(Combo))
                Combo = 999999;
            List<int> dig = new List<int>();
            int delta = Menu.DeltaGame;
            int start = Screen.width / 2;
            int digits = 0;
            int height = (int)ComboImage[0].GetComponent<RectTransform>().position.y;
            while (true)
            {
                dig.Add(Combo % 10);
                digits++;
                Combo = Combo / 10;
                if (Combo == 0)
                    break;
            }
            if (digits % 2 != 0)
                start -= (digits / 2) * delta;
            else
                start -= delta / 2 + delta * (digits / 2 - 1);
            for (int i = 0; i < ComboImage.Count; i++)
                ComboImage[i].enabled = false;
            start -= delta/2;
            ComboX.GetComponent<RectTransform>().position = new Vector2(start, height);
            ComboX.enabled = false;
            ComboX.enabled = true;
            start += 2*(delta/3);
            for (int i = dig.Count - 1, j = 0; i > -1; i--, j++)
            {
                ComboImage[j].sprite = RedSprites[dig[i]];
                ComboImage[j].GetComponent<RectTransform>().position = new Vector2(start, height);
                ComboImage[j].enabled = true;
                start += delta;
            }
            ComboBlock.SetActive(false);
            ComboBlock.SetActive(true);
        }
        public void BestScore(int score, bool change)
        {
            if (!CheckNum(score))
                score = 999999;
            List<int> dig = new List<int>();
            int delta = Menu.DeltaBest;
            float start = BestScoreObj.GetComponent<RectTransform>().position.x;
            int digits = 0;
            int height = (int)BestScoreImage[0].GetComponent<RectTransform>().position.y;
            while (true)
            {
                dig.Add(score % 10);
                digits++;
                score = score / 10;
                if (score == 0)
                    break;
            }
            if (digits % 2 != 0)
                start -= (digits / 2) * delta;
            else
                start -= delta / 2 + delta * (digits / 2 - 1);
            for (int i = 0; i < BestScoreImage.Count; i++)
                BestScoreImage[i].enabled = false;

            for (int i = dig.Count - 1, j = 0; i > -1; i--, j++)
            {
                BestScoreImage[j].sprite = RedSprites[dig[i]];
                BestScoreImage[j].GetComponent<RectTransform>().position = new Vector2(start, height);
                BestScoreImage[j].enabled = true;
                start += delta;
            }
            if (change)
            {
                BestScoreBlock.SetActive(false);
                BestScoreBlock.SetActive(true);
            }
            
        }
        public void BestCombo(int score, bool change)
        {
            if (!CheckNum(score))
                score = 999999;
            List<int> dig = new List<int>();
            int delta = Menu.DeltaBest;
            float start = BestComboObj.GetComponent<RectTransform>().position.x;
            int digits = 0;
            int height = (int)BestComboImage[0].GetComponent<RectTransform>().position.y;
            while (true)
            {
                dig.Add(score % 10);
                digits++;
                score = score / 10;
                if (score == 0)
                    break;
            }
            if (digits % 2 != 0)
                start -= (digits / 2) * delta;
            else
                start -= delta / 2 + delta * (digits / 2 - 1);
            for (int i = 0; i < BestComboImage.Count; i++)
                BestComboImage[i].enabled = false;
            start -= 4*delta/5;
            BestComboX.GetComponent<RectTransform>().position = new Vector2(start, height);
            BestComboX.enabled = false;
            BestComboX.enabled = true;
            start += 4 * delta / 5;
            for (int i = dig.Count - 1, j = 0; i > -1; i--, j++)
            {
                BestComboImage[j].sprite = RedSprites[dig[i]];
                BestComboImage[j].GetComponent<RectTransform>().position = new Vector2(start, height);
                BestComboImage[j].enabled = true;
                start += delta;
            }
            if (change)
            {
                BestComboBlock.SetActive(false);
                BestComboBlock.SetActive(true);
            }   
        }
        public void Lavaxium(int num, bool change)
        {
            if (!CheckNum(num))
                num = 999999;
            List<int> dig = new List<int>();
            while (true)
            {
                dig.Add(num % 10);
                num = num / 10;
                if (num == 0)
                    break;
            }
            for (int i = 0; i < LavaxiumImage.Count; i++)
                LavaxiumImage[i].enabled = false;
            for (int i = dig.Count - 1, j = 0; i > -1; i--, j++)
            {
                LavaxiumImage[j].sprite = Sprites[dig[i]];
                LavaxiumImage[j].enabled = true;
            }
            if (change)
            {
                LavaxBlock.SetActive(false);
                LavaxBlock.SetActive(true);
            }
        }
        public void Arrow(int num, bool change)
        {
            if (!CheckNum(num))
                num = 999999;
            List<int> dig = new List<int>();
            while (true)
            {
                dig.Add(num % 10);
                num = num / 10;
                if (num == 0)
                    break;
            }
            for (int i = 0; i < ArrowImage.Count; i++)
                ArrowImage[i].enabled = false;
            for (int i = dig.Count - 1, j = 0; i > -1; i--, j++)
            {
                ArrowImage[j].sprite = Sprites[dig[i]];
                ArrowImage[j].enabled = true;
            }
            if (change)
            {
                ArrowBlock.SetActive(false);
                ArrowBlock.SetActive(true);
            }  
        }
        public void Statistic(Dictionary<string, int> mobs)
        {
            SetStatisticNotActive();
            foreach (KeyValuePair<string, int> key in mobs)
            {
                if (mobs[key.Key] > 9999)
                    mobs[key.Key] = 9999;
            }
            foreach(KeyValuePair<string, int> key in mobs)
            {
                int num = mobs[key.Key];
                if (num > 9999)
                    num = 9999;
                if (num == 0)
                    continue;
                List<int> dig = new List<int>();
                while (true)
                {
                    dig.Add(num % 10);
                    num = num / 10;
                    if (num == 0)
                        break;
                }
                if (key.Key == "skeleton")
                {
                    for (int i = 0; i < SkeletonImage.Count; i++)
                        SkeletonImage[i].enabled = false;
                    for (int i = dig.Count - 1, j = 0; i > -1; i--, j++)
                    {
                        SkeletonImage[j].sprite = Sprites[dig[i]];
                        SkeletonImage[j].enabled = true;
                    }
                    SkeletonBlock.SetActive(false);
                    SkeletonBlock.SetActive(true);
                }
                else if(key.Key == "ghost")
                {
                    for (int i = 0; i < GhostImage.Count; i++)
                        GhostImage[i].enabled = false;
                    for (int i = dig.Count - 1, j = 0; i > -1; i--, j++)
                    {
                        GhostImage[j].sprite = Sprites[dig[i]];
                        GhostImage[j].enabled = true;
                    }
                    GhostBlock.SetActive(false);
                    GhostBlock.SetActive(true);
                }
                else if(key.Key == "knight")
                {
                    for (int i = 0; i < KnightImage.Count; i++)
                        KnightImage[i].enabled = false;
                    for (int i = dig.Count - 1, j = 0; i > -1; i--, j++)
                    {
                        KnightImage[j].sprite = Sprites[dig[i]];
                        KnightImage[j].enabled = true;
                    }
                    KnightBlock.SetActive(false);
                    KnightBlock.SetActive(true);
                }
                else if(key.Key == "boss")
                {
                    for (int i = 0; i < BossImage.Count; i++)
                        BossImage[i].enabled = false;
                    for (int i = dig.Count - 1, j = 0; i > -1; i--, j++)
                    {
                        BossImage[j].sprite = Sprites[dig[i]];
                        BossImage[j].enabled = true;
                    }
                    BossBlock.SetActive(false);
                    BossBlock.SetActive(true);
                }
                else if(key.Key == "necromant")
                {
                    for (int i = 0; i < NecromantImage.Count; i++)
                        NecromantImage[i].enabled = false;
                    for (int i = dig.Count - 1, j = 0; i > -1; i--, j++)
                    {
                        NecromantImage[j].sprite = Sprites[dig[i]];
                        NecromantImage[j].enabled = true;
                    }
                    NecromantBlock.SetActive(false);
                    NecromantBlock.SetActive(true);
                }
            }

        }
        public void SetNotActiveScore()
        {
            for (int i = 0; i < ScoreImage.Count; i++)
                ScoreImage[i].enabled = false;
            ScoreBlock.SetActive(false);
            RightRecord.SetActive(false);
            LeftRecord.SetActive(false);
        }
        public void SetNotActiveArr()
        {
            for (int i = 0; i < ArrowImage.Count; i++)
                ArrowImage[i].enabled = false;
            ArrowBlock.SetActive(false);
        }
        public void SetNotActiveCombo()
        {
            for (int i = 0; i < ComboImage.Count; i++)
                ComboImage[i].enabled = false;
            ComboBlock.SetActive(false);
        }
        public void SetBestNotActive()
        {
            for (int i = 0; i < BestComboImage.Count; i++)
                BestComboImage[i].enabled = false;
            for (int i = 0; i < BestScoreImage.Count; i++)
                BestScoreImage[i].enabled = false;
            BestComboX.enabled = false;
        }
        public void SetStatisticNotActive()
        {
            SkeletonBlock.SetActive(false);
            GhostBlock.SetActive(false);
            KnightBlock.SetActive(false);
            BossBlock.SetActive(false);
            NecromantBlock.SetActive(false);
        }
    }
    /*Монетки и сердечки*/
    public class Lavaxium
    {
        private GameObject Obj;
        private Transform Transform;
        private Animator Animator;
        public Vector2 Dest;
        public float _Time;
        public short Status;
        public short Type; // 1 - лаваксий/2-hp
        public Lavaxium(GameObject obj, Vector2 spawn_pos, short type, short status)
        {
            Obj = Instantiate(obj, spawn_pos, Quaternion.identity);
            Transform = Obj.GetComponent<Transform>();
            Dest = new Vector2(spawn_pos.x + Random.Range(-1.5f, 1.5f), spawn_pos.y + Random.Range(0f, 1.5f));
            Status = status; // 0 - летит к месту отдыха/1 - стоит на месте отдыха/2-летит к левому верхнему углу/3-долетела
            Type = type;
        }
        public void Move(Vector2 dest)
        {
            Transform.position = new Vector3(Mathf.Lerp(Transform.position.x, dest.x, 5 * Time.deltaTime),
               Mathf.Lerp(Transform.position.y, dest.y, 5 * Time.deltaTime), Transform.position.z);
            if (Vector2.Distance(dest, Transform.position)<0.5f)
            {
                if (Status == 0) { Status = 1; _Time = Time.realtimeSinceStartup; }
                else if (Status == 2) Status = 3;
            }
                
        }
        public void Remove() { Destroy(Obj); }
    }
    /*Мусорная корзина*/
    public class Basket
    {
        private GameObject Obj;
        public int Time;
        public Basket(GameObject obj, int time)
        {
            Obj = obj;
            Time = time;
        }
        public void Remove() { Destroy(Obj); }
        public GameObject GetGameObject() { return Obj; }
    }
    /*Класс спавна*/
    public class Spawn
    {
        private int K; // коэффицент value для кванта
        private int X; // переменная регулирования
        private int Limit; // предел мобов в квант

        private int SkeletonValue; // value мобов
        private int GhostValue;
        private int KnightValue;
        private int NecromantValue;
        private int BossValue;

        private float PSkeleton; // вероятности мобов
        private float PGhost;
        private float PKnight;
        private float PNecromant;
        private float PBoss;

        public List<int> Sec1=new List<int>(); // массивы мобов
        public List<int> Sec2 = new List<int>(); // 1-skeleton/2-ghost/3-knight/4-necromant/5-boss
        public List<int> Sec3 = new List<int>();
        public List<int> Sec4 = new List<int>();
        public List<int> Sec5 = new List<int>();

        private bool PChange; // нужен ли перерасчет вероятностей и квант отдыха
        private int CurrentSec; // текущий массив спавна (1-6)
        public Spawn()
        {
            /*Переменные настройки*/
            X = 50;
            Limit = 8;
            SkeletonValue = 40;
            GhostValue = 120;
            KnightValue = 150;
            NecromantValue = 600;
            BossValue = 450;

            PSkeleton = 1;
            PGhost = 0;
            PKnight = 0;
            PNecromant = 0;
            PBoss = 0;
            PChange = false;
            CurrentSec = 1;
        }
        private void CalculateK(int time, int score) { K = time + score + X; }
        private void CalculateX(int units) { X -= units; }
        private void CalculateP()
        {
            if (PSkeleton <= 0.2f)
                return;
            PSkeleton -= 0.1f;
            if (PGhost <= 0.2f)
                PGhost += 0.1f;
            else if (PKnight <= 0.2f)
                PKnight += 0.1f;
            else if (PBoss <= 0.2f)
                PBoss += 0.1f;
            else if (PNecromant <= 0.2f)
                PNecromant += 0.1f;
        }
        public int GetCurrentSec() { return CurrentSec; }
        public void CurrentSecInc() { CurrentSec++; }
        private void InputSec(int type, int value)
        {
            int rand = Random.Range(1, 6);
            if (rand == 1)
                Sec1.Add(type);
            else if (rand == 2)
                Sec2.Add(type);
            else if (rand == 3)
                Sec3.Add(type);
            else if (rand == 4)
                Sec4.Add(type);
            else if (rand == 5)
                Sec5.Add(type);
            K -= value;
        }
        private int MakeSec()
        {
            int p1 = (int)(PSkeleton * 10)+1;
            int p2 = (int)(PGhost * 10)+p1;
            int p3 = (int)(PKnight * 10)+p2;
            int p4 = (int)(PBoss * 10)+p3;
            int p5 = (int)(PNecromant * 10)+p4;
            int mobs = 0;
            while(K>=0)
            {
                int rand = Random.Range(0, 10);
                if (rand < p1)
                    InputSec(1, SkeletonValue);
                else if (rand >= p1 && rand < p2)
                    InputSec(2, GhostValue);
                else if (rand >= p2 && rand < p3)
                    InputSec(3, KnightValue);
                else if (rand >= p3 && rand < p4)
                    InputSec(4, BossValue);
                else if (rand >= p4)
                    InputSec(5, NecromantValue);   
                mobs++;
            }
            return mobs;
        }
        public void CalculateNewQuant(int time, int score, int units)
        {
            CurrentSec = 1;
            Sec1.Clear();
            Sec2.Clear();
            Sec3.Clear();
            Sec4.Clear();
            Sec5.Clear();
            CalculateX(units);
            if (PChange && score<2000)
            {
                PChange = false;
                CalculateP();
                CalculateX(K / 2);
                return;
            }
            CalculateK(time, score);
            if (score >= 2000)
            {
                PSkeleton = 0.3f;
                PGhost = 0.1f;
                PKnight = 0.2f;
                PBoss = 0.2f;
                PNecromant = 0.2f;
                int rand = Random.Range(1, 5);
                CalculateX(units/3);
                K /= rand;
            }
            int mobs = MakeSec();
            if (mobs > Limit)
                PChange = true;
        }
    }
}
