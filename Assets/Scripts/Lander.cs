using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Lander : MonoBehaviour {

    public static Lander Instance { get; private set; }

    public event EventHandler OnUpForce;
    public event EventHandler OnLeftForce;
    public event EventHandler OnRightForce;
    public event EventHandler OnBeforeForce;
    public event EventHandler OnCoinPickup;
    public event EventHandler <OnLandedEventArgs> OnLanded;
    public class OnLandedEventArgs : EventArgs {
        public int score;
    }

    private Rigidbody2D landerRigidbody2D;

    private float fuelAmount = 10f;
    private void Awake() {
        Instance = this;
        landerRigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate() {
        OnBeforeForce?.Invoke(this, EventArgs.Empty);

        if (fuelAmount <= 0f) {
            return;
        }

        if (
            Keyboard.current.upArrowKey.isPressed
            || Keyboard.current.leftArrowKey.isPressed
            || Keyboard.current.rightArrowKey.isPressed
        ) {
            ConsumeFuel();
        }
        if (Keyboard.current.upArrowKey.isPressed) {
            float force = 700f;
            landerRigidbody2D.AddForce(force * transform.up * Time.deltaTime);
            OnUpForce?.Invoke(this, EventArgs.Empty);
        }
        if (Keyboard.current.leftArrowKey.isPressed) {
            float turnSpeed = +100f;
            landerRigidbody2D.AddTorque(turnSpeed * Time.deltaTime);
            OnLeftForce?.Invoke(this, EventArgs.Empty);
        }
        if (Keyboard.current.rightArrowKey.isPressed) {
            float turnSpeed = -100f;
            landerRigidbody2D.AddTorque(turnSpeed * Time.deltaTime);
            OnRightForce?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision2D) {
        if (!collision2D.gameObject.TryGetComponent(out LandingPad landingPad)) {
            Debug.Log("Crashed on the Terrain!");
            return;
        }
        float softLandingVelocityMagnitude = 4f;
        float relativeVelocityMagnitude = collision2D.relativeVelocity.magnitude;
        if (relativeVelocityMagnitude > softLandingVelocityMagnitude) {
            Debug.Log("Landed to hard!");
            return;
        }

        float dotVector = Vector2.Dot(Vector2.up, transform.up);
        float minDotVector = .90f;
        if (dotVector < minDotVector) {
            Debug.Log("Landed on a too steep angle!");
            return;
        }
        Debug.Log("Successful landing!");

        float maxScoreAmountLandingAngle = 100;
        float scoreDotVectorMultiplier = 10f;
        float landingAngleScore =
            maxScoreAmountLandingAngle
            - Mathf.Abs(dotVector - 1f) * scoreDotVectorMultiplier * maxScoreAmountLandingAngle;

        float maxScoreAmounLandingSpeed = 100;
        float landingSpeedScore =
            (softLandingVelocityMagnitude - relativeVelocityMagnitude) * maxScoreAmounLandingSpeed;

        Debug.Log("landingAngleSAcore" + landingAngleScore);
        Debug.Log("landingSpeedScore" + landingSpeedScore);

        int score = Mathf.RoundToInt(
            (landingAngleScore + landingSpeedScore) * landingPad.getScoreMultiplier()
        );
        Debug.Log("Score:" + score);
        OnLanded?.Invoke(this, new OnLandedEventArgs { score = score, });
    }

    private void OnTriggerEnter2D(Collider2D collision2d) {
        if (collision2d.gameObject.TryGetComponent(out FuelPickup fuelPickup)) {
            float addFuelAmount = 10f;
            fuelAmount += addFuelAmount;
            fuelPickup.DestroySelf();
            Debug.Log("Fuel Amount:" + fuelAmount);
        }
        if (collision2d.gameObject.TryGetComponent(out CoinPickup coinPickup)) {
            OnCoinPickup?.Invoke(this, EventArgs.Empty);
            coinPickup.DestroySelf();
        }
    }

    private void ConsumeFuel() {
        float fuelConsumptionAmount = 1f;
        fuelAmount -= fuelConsumptionAmount * Time.deltaTime;
    }
}
