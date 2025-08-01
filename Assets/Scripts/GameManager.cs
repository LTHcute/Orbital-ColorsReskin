

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniPay;
using UnityEngine;
using UnityEngine.UI;

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
	public int playerSpeed = 1;

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
    public Text currency;

    public Sprite[] imageTable;
	public static GameManager Instance
	{
		get;
		set;
	}
    public GameObject store;

    [Header("Wind & Trajectory")]
    public LineRenderer trajectoryLine;
    public int trajectoryPointCount = 30;
    public Vector2 windForce = new Vector2(1f, 0f);
    private bool isCurvedTrajectory = false; // NEW
    private Vector2 cachedControlPoint;
    private bool controlPointGenerated = false;
    private List<Vector2> flyPathPoints = new List<Vector2>();

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
        DrawLineToTarget(player.transform.position, tempObstacle.transform.position);
    }

	private void Update()
	{
        currency.text = DBManager.GetCurrency("map").ToString();
        if (uIManager.gameState == GameState.PLAYING && Input.GetMouseButtonDown(0))
        {
            if (!uIManager.IsButton() && readyToShoot && !movingPlayer)
            {
                ShotBall();
            }
        }
        else if (uIManager.gameState == GameState.PLAYING && Input.GetMouseButtonUp(0))
        {
            readyToShoot = true;
        }

        if (uIManager.gameState == GameState.PLAYING && readyToShoot && !movingPlayer)
        {
         
            DrawLineToTarget(player.transform.position, tempObstacle.transform.position);
        }
        Obstacle currentObs = GetCurrentObstacleUnderPlayer();
    }
    private void DrawCurvedLine(Vector2 startPos, Vector2 endPos)
    {
        trajectoryLine.positionCount = trajectoryPointCount;

        if (!controlPointGenerated)
        {
            Vector2 midPoint = (startPos + endPos) / 2f;
            Vector2 dir = endPos - startPos;

           
            Vector2 perpendicular = Vector2.Perpendicular(dir).normalized;

         
            float flip = Random.value < 0.5f ? 1f : -1f;

            float curveStrength = Random.Range(1.5f, 3.5f);
            cachedControlPoint = midPoint + perpendicular * flip * curveStrength;

            controlPointGenerated = true;
        }

        for (int i = 0; i < trajectoryPointCount; i++)
        {
            float t = i / (float)(trajectoryPointCount - 1);
            Vector2 point = Mathf.Pow(1 - t, 2) * startPos +
                            2 * (1 - t) * t * cachedControlPoint +
                            Mathf.Pow(t, 2) * endPos;

            trajectoryLine.SetPosition(i, point);
        }
    }

    private void DrawStraightLine(Vector2 startPos, Vector2 targetPos)
    {
        trajectoryLine.positionCount = 2;
        trajectoryLine.SetPosition(0, startPos);
        trajectoryLine.SetPosition(1, targetPos);
    }
    private IEnumerator MovePlayerAlongCurve(Vector2 startPos, Vector2 controlPoint, Vector2 endPos, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            float t = time / duration;
            Vector2 pos = Mathf.Pow(1 - t, 2) * startPos +
                          2 * (1 - t) * t * controlPoint +
                          Mathf.Pow(t, 2) * endPos;

            player.transform.position = pos;

            time += Time.deltaTime;
            yield return null;
        }

        player.transform.position = endPos;

        OnPlayerArrived();
    }
    private void OnPlayerArrived()
    {
        movingPlayer = false;
        readyToShoot = true;
    //    flyDestination = tempObstacle.transform.position;
        CreateNextObstacle();
    }

    private IEnumerator MovePlayerAlongStraight(Vector2 startPos, Vector2 endPos, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            float t = time / duration;
            Vector2 pos = Vector2.Lerp(startPos, endPos, t);
            player.transform.position = pos;

            time += Time.deltaTime;
            yield return null;
        }

        player.transform.position = endPos;

        OnPlayerArrived();
    }


    public Obstacle GetCurrentObstacleUnderPlayer()
    {
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        Vector2 playerPos = player.transform.position;

        foreach (GameObject obs in obstacles)
        {
          
            if (Vector2.Distance(obs.transform.position, playerPos) < 0.1f)
            {
                Debug.Log("Player đang đứng trên obstacle ID: "+ obs.transform.position);
				Destroy(obs);
            }
        }

        return null; 
    }


    public void CreateScene()
	{
	

      //  ResetPlayerAnimation();
        obstacleId = 0;
        tempColor = colorTable[Random.Range(0, colorTable.Length)];
        tempObstacle = UnityEngine.Object.Instantiate(obstaclePrefab);
        tempObstacle.transform.position = new Vector2(0f, 3f);
        flyDestination = tempObstacle.transform.position;
        obstacleId++;
        Debug.Log(colorTable.Length);
        Debug.Log(tempColor);
        tempColor = colorTable[Random.Range(0, colorTable.Length)];
        tempObstacle.GetComponent<Obstacle>().SetObstacle(tempColor, obstacleId);
        camObject.transform.position = new Vector3(0f, 0f, -10f);
        player.transform.position = new Vector2(0f, -3f);
        readyToShoot = true;
        controlPointGenerated = false;

    }


    public void ShotBall()
	{
		

        Debug.Log("Shotball");
        readyToShoot = false;
        movingPlayer = true;
        camObject.GetComponent<CameraFollowTarget>().EnableDisableFollow(status: true);
        isCurvedTrajectory = Random.value < 0.5f; // 50% cong - 50% thẳng
        trajectoryLine.positionCount = 0;

        Vector2 startPos = player.transform.position;
        Vector2 endPos = tempObstacle.transform.position;

      
        DrawCurvedLine(startPos, endPos);
        StartCoroutine(MovePlayerAlongCurve(startPos, cachedControlPoint, endPos, 0.7f));
      
      
    }

    private void DrawLineToTarget(Vector2 startPos, Vector2 endPos)
    {
            DrawCurvedLine(startPos, endPos);
       
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
        controlPointGenerated = false;

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

    public void OpenStore()
    {
        if (uIManager.gameState == GameState.PAUSED)
        {
            Time.timeScale = 1f;
        }
        store.SetActive(true);
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
