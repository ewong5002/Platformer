using UnityEngine;

public class Exit : MonoBehaviour
{
	public static bool CanExit { get; private set; } = false;
	public Animator exitAnimator;

	void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			GameManager.Instance.LoadCanvas.SetActive(true);
			CanExit = true;
		}
	}

	void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			GameManager.Instance.LoadCanvas.SetActive(false);
			CanExit = false;
		}
	}

	public void ExitAnimation()
	{
		exitAnimator.SetTrigger("exit");
	}

	public void ResetExit()
	{
		CanExit = false;
	}
}