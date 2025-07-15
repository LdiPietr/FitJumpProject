using UnityEngine;
using UnityEngine.Purchasing;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using TMPro;

public class IAPManager : MonoBehaviour, IStoreListener
{
    private IStoreController controller;
    private IExtensionProvider extensions;
    public TextMeshProUGUI ticketText;

    private Dictionary<string, int> productToAmount = new Dictionary<string, int>
    {
        { "tickets_1", 1 },
        { "tickets_5", 5 },
        { "tickets_10", 10 }
    };

    void Start()
    {
        var module = StandardPurchasingModule.Instance();

#if UNITY_EDITOR
        module.useFakeStoreUIMode = FakeStoreUIMode.StandardUser;
#endif

        var builder = ConfigurationBuilder.Instance(module);

        foreach (var entry in productToAmount)
            builder.AddProduct(entry.Key, ProductType.Consumable);

        UnityPurchasing.Initialize(this, builder);

        if (PlayFabClientAPI.IsClientLoggedIn())
            RefreshTicketBalance();
    }

    public void Buy(string productId)
    {
        if (controller == null)
        {
            Debug.LogError("❌ IAP Controller non inizializzato.");
            return;
        }

        var product = controller.products.WithID(productId);
        if (product == null)
        {
            Debug.LogError("❌ Prodotto non trovato: " + productId);
            return;
        }

        if (!product.availableToPurchase)
        {
            Debug.LogWarning("⚠️ Prodotto non disponibile: " + productId);
            return;
        }

        Debug.Log($"🛒 Acquisto avviato: {productId}");
        controller.InitiatePurchase(product);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        this.controller = controller;
        this.extensions = extensions;
        Debug.Log("🛒 IAP inizializzato");
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogError("❌ Inizializzazione IAP fallita: " + error);
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        throw new System.NotImplementedException();
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
    {
        Debug.LogError($"❌ Acquisto fallito per {product.definition.id}: {reason}");
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        string id = args.purchasedProduct.definition.id;

        if (!productToAmount.ContainsKey(id))
            return PurchaseProcessingResult.Complete;

        int amount = productToAmount[id];

#if UNITY_EDITOR
        Debug.Log($"🧪 Simulazione acquisto in editor: {id}, +{amount} tickets");
        GrantTickets(amount);
        return PurchaseProcessingResult.Complete;
#endif

#if UNITY_ANDROID
        var receipt = MiniJson.JsonDecode(args.purchasedProduct.receipt) as Dictionary<string, object>;
        var payload = MiniJson.JsonDecode(receipt["Payload"].ToString()) as Dictionary<string, object>;
        var json = payload["json"].ToString();
        var signature = payload["signature"].ToString();

        PlayFabClientAPI.ValidateGooglePlayPurchase(new ValidateGooglePlayPurchaseRequest
        {
            ReceiptJson = json,
            Signature = signature
        },
        res =>
        {
            Debug.Log("✅ Android validato: " + id);
            GrantTickets(amount);
        },
        err => Debug.LogError("❌ Errore validazione Android: " + err.GenerateErrorReport()));
#elif UNITY_IOS
        PlayFabClientAPI.ValidateApplePurchase(new ValidateApplePurchaseRequest
        {
            ReceiptData = args.purchasedProduct.receipt
        },
        res =>
        {
            Debug.Log("✅ iOS validato: " + id);
            GrantTickets(amount);
        },
        err => Debug.LogError("❌ Errore validazione iOS: " + err.GenerateErrorReport()));
#endif

        return PurchaseProcessingResult.Complete;
    }

    private void GrantTickets(int amount)
    {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest
        {
            FunctionName = "grantTickets",
            FunctionParameter = new { amount }
        },
        res =>
        {
            Debug.Log($"🎟️ Tickets accreditati: {amount}");
            RefreshTicketBalance();
        },
        err => Debug.LogError("❌ Errore CloudScript: " + err.GenerateErrorReport()));
    }

    public void RefreshTicketBalance()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
        result =>
        {
            if (result.VirtualCurrency.TryGetValue("TK", out int currentTickets))
            {
                GameManager.Instance.tickets = currentTickets;
                if (ticketText != null)
                    ticketText.text = $"Tickets: {currentTickets}";
                Debug.Log($"🎟️ Tickets dal server: {currentTickets}");
            }
            else
            {
                GameManager.Instance.tickets = 0;
                if (ticketText != null)
                    ticketText.text = $"Tickets: 0";
                Debug.Log("⚠️ Nessun ticket trovato per questo utente");
            }
        },
        error =>
        {
            GameManager.Instance.tickets = 0;
            Debug.LogError("❌ Errore nel recupero tickets: " + error.GenerateErrorReport());
        });
    }

    public void SpendTickets(int amount)
    {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest
        {
            FunctionName = "spendTickets",
            FunctionParameter = new { amount }
        },
        result =>
        {
            Debug.Log($"✅ Spesi {amount} tickets con successo");
            RefreshTicketBalance();
        },
        error =>
        {
            Debug.LogError("❌ Errore nella spesa tickets: " + error.GenerateErrorReport());
        });
    }

    // ✅ Questo metodo spende i ticket e avvia la partita se va tutto bene
    public void SpendTicketsAndStartMatch(int amount, Action onSuccess)
    {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest
        {
            FunctionName = "spendTickets",
            FunctionParameter = new { amount }
        },
        result =>
        {
            Debug.Log($"✅ Ticket speso: {amount}");
            RefreshTicketBalance();
            onSuccess?.Invoke();
        },
        error =>
        {
            Debug.LogError("❌ Errore nello spendere il ticket: " + error.GenerateErrorReport());
            // Qui puoi mostrare un popup o bloccare l'accesso alla partita
        });
    }
}
