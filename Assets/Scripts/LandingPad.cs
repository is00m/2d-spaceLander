using UnityEngine;

public class LandingPad : MonoBehaviour {
    [SerializeField] private int scoreMultiplier;

    public float getScoreMultiplier() {
        return scoreMultiplier;
    }
}
