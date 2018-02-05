 using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LoopManager : MonoBehaviour {

	public GameObject Bars;
	public float lerpSpeed;
	public bool isMoving;
	public Vector3 check;
	public int check1;
	public GameObject rightWall;
	public GameObject leftWall;
	public Color32[] barColors;
	private float colorLerper;
	private float colorLerperSpeed;

	private GameObject[] barArray;
	private int barCount;

	private int barIndex;


	private GameObject[] leftCube;
	private GameObject[] rightCube;
	private float[] l1,l2;
	private float[] r1,r2;
	private float[] lerpValue;

	private float parentLeft=4.5f,parentRight=7.5f;
	private float upCubeLeft=-2f,upCubeRight=1.5f;
	private float downCubeLeft=-3.5f,downCubeRight=-0.5f;

	void initialisePositions()
	{
		Vector3 tempPosition;
		GameObject parentBar;
		for (int i = 0; i < barCount; i++) {
			parentBar = barArray [i];
			tempPosition = parentBar.transform.localPosition;
			tempPosition.x = Random.Range (parentLeft,parentRight);
			parentBar.transform.localPosition = tempPosition;


			l1[i] = (upCubeLeft - (tempPosition.x - parentLeft));
			l2[i] = (upCubeRight - (tempPosition.x - parentLeft));
			r1[i] = (downCubeLeft - (tempPosition.x - parentLeft));
			r2[i] = (downCubeRight - (tempPosition.x - parentLeft));
			lerpValue [i] = 0f;

			leftCube[i] = parentBar.transform.GetChild (2).gameObject;
			rightCube[i] = parentBar.transform.GetChild (3).gameObject;

			tempPosition = parentBar.transform.GetChild (2).gameObject.transform.localPosition;
			tempPosition.x = r1 [i];
			parentBar.transform.GetChild(2).gameObject.transform.localPosition = tempPosition;

			tempPosition = parentBar.transform.GetChild (3).gameObject.transform.localPosition;
			tempPosition.x = r2[i];
			parentBar.transform.GetChild (3).gameObject.transform.localPosition = tempPosition;


		}
		check = leftCube [0].transform.localPosition;

		leftWall.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0f,0.5f,0f));
		rightWall.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(1f,0.5f,0f));

		tempPosition = leftWall.transform.position;
		tempPosition.z = 0f;
		leftWall.transform.position = tempPosition;


		tempPosition = rightWall.transform.position;
		tempPosition.z = 0f;
		rightWall.transform.position = tempPosition;


		/*randomizeBarColorArray ();
		for (int index = 0; index < barCount; index++)
			for (int i = 0; i < 4; i++)
				barArray [index].transform.GetChild (i).gameObject.GetComponent<MeshRenderer> ().material.color = barColors [0];
		*/
	}

	void randomizeBarColorArray()
	{
		for (int i = 0; i < 4; i++) {
			Color32 tempColor = barColors [i];
			int j = Random.Range (i, 4);
			barColors [i] = barColors [j];
			barColors [j] = tempColor;
		}
	}

	// Use this for initialization
	void Start () {
		barCount = Bars.transform.childCount;
		barArray = new GameObject[barCount];

		leftCube = new GameObject[barCount];
		rightCube = new GameObject[barCount];
		l1 = new float[barCount];
		l2 = new float[barCount];
		r1 = new float[barCount];
		r2 = new float[barCount];
		lerpValue = new float[barCount];

		barIndex = 0;
		for (int i = 0; i < barCount; i++)
			barArray [i] = Bars.transform.GetChild (i).gameObject;
		initialisePositions ();

		colorLerper = 0f;
		colorLerperSpeed = 3.5f;
	}


	void setColors(int index)
	{
		float p = Mathf.Abs(Mathf.Sin (colorLerper));
		Color32 color;
		if (p < 0.33f) {
			color = Color.Lerp (barColors [0], barColors [1], p/0.33f);
		} else if (p < 0.66f){
			color = Color.Lerp (barColors [1], barColors [2], (p-0.33f)/0.33f);
		}
		else {
			color = Color.Lerp (barColors [2], barColors [3], (p-0.66f)/0.34f);
		}

		for(int i=0; i<4; i++)
			barArray [index].transform.GetChild (i).gameObject.GetComponent<MeshRenderer> ().material.color = color;
	}

	// Update is called once per frame
	void Update () {
		

		if (isMoving) {
			lerpSpeed = PlayerPrefs.GetFloat("lerpSpeed");
			Vector3 tempPosition;
			for (int i = 0; i <barCount; i++) {
				lerpValue[i] += lerpSpeed * Time.deltaTime;
				float p = Mathf.Abs (Mathf.Sin (lerpValue[i]));
				GameObject parentBar = barArray [i];

				leftCube[i] = parentBar.transform.GetChild (2).gameObject;
				rightCube[i] = parentBar.transform.GetChild (3).gameObject;

				tempPosition = leftCube [i].transform.localPosition;
				tempPosition.x =  Mathf.Lerp (l1 [i], l2 [i], p);
				leftCube [i].transform.localPosition = tempPosition;


				tempPosition = rightCube [i].transform.localPosition;
				check = tempPosition;
				tempPosition.x = Mathf.Lerp (r1 [i], r2 [i], (1f - p));
				rightCube [i].transform.localPosition = tempPosition;
			}

		}
	}

	void OnTriggerEnter(Collider collider)
	{
		//Debug.Log ("Move Trigger!");
		if (collider.tag != "Obstacle")
			return;
		Vector3 position;
		GameObject parentBar = collider.transform.parent.gameObject; 
		position = parentBar.transform.localPosition;
		position.y += 18f;
		position.x = Random.Range (parentLeft,parentRight);
		parentBar.transform.localPosition = position;

		//float xLeft = Random.Range (-3.42f, -1.46f) - (position.x - 4.86f);
		//float xRight = Random.Range (-1.46f, 0.57f) - (position.x - 4.86f);

		l1[barIndex] = (upCubeLeft - (position.x - parentLeft));
		l2[barIndex] = (upCubeRight - (position.x - parentLeft));
		r1[barIndex] = (downCubeLeft - (position.x - parentLeft));
		r2[barIndex] = (downCubeRight - (position.x - parentLeft));
		lerpValue [barIndex] = Random.Range (0f, 100f);
			
		position = parentBar.transform.GetChild (2).gameObject.transform.localPosition;
		position.x = r1 [barIndex];
		parentBar.transform.GetChild(2).gameObject.transform.localPosition = position;

		position = parentBar.transform.GetChild (3).gameObject.transform.localPosition;
		position.x = r2 [barIndex];
		parentBar.transform.GetChild (3).gameObject.transform.localPosition = position;

		colorLerper += colorLerperSpeed * Time.deltaTime;
		setColors (barIndex);

		barIndex = (barIndex + 1) % barCount;

	}
}
