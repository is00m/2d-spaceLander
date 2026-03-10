using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour {

    private int score;

    private void Start() {
        Lander.Instance.OnLanded += Lander_OnLanded;
        Lander.Instance.OnCoinPickup += Lander_OnCoinPickup;
    }

    private void Lander_OnLanded(object sender, Lander.OnLandedEventArgs e) {
        AddScore(e.score);
    }

    private void Lander_OnCoinPickup(object sender, System.EventArgs e) {
        AddScore(500);
    }

    public void AddScore(int scoreAmount) {
        score += scoreAmount;
        Debug.Log("Score: " + score);
    }
}