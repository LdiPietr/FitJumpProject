using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PlayFab;
using PlayFab.ClientModels;
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
    public TextMeshProUGUI wrongRegistrationMessage;
    public GameObject regWrongLoginMessage;
    public GameObject regFaqMessage;
    public GameObject regSuccessMessage;
    public GameObject regPrivacyMessage;

    [Space(10)] [Header("GamePanel")] public GameObject gameInit;
    public GameObject tournamentPanel;
    public GameObject leaderboardPanel;
    public TextMeshProUGUI leaderboardText;
    public GameObject shopPanel;
    public TextMeshProUGUI shopTicketsText;
    public GameObject shopSuccessMessage;
    public TextMeshProUGUI shopMessageTitle;
    public TextMeshProUGUI shopMessageText;
    public List<PlayerLeaderboardEntry> mon_leaderboard = new List<PlayerLeaderboardEntry>();
    public List<PlayerLeaderboardEntry> wee_leaderboard = new List<PlayerLeaderboardEntry>();


    [Space(10)] [Header("Misc")] public TextMeshProUGUI nameText;
    public TextMeshProUGUI ticketsText;


    private void Start()
    {
        AudioManager.Instance.PlayMusic(AudioManager.Instance.musicClipMenu);
        if (GameManager.Instance.loged)
        {
            nameText.text = GameManager.Instance.userName;
            ShowGamePanel();
            GameManager.Instance.isTournamentMode = false;
        }
        else
        {
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

        wee_leaderboard = PlayFabManager.Instance.GetLeaderboard("WeeklyScore");
        mon_leaderboard = PlayFabManager.Instance.GetLeaderboard("YearlyScore");
    }

    public void Login()
    {
        if (string.IsNullOrEmpty(usernameInput.text) || string.IsNullOrEmpty(passwordInput.text))
        {
            wrongLoginMessage.SetActive(true);
        }
        else
        {
            PlayFabManager.Instance.LoginWithEmail(usernameInput.text, passwordInput.text, this);
        }
    }

    public void LoginEnd(bool success)
    {
        if (success)
        {
            nameText.text = GameManager.Instance.userName;
            GameManager.Instance.loged = true;
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

    public void PlayTournament()
    {
        GameManager.Instance.isTournamentMode = true;
        SceneManager.LoadScene("GameplayTournament");
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

    public void RegPrivacyMessageClose()
    {
        regPrivacyMessage.SetActive(false);
    }

    private void RegisterWithPlayFab(string username, string password, string email, string city, string birthDate)
    {
        var request = new RegisterPlayFabUserRequest
        {
            Username = username,
            Password = password,
            Email = email,
            RequireBothUsernameAndEmail = true
        };

        PlayFabClientAPI.RegisterPlayFabUser(request,
            result =>
            {
                // Registrazione riuscita
                Debug.Log("Registrazione completata con successo!");

                // Aggiorna i dati del profilo utente con città e data di nascita
                UpdateUserData(city, birthDate);

                GameManager.Instance.userName = regUsernameInput.text;
                GameManager.Instance.loged = true;
                nameText.text = regName.text;
                regSuccessMessage.SetActive(true);
                regPrivacyMessage.SetActive(false);
                var request = new UpdateUserTitleDisplayNameRequest
                {
                    DisplayName = username
                };

                PlayFabClientAPI.UpdateUserTitleDisplayName(request,
                    result => { Debug.Log("Display name impostato con successo: " + result.DisplayName); },
                    error =>
                    {
                        Debug.LogError("Errore nell'impostazione del display name: " + error.GenerateErrorReport());
                    });
            },
            error =>
            {
                // Gestione dettagliata degli errori
                string errorMessage = "Errore durante la registrazione: ";

                // Controlla il tipo di errore specifico
                switch (error.Error)
                {
                    case PlayFabErrorCode.UsernameNotAvailable:
                        errorMessage += "Nome utente già in uso.";
                        break;
                    case PlayFabErrorCode.EmailAddressNotAvailable:
                        errorMessage += "Indirizzo email già in uso.";
                        break;
                    case PlayFabErrorCode.InvalidEmailAddress:
                        errorMessage += "Indirizzo email non valido.";
                        break;
                    case PlayFabErrorCode.InvalidPassword:
                        errorMessage += "Password non valida.";
                        break;
                    case PlayFabErrorCode.InvalidParams:
                        errorMessage += "Parametri non validi. Verifica tutti i campi.";
                        break;
                    default:
                        // Mostra il messaggio di errore specifico se disponibile
                        errorMessage += error.ErrorMessage ?? "Errore sconosciuto.";
                        break;
                }

                Debug.LogError(errorMessage);

                // Mostra il messaggio di errore all'utente
                regPrivacyMessage.SetActive(false);
                regWrongLoginMessage.SetActive(true);
                wrongRegistrationMessage.text = errorMessage;
            });
    }

// Funzione per aggiornare i dati utente dopo la registrazione
    private void UpdateUserData(string city, string birthDate)
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { "City", city },
                { "BirthDate", birthDate }
            }
        };

        PlayFabClientAPI.UpdateUserData(request,
            result => { Debug.Log("Dati utente aggiornati con successo!"); },
            error => { Debug.LogError("Errore nell'aggiornamento dei dati utente: " + error.ErrorMessage); });
    }


    public void RegSuccess()
    {
        RegisterWithPlayFab(regUsernameInput.text, regPasswordInput.text, regEmailInput.text, regPasswordInput.text,
            regData.text);
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
        if (string.IsNullOrWhiteSpace(regName.text))
        {
            regWrongLoginMessage.SetActive(true);
            wrongRegistrationMessage.text = "Inserire nome";
        }

        else if (string.IsNullOrWhiteSpace(regSurname.text))
        {
            regWrongLoginMessage.SetActive(true);
            wrongRegistrationMessage.text =
                "Inserire cognnome";
        }

        else if (string.IsNullOrWhiteSpace(regCity.text))
        {
            regWrongLoginMessage.SetActive(true);
            wrongRegistrationMessage.text = "La città deve contenere un campo valido.";
        }

        else if (string.IsNullOrWhiteSpace(regData.text) || !IsValidDate(regData.text))
        {
            regWrongLoginMessage.SetActive(true);
            wrongRegistrationMessage.text = "Il campo deve contenere una data valida.";
        }

        else if (string.IsNullOrWhiteSpace(regUsernameInput.text))
        {
            regWrongLoginMessage.SetActive(true);
            wrongRegistrationMessage.text = "Inserire un nome utente valido.";
        }

        else if (string.IsNullOrWhiteSpace(regPasswordInput.text) || !IsValidPassword(regPasswordInput.text))
        {
            regWrongLoginMessage.SetActive(true);
            wrongRegistrationMessage.text =
                "La password deve contenere almeno 8 caratteri, almeno una lettera maiuscola e almeno un numero.";
        }

        else if (string.IsNullOrWhiteSpace(regEmailInput.text) || !IsValidEmail(regEmailInput.text))
        {
            regWrongLoginMessage.SetActive(true);
            wrongRegistrationMessage.text = "Email errata";
        }

        else
        {
            //Formatta la città

            var formattedCity = regCity.text.Trim();
            if (formattedCity.Length > 0)
            {
                formattedCity = char.ToUpper(formattedCity[0]) +
                                (formattedCity.Length > 1 ? formattedCity.Substring(1).ToLower() : "");
                regCity.text = formattedCity;
            }

            regPrivacyMessage.SetActive(true);
        }
    }

    #endregion

    #region Validation

// Funzione per validare il formato della data
    private bool IsValidDate(string date)
    {
        // Verifica il formato GG/MM/AAAA
        if (!System.Text.RegularExpressions.Regex.IsMatch(date, @"^\d{2}/\d{2}/\d{4}$"))
            return false;

        string[] parts = date.Split('/');
        int day = int.Parse(parts[0]);
        int month = int.Parse(parts[1]);
        int year = int.Parse(parts[2]);

        // Verifica che l'anno sia ragionevole (ad esempio tra 1900 e 2100)
        if (year < 1900 || year > 2100)
            return false;

        // Verifica che il mese sia valido
        if (month < 1 || month > 12)
            return false;

        // Verifica che il giorno sia valido per il mese specificato
        int daysInMonth = DateTime.DaysInMonth(year, month);
        if (day < 1 || day > daysInMonth)
            return false;

        return true;
    }

// Funzione per validare la password
    private bool IsValidPassword(string password)
    {
        // Almeno 8 caratteri
        if (password.Length < 8)
            return false;

        // Almeno una lettera maiuscola
        if (!password.Any(char.IsUpper))
            return false;

        // Almeno una lettera minuscola
        if (!password.Any(char.IsLower))
            return false;

        // Almeno un numero
        if (!password.Any(char.IsDigit))
            return false;

        return true;
    }

// Funzione per validare l'email
    private bool IsValidEmail(string email)
    {
        try
        {
            // Usa la classe MailAddress per validare l'email
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
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
        leaderboardText.text = "CLASSIFICA SETTIMANALE\n\n\n";

        foreach (var pos in wee_leaderboard)
        {
            leaderboardText.text += pos.DisplayName + "  " + pos.StatValue + "\n";
        }

        var request = new GetPlayerStatisticsRequest
        {
            StatisticNames = new List<string> { "WeeklyScore" }
        };

        PlayFabClientAPI.GetPlayerStatistics(request, result =>
            {
                foreach (var stat in result.Statistics)
                {
                    if (stat.StatisticName == "WeeklyScore")
                    {
                        leaderboardText.text += "\n\n" + GameManager.Instance.userName + "  " + stat.Value;
                        return;
                    }
                }

                Debug.Log($"Nessun punteggio registrato per '{"WeeklyScore"}'");
            },
            error => { Debug.LogError("Errore nel recupero delle statistiche: " + error.GenerateErrorReport()); });
    }

    public void MonthlyLeaderboard()
    {
        leaderboardText.text = "CLASSIFICA MENSILE\n\n\n";

        foreach (var pos in mon_leaderboard)
        {
            leaderboardText.text += pos.DisplayName + "  " + pos.StatValue + "\n";
        }

        var request = new GetPlayerStatisticsRequest
        {
            StatisticNames = new List<string> { "MonthlyScore" }
        };

        PlayFabClientAPI.GetPlayerStatistics(request, result =>
            {
                foreach (var stat in result.Statistics)
                {
                    if (stat.StatisticName == "MonthlyScore")
                    {
                        leaderboardText.text += "\n\n" + GameManager.Instance.userName + "  " + stat.Value;
                        return;
                    }
                }

                Debug.Log($"Nessun punteggio registrato per '{"MonthlyScore"}'");
            },
            error => { Debug.LogError("Errore nel recupero delle statistiche: " + error.GenerateErrorReport()); });
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