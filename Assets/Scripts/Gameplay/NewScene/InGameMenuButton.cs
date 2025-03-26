using UnityEngine;
using UnityEngine.UI;

public class InGameMenuButton : MonoBehaviour
{
    public Color[] colors;
    private Image spriteRenderer;
    public GameObject panel;
    private bool isPaused;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<Image>();
        spriteRenderer.color = colors[0];
        isPaused = false;
    }

    // Update is called once per frame
    public void Click()
    {
        if (isPaused)
        {
            Time.timeScale = 1f;
            isPaused = false;
            spriteRenderer.color = colors[0];
            panel.SetActive(false);
        }
        else
        {
            Time.timeScale = 0f;
            isPaused = true;
            spriteRenderer.color = colors[1];
            panel.SetActive(true);
        }
    }
}
