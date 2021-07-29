using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    //The script responsible for keeping track of the score.
    private ScoreManager scoreManager;

    //The script responsible for all common game and UI events.
    private EventManager eventManager;

    //The rigidbody attached to the player.
    private Rigidbody player;

    [Header("Player Movement")]
    //The initial speed of the player.
    [Tooltip("The speed of the player object.")]
    public float speed = 12f;

    //The speed at which the object will rotate.
    [Tooltip("The speed at which the player object will rotate.")]
    public float rotationSpeed = 10f;

    [Tooltip("The speed at which the player object will translate.")]
    public float horizontalSpeed = 10f;

    [Tooltip("The speed at which the player object will drop.")]
    public float gravity = -0.98f;

    [Header("Miscellaneous")]

    public LayerMask threatLayer;
    public bool acceptInput = false;

    private float timeScale = 1f;
    private Transform eagle;
    private Transform camera;
    private Image booster;
    Vector3 wantedRotation;
    Vector3 wantedPosition;
    Vector3 startPosition;
    private Vector3 detectorPosition;
    private Vector3 endPosition;
    private float _x, _y;
    private void Awake() {
        scoreManager = GameObject.FindObjectOfType<ScoreManager>();
        eventManager = GameObject.FindObjectOfType<EventManager>();
        eagle = transform.Find("Eagle");
        booster = GameObject.Find("Booster").GetComponent<Image>();
        startPosition = eagle.localPosition;
        InitPosition();
        eventManager.playerController = this;
        eventManager.scoreManager = scoreManager;
        player = GetComponent<Rigidbody>();
        camera = Camera.main.transform;
    }
    public void InitPosition()
    {
        eagle.localPosition = startPosition;
        wantedPosition = startPosition;
    }
    void Update()
    {
        if (acceptInput)
        {
            //If we press the right arrow key, set the new wantedRotation.
            _x = Input.GetAxis("Horizontal");
            _y = Input.GetAxis("Vertical");
            if (Input.GetKey(KeyCode.LeftShift))
            {
                wantedRotation.y = Mathf.Clamp(wantedRotation.y + (rotationSpeed * Time.fixedDeltaTime * _x) * 10, -30, 30);
                wantedRotation.x = Mathf.Clamp(wantedRotation.y + (rotationSpeed * Time.fixedDeltaTime * _y) * 10, -30, 30);
            }
            else
            {
                wantedPosition.x += _x * horizontalSpeed * Time.fixedDeltaTime;
                wantedPosition.y += _y * horizontalSpeed * Time.fixedDeltaTime;
            }
            if (Input.GetKey(KeyCode.Space))
            {
                timeScale = Mathf.Min(timeScale + Time.deltaTime * 0.3f, 2);
            }
            else
            {
                timeScale = Mathf.Max(1, timeScale - Time.deltaTime);
            }
            Time.timeScale = timeScale;
            booster.fillAmount = timeScale - 1;
            wantedPosition.y += gravity * Time.fixedDeltaTime;
        }
    }
    void FixedUpdate()
    {
        //Move forward at a constant speed.
        player.MovePosition(transform.position + transform.forward * Time.deltaTime * speed);
        eagle.localRotation = Quaternion.Lerp(eagle.localRotation, Quaternion.Euler(wantedRotation), Time.deltaTime * rotationSpeed);
        eagle.localPosition = Vector3.Lerp(eagle.localPosition, wantedPosition, Time.deltaTime * horizontalSpeed);
        camera.LookAt(eagle);
    }
    void OnTriggerEnter(Collider threat)
    {
        //Does the object that we have collided with belong to the threat layer that we have set?
        if (((1 << threat.gameObject.layer) & threatLayer) != 0)
        {
            threat.gameObject.SetActive(false);

            //If we aren't accepting input, we do not want to provide feedback.
            if (acceptInput)
            {
                //Camera.main.DOShakePosition(.3f, .2f, 50, 50);
                eventManager.GameOver();
            }

        }
    }
}
