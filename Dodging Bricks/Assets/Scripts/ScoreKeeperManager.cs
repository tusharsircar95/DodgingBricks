using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ScoreKeeperManager : MonoBehaviour {

	private int score;
	private Vector3 position;

	public Text scoreText;
	public GameObject player;
	private float lerpSpeed;
	private AudioSource scoreSound;

	// Use this for initialization
	void Start () {
		score = 0;
		scoreText.text = score.ToString();
		PlayerPrefs.SetInt ("currentScore", score);
		scoreSound = GetComponent<AudioSource> ();
		PlayerPrefs.SetFloat ("lerpSpeed", 1.60f);
		lerpSpeed = 1.50f;
	}




	// Update is called once per frame
	void Update () {
		position = player.transform.position;
		position.y = Mathf.Max (position.y - 0.50f, transform.position.y);
		transform.position = position;
	}

	void OnTriggerEnter(Collider collider)
	{
		if (collider.tag == "Obstacle") {
			score++;
			lerpSpeed = Mathf.Min (2.00f, lerpSpeed + 0.05f);
			PlayerPrefs.SetFloat ("lerpSpeed", lerpSpeed);
		
			PlayerPrefs.SetInt ("currentScore", score);
			scoreText.text = score.ToString ();
			scoreSound.Play ();
		}

	}

	/*private void RequestBanner()
	{
		string adUnitId = "ca-app-pub-4900343252600013/4944014085";
		/*#if UNITY_EDITOR
		string adUnitId = "unused";
		#elif UNITY_ANDROID

		#elif UNITY_IPHONE
		string adUnitId = "INSERT_IOS_BANNER_AD_UNIT_ID_HERE";
		#else
		string adUnitId = "unexpected_platform";
		#endif

		// Create a 320x50 banner at the top of the screen.
		BannerView bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);
		Debug.Log ("created");
		// Create an empty ad request.
		AdRequest request = new AdRequest.Builder().Build();
		Debug.Log ("built");
		// Load the banner with the request.
		bannerView.LoadAd(request);
		Debug.Log ("loaded");
	}*/

}
