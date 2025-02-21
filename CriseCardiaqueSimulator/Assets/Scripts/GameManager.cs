using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityUtility.CustomAttributes;
using UnityUtility.SerializedDictionary;

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
    [SerializeField] private DMXSpotConfiguration m_gameStartConfig;
    [SerializeField] private DMXSpotConfiguration m_gameOverConfig;

    [Title("UI")]
    [SerializeField] private UITextController m_bpmTextController;
    [SerializeField] private UITextController m_scoreTextController;


    // Cache
    [NonSerialized] private GameState m_gameState;
    [NonSerialized] private bool[] m_tutoButtonPressedOnce;
    [NonSerialized] private float m_score;

    [NonSerialized] private Button m_currentButton;
    [NonSerialized] private Timing m_previousTiming;

    [NonSerialized] private float m_nextTimeToClick;
    [NonSerialized] private float m_nextRangeToClick;

    [NonSerialized] private float m_lastButtonPressTime;


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
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReloadGame();
        }

        m_bpmTextController.UpdateText(m_bpmFileReader.CurrentBPM);
        switch (m_gameState)
        {
            case GameState.Tuto:
                UpdateTuto();
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
            m_gameState = GameState.MainLoop;
            Debug.LogError("Switch state To Mainloop");


            m_currentButton = Button.Button0;
            SetupNextButtonClick();
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
        AkSoundEngine.PostEvent("Play_set_gameover", gameObject);
    }

    #region Debug
    //private void ApplyButton0DMXSpotConfig()
    //{
    //    if (!Application.isPlaying)
    //    {
    //        return;
    //    }
    //    m_button0SpotConfiguration.ApplyConfigration(m_arduinoManager);
    //}
    //private void ApplyButton1DMXSpotConfig()
    //{
    //    if (!Application.isPlaying)
    //    {
    //        return;
    //    }
    //    m_button1SpotConfiguration.ApplyConfigration(m_arduinoManager);
    //}
    #endregion
}
