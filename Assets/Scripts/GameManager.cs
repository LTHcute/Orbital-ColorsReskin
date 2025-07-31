

using UnityEngine;

public class GameManager : MonoBehaviour
{
	public UIManager uIManager;

	public ScoreManager scoreManager;

	[Header("Game settings")]
	[Space(5f)]
	public GameObject camObject;

	[Space(5f)]
	public GameObject player;

	[Space(5f)]
	public int playerSpeed;

	[Space(5f)]
	public Color[] colorTable;

	[Space(5f)]
	public GameObject obstaclePrefab;

	[Space(5f)]
	public float yMinDistanceBetweenObstacles = 5f;

	[Space(5f)]
	public float yMaxDistanceBetweenObstacles = 10f;

	[Space(5f)]
	public float maxXDistanceNextObstacle = 5f;

	[Space(5f)]
	public bool readyToShoot;

	private GameObject previousObstacle;

	private GameObject tempObstacle;

	private Color tempColor;

	private int obstacleId;

	public bool movingPlayer;

	private float step;

	private Vector2 flyDestination;

	private GameObject currentObstacle;


	public Sprite[] imageTable;
	public static GameManager Instance
	{
		get;
		set;
	}

	private void Awake()
	{
		Sprite[] imageObstacle = Resources.LoadAll<Sprite>("ObstacleSprite");
		Sprite[] imagePlayer = Resources.LoadAll<Sprite>("PlayerSprite");
        imageTable = new Sprite[imageObstacle.Length + imagePlayer.Length];

        imagePlayer.CopyTo(imageTable, 0);
        imageObstacle.CopyTo(imageTable, imagePlayer.Length);
		
		Debug.Log(imageTable.Length);
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

	private void Start()
	{
		Physics2D.gravity = new Vector2(0f, 0f);
		Application.targetFrameRate = 30;
		step = (float)playerSpeed * Time.deltaTime;
		CreateScene();
	}

	private void Update()
	{
		
		if (uIManager.gameState == GameState.PLAYING && Input.GetMouseButtonDown(0))
		{
			if (!uIManager.IsButton() && readyToShoot && !movingPlayer)
			{
				ShotBall();
			}
		}
		else if (uIManager.gameState == GameState.PLAYING && movingPlayer)
		{
			player.transform.position = Vector2.MoveTowards(player.transform.position, flyDestination, step);
			if (Vector2.Distance(player.transform.position, flyDestination) < 0.001f)
			{
				movingPlayer = false;
				readyToShoot = true;
				flyDestination = tempObstacle.transform.position;
             
            }
		}
		else if (uIManager.gameState == GameState.PLAYING && Input.GetMouseButtonUp(0))
		{
			readyToShoot = true;
		}
	}

	public void CreateScene()
	{
		ResetPlayerAnimation();
		obstacleId = 0;
		tempColor = colorTable[Random.Range(0, colorTable.Length)];
		//player.GetComponent<SpriteRenderer>().color = tempColor;
		previousObstacle = UnityEngine.Object.Instantiate(obstaclePrefab);
		previousObstacle.transform.position = new Vector2(0f, -3f);

		tempObstacle = UnityEngine.Object.Instantiate(obstaclePrefab);
		tempObstacle.transform.position = new Vector2(0f, 3f);
		flyDestination = tempObstacle.transform.position;
		previousObstacle.GetComponent<Obstacle>().SetObstacle(tempColor, obstacleId);
		previousObstacle.GetComponent<Obstacle>().SetNextObstaclePosition(tempObstacle.transform.position);
		obstacleId++;
		Debug.Log(colorTable.Length);
		Debug.Log(tempColor);
		tempColor = colorTable[Random.Range(0, colorTable.Length)];
		tempObstacle.GetComponent<Obstacle>().SetObstacle(tempColor, obstacleId);
		camObject.transform.position = new Vector3(0f, 0f, -10f);
		player.transform.position = previousObstacle.transform.position;
		readyToShoot = true;
	}


	public void ShotBall()
	{
		Debug.Log("Shotball");
		readyToShoot = false;
		movingPlayer = true;
		CreateNextObstacle();
		camObject.GetComponent<CameraFollowTarget>().EnableDisableFollow(status: true);

	}

	private void CreateNextObstacle()
	{
		
        Debug.Log("CreateNextObstacle");
        obstacleId++;
		tempColor = colorTable[Random.Range(0, colorTable.Length)];
		float num = UnityEngine.Random.Range(yMinDistanceBetweenObstacles, yMaxDistanceBetweenObstacles);
		previousObstacle = UnityEngine.Object.Instantiate(obstaclePrefab);
		previousObstacle.transform.position = new Vector2(tempObstacle.transform.position.x + UnityEngine.Random.Range(0f - maxXDistanceNextObstacle, maxXDistanceNextObstacle), tempObstacle.transform.position.y + num);

        previousObstacle.GetComponent<Obstacle>().SetObstacle(tempColor, obstacleId);
		tempObstacle.GetComponent<Obstacle>().SetNextObstaclePosition(previousObstacle.transform.position);
       
        tempObstacle = previousObstacle;
     
    }

	public void PlayerDeath()
	{
		camObject.GetComponent<CameraFollowTarget>().ShakeCamera();
		player.GetComponent<Player>().PlayGameOver();
	}

	public void ResetPlayerAnimation()
	{
		player.GetComponent<Player>().ResetPlayer();
	}

	public void RestartGame()
	{
        Debug.Log("2");
        if (uIManager.gameState == GameState.PAUSED)
		{
			Time.timeScale = 1f;
		}
		ClearScene();
		CreateScene();
		readyToShoot = false;
		movingPlayer = false;
		scoreManager.ResetCurrentScore();
		uIManager.ShowGameplay();
		camObject.GetComponent<CameraFollowTarget>().EnableDisableFollow(status: false);
	}

	public void ClearScene()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("Obstacle");
		for (int i = 0; i < array.Length; i++)
		{
			UnityEngine.Object.Destroy(array[i]);
		}
	}

	public void GameOver()
	{
		if (uIManager.gameState == GameState.PLAYING)
		{
			movingPlayer = false;
			AudioManager.Instance.PlayEffects(AudioManager.Instance.gameOver);
			uIManager.ShowGameOver();
			scoreManager.UpdateScoreGameover();
		}
	}
	public void ShowPlayer()
	{
		player.SetActive(true);
	}	
	public void HidePlayer()
	{
		player.SetActive(false);
	}	
}
