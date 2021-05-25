using Mirror;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : NetworkBehaviour
{
    [SerializeField]
    private RectTransform healthFill;
    [SerializeField]
    private RectTransform armorFill;
    [SerializeField]
    private RectTransform thrusterFuelFill;
    [SerializeField]
    private RectTransform timeFill;
    [SerializeField]
    private Text bulletsCount;
    [SerializeField]
    private Text scoreCount;
    [SerializeField]
    private Text endText;
    [SerializeField]
    private Text winnerText;
    [SerializeField]
    private GameObject winnerBar;
    [SerializeField]
    private Image winnerImage;

    private bool isGameOn = true;
    private bool isStarted;
    private float currTime;
    private float endTime;

    private PlayerController controller;
    private Player player;

    public void SetController(PlayerController _controller)
    {
        controller = _controller;
    }

    public void SetPlayer(Player _player)
    {
        player = _player;
    }

    private void Update()
    {
        if (controller != null)
        {
            SetFuelAmount(controller.GetThrusterFuelAmount());
        }

        if (player != null)
        {
            SetHealthAmount(Mathf.Clamp(player.GetHealth() / player.GetMaxHealth(), 0f, 1f));
            SetArmorAmount(Mathf.Clamp(player.GetArmor() / player.GetMaxArmor(), 0f, 1f));

            bulletsCount.text = player.GetBullets().ToString();
            scoreCount.text = player.GetScore().ToString();
        }

        // Round ending ui
        currTime = Time.time;

        // Start timer
        if (isGameOn &&
            !isStarted &&
            GameManager.GetOnline() >= GameManager.instance.matchSettings.playersCount)
        {
            isGameOn = true;
            isStarted = true;
            currTime = Time.time;
            endTime = currTime + GameManager.instance.matchSettings.raundDuration;
        }

        // Display time
        if (isStarted)
        {
            var difference = endTime - currTime;

            try
            {
                endText.text = $"{difference}";
                timeFill.localScale = new Vector3(difference / GameManager.instance.matchSettings.raundDuration, 1f, 1f);
            }
            catch (Exception ex)
            {

            }

            // Stop timer
            if (Time.time >= endTime)
            {
                isStarted = false;
                isGameOn = false;
            }
        }

        // Показываем победителя
        if (!isGameOn)
        {
            try
            {
                scoreCount.gameObject.SetActive(false);
                winnerBar.gameObject.SetActive(true);
                winnerText.text = GameManager.GetWinner().name;
                endText.text = GameManager.GetWinner().name;

                if (GameManager.GetWinner() == player)
                {
                    winnerImage.color = Color.green / 2f;
                }
                else
                {
                    winnerImage.color = Color.red / 2f;
                }
            }
            catch (Exception ex)
            {

            }
        }
    }

    [Client]
    private void ChangeColor()
    {
        
    }

    private void SetHealthAmount(float _amount)
    {
        healthFill.localScale = new Vector3(_amount, 1f, 1f);
    }

    private void SetArmorAmount(float _amount)
    {
        armorFill.localScale = new Vector3(_amount, 1f, 1f);
    }

    private void SetFuelAmount(float _amount)
    {
        thrusterFuelFill.localScale = new Vector3(_amount, 1f, 1f);
    }
}
