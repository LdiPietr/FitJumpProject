using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.Linq;

public class PlayFabManager : MonoBehaviour
{
    public static PlayFabManager Instance { get; private set; }

    [SerializeField] private string titleId;

    private TitleScreenManager titleScreenManager;

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

    public void LoginWithEmail(string email, string password, TitleScreenManager tScreenManager)
    {
        var request = new LoginWithEmailAddressRequest
        {
            Email = email,
            Password = password,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetUserAccountInfo = true,
                GetPlayerProfile = true,
                GetPlayerStatistics = true, // Ottieni anche i punteggi
                ProfileConstraints = new PlayerProfileViewConstraints
                {
                    ShowDisplayName = true
                }
            }
        };
        
        titleScreenManager = tScreenManager;

        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginError);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Login effettuato con successo!");

        // ✅ Info di base
        string playFabId = result.PlayFabId;
        Debug.Log("PlayFabId: " + playFabId);

        // ✅ Info account
        var accountInfo = result.InfoResultPayload.AccountInfo;
        if (accountInfo != null)
        {
            Debug.Log("Email: " + accountInfo.PrivateInfo.Email);
            Debug.Log("Username: " + accountInfo.Username);
            Debug.Log("DisplayName: " + accountInfo.TitleInfo.DisplayName);
            
            GameManager.Instance.userName = accountInfo.Username;
            
            titleScreenManager.LoginEnd(true);
        }
    }

    private void OnLoginError(PlayFabError error)
    {
        titleScreenManager.LoginEnd(false);
    }

    #endregion

    #region Leaderboards

    public void SubmitScore(int score)
    {
        var request = new GetPlayerStatisticsRequest
        {
            StatisticNames = new List<string> { "WeeklyScore", "MonthlyScore", "YearlyScore" }
        };

        PlayFabClientAPI.GetPlayerStatistics(request, result =>
            {
                List<StatisticUpdate> statsToUpdate = new List<StatisticUpdate>();

                foreach (var stat in result.Statistics)
                {
                    if ((stat.StatisticName == "WeeklyScore" && score > stat.Value) ||
                        (stat.StatisticName == "MonthlyScore" && score > stat.Value) ||
                        (stat.StatisticName == "YearlyScore" && score > stat.Value))
                    {
                        statsToUpdate.Add(new StatisticUpdate
                        {
                            StatisticName = stat.StatisticName,
                            Value = score
                        });
                    }
                }

                // Se la statistica non esiste ancora, aggiungila
                if (!result.Statistics.Any(s => s.StatisticName == "WeeklyScore"))
                {
                    statsToUpdate.Add(new StatisticUpdate { StatisticName = "WeeklyScore", Value = score });
                }

                if (!result.Statistics.Any(s => s.StatisticName == "MonthlyScore"))
                {
                    statsToUpdate.Add(new StatisticUpdate { StatisticName = "MonthlyScore", Value = score });
                }

                if (!result.Statistics.Any(s => s.StatisticName == "YearlyScore"))
                {
                    statsToUpdate.Add(new StatisticUpdate { StatisticName = "YearlyScore", Value = score });
                }

                if (statsToUpdate.Count > 0)
                {
                    var updateRequest = new UpdatePlayerStatisticsRequest
                    {
                        Statistics = statsToUpdate
                    };

                    PlayFabClientAPI.UpdatePlayerStatistics(updateRequest,
                        updateResult => { Debug.Log("Statistiche aggiornate con successo!"); },
                        error => Debug.LogError("Errore durante l'aggiornamento delle statistiche: " +
                                                error.GenerateErrorReport()));
                }
            },
            error => Debug.LogError("Errore durante il recupero delle statistiche: " + error.GenerateErrorReport()));
    }

    public List<PlayerLeaderboardEntry> GetLeaderboard(string statisticName)
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = statisticName,
            StartPosition = 0,
            MaxResultsCount = 10
        };

        List<PlayerLeaderboardEntry> leaderboard = new List<PlayerLeaderboardEntry>();

        PlayFabClientAPI.GetLeaderboard(request, result =>
            {
                foreach (var entry in result.Leaderboard)
                {
                    Debug.Log($"{entry.Position + 1}: {entry.DisplayName ?? entry.PlayFabId} - {entry.StatValue}");
                    leaderboard.Add(entry);
                }
            },
            error =>
            {
                Debug.LogError("Errore durante il recupero della classifica: " + error.GenerateErrorReport());
            });
        return leaderboard;
    }

    public int GetPlayerScore(string statisticName)
    {
        var request = new GetPlayerStatisticsRequest
        {
            StatisticNames = new List<string> { statisticName } // Esempio: "WeeklyScore"
        };
        int score = 0;
        PlayFabClientAPI.GetPlayerStatistics(request, result =>
            {
                foreach (var stat in result.Statistics)
                {
                    if (stat.StatisticName == statisticName)
                    {
                        Debug.Log($"Punteggio attuale ({statisticName}): {stat.Value}");
                        score = stat.Value;
                        return;
                    }
                }

                Debug.Log($"Nessun punteggio registrato per '{statisticName}'");
            },
            error => { Debug.LogError("Errore nel recupero delle statistiche: " + error.GenerateErrorReport()); });

        return score;
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

    #region TEST

    public void UpdateLeaderboardViaCloudScript(string statisticName, int score, Action<bool> callback)
    {
        Debug.Log($"Tentativo di aggiornare {statisticName} via Cloud Script con punteggio: {score}");

        var request = new ExecuteCloudScriptRequest
        {
            FunctionName = "updateLeaderboard",
            FunctionParameter = new
            {
                statisticName = statisticName,
                value = score
            },
            GeneratePlayStreamEvent = true
        };

        PlayFabClientAPI.ExecuteCloudScript(request,
            result =>
            {
                if (result.FunctionResult != null)
                {
                    var jsonResult = JsonUtility.FromJson<CloudScriptResult>(result.FunctionResult.ToString());
                    if (jsonResult.success)
                    {
                        Debug.Log($"Cloud Script eseguito con successo: {jsonResult.message}");
                        callback(true);
                    }
                    else
                    {
                        Debug.LogError($"Errore nel Cloud Script: {jsonResult.error}");
                        callback(false);
                    }
                }
                else
                {
                    Debug.Log("Cloud Script eseguito ma senza risultato");
                    callback(true);
                }
            },
            error =>
            {
                Debug.LogError($"Errore nell'esecuzione del Cloud Script: {error.ErrorMessage}");
                callback(false);
            }
        );
    }

    [Serializable]
    private class CloudScriptResult
    {
        public bool success;
        public string message;
        public string error;
    }

    #endregion
}

public class PlayerLeaderboardInfo
{
    public int WeeklyScore { get; set; } = 0;
    public int MonthlyScore { get; set; } = 0;
    public int YearlyScore { get; set; } = 0;
    public int WeeklyPosition { get; set; } = 0;
    public int MonthlyPosition { get; set; } = 0;
    public int YearlyPosition { get; set; } = 0;
}