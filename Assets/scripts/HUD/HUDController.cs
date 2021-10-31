using System;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    public GliderController controller;
    [Header("Main")]
    public Text speedDisplay;
    public Text accelerationDisplay;
    public Image nitroBar;
    public RectTransform plane;
    public Text pitch;
    public Text roll;
    public Text timer;
    [Header("Debug")]
    public GameObject debugScreen;
    public float refreshTime = 0.5f;
    public Text leftSide;
    [Header("Curves")]
    public AnimationCurve nitroBarCurve;

    float accel = 0;
    float prevV = 0;
    bool f3Screen = false;

    int frameCounter = 0;
    float timeCounter = 0.0f;
    float fps = 0.0f;

    private void Awake()
    {
        controller.input.actions["Debug"].performed += _ => { f3Screen = !f3Screen; };
    }

    private void Update()
    {
        timer.text = "";

        speedDisplay.text = (int) controller.GetRB().velocity.magnitude + " m/s";
        accelerationDisplay.text = controller.currentScore + " pts";
        debugScreen.SetActive(f3Screen);

        Vector3 angles = controller.transform.localEulerAngles;
        if (angles.x > 180) angles.x -= 360;
        if (angles.z > 180) angles.z -= 360;
        pitch.text = (int)-angles.x + "�";
        roll.text = (int) angles.z + "�";
        angles.x += 90;
        plane.rotation = Quaternion.Euler(angles.x, 0, angles.z);

        nitroBar.fillAmount = controller.RemainingFuel();
        Color32 redColor = new Color32(0xe7, 0x4c, 0x3c, 0xff);
        Color32 yellowColor = new Color32(0xf3, 0x9c, 0x12, 0xff);
        Color32 greenColor = new Color32(0x27, 0xae, 0x60, 0xff);
        if (controller.RemainingFuel() < 0.2f) nitroBar.color = Color32.Lerp(redColor, yellowColor, nitroBarCurve.Evaluate(controller.RemainingFuel() * 5));
        else if (controller.RemainingFuel() < 0.6f) nitroBar.color = Color32.Lerp(yellowColor, greenColor, nitroBarCurve.Evaluate((controller.RemainingFuel() - 0.2f) * 2.5f));
        else nitroBar.color = greenColor;

        if (timeCounter < refreshTime)
        {
            timeCounter += Time.smoothDeltaTime;
            frameCounter++;
        }
        else
        {
            fps = (float)frameCounter / timeCounter;
            frameCounter = 0;
            timeCounter = 0.0f;
        }

        leftSide.text = @$" FPS: {(int)fps}
 Position: {controller.transform.position.x} / {controller.transform.position.y} / {controller.transform.position.z}
 Distance to Nearest: {controller.GetMinDistance()}
";
    }

    private void FixedUpdate()
    {
        accel = (controller.GetRB().velocity.magnitude - prevV) / Time.fixedDeltaTime;
        prevV = controller.GetRB().velocity.magnitude;
    }
}
