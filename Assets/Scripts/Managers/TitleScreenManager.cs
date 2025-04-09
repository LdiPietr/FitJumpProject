using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreenManager : MonoBehaviour
{
    [Header("Panels")] public GameObject titlePanel;
    public GameObject loginPanel;
    public GameObject gamePanel;
    public GameObject registerPanel;

    [Space(10)] [Header("Login")] public TMPro.TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public GameObject wrongLoginMessage;


    [Space(10)] [Header("Registration")] public TMPro.TMP_InputField regUsernameInput;
    public TMP_InputField regPasswordInput;
    public TMP_InputField regEmailInput;
    public TMP_InputField regCity;
    public TMP_InputField regData;
    public TMP_InputField regName;
    public TMP_InputField regSurname;
    public GameObject regWrongLoginMessage;
    public GameObject regFaqMessage;
    public GameObject regSuccessMessage;
    public GameObject regPrivacyMessage;
    public Button regConfirmButton;

    [Space(10)] [Header("GamePanel")] public GameObject gameInit;
    public GameObject tournamentPanel;
    public GameObject leaderboardPanel;
    public TextMeshProUGUI leaderboardText;
    public GameObject shopPanel;
    public TextMeshProUGUI shopTicketsText;
    public GameObject shopSuccessMessage;
    public TextMeshProUGUI shopMessageTitle;
    public TextMeshProUGUI shopMessageText;


    [Space(10)] [Header("Misc")] public TextMeshProUGUI nameText;
    public TextMeshProUGUI ticketsText;


    private void Start()
    {
        AudioManager.Instance.PlayMusic(AudioManager.Instance.musicClipMenu);
        if (GameManager.Instance.loged)
        {
            nameText.text = usernameInput.text;
            ShowGamePanel();
        }
        else
        {
            regConfirmButton.interactable = false;
            titlePanel.SetActive(true);
            loginPanel.SetActive(false);
            registerPanel.SetActive(false);
            gamePanel.SetActive(false);
        }
    }

    #region Login

    public void ShowLoginPanel()
    {
        registerPanel.SetActive(false);
        titlePanel.SetActive(false);
        loginPanel.SetActive(true);
    }

    public void ShowGamePanel()
    {
        registerPanel.SetActive(false);
        titlePanel.SetActive(false);
        loginPanel.SetActive(false);
        gamePanel.SetActive(true);
    }

    public void Login()
    {
        if (usernameInput.text == "luca" || passwordInput.text == "luca")
        {
            GameManager.Instance.userName = usernameInput.text;
            GameManager.Instance.loged = true;
            nameText.text = usernameInput.text;
            ShowGamePanel();
        }
        else
        {
            wrongLoginMessage.SetActive(true);
        }
    }

    public void RegisterOperations()
    {
        registerPanel.SetActive(true);
        titlePanel.SetActive(false);
        loginPanel.SetActive(false);
    }

    public void BackToLogin()
    {
        registerPanel.SetActive(false);
        titlePanel.SetActive(true);
    }

    public void WrongLoginMessageClose()
    {
        wrongLoginMessage.SetActive(false);
    }

    public void MenuInLoginPanel()
    {
        titlePanel.SetActive(true);
        loginPanel.SetActive(false);
    }

    #endregion


    public void PlayGame()
    {
        SceneManager.LoadScene("GameplayTraining");
    }

    #region Registration

    public void RegFaqMessage()
    {
        regFaqMessage.SetActive(true);
    }

    public void RegFaqMessageClose()
    {
        regFaqMessage.SetActive(false);
    }

    public void RegPrivacyMessage()
    {
        regPrivacyMessage.SetActive(true);
        regConfirmButton.interactable = true;
    }

    public void RegPrivacyMessageClose()
    {
        regPrivacyMessage.SetActive(false);
    }

    public void RegSuccessMessageClose()
    {
        regSuccessMessage.SetActive(false);
        ShowGamePanel();
    }

    public void RegWrongLoginMessageClose()
    {
        regWrongLoginMessage.SetActive(false);
    }

    public void Register()
    {
        if (regUsernameInput.text == "")
        {
            regWrongLoginMessage.SetActive(true);
        }

        else if (regPasswordInput.text == "")
        {
            regWrongLoginMessage.SetActive(true);
        }

        else if (regEmailInput.text == "")
        {
            regWrongLoginMessage.SetActive(true);
        }

        else if (regCity.text == "")
        {
            regWrongLoginMessage.SetActive(true);
        }

        else if (regData.text == "")
        {
            regWrongLoginMessage.SetActive(true);
        }

        else if (regName.text == "")
        {
            regWrongLoginMessage.SetActive(true);
        }

        else if (regSurname.text == "")
        {
            regWrongLoginMessage.SetActive(true);
        }

        else
        {
            GameManager.Instance.userName = regUsernameInput.text;
            GameManager.Instance.loged = true;
            nameText.text = regName.text;
            regSuccessMessage.SetActive(true);
        }
    }

    #endregion

    #region Tournament

    public void GoToTournament()
    {
        gameInit.SetActive(false);
        tournamentPanel.SetActive(true);
    }

    public void BackToGamePanel()
    {
        tournamentPanel.SetActive(false);
        gameInit.SetActive(true);
    }

    #region Leaderboard

    public void Leaderboard()
    {
        leaderboardPanel.SetActive(true);
        WeeklyLeaderboard();
    }

    public void LeaderboardClose()
    {
        leaderboardPanel.SetActive(false);
    }

    public void WeeklyLeaderboard()
    {
        leaderboardText.text = "CLASSIFICA SETTIMANALE\n\n\n" +
                               "PePPE\n" +
                               "PePPE\n" +
                               "PePPE\n" +
                               "PePPE\n" +
                               "PePPE\n" +
                               "PePPE\n" +
                               "PePPE\n" +
                               "PePPEPePPE\n" +
                               "PePPE\n" +
                               "PePPE\n" +
                               "PePPE\n\n\n" +
                               "Il tuo nome - 525646";
    }

    public void MonthlyLeaderboard()
    {
        leaderboardText.text = "CLASSIFICA MENSILE\n\n\n" +
                               "PePPE\n" +
                               "PePPE\n" +
                               "PePPE\n" +
                               "PePPE\n" +
                               "PePPE\n" +
                               "PePPE\n" +
                               "PePPE\n" +
                               "PePPEPePPE\n" +
                               "PePPE\n" +
                               "PePPE\n" +
                               "PePPE\n\n\n" +
                               "Il tuo nome - 525646";
    }

    #endregion

    #region Shop

    public void Shop()
    {
        shopPanel.SetActive(true);
        shopTicketsText.text = "TICKETS: " + GameManager.Instance.tickets;
    }

    public void ShopClose()
    {
        shopPanel.SetActive(false);
    }

    public void ShopBuyTicket(int ticket)
    {
        GameManager.Instance.tickets += ticket;
        shopTicketsText.text = "TICKETS: " + GameManager.Instance.tickets;

        shopSuccessMessage.SetActive(true);
        shopMessageTitle.text = "Acquisto effettuato!";
        shopMessageText.text = "Hai acquistato " + ticket + " ticket";
    }

    public void ShopBuyTicketClose()
    {
        shopSuccessMessage.SetActive(false);
    }

    #endregion

    #endregion
}