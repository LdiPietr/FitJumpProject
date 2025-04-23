using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;

public class PlayFabManager : MonoBehaviour
{
    public static PlayFabManager Instance { get; private set; }

    [SerializeField] private string titleId;

    public bool IsLoggedIn { get; private set; }
    public string PlayFabId { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Imposta il TitleId di PlayFab
            if (string.IsNullOrEmpty(PlayFabSettings.TitleId))
            {
                PlayFabSettings.TitleId = titleId;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #region Authentication

    public void RegisterUser(string username, string email, string password, string data, string city,
        Action<bool> callback)
    {
        // Aggiungi questo controllo prima della registrazione
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.LogError("Nessuna connessione internet disponibile");
            callback(false);
            return;
        }

        var request = new RegisterPlayFabUserRequest
        {
            Username = username,
            DisplayName = username,
            Email = email,
            Password = password,
            PlayerSecret = data + " " + city,
            RequireBothUsernameAndEmail = true
        };

        PlayFabClientAPI.RegisterPlayFabUser(request,
            result =>
            {
                Debug.Log("Registrazione completata con successo!");
                PlayFabId = result.PlayFabId;
                var updateRequest = new UpdateUserDataRequest
                {
                    Data = new Dictionary<string, string>
                    {
                        { "City", city },
                        { "Data", data }
                    }
                };
                PlayFabClientAPI.UpdateUserData(updateRequest, updateResult => { callback(true); },
                    error => Debug.LogError($"Errore durante l'aggiornamento dei dati: {error.ErrorMessage}"));
                IsLoggedIn = true;
                callback(true);
            },
            error =>
            {
                Debug.LogError($"Errore durante la registrazione: {error.ErrorMessage}");
                callback(false);
            }
        );
    }

    public void LoginWithEmail(string email, string password, Action<bool> callback)
    {
        var request = new LoginWithEmailAddressRequest
        {
            Email = email,
            Password = password,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true,
                GetUserAccountInfo = true
            }
        };


        PlayFabClientAPI.LoginWithEmailAddress(request,
            result =>
            {
                Debug.Log("Login completato con successo!");
                PlayFabId = result.PlayFabId;
                if (result.InfoResultPayload != null)
                {
                    if (result.InfoResultPayload.PlayerProfile != null)
                    {
                        GameManager.Instance.userName = result.InfoResultPayload.PlayerProfile.DisplayName;
                    }
                    else if (result.InfoResultPayload.AccountInfo != null)
                    {
                        GameManager.Instance.userName = result.InfoResultPayload.AccountInfo.Username;
                    }
                }

                IsLoggedIn = true;
                callback(true);
            },
            error =>
            {
                Debug.LogError($"Errore durante il login: {error.ErrorMessage}");
                callback(false);
            }
        );
    }

    #endregion

    #region Leaderboards

    public void UpdateMonthlyLeaderboard(int score, Action<bool> callback)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = "MonthlyScore",
                    Value = score
                }
            }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request,
            result =>
            {
                Debug.Log("Punteggio mensile aggiornato con successo!");
                callback(true);
            },
            error =>
            {
                Debug.LogError($"Errore nell'aggiornamento del punteggio mensile: {error.ErrorMessage}");
                callback(false);
            }
        );
    }

    public void UpdateYearlyLeaderboard(int score, Action<bool> callback)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = "YearlyScore",
                    Value = score
                }
            }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request,
            result =>
            {
                Debug.Log("Punteggio annuale aggiornato con successo!");
                callback(true);
            },
            error =>
            {
                Debug.LogError($"Errore nell'aggiornamento del punteggio annuale: {error.ErrorMessage}");
                callback(false);
            }
        );
    }

    public void GetMonthlyLeaderboard(Action<List<PlayerLeaderboardEntry>> callback)
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "MonthlyScore",
            StartPosition = 0,
            MaxResultsCount = 100
        };

        PlayFabClientAPI.GetLeaderboard(request,
            result =>
            {
                Debug.Log("Classifica mensile ottenuta con successo!");
                callback(result.Leaderboard);
            },
            error =>
            {
                Debug.LogError($"Errore nel recupero della classifica mensile: {error.ErrorMessage}");
                callback(new List<PlayerLeaderboardEntry>());
            }
        );
    }

    public void GetYearlyLeaderboard(Action<List<PlayerLeaderboardEntry>> callback)
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "YearlyScore",
            StartPosition = 0,
            MaxResultsCount = 100
        };

        PlayFabClientAPI.GetLeaderboard(request,
            result =>
            {
                Debug.Log("Classifica annuale ottenuta con successo!");
                callback(result.Leaderboard);
            },
            error =>
            {
                Debug.LogError($"Errore nel recupero della classifica annuale: {error.ErrorMessage}");
                callback(new List<PlayerLeaderboardEntry>());
            }
        );
    }

    #endregion

    #region Tournament Mode

    public void GetTournamentStatus(Action<bool> callback)
    {
        var request = new GetTitleDataRequest
        {
            Keys = new List<string> { "TournamentActive" }
        };

        PlayFabClientAPI.GetTitleData(request,
            result =>
            {
                if (result.Data.TryGetValue("TournamentActive", out string value))
                {
                    bool isActive = value.ToLower() == "true";
                    callback(isActive);
                }
                else
                {
                    callback(false);
                }
            },
            error =>
            {
                Debug.LogError($"Errore nel recupero dello stato del torneo: {error.ErrorMessage}");
                callback(false);
            }
        );
    }

    #endregion

    #region Virtual Currency

    public void GetUserTokens(Action<int> callback)
    {
        PlayFabClientAPI.GetUserInventory(
            new GetUserInventoryRequest(),
            result =>
            {
                int tokens = result.VirtualCurrency.ContainsKey("TK") ? result.VirtualCurrency["TK"] : 0;
                callback(tokens);
            },
            error =>
            {
                Debug.LogError($"Errore nel recupero dei token: {error.ErrorMessage}");
                callback(0);
            }
        );
    }

    public void PurchaseTokens(string storeId, string itemId, Action<bool> callback)
    {
        var request = new PurchaseItemRequest
        {
            StoreId = storeId,
            ItemId = itemId,
            VirtualCurrency = "RM", // Valuta reale
            Price = 1 // Il prezzo effettivo sarà determinato dal catalogo
        };

        PlayFabClientAPI.PurchaseItem(request,
            result =>
            {
                Debug.Log("Acquisto token completato con successo!");
                callback(true);
            },
            error =>
            {
                Debug.LogError($"Errore nell'acquisto dei token: {error.ErrorMessage}");
                callback(false);
            }
        );
    }

    public void UseTokens(int amount, Action<bool> callback)
    {
        var request = new SubtractUserVirtualCurrencyRequest
        {
            VirtualCurrency = "TK",
            Amount = amount
        };

        PlayFabClientAPI.SubtractUserVirtualCurrency(request,
            result =>
            {
                Debug.Log($"Utilizzati {amount} token con successo!");
                callback(true);
            },
            error =>
            {
                Debug.LogError($"Errore nell'utilizzo dei token: {error.ErrorMessage}");
                callback(false);
            }
        );
    }

    #endregion
}