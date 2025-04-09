using UnityEngine;
using UnityEngine.UI;

public class TournamentButtonEnable : MonoBehaviour
{
    public Button tournamentButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        tournamentButton.interactable = GameManager.Instance.tickets > 0;
    }
}