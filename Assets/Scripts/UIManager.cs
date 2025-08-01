

using System.Collections;
using System.Collections.Generic;
using UniPay;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	[Header("GUI Components")]
	public GameObject introGui;
	public Button play;
	public GameObject mainMenuGui;

	public GameObject pauseGui;

	public GameObject gameplayGui;

	public GameObject gameOverGui;

	public GameState gameState;

	private bool clicked;


	public GameObject notications;
	private void Start()
	{
		introGui.SetActive(true);
		//GameManager.Instance.HidePlayer();
		mainMenuGui.SetActive(value: false);
		pauseGui.SetActive(value: false);
		gameplayGui.SetActive(value: false);
		gameOverGui.SetActive(value: false);
		gameState = GameState.INTRO;
		play.onClick.AddListener(ShowMainMenuGui);

	}


    public void ShowNotification()
    {
        notications.SetActive(true);
        StartCoroutine(HideNotification());
    }

    IEnumerator HideNotification()
    {
        yield return new WaitForSeconds(1f);
        notications.SetActive(false);
    }
    void ShowMainMenuGui()
	{
	GameManager.Instance.ShowPlayer();
		mainMenuGui.SetActive(true);
		introGui.SetActive(false);
        pauseGui.SetActive(value: false);
        gameplayGui.SetActive(value: false);
        gameOverGui.SetActive(value: false);
        gameState = GameState.MENU;
    }	

	private void Update()
	{
		if (Input.GetMouseButtonDown(0) && gameState == GameState.MENU && !clicked)
		{
			if (!IsButton())
			{
				AudioManager.Instance.PlayEffects(AudioManager.Instance.buttonClick);
				ShowGameplay();
				GameManager.Instance.ShotBall();
			}
		}
		else if (Input.GetMouseButtonUp(0) && clicked && gameState == GameState.MENU)
		{
			clicked = false;
		}
	}

	public void ContinuePlay()
	{
        int playCount = DBManager.GetCurrency("map");
       
        if (playCount == 0)
		{
			ShowNotification();
			return;

        }	
        gameplayGui.SetActive(value: true);
        pauseGui.SetActive(value: false);
        gameOverGui.SetActive(value: false);
        Time.timeScale = 1f;
        gameState = GameState.PLAYING;
        AudioManager.Instance.PlayEffects(AudioManager.Instance.buttonClick);
        DBManager.SetCurrency("map", playCount - 1);
    }	

	public void ShowMainMenu()
	{
		
		Debug.Log("show main menu");
		ScoreManager.Instance.ResetCurrentScore();
		clicked = true;
		mainMenuGui.SetActive(value: true);
		pauseGui.SetActive(value: false);
		gameplayGui.SetActive(value: false);
		gameOverGui.SetActive(value: false);
		if (gameState == GameState.PAUSED)
		{
			Time.timeScale = 1f;
		}
		gameState = GameState.MENU;
		AudioManager.Instance.PlayEffects(AudioManager.Instance.buttonClick);
		GameManager.Instance.ClearScene();
		GameManager.Instance.CreateScene();
	}

	public void ShowPauseMenu()
	{
		if (gameState != GameState.PAUSED)
		{
            gameplayGui.SetActive(value: false);
            pauseGui.SetActive(value: true);
			Time.timeScale = 0f;
			gameState = GameState.PAUSED;
			AudioManager.Instance.PlayEffects(AudioManager.Instance.buttonClick);
		}
	}

	public void HidePauseMenu()
	{
        gameplayGui.SetActive(value: true);
        pauseGui.SetActive(value: false);
		Time.timeScale = 1f;
		gameState = GameState.PLAYING;
		AudioManager.Instance.PlayEffects(AudioManager.Instance.buttonClick);
	}

	public void ShowGameplay()
	{
		mainMenuGui.SetActive(value: false);
		pauseGui.SetActive(value: false);
		gameplayGui.SetActive(value: true);
		gameOverGui.SetActive(value: false);
		gameState = GameState.PLAYING;
		AudioManager.Instance.PlayEffects(AudioManager.Instance.buttonClick);
	}

	public void ShowGameOver()
	{
		mainMenuGui.SetActive(value: false);
		pauseGui.SetActive(value: false);
		gameplayGui.SetActive(value: false);
		gameOverGui.SetActive(value: true);
		gameState = GameState.GAMEOVER;
	}

	public bool IsButton()
	{
		bool flag = false;
		PointerEventData eventData = new PointerEventData(EventSystem.current)
		{
			position = UnityEngine.Input.mousePosition
		};
		List<RaycastResult> list = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventData, list);
		foreach (RaycastResult item in list)
		{
			flag |= (item.gameObject.GetComponent<Button>() != null);
		}
		return flag;
	}
}
