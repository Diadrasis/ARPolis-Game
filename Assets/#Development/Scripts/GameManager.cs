using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using eChrono;

public class GameManager : Singleton<GameManager>
{
	private GameManager() { }

	[SerializeField]
	public List<PoiQuest> poiQuestions = new List<PoiQuest>();
	public PoiQuest poiSearchingNow;
	public bool isGameStarted, isGameInstructionVisible, isGameLost, isGameCompleted, isInfoUnlocked, aPoiFound, alertTime;
	public float gameTotalTime;
	public int gameTotalScore, gameSceneBestScore, totalPoisInScene, totalPoisUnlocked, currentQuestionIndex;

	public Text introText, introTitle, timeText, scoreText;
	public GameObject introPanel, gameWindow, alertPanel;
	public ButtonsMenu buttonsMenu;

	public void CreateGameManager() {
		alertPanel = Diadrasis.Instance.menuUI.alertTime;
		gameWindow = Diadrasis.Instance.menuUI.gamePanel;
		timeText = Diadrasis.Instance.menuUI.timeText;
		scoreText = Diadrasis.Instance.menuUI.scoreText;
		introText = Diadrasis.Instance.menuUI.introKeimeno;
		introTitle = Diadrasis.Instance.menuUI.introTitle;
		introPanel = Diadrasis.Instance.menuUI.introKeimenoPanel;
		buttonsMenu = Diadrasis.Instance.menuUI.btnsMenu;
	}

	public void StartNewGame()
    {
		ResetVariables();
		isGameInstructionVisible = true;

		totalPoisInScene = poiQuestions.Count;

		gameSceneBestScore = PlayerPrefs.GetInt("gameSceneBestScore" + Diadrasis.Instance.sceneName);
		gameTotalScore = 0;
		gameTotalTime = 1800f;

		buttonsMenu.ShowPoiQuestionButton(false);

		//show instructions
		ShowIntroMessage("game_instructions", "game_instructions_title", "\n\n"+ Diadrasis.Instance.introText);
	}

	void SetGameText()
    {
		TimeSpan timeSpan = TimeSpan.FromSeconds(gameTotalTime);
		string timeFormatText = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
        if (alertTime)
        {
			timeFormatText = "<color=red>" + timeFormatText + "</color>";
        }
		timeText.text = appData.FindTerm_text("time") + ": " + timeFormatText;
		timeText.text += " " + appData.FindTerm_text("poi_found") + ": " + totalPoisUnlocked.ToString() + "/" + totalPoisInScene.ToString();

		scoreText.text = appData.FindTerm_text("score") + ": " + gameTotalScore.ToString() + " " + appData.FindTerm_text("best_score") + ": " + gameSceneBestScore.ToString();
	}

	public void ResetVariables()
    {
        if (timeText) timeText.text = string.Empty;
        if (scoreText) scoreText.text = string.Empty;
		totalPoisUnlocked = 0;
		currentQuestionIndex = 0;
		isInfoUnlocked = false;
		isGameCompleted = false;
		isGameLost = false;
		isGameInstructionVisible = false;
		isGameStarted = false;
		aPoiFound = false;
		alertTime = false;
		poiSearchingNow = null;
		totalPoisInScene = 0;


		if (gameWindow) gameWindow.SetActive(false);

	}

	public void OnIntroMessageClosed()
    {
		if (!isGameStarted)
		{
			if (isGameInstructionVisible)
			{
				//set user current status
				Diadrasis.Instance.ChangeStatus(Diadrasis.User.inFullMap);
				Diadrasis.Instance.escapeUser = Diadrasis.EscapeUser.inFullMap;
				Diadrasis.Instance.userPrin = Diadrasis.UserPrin.isNavigating;
			}
		}
		else
		{
            if (isGameCompleted && !isInfoUnlocked)
            {
				StartCoroutine(DelayShowMessage("win_game", "win_game_title","", " "+appData.FindTerm_text("score")+": "+gameTotalScore.ToString()));
				//ShowIntroMessage("win_game");
				Debug.Log("HIDE GAME ELEMENTS ????");
				isInfoUnlocked = true;
            }
			else if(isGameCompleted && isInfoUnlocked)
            {
				gameWindow.SetActive(false);
            }
			else if (aPoiFound)
            {
				Invoke("ShowRandomQuestion", 2f);
			}
			else if (isGameLost)
            {
				//return to menu
				Diadrasis.Instance.ReturnToMainMenu();
			}
		}
	}

	public void TapMapAction()
	{
		Debug.LogWarning("TAP MAP ACTION");
		//show the first question
		StartCoroutine(DelayShowFirstQuestion());
		SetGameText();
		gameWindow.SetActive(true);
	}

	IEnumerator DelayShowFirstQuestion()
    {
		yield return new WaitForSeconds(2f);

		ShowRandomQuestion();
		isGameInstructionVisible = false;

		if (HasCurrentSceneQuestion()) isGameStarted = true;

		yield break;
    }

    void Update()
    {
		if (isGameStarted && !isGameCompleted && !isGameLost)
		{
			gameTotalTime -= Time.deltaTime;

            if (!alertTime)
            {
				if(gameTotalTime<300f) alertTime = true;
			}

			SetGameText();
		}

	}

	public bool HasCurrentSceneQuestion() { return poiQuestions.Count > 0 && Diadrasis.Instance.isGameMode; }

	public void CheckAnswer()
	{
		if (!Diadrasis.Instance.isGameMode) return;

		if (isGameStarted && poiSearchingNow != null && !isGameCompleted)
		{
			if (Diadrasis.Instance.currentPoi == poiSearchingNow.key)//correct answer
			{
				if (poiQuestions.Count > 0)
				{
					if (Application.isEditor) Debug.LogWarning("Next Question");
					poiQuestions.Remove(poiSearchingNow);

					totalPoisUnlocked++;

					gameTotalScore += 150;
					SetGameText();

					cPoi myPoi;
					appData.myPoints.TryGetValue(Diadrasis.Instance.currentPoi, out myPoi);

					ShowIntroMessage("poiFound", "poi_found_title", myPoi.title);

					if (poiQuestions.Count <= 0)
					{
						if (Application.isEditor) Debug.LogWarning("YOU WIN !!!!!!!!!!!!!!!!!!!");
						Handheld.Vibrate();
						aPoiFound = false;
						isGameCompleted = true;
						gameTotalScore += Mathf.RoundToInt(gameTotalTime / 10f);
						buttonsMenu.ShowPoiQuestionButton(false);
						SetGameText();
						//Save score
						if (gameTotalScore > gameSceneBestScore)
						{
							PlayerPrefs.SetInt("gameSceneBestScore" + Diadrasis.Instance.sceneName, gameTotalScore);
							PlayerPrefs.Save();
						}
                    }
                    else
                    {
						aPoiFound = true;
                    }
				}
			}
			else//wrong answer
			{
				if (aPoiFound) return;

				gameTotalTime -= 300f;
				gameTotalScore -= 50;
				if (gameTotalScore < 0) gameTotalScore = 0;
                if (gameTotalTime <= 0f)
                {
					isGameLost = true;
					gameTotalTime = 0f;
					ShowIntroMessage("game_over", "game_over_title");
                }

				if (Application.isEditor) Debug.LogWarning("NO NO NO - Try another poi");
				Handheld.Vibrate();
			}
		}
	}

	IEnumerator DelayShowMessage(string msg, string title, string suffix = "", string suffixTitle = "")
    {
		yield return new WaitForSeconds(2f);

		ShowIntroMessage(msg, title, suffix, suffixTitle);

		yield break;
    }

	public void ShowIntroMessage(string msg, string title, string suffix = "", string suffixTitle = "")
	{
		introText.text = appData.FindTerm_text(msg)+" "+suffix;

		introTitle.text = appData.FindTerm_text(title) + " " + suffixTitle;

		introText.fontSize = appSettings.fontSize_keimeno;
		introTitle.fontSize = appSettings.fontSize_titlos;

		//show intro keimeno
		introPanel.SetActive(true);

	}

	public void ShowRandomQuestion()
	{
		aPoiFound = false;

		currentQuestionIndex++;

		int rand = UnityEngine.Random.Range(0, poiQuestions.Count);
		poiSearchingNow = poiQuestions[rand];

		introText.text = appSettings.language == "en" ? poiSearchingNow.questionEn : poiSearchingNow.questionGR;

		introTitle.text = appData.FindTerm_text("quest")+" "+ currentQuestionIndex.ToString() + "/" + totalPoisInScene.ToString();

		introText.fontSize = appSettings.fontSize_keimeno;
		introTitle.fontSize = appSettings.fontSize_titlos;

		//show intro keimeno
		introPanel.SetActive(true);

	}

}

[Serializable]
public class PoiQuest
{
	public string key, questionEn, questionGR;
}
