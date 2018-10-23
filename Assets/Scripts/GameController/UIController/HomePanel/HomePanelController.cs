using UnityEngine;
using System.Collections;
using System;

public class HomePanelController : MonoBehaviour
{
    GameObject freeReward;
    GameObject freeRewardTitle;
    private GameObject levelButtons;
    GameObject levelSelectPanel;
    UITexture soundTexture;
    UITexture musicTexture;
    GameObject alertQuestCompleteIcon;

    void Awake()
    {
        AssignObject();
    }

    void Start()
    {
        InvokeRepeating("CheckFreeReward", 0, 2);
    }

    void Update()
    {
        if (levelSelectPanel.transform.localPosition.x > 0)
        {
            levelSelectPanel.transform.localPosition = new Vector3(0, 0, 0);
            levelSelectPanel.GetComponent<SpringPanel>().enabled = false;
            //levelSelectPanel.GetComponent<SpringPanel>().strength = 100;
            //levelSelectPanel.GetComponent<SpringPanel>().target = new Vector3(0, 0, 0);
            levelSelectPanel.GetComponent<UIPanel>().clipOffset = new Vector3(0, 0, 0);
        }
        if (levelSelectPanel.transform.localPosition.x < -2320)
        {
            levelSelectPanel.transform.localPosition = new Vector3(-2320, 0, 0);

            levelSelectPanel.GetComponent<SpringPanel>().enabled = false;
            //levelSelectPanel.GetComponent<SpringPanel>().strength = 100;
            //levelSelectPanel.GetComponent<SpringPanel>().target = new Vector3(-2323, 0, 0);
            levelSelectPanel.GetComponent<UIPanel>().clipOffset = new Vector3(2323, 0, 0);

        }
    }

    void AssignObject()
    {
        levelButtons = Master.GetChildByName(gameObject, "LevelButtons");
        levelSelectPanel = Master.GetChildByName(gameObject, "LevelSelect");
        freeReward = Master.GetChildByName(gameObject, "FreeReward");
        freeRewardTitle = Master.GetChildByName(gameObject, "FreeRewardTitle");
        soundTexture = Master.GetChildByName(gameObject, "Sound").GetComponent<UITexture>();
        musicTexture = Master.GetChildByName(gameObject, "Music").GetComponent<UITexture>();
        alertQuestCompleteIcon = Master.GetChildByName(gameObject, "AlertQuestCompleteIcon");
    }

    public void OnOpen()
    {
        SetSettings();
        SetLevelButton();
        CheckAlertQuestComplete();

        if (!Master.Tutorial.CheckAndStartTutorial(TutorialController.TutorialsIndex.BuildUnitInGameplay))
        {
            if (Master.Stats.Star >= 150)
            {
                Master.Tutorial.CheckAndStartTutorial(TutorialController.TutorialsIndex.UpgradeStatsOfUnit);
            }

            if (!Master.Tutorial.isDoingTutorial)
            {
                if (Master.LevelData.lastLevel >= FreeRewardController.levelCanGetFreeReward)
                {
                    Master.Tutorial.CheckAndStartTutorial(TutorialController.TutorialsIndex.GetFreeReward);
                }
            }

        }

    }

    public void SetLevelButton()
    {
        Vector3 posLastLevelButton = Vector3.zero;
        foreach (Transform level in levelButtons.transform)
        {
            if (level.childCount > 0)
            {
                Destroy(level.GetChild(0).gameObject);
            }
            int levelIndex = int.Parse(level.gameObject.name);
            GameObject pf_levelButton = Master.GetGameObjectInPrefabs("UI/LevelButton");
            GameObject levelButton = NGUITools.AddChild(level.gameObject, pf_levelButton);
            levelButton.name = "LevelSelect_" + levelIndex;
            levelButton.GetComponentInChildren<LevelButton>().SetAttribute(levelIndex);
            if (levelIndex == Master.LevelData.lastLevel + 1)
            {
                posLastLevelButton = level.transform.localPosition;
            }
        }
        Debug.Log(Master.LevelData.lastLevel + " | " + posLastLevelButton.x);
        //set screen center of last level
        if (Master.LevelData.lastLevel >= 12)
        {
            levelSelectPanel.GetComponent<SpringPanel>().enabled = true;
            levelSelectPanel.GetComponent<SpringPanel>().strength = 100;
            levelSelectPanel.GetComponent<SpringPanel>().target = new Vector3(-posLastLevelButton.x, 0, 0);
        }
    }

    void CheckFreeReward()
    {
#if UNITY_ANDROID || UNITY_IOS
        bool isEnable = IsAvailableToShow();

        freeReward.SetActive(isEnable);
        freeRewardTitle.GetComponent<MoveObject>().enabled = isEnable;
#else
        freeReward.SetActive(false);
        freeRewardTitle.GetComponent<MoveObject>().enabled = false;
#endif
    }

    //public void HandleRewardBasedVideoRewarded(object sender, Reward args)
    //{
    //    Master.WaitAndDo(0.3f, OnRewardBasedVideoRewarded);
    //}

    private void OnRewardBasedVideoRewarded()
    {
        freeReward.SetActive(false);
        freeRewardTitle.GetComponent<MoveObject>().enabled = false;

        int randomGem = UnityEngine.Random.Range(1, 3);
        int randomStar = UnityEngine.Random.Range(15, 25);

        Master.Stats.Gem += randomGem;
        Master.Stats.Star += randomStar;
        FreeRewardController.SetDateTimeGetReward();
        Master.QuestData.IncreaseProgressValue("07");
        // SetRandomReward();
        Master.UIMenu.ShowDialog("GotRewardDialog", 0.3f, new string[] { randomGem.ToString(), randomStar.ToString() });

        SetActionTime("rewarded_video");
        //GoogleAnalyticsV3.instance.LogEvent("Rewarded Video", "On Rewarded", "On Rewarded", 0);
    }

    public bool IsAvailableToShow()
    {
        return false;
        //return IsActionAvailable() && IsAdAvailable();
    }

    private bool IsActionAvailable()
    {
        return IsActionAvailable("rewarded_video", 120);
    }

    //private bool IsAdAvailable()
    //{
    //    if (AdmobController.instance.rewardBasedVideo == null) return false;
    //    bool isLoaded = AdmobController.instance.rewardBasedVideo.IsLoaded();
    //    if (!isLoaded)
    //    {
    //        AdmobController.instance.RequestRewardBasedVideo();
    //    }
    //    return isLoaded;
    //}

    public void FreeRewardButton_OnClick()
    {
        Master.Tutorial.CheckAndContinueNextStepTutorial(TutorialController.TutorialsIndex.GetFreeReward);

        Master.PlaySoundButtonClick();
        //AdmobController.instance.ShowRewardBasedVideo();
        //GoogleAnalyticsV3.instance.LogEvent("Rewarded Video", "Button Click", "Button Click", 0);
    }


    public void SetSettings()
    {
        if (Master.Audio.isSoundOn)
        {
            Master.GetChildByName(soundTexture.gameObject, "X").SetActive(false);
        }
        else
        {
            Master.GetChildByName(soundTexture.gameObject, "X").SetActive(true);
        }

        if (Master.Audio.isBackgroundMusicOn)
        {
            Master.GetChildByName(musicTexture.gameObject, "X").SetActive(false);
        }
        else
        {
            Master.GetChildByName(musicTexture.gameObject, "X").SetActive(true);
        }

    }

    public void ToggleAudioSettingButton_OnClick(GameObject go)
    {
        Master.PlaySoundButtonClick();
        string goName = go.name;
        if (goName == "Sound")
        {
            Master.Audio.ToggleSound();
        }
        else if (goName == "Music")
        {
            Master.Audio.ToggleBackgroundMusic();
        }

        SetSettings();
    }

    void CheckAlertQuestComplete()
    {
        if (Master.QuestData.isHaveQuestComplete())
        {
            alertQuestCompleteIcon.SetActive(true);
        }
        else
        {
            alertQuestCompleteIcon.SetActive(false);
        }
    }


    public static bool IsActionAvailable(string action, int time, bool availableFirstTime = true)
    {
        if (!PlayerPrefs.HasKey(action + "_time")) // First time.
        {
            if (availableFirstTime == false)
            {
                SetActionTime(action);
            }
            return availableFirstTime;
        }

        int delta = (int)(GetCurrentTime() - GetActionTime(action));
        return delta >= time;
    }

    public static double GetActionDeltaTime(string action)
    {
        if (GetActionTime(action) == 0)
            return 0;
        return GetCurrentTime() - GetActionTime(action);
    }

    public static void SetActionTime(string action)
    {
        SetDouble(action + "_time", GetCurrentTime());
    }

    public static void SetActionTime(string action, double time)
    {
        SetDouble(action + "_time", time);
    }

    public static double GetActionTime(string action)
    {
        return GetDouble(action + "_time");
    }

    public static double GetCurrentTime()
    {
        TimeSpan span = DateTime.Now.Subtract(new DateTime(1970, 1, 1, 0, 0, 0));
        return span.TotalSeconds;
    }


    public static void SetDouble(string key, double value)
    {
        PlayerPrefs.SetString(key, DoubleToString(value));
    }

    public static double GetDouble(string key, double defaultValue)
    {
        string defaultVal = DoubleToString(defaultValue);
        return StringToDouble(PlayerPrefs.GetString(key, defaultVal));
    }

    public static double GetDouble(string key)
    {
        return GetDouble(key, 0d);
    }

    private static string DoubleToString(double target)
    {
        return target.ToString("R");
    }

    private static double StringToDouble(string target)
    {
        if (string.IsNullOrEmpty(target))
            return 0d;

        return double.Parse(target);
    }

    private void OnDestroy()
    {

    }
}
