using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerContainerScript : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public Image healthbarFill;
    public Image chargeBarFill;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void initialize(Color color)
    {
        scoreText.color = color;
        healthbarFill.color = color;

        scoreText.text = "0";
        healthbarFill.fillAmount = 1;
        healthbarFill.fillAmount = 0;
    }
    public void updateScoreText(int score)
    {
        scoreText.text = score.ToString();
    }
    public void updateHealthBar(int curHp, int maxHp)
    {
        healthbarFill.fillAmount = ((float)curHp / (float)maxHp);
    }
    public void updateChargeBar(float chargedmg, float maxchargedmg)
    {
        chargeBarFill.fillAmount = (chargedmg / maxchargedmg);
    }
}
