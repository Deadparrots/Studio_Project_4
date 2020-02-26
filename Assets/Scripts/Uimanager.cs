using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class Uimanager : MonoBehaviour
{
    public Image HP;
    public Text ScoreText;
    public Text MoneyText;

    private int score = 0;
    private int money = 100;
    private int hp = 100;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            score += 10;
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            money += 10;
            Debug.Log(hp);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            hp -= 10;
            Debug.Log(hp);
        }

        ScoreText.text = score.ToString();
        MoneyText.text = money.ToString();
        HP.fillAmount = hp / 100;
    }
}
