//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine.UI;
//public class Uimanager : MonoBehaviour
//{
//    public Image HP;
//    public Text ScoreText;
//    public Text MoneyText;

//    private float score = 0.0f;
//    private float money = 100.0f;
//    private float hp = 100.0f;
    
//    // Use this for initialization
//    void Start()
//    {
           
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        ScoreText.text = score.ToString();
//        MoneyText.text = money.ToString();
//        HP.fillAmount = hp / 100.0f;
//    }

//    public void Scoreset(float newscore)
//    {
//        score = newscore;
//    }
//    public void Moneyup(float add)
//    {
//        money += add;
//    }
//    public void Moneydown(float minus)
//    {
//        money -= minus;
//    }
//    public void Hpup(float add)
//    {
//        hp += add;
//    }
//    public void Hpdown(float minus)
//    {
//        hp -= minus;
//    }
//}
