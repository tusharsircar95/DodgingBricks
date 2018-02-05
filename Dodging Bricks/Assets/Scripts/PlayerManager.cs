using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour {

	public bool canDie;
	public float upForce, sideForce;
	public GameObject mainCamera;
	public Text StartText, TitleText;

	private float alphaValue;
	private Rigidbody rb;
	private bool justClicked;
	private bool gameOver;
	private bool gameStarted;


	private AudioSource tapSound;
	private AudioSource gameOverSound;

	void setupSounds()
	{
		AudioSource[] sources = GetComponents<AudioSource> ();
		tapSound = sources[0];
		gameOverSound = sources[1];
	}

	// Use this for initialization
	void Start () {
		alphaValue = 1f;
		justClicked = false;
		gameOver = false;
		gameStarted = false;
		rb = transform.GetComponent<Rigidbody> ();
		rb.freezeRotation = true;
		rb.useGravity = false;
		Physics.gravity = new Vector3 (0f, -15f, 0f);
		setupSounds ();
	}
	
	// Update is called once per frame
	void Update () {
		if (!gameStarted) {
			if (Input.GetMouseButtonUp (0)) {
				tapSound.Play ();
				gameStarted = true;
				//StartText.gameObject.SetActive (false);
				//TitleText.gameObject.SetActive (false);

				rb.useGravity = true;
				if (Input.mousePosition.x < (Screen.width / 2))
					rb.AddForce (new Vector3 (-sideForce, upForce, 0f));
				else
					rb.AddForce (new Vector3 (sideForce, upForce, 0f));
			}
			alphaValue += Time.deltaTime * 1.5f;
			Color tempColor = StartText.color;
			tempColor.a = Mathf.Abs (Mathf.Sin (alphaValue));
			StartText.color = tempColor;
				
		}
		else if (!gameOver) {

			Color tempColor = TitleText.color;
			tempColor.a -= Time.deltaTime * 2f;
			TitleText.color = tempColor;

			tempColor = StartText.color;
			tempColor.a -= Time.deltaTime * 2f;
			StartText.color = tempColor;

			if (Input.GetMouseButtonDown (0)) {
				tapSound.Play ();
				justClicked = true;
				rb.velocity = Vector3.zero;
				if (Input.mousePosition.x < (Screen.width / 2))
					rb.AddForce (new Vector3 (-sideForce, upForce, 0f));
				else
					rb.AddForce (new Vector3 (sideForce, upForce, 0f));
			

			} else if (Input.GetMouseButtonUp (0))
				justClicked = false;

			moveCamera ();
		}


	}

	void moveCamera()
	{
		Vector3 position = mainCamera.transform.position;
		position.y = Mathf.Max (position.y, transform.position.y);
		mainCamera.transform.position = position;
	}



	void endGame()
	{
		gameOverSound.Play ();
		//Handheld.Vibrate ();
		StartCoroutine (endGameAux ());
	}

	IEnumerator endGameAux()
	{
		//if (!canDie)
		//	return;
		if (canDie) {
			gameOver = true;
			int tempScore = PlayerPrefs.GetInt ("currentScore");
			if (tempScore > PlayerPrefs.GetInt ("highScore"))
				PlayerPrefs.SetInt ("highScore", tempScore);
		
			rb.freezeRotation = false;
			rb.AddTorque (new Vector3 (0f, 0f, 10f));


			rb.velocity = Vector3.zero;
			Vector3 tempPosition = rb.transform.position;
			tempPosition.z = 100f;
			rb.transform.position = tempPosition;
			yield return new WaitForSeconds (1.0f);

			SceneManager.LoadScene ("EndScene");
		}
	}
	void OnCollisionEnter(Collision collision)
	{
		Vector3 velocity;
		if (collision.collider.tag == "SideWallLeft") {
			velocity = rb.velocity;
			velocity.x = Mathf.Max (velocity.x, 0f);
			return;
		}
		else if (collision.collider.tag == "SideWallRight") {
			velocity = rb.velocity;
			velocity.x = Mathf.Min (velocity.x, 0f);
			return;
		}
		endGame ();
		//Debug.Log ("Crash!");
	}
}
