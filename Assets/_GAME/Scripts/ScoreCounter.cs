using TMPro;
using UnityEngine;

public sealed class ScoreCounter : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI scoreText;

    public static ScoreCounter Instance { get; private set; }


    private int score = 0;

    public int Score
    {
        get => score;

        set
        {
            if (score == value) return;

            score = value;

            scoreText.SetText($"Score: {score}");
        }
    }

    private void Awake() => Instance = this;



}
