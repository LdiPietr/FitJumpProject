using TMPro;
using UnityEngine;

public class SwipeTest : MonoBehaviour
{
    public enum TouchPhase
    {
        Start,
        Stop
    }

    public TouchPhase touchPhase = TouchPhase.Stop;

    public float speed = 19.5f;
    public float lastPosition = 0.0f;
    public float tolerance = 0.1f;
    public float moveInput;

    public TextMeshProUGUI touch;
    public TextMeshProUGUI swipe;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (touchPhase == TouchPhase.Stop)
            {
                touchPhase = TouchPhase.Start;
                lastPosition = Input.mousePosition.x;
            }
            else
            {
                var delta = Input.mousePosition.x - lastPosition;
                if (Mathf.Abs(delta) > tolerance)
                {
                    var screen = Screen.width * 0.00001f;
                    moveInput = delta;
                    lastPosition = Input.mousePosition.x;
                    var vector3 = transform.position;
                    vector3.x += moveInput * screen * Time.deltaTime * speed;
                    transform.position = vector3;
                }
                else
                {
                    moveInput = 0.0f;
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            touchPhase = TouchPhase.Stop;
            moveInput = 0.0f;
        }

        touch.text = "Touch Phase =" + touchPhase;
        swipe.text = "Move Input =" + moveInput;
    }
}