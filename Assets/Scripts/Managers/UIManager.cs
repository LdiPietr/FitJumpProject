using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject loginPanel;
    public GameObject registerPanel;
    public GameObject mainMenuPanel;
    public GameObject tournamentPanel;

    public InputField loginEmail;
    public InputField loginPassword;
    public InputField registerEmail;
    public InputField registerPassword;
    public InputField registerUsername;
    public InputField registerName;
    public InputField registerSurname;
    public InputField registerBirthDate;
    public InputField registerCountry;
    public InputField registerDomicile;

    public Text errorText;
    public Text ticketsText;
    public Text tournamentScoreText;

    public Toggle privacyToggle;
/*
    void Start()
    {
        ShowLoginPanel();
    }

    public void ShowLoginPanel()
    {
        loginPanel.SetActive(true);
        registerPanel.SetActive(false);
        mainMenuPanel.SetActive(false);
        tournamentPanel.SetActive(false);
    }

    public void ShowRegisterPanel()
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
        tournamentPanel.SetActive(false);
    }

    public void ShowMainMenuPanel()
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        tournamentPanel.SetActive(false);
    }

    public void ShowTournamentPanel()
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(false);
        mainMenuPanel.SetActive(false);
        tournamentPanel.SetActive(true);
    }

    public async void Login()
    {
        bool success = await DatabaseManager.Instance.LoginUser(loginEmail.text, loginPassword.text);
        if (success)
        {
            ShowMainMenuPanel();
        }
        else
        {
            errorText.text = "Login failed. Please try again.";
        }
    }

    public async void Register()
    {
        if (!privacyToggle.isOn)
        {
            errorText.text = "Please accept the privacy policy.";
            return;
        }

        bool success = await DatabaseManager.Instance.RegisterUser(
            registerEmail.text,
            registerPassword.text,
            registerUsername.text,
            registerName.text,
            registerSurname.text,
            registerBirthDate.text,
            registerCountry.text,
            registerDomicile.text
        );

        if (success)
        {
            ShowMainMenuPanel();
        }
        else
        {
            errorText.text = "Registration failed. Please try again.";
        }
    }

    public void StartTrainingMode()
    {
        GameManager.Instance.StartGame(false);
    }

    public async void StartTournamentMode()
    {
        UserData userData = await DatabaseManager.Instance.GetUserData();
        if (userData != null && userData.tickets > 0)
        {
            GameManager.Instance.StartGame(true);
        }
        else
        {
            errorText.text = "Not enough tickets to start a tournament.";
        }
    }

    public void OpenShop()
    {
        SceneManager.LoadScene("Shop");
    }

    public void OpenLeaderboard()
    {
        SceneManager.LoadScene("Leaderboard");
    }

    public async void UpdateTournamentInfo()
    {
        UserData userData = await DatabaseManager.Instance.GetUserData();
        if (userData != null)
        {
            ticketsText.text = "Tickets: " + userData.tickets;
            tournamentScoreText.text = "Tournament Score: " + userData.score;
        }
    }*/
}