using UnityEngine;
using UnityEngine.UI;

public class MusicButton : MonoBehaviour
{
    public Color[] colors;
    private Image spriteRenderer;
    private bool playing;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<Image>();
        playing = AudioManager.Instance.musicSource.volume > 0;
        
        spriteRenderer.color = playing ? colors[0] : colors[1];
    }

    public void Click()
    {
        if (playing)
        {
            AudioManager.Instance.musicSource.volume = 0f;
            spriteRenderer.color = colors[1];
            playing = false;
        }
        else
        {
            AudioManager.Instance.musicSource.volume = 1f;
            spriteRenderer.color = colors[0];
            playing = true;
        }
    }
}
