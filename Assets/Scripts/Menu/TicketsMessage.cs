using TMPro;
using UnityEngine;

public class TicketsMessage : MonoBehaviour
{
    public TextMeshProUGUI messageObject;

    // Update is called once per frame
    void Update()
    {
        messageObject.text = "TICKETS: " + GameManager.Instance.tickets;
    }
}