using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using GoogleMobileAds.Api;

public class EndGameManager : MonoBehaviour {

	public float speed = 0.2f;
	public GameObject endPanel;
	public Text gameOverLabel;
	public Button retryButton;
	public Button rateButton;
	public Button shareButton;
	public Text highScoreText;
	public Text scoreText;

	private float x = -10f;
	private int highScore;
	private int score;
	private string gameURL = "https://play.google.com/store/apps/details?id=com.tushar1995.DodgingBricks&hl=en";




	private bool isProcessing = false;
	public float startX;
	public float startY;
	public int valueX;
	public int valueY;

	private AudioSource clickSound;

	// Use this for initialization
	void Start () {
		score = PlayerPrefs.GetInt ("currentScore");
		highScore = PlayerPrefs.GetInt ("highScore");

		scoreText.text = score.ToString ();
		highScoreText.text = highScore.ToString ();

		clickSound = GetComponent<AudioSource> ();
		RequestBanner ();
	}
	
	// Update is called once per frame
	void Update () {
		x = x + (speed * Time.deltaTime);
		x = Mathf.Min (0f, x);

		Vector3 position = endPanel.transform.position;
		position.x += speed * Time.deltaTime;
		if (position.x > 0)
			position.x = 0;
		endPanel.transform.position = position;

		position = retryButton.transform.position;
		position.x -= speed * Time.deltaTime;
		if (position.x < 0)
			position.x = 0;
		retryButton.transform.position = position;

		position = gameOverLabel.transform.position;
		position.x -= speed * Time.deltaTime;
		if (position.x < 0)
			position.x = 0;
		gameOverLabel.transform.position = position;

		/*position = shareButton.transform.position;
		position.x -= speed * Time.deltaTime;
		if (position.x < 0)
			position.x = 0;
		shareButton.transform.position = position;

		position = rateButton.transform.position;
		position.x -= speed * Time.deltaTime;
		if (position.x < 0)
			position.x = 0;
		rateButton.transform.position = position;*/

		if (Input.GetKey ("escape"))
			Application.Quit ();


	}

	public void RestartGame(string sceneName)
	{
		clickSound.Play ();
		SceneManager.LoadScene(sceneName);
	}

	public void shareScreenshot(){

		if(!isProcessing)
			StartCoroutine( captureScreenshot() );
	}
	public IEnumerator captureScreenshot(){
		isProcessing = true;
		yield return new WaitForEndOfFrame();

		Texture2D screenTexture = new Texture2D(Screen.width, Screen.height,TextureFormat.RGB24,true);

		// put buffer into texture
		//screenTexture.ReadPixels(new Rect(0f, 0f, Screen.width, Screen.height),0,0);
		//create a Rect object as per your needs.
		screenTexture.ReadPixels(new Rect
			(0, 0, Screen.width, Screen.height),0,0);

		// apply
		screenTexture.Apply();

		//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- PHOTO

		//byte[] dataToSave = Resources.Load<TextAsset>("everton").bytes;
		byte[] dataToSave = screenTexture.EncodeToPNG();

		string destination = Path.Combine(Application.persistentDataPath,System.DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + ".png");

		File.WriteAllBytes(destination, dataToSave);


		if(!Application.isEditor)
		{
			// block to open the file and share it ------------START
			AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
			AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
			intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
			intentObject.Call<AndroidJavaObject>("setType", "image/jpeg");
			AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
			AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse","file://" + destination);
			intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);
			intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), "Hey! I just got a new high score in Dodging Bricks! Can you beat it?\n" +
				"Download the game on the play store at "+"\n" + gameURL);
			AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");

			// option one WITHOUT chooser:
			currentActivity.Call("startActivity", intentObject);

			// block to open the file and share it ------------END

		}
		isProcessing = false;

	}

	public void RateGame()
	{
		Application.OpenURL(gameURL);
	}

	private void RequestBanner()
	{
		string adUnitId = "ca-app-pub-4900343252600013/4944014085";
		/*#if UNITY_EDITOR
		string adUnitId = "unused";
		#elif UNITY_ANDROID

		#elif UNITY_IPHONE
		string adUnitId = "INSERT_IOS_BANNER_AD_UNIT_ID_HERE";
		#else
		string adUnitId = "unexpected_platform";
		#endif*/

		// Create a 320x50 banner at the top of the screen.
		BannerView bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);
		Debug.Log ("created");
		// Create an empty ad request.
		AdRequest request = new AdRequest.Builder().Build();
		Debug.Log ("built");
		// Load the banner with the request.
		bannerView.LoadAd(request);
		Debug.Log ("loaded");
	}
}
