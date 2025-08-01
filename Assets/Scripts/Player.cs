

using UnityEngine;

public class Player : MonoBehaviour
{
	private SpriteRenderer rend;

	private Animator animator;

	private bool inside;

	private bool changedColor;

	private void Start()
	{
		rend = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		//if (collision.CompareTag("Obstacle"))
		//{
		//	inside = false;
		//	changedColor = false;
		//}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
        Debug.Log($"Player:{collision.gameObject.tag}");
        //if (GameManager.Instance.uIManager.gameState != GameState.GAMEOVER)
        //{
        //	if (collision.CompareTag("SmallBall") && !inside && !changedColor)
        //	{
        //		AudioManager.Instance.PlayEffects(AudioManager.Instance.pickColor);
        //		ScoreManager.Instance.UpdateScore(1);
        //		changedColor = true;
        //		rend.color = collision.GetComponent<SpriteRenderer>().color;
        //		UnityEngine.Object.Destroy(collision.gameObject.transform.parent.gameObject);
        //	}
        //	if (collision.CompareTag("Obstacle") && !inside)
        //	{
        //		inside = true;
        //	}
        //}




        if (GameManager.Instance.uIManager.gameState == GameState.PLAYING)
        {
            if (collision.gameObject.tag == "Gift")
            {
                Debug.Log($"Player:1");
                AudioManager.Instance.PlayEffects(AudioManager.Instance.pickColor);
                ScoreManager.Instance.UpdateScore(1);
                //changedColor = true;
                //rend.color = collision.GetComponent<SpriteRenderer>().color;
                UnityEngine.Object.Destroy(collision.gameObject.transform.parent.gameObject);
                return;
            }
           
            if (collision.gameObject.tag == "Shark")
            {
                Debug.Log($"Player:2");
                Debug.Log("thua");
                //PlayWaveOut();
                GameManager.Instance.PlayerDeath();
                GameManager.Instance.GameOver();
                GameManager.Instance.camObject.GetComponent<CameraFollowTarget>().ShakeCamera();
                return;
            }
        }
    }

    public void PlayGameOver()
	{
		animator.Play("PlayerDeath");
	}

	public void ResetPlayer()
	{
		animator.Play("PlayerIdle");
	}
}
