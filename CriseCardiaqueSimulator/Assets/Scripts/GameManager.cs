using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityUtility.CustomAttributes;
using UnityUtility.SerializedDictionary;
using UnityUtility.Timer;

public class GameManager : MonoBehaviour
{
    [Serializable]
    private class ButtonConfig
    {
        public Func<bool> GetButtonState;

        [Title("DMX Spot Configs")]
        //[Button(nameof(ApplyButton0DMXSpotConfig), "Apply configuration")]
        public DMXSpotConfiguration SpotTooEarlyConfiguration;
        [Space]
        //[Button(nameof(ApplyButton1DMXSpotConfig), "Apply configuration")]
        public DMXSpotConfiguration SpotGoodTimingConfiguration;

        [Title("Side Panel")]
        public Image SidePanel;
        public Color TooEarlyColor;
        public Color GoodColor;
    }

    private enum GameState
    {
        Tuto,
        Countdown,
        MainLoop,
        End,
    }

    private enum Button
    {
        Button0 = 0,
        Button1 = 1,
    }

    private enum Timing
    {
        TooEarly,
        Good,
        TooLate,
    }

    [Title("References")]
    [SerializeField] private ArduinoConnectorManager m_arduinoManager;
    [SerializeField] private BPMFileReader m_bpmFileReader;

    [Title("Curves")]
    [SerializeField] private AnimationCurve m_timeToClickOverBPM;
    [SerializeField] private AnimationCurve m_rangeToClickOverBPM;
    [SerializeField] private AnimationCurve m_scoringPerSecondOverBPM;

    [Title("Buttons config")]
    [SerializeField] private SerializedDictionary<Button, ButtonConfig> m_buttonConfigs;

    [Title("Addtional DMX Configs")]
    [Button(nameof(ApplyButton0GoodDMXSpotConfig))]
    [Button(nameof(ApplyButton0TooEarlyDMXSpotConfig))]
    [Button(nameof(ApplyButton1GoodDMXSpotConfig))]
    [Button(nameof(ApplyButton1TooEarlyDMXSpotConfig))]
    [Button(nameof(ApplyGameOverDMXSpotConfig))]
    [Button(nameof(ApplyGameStartDMXSpotConfig))]

    [SerializeField] private DMXSpotConfiguration m_gameStartConfig;
    [SerializeField] private DMXSpotConfiguration m_gameOverConfig;

    [Title("UI")]
    [SerializeField] private RectTransform m_heartRateScorePanel;
    [SerializeField] private UITextController m_bpmTextController;
    [SerializeField] private UITextController m_scoreTextController;

    [SerializeField] private UITextController m_startTextController;
    [SerializeField] private UITextController m_endScoreTextController;
    [SerializeField] private Image m_background;

    [SerializeField] private Timer m_countdownStepTimer;
    [SerializeField] private string[] m_countdownSteps;


    // Cache
    [SerializeField] private int m_countdownStepPassed;

    [NonSerialized] private GameState m_gameState;
    [NonSerialized] private bool[] m_tutoButtonPressedOnce;
    [NonSerialized] private float m_score;

    [NonSerialized] private Button m_currentButton;
    [NonSerialized] private Timing m_previousTiming;

    [NonSerialized] private float m_nextTimeToClick;
    [NonSerialized] private float m_nextRangeToClick;

    [NonSerialized] private float m_lastButtonPressTime;
    [NonSerialized] private bool m_mute;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_gameState = GameState.Tuto;
        m_score = 0.0f;
        m_scoreTextController.UpdateText(m_score);

        m_gameStartConfig.ApplyConfigration(m_arduinoManager);
        
        m_buttonConfigs[Button.Button0].GetButtonState = () => m_arduinoManager.Button0State;
        m_buttonConfigs[Button.Button0].SidePanel.gameObject.SetActive(false);
        m_buttonConfigs[Button.Button1].GetButtonState = () => m_arduinoManager.Button1State;
        m_buttonConfigs[Button.Button1].SidePanel.gameObject.SetActive(false);

        m_tutoButtonPressedOnce = new bool[2];
        m_tutoButtonPressedOnce[0] = false;
        m_tutoButtonPressedOnce[1] = false;

        m_mute = false;

        m_startTextController.gameObject.SetActive(false);
        m_endScoreTextController.gameObject.SetActive(false);
        m_background.gameObject.SetActive(false);
        m_heartRateScorePanel.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReloadGame();
            return;
        }

        if(Input.GetKeyDown(KeyCode.T))
        {
            m_mute = !m_mute;
            if (m_mute)
            {
                AkSoundEngine.PostEvent("Play_mute_music", gameObject);
            }
            else
            {

                AkSoundEngine.PostEvent("Play_unmute_music", gameObject);
            }
        }

        m_bpmTextController.UpdateText(m_bpmFileReader.CurrentBPM);
        switch (m_gameState)
        {
            case GameState.Tuto:
                UpdateTuto();
                break;
            case GameState.Countdown:
                UpdateCountdown();
                break;
            case GameState.MainLoop:
                UpdateMainLoop();
                break;
            case GameState.End:
                break;
        }
    }

    private void UpdateTuto()
    {
        if (m_tutoButtonPressedOnce.All(b => b) && 
            !m_buttonConfigs[Button.Button0].GetButtonState() && 
            !m_buttonConfigs[Button.Button1].GetButtonState())
        {
            StartCountdown();
            return;
        }

        if (m_buttonConfigs[Button.Button0].GetButtonState())
        {
            m_tutoButtonPressedOnce[(int)Button.Button0] = true;
        }

        if (m_buttonConfigs[Button.Button1].GetButtonState())
        {
            m_tutoButtonPressedOnce[(int)Button.Button1] = true;
        }
    }

    private void StartCountdown()
    {
        m_gameState = GameState.Countdown;
        Debug.LogError("Switch state To Countdown");

        m_countdownStepPassed = 0;
        m_countdownStepTimer.Start();
        m_startTextController.UpdateText(m_countdownSteps[m_countdownStepPassed]);

        m_startTextController.gameObject.SetActive(true);
        m_background.gameObject.SetActive(true);
        m_heartRateScorePanel.gameObject.SetActive(false);

        AkSoundEngine.PostEvent("Play_gameready", gameObject);
    }

    private void StartMainloop()
    {
        m_gameState = GameState.MainLoop;
        Debug.LogError("Switch state To Mainloop");

        m_startTextController.gameObject.SetActive(false);
        m_background.gameObject.SetActive(false);
        m_heartRateScorePanel.gameObject.SetActive(true);

        m_currentButton = Button.Button0;
        SetupNextButtonClick();
        AkSoundEngine.PostEvent("Play_set_game", gameObject);
    }

    private void UpdateCountdown()
    {
        if (m_countdownStepTimer.Update(Time.deltaTime))
        {
            m_countdownStepPassed++;
            if (m_countdownStepPassed >= m_countdownSteps.Length)
            {
                m_countdownStepTimer.Stop();
                StartMainloop();
                return;
            }
            m_startTextController.UpdateText(m_countdownSteps[m_countdownStepPassed]);
        }
    }

    private void UpdateMainLoop()
    {
        ButtonConfig currentConfig = m_buttonConfigs[m_currentButton];

        m_score += m_scoringPerSecondOverBPM.Evaluate(m_bpmFileReader.CurrentBPM);
        m_scoreTextController.UpdateText(m_score);


        float pressTimeRelativeToPerfect = Time.time - m_lastButtonPressTime - m_nextTimeToClick;

        Timing timing;
        if (pressTimeRelativeToPerfect < -m_nextRangeToClick)
        {
            timing = Timing.TooEarly;
        }
        else if (m_nextRangeToClick < pressTimeRelativeToPerfect)
        {
            timing = Timing.TooLate;
        }
        else
        {
            timing = Timing.Good;
        }

        if (timing != m_previousTiming)
        {
            Debug.LogWarning($"Different Timing ({timing} and {m_previousTiming})");
            m_previousTiming = timing;
            switch (timing)
            {
                case Timing.TooEarly:
                    Debug.LogWarning($"Applied DMX config Too Early");
                    currentConfig.SpotTooEarlyConfiguration.ApplyConfigration(m_arduinoManager);
                    AkSoundEngine.PostEvent("Play_button_too_early", gameObject);

                    currentConfig.SidePanel.gameObject.SetActive(true);
                    currentConfig.SidePanel.color = currentConfig.TooEarlyColor;
                    break;
                case Timing.Good:
                    Debug.LogWarning($"Applied DMX config Good");
                    currentConfig.SpotGoodTimingConfiguration.ApplyConfigration(m_arduinoManager);
                    AkSoundEngine.PostEvent("Play_button_good", gameObject);

                    currentConfig.SidePanel.gameObject.SetActive(true);
                    currentConfig.SidePanel.color = currentConfig.GoodColor;
                    break;
                case Timing.TooLate:
                    AkSoundEngine.PostEvent("Play_button_too_late", gameObject);
                    currentConfig.SidePanel.gameObject.SetActive(true);
                    currentConfig.SidePanel.color = currentConfig.TooEarlyColor;
                    break;
            }
        }

        if (timing == Timing.TooLate)
        {
            LoseGame(timing);
            return;
        }

        if (currentConfig.GetButtonState())
        {
            Debug.LogWarning($"Button {m_currentButton} pressed");
            currentConfig.SidePanel.gameObject.SetActive(false);
            switch (timing)
            {
                case Timing.TooEarly:
                    LoseGame(timing);
                    break;
                case Timing.Good:
                    SetupNextButtonClick();
                    break;
                case Timing.TooLate:
                    LoseGame(timing);
                    break;
            }
        }
    }

    private void ReloadGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        AkSoundEngine.PostEvent("Play_stop_All", gameObject);
    }

    private void SetupNextButtonClick()
    {
        int currentBPM = m_bpmFileReader.CurrentBPM;

        m_lastButtonPressTime = Time.time;

        m_nextTimeToClick = m_timeToClickOverBPM.Evaluate(currentBPM);
        m_nextRangeToClick = m_rangeToClickOverBPM.Evaluate(currentBPM);

        m_previousTiming = Timing.TooLate;

        switch (m_currentButton)
        {
            case Button.Button0:
                m_currentButton = Button.Button1;
                break;

            case Button.Button1:
                m_currentButton = Button.Button0;
                break;
        }
        Debug.LogWarning($"Next button {m_currentButton}");
    }

    private void LoseGame(Timing timing)
    {
        Debug.LogError($"You lose because you were {timing}");
        Debug.Log($"Score : {m_score}");

        m_gameState = GameState.End;
        m_gameOverConfig.ApplyConfigration(m_arduinoManager);

        m_background.gameObject.SetActive(true);
        m_endScoreTextController.gameObject.SetActive(true);
        m_endScoreTextController.UpdateText(m_score);
        m_heartRateScorePanel.gameObject.SetActive(false);

        AkSoundEngine.PostEvent("Play_set_gameover", gameObject);
    }

    #region Debug
    private void ApplyButton0GoodDMXSpotConfig()
    {
        if (!Application.isPlaying)
        {
            return;
        }
        m_buttonConfigs[Button.Button0].SpotGoodTimingConfiguration.ApplyConfigration(m_arduinoManager);
    }
    private void ApplyButton0TooEarlyDMXSpotConfig()
    {
        if (!Application.isPlaying)
        {
            return;
        }
        m_buttonConfigs[Button.Button0].SpotTooEarlyConfiguration.ApplyConfigration(m_arduinoManager);
    }
    private void ApplyButton1GoodDMXSpotConfig()
    {
        if (!Application.isPlaying)
        {
            return;
        }
        m_buttonConfigs[Button.Button1].SpotGoodTimingConfiguration.ApplyConfigration(m_arduinoManager);
    }
    private void ApplyButton1TooEarlyDMXSpotConfig()
    {
        if (!Application.isPlaying)
        {
            return;
        }
        m_buttonConfigs[Button.Button1].SpotTooEarlyConfiguration.ApplyConfigration(m_arduinoManager);
    }
    private void ApplyGameStartDMXSpotConfig()
    {
        if (!Application.isPlaying)
        {
            return;
        }
        m_gameStartConfig.ApplyConfigration(m_arduinoManager);
    }
    private void ApplyGameOverDMXSpotConfig()
    {
        if (!Application.isPlaying)
        {
            return;
        }
        m_gameOverConfig.ApplyConfigration(m_arduinoManager);
    }
    #endregion
}
