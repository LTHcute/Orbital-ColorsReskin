

using UniPay;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
	public Text currentScoreLabel;

	public Text highScoreLabel;

	public Text currentScoreGameOverLabel;

	public Text highScoreGameOverLabel;

	public int currentScore;

	public int highScore;

	private bool counting;

    public Text currency;
    public static ScoreManager Instance
	{
		get;
		set;
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	void Update()
	{
        currency.text = DBManager.GetCurrency("map").ToString();
    }	

	private void Start()
	{
       

        if (!PlayerPrefs.HasKey("HighScore"))
		{
			PlayerPrefs.SetInt("HighScore", 0);
		}
		highScore = PlayerPrefs.GetInt("HighScore");
		UpdateHighScore();
		ResetCurrentScore();
	}

	private void UpdateHighScore()
	{
        currency.text = DBManager.GetCurrency("map").ToString();
        if (currentScore > highScore)
		{
			highScore = currentScore;
		}
		highScoreLabel.text = highScore.ToString();
		PlayerPrefs.SetInt("HighScore", highScore);
	}

	public void UpdateScore(int value)
	{
		currentScore += value;
		currentScoreLabel.text = currentScore.ToString();
	}

	public void ResetCurrentScore()
	{
		currentScore = 0;
		UpdateScore(0);
	}

	public void UpdateScoreGameover()
	{
		UpdateHighScore();
		currentScoreGameOverLabel.text = currentScore.ToString();
		highScoreGameOverLabel.text = highScore.ToString();
	}
}
