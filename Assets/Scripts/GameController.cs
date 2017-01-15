﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using com.kleberswf.lib.core;

public enum GameState {Running, Paused, GameOver};

public class GameController : Singleton<GameController> {

	protected GameController () {} // guarantee this will be always a singleton only - can't use the constructor!

    public GameObject gamePanel;
	public GameObject gameOverPanel;

    public GameObject hazard;

    public Vector3 spawnValues;

    public float spawnWait;
    public float startWait;
    public float waveWait;

    public int hazardCount;

    public int xMean;

    public Text progressText;
	public Text timeLeftText;
	public Text debugText;
    public int numberOfCratesOnBoat = 0;
    public int maxNumberOfCratesOnBoat = 0;
	public int numberOfCratesDelivered = 0;
	public float timeLeft;
	public float crateDeliveredTimeBonus = 5;
	public float crateDroppedTimePunishment = 10;
	public float crateDeliveredTimeBonusDecrementFactor;
	public GameState gameState = GameState.Running;

	[System.Serializable]
	public class CrateTimers
	{
		public int timerStart;
		public int yellowThreshold;
		public int brownThreshold;
		public int rottenThreshold;

		public float timeBonusGreen;
		public float timeBonusYellow;
		public float timeBonusBrown;
		public float timeBonusRotten;
	}
	public CrateTimers crateTimers;


    Ray ray;
    RaycastHit hit;
    public GameObject cratePrefab;

	public GameObject craneObject;
	private Crane craneScript;

    // Use this for initialization
    void Start () {
		craneScript = craneObject.GetComponent<Crane>();

        // force landscape on mobile
        Screen.orientation = ScreenOrientation.LandscapeLeft;

		// start countdown timer
		StartCoroutine("GameOverTimer");
    }
	
	// Update is called once per frame
	void Update () {
        // menu control
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            ToggleMenu();
        }

        // create crate on touch and mouse click
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if (Input.GetMouseButtonDown(0))
            {
				Debug.Log (hit.transform.gameObject.name);

				if (AppInfo.debugRelease && hit.transform.gameObject.name == "DebugCrateRaycastTarget") {
                    GameObject obj = Instantiate (cratePrefab, new Vector3 (hit.point.x, hit.point.y, 0), Quaternion.identity) as GameObject;
				}
            }
        }

		if (Input.GetKeyDown ("space")) 
		{
			craneScript.LetGoOfCrate ();
		}

        // calculate crates on the boat
        numberOfCratesOnBoat = 0;
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Crate");
        foreach (GameObject go in gos)
        {
            Crate script = go.GetComponent<Crate>();
            bool crateOnBoat = script.onBoat;
            if (crateOnBoat)
            {
                numberOfCratesOnBoat++;
            }

        }
        if (numberOfCratesOnBoat > maxNumberOfCratesOnBoat)
        {
            maxNumberOfCratesOnBoat = numberOfCratesOnBoat;
        }

		progressText.text = numberOfCratesDelivered.ToString();
		timeLeftText.text = Mathf.Floor(timeLeft).ToString();

		debugText.text = "crateDeliveredTimeBonus: " + crateDeliveredTimeBonus + 
		"\ncrateDroppedTimePunishment: " + crateDroppedTimePunishment +
		"\nver " + AppInfo.fullVersion;
    }

	public void IncrementNumberOfCratesDelivered(Crate.CrateState crateState) {
		numberOfCratesDelivered++;
		switch (crateState) {
		case Crate.CrateState.Green:
			timeLeft += crateTimers.timeBonusGreen;
			break;
		case Crate.CrateState.Yellow:
			timeLeft += crateTimers.timeBonusYellow;
			break;
		case Crate.CrateState.Brown:
			timeLeft += crateTimers.timeBonusBrown;
			break;
		case Crate.CrateState.Rotten:
			timeLeft += crateTimers.timeBonusRotten;
			break;
		}

	}

	public void PunishForDroppedCrate() {
		timeLeft -= crateDroppedTimePunishment;
		if (timeLeft < 0) {
			timeLeft = 0;
		}
	}

    public void ToggleMenu()
    {
		if (gameState == GameState.GameOver && AppInfo.debugRelease) {
			// do nothing
		}
		else if (gameState == GameState.GameOver) {
			// pause game and show menu
			gameOverPanel.SetActive(true);
			//gameState = GameState.Paused;
			Time.timeScale = 0f;
		}
		else if (gameState == GameState.Paused)
        {   
            // run game
            gamePanel.SetActive(false);
			gameState = GameState.Running;
            Time.timeScale = 1.0f;
        }
        else
        {
            // pause game and show menu
            gamePanel.SetActive(true);
			gameState = GameState.Paused;
            Time.timeScale = 0f;
        }
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gamePanel.SetActive(false);
        Time.timeScale = 1.0f;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

	IEnumerator GameOverTimer() {
		while (true) {
			yield return new WaitForSeconds (1f);

			if (crateDeliveredTimeBonus > 2) {
				crateDeliveredTimeBonus = crateDeliveredTimeBonus - Time.timeSinceLevelLoad * crateDeliveredTimeBonusDecrementFactor;
			} else {
				crateDeliveredTimeBonus = 2;
			}

			if (gameState == GameState.Running && timeLeft > 0) {
				timeLeft--;
				if (timeLeft < 0) {
					timeLeft = 0;
				}
			} else if (gameState == GameState.Running) {
				gameState = GameState.GameOver;
				ToggleMenu ();
			} else {
				// do nothing
			}
		}
	}
		
}
