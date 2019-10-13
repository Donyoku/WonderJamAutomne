﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Image curtain = null;
    [SerializeField]
    private Text nbSeedText = null,
        nbLevelText = null,
        nbTreeText = null,
        timerText = null;
    [SerializeField]
    private Image bucketImg = null;
    [SerializeField]
    private float limitTime = 120;
    [SerializeField]
    private Sprite emptyBucketSprite = null;
    [SerializeField]
    private Sprite filledBucketSprite = null;
    [SerializeField]
    private RectTransform endScreen;
    [SerializeField]
    private GameObject popup;

    private int nbActualTree;
    private int nbGoalTree = 5;//
    private int level = 1;//
    private int nbSeeds = 10;
    private float startTime;

    public static UIManager instance;
    private bool paused = false;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        startTime = Time.time;
        SetLevel(level, nbGoalTree);
        bucketImg.enabled = false;
        curtain.enabled = true;
        LeanTween.alpha((RectTransform)curtain.transform, 0f, .2f);
    }

    // Update is called once per frame
    void Update()
    {
        if(paused)
            return;
        float t = Time.time - startTime;
        float seconds = (limitTime - t);
        if (seconds >= 100)
        {
            timerText.text = seconds.ToString("f0");
        }
        else if (seconds >= 10)
        {
            timerText.text = seconds.ToString("f1");
        }
        else if (seconds >= 0)
        {
            timerText.text = seconds.ToString("f2");
        }
        else
        {
            timerText.text = "OUT OF TIME";
            DisplayEndScreen();
        }

        var f2 = ForestTree.trees.FindAll(f => !f.IsSoilOrBurnt);
        if (f2.Count == 0 && nbSeeds == 0)
            DisplayEndScreen();
    }

    public void SetTime(float newTime)
    {
        startTime = Time.time;
        limitTime = newTime;
    }

    public void SetLevel(int newLevel, int newNbGoalTree)
    {
        level = newLevel;
        nbGoalTree = newNbGoalTree;
        nbLevelText.text = "Niv " + level + " :";
        nbTreeText.text = nbActualTree + "/" + nbGoalTree;
    }

    public void AddTree()
    {
        nbActualTree += 1;
        nbTreeText.text = nbActualTree + "/" + nbGoalTree;
    }
    public void DeleteTree()
    {
        nbActualTree -= 1;
        nbTreeText.text = nbActualTree + "/" + nbGoalTree;
    }

    public void UpdateSeeds(int nbSeed)
    {
        nbSeeds = nbSeed;
        nbSeedText.text = "x " + nbSeed;
    }

    public void EmptyBucket()
    {
        bucketImg.sprite = emptyBucketSprite;
    }
    public void FilledBucket()
    {
        bucketImg.sprite = filledBucketSprite;
    }

    public void DropBucket()
    {
        bucketImg.enabled = false;
    }

    public void PickUpBucket(bool isFilled)
    {
        bucketImg.enabled = true;
        bucketImg.sprite = isFilled ? filledBucketSprite : emptyBucketSprite;
    }

    public void DisplayEndScreen()
    {
        paused = true;
        endScreen.GetChild(2).GetComponent<Text>().text = level.ToString();
        StartCoroutine(WaitForUser());
    }

    private IEnumerator WaitForUser()
    {
        LeanTween.alpha(endScreen, 0f, 0f);
        LeanTween.alpha((RectTransform) popup.transform, 0f, 0f);

        yield return null;
        endScreen.gameObject.SetActive(true);
        LeanTween.alpha(endScreen, 1f, .2f).setRecursive(false);
        yield return new WaitForSeconds(.6f);
        LeanTween.alphaText((RectTransform)endScreen.GetChild(0), 1f, 0f);
        yield return new WaitForSeconds(1.4f);
        LeanTween.alphaText((RectTransform)endScreen.GetChild(1), 1f, 0f);
        yield return new WaitForSeconds(.4f);
        LeanTween.alphaText((RectTransform)endScreen.GetChild(2), 1f, 0f);
        yield return new WaitForSeconds(.8f);
        Color color = new Color(168f / 255f, 168f / 255f, 168f / 255f, 1);
        LeanTween.value(0, 1, 1.5f).setLoopClamp().setOnUpdate((float val)
             => endScreen.GetChild(3).GetComponent<Text>().color = color * Mathf.Round(val));
        yield return new WaitUntil(() => InputManager.GetButtonDown(Button.A));
        LeanTween.alpha((RectTransform) curtain.transform, 1f, .2f).setOnComplete(()=> {
            LeanTween.cancelAll();
            SceneManager.LoadScene("StartScene");
        });
    }
}
