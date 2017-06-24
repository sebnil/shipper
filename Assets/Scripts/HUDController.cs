﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour {
    public Transform progressTransform;
    public Text progressText;
    public Transform timeLeftTransform;
    public Text timeLeftText;
    public Transform ripenFactorTransform;
    public Text ripenFactorText;
    public Transform wavesFactorTransform;
    public Text wavesFactorText;
    public Text debugText;

    float animationRate = 1f;

    int timeLeftOriginalFontSize = 30;
    int timeLeftWarningFontSize = 50;

    // Use this for initialization
    void Start () {
		
	}

    // Update is called once per frame
    float newCrateDeliveredAnimationTimer = 1;
    float ripenFactorAnimationTimer = 1;
    float wavesFactorAnimationTimer = 1;

    void Update () {
        progressText.text = GameController.Instance.numberOfCratesDelivered.ToString();
        timeLeftText.text = Mathf.Floor(GameController.Instance.timeLeft).ToString();
        ripenFactorText.text = "Ripen x" + GameController.Instance.currentRipenFactor;
        wavesFactorText.text = "Waves x" + WeatherController.Instance.currentWaveFactor * 10f;

        debugText.text = "\nver " + BuildConstants.version;


        // animations when crates get delivered
        float scale = stepAnimationTimer(ref newCrateDeliveredAnimationTimer, 2, 1, animationRate, newCrateDelivered());
        timeLeftTransform.localScale = new Vector3(scale, scale, 1);
        progressTransform.localScale = new Vector3(scale, scale, 1);

        scale = stepAnimationTimer(ref ripenFactorAnimationTimer, 2, 1, animationRate, ripenFactorChanged());
        ripenFactorTransform.localScale = new Vector3(scale, scale, 1);

        scale = stepAnimationTimer(ref wavesFactorAnimationTimer, 2, 1, animationRate, WeatherController.Instance.WavesFactorChanged());
        wavesFactorTransform.localScale = new Vector3(scale, scale, 1);

        // warn the gamer that time is almost up
        if (GameController.Instance.timeLeft < 10)
        {
            timeLeftText.fontSize = timeLeftWarningFontSize;
            timeLeftText.color = Color.red;
        }
        else
        {
            timeLeftText.fontSize = timeLeftOriginalFontSize;
            timeLeftText.color = Color.black;
        }
    }

    // helper function for scale animation
    float stepAnimationTimer(ref float timer, float startValue, float stopValue, float animationRate, bool reset)
    {
        if (reset)
        {
            timer = 0;
        }
        else if (timer < 1)
        {
            timer += Time.deltaTime * animationRate;
        }
        else
        {
            timer = 1;
        }
        return Mathf.Lerp(startValue, stopValue, timer);
    }

    // helper functions for status updates
    int? cratesDelivered = null;
    bool newCrateDelivered()
    {
        bool value = false;
        if (GameController.Instance.numberOfCratesDelivered != cratesDelivered && cratesDelivered != null)
        {
            value = true;
        }
        cratesDelivered = GameController.Instance.numberOfCratesDelivered;
        return value;
    }

    float? ripenFactor = null;
    bool ripenFactorChanged()
    {
        bool value = false;
        if (GameController.Instance.currentRipenFactor != ripenFactor && ripenFactor != null)
        {
            value = true;
        }
        ripenFactor = GameController.Instance.currentRipenFactor;
        return value;
    }
}
