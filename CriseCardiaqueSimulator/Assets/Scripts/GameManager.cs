using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityUtility.CustomAttributes;
using UnityUtility.SerializedDictionary;
using UnityUtility.Utils;

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
    }

    private enum Button
    {
        Button0,
        Button1,
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


    // Cache
    [NonSerialized] private bool m_lost;
    [NonSerialized] private float m_score;

    [NonSerialized] private Button m_currentButton;
    [NonSerialized] private Timing m_previousTiming;

    [NonSerialized] private float m_nextTimeToClick;
    [NonSerialized] private float m_nextRangeToClick;

    [NonSerialized] private float m_lastButtonPressTime;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_lost = false;
        m_score = 0.0f;

        m_gameStartConfig.ApplyConfigration(m_arduinoManager);

        m_currentButton = Button.Button0;
        SetupNextButtonClick();
        
        m_buttonConfigs[Button.Button0].GetButtonState = () => m_arduinoManager.Button0State;
        m_buttonConfigs[Button.Button1].GetButtonState = () => m_arduinoManager.Button1State;
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_lost)
        {
            ButtonConfig currentconfig = m_buttonConfigs[m_currentButton];

            m_score += m_scoringPerSecondOverBPM.Evaluate(m_bpmFileReader.CurrentBPM);

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
                        currentconfig.SpotTooEarlyConfiguration.ApplyConfigration(m_arduinoManager);
                        break;
                    case Timing.Good:
                        Debug.LogWarning($"Applied DMX config Good");
                        currentconfig.SpotGoodTimingConfiguration.ApplyConfigration(m_arduinoManager);
                        break;
                    case Timing.TooLate:
                        break;
                }
            }

            if (timing == Timing.TooLate)
            {
                LoseGame(timing);
                return;
            }

            if (currentconfig.GetButtonState())
            {
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

        m_lost = true;
        m_gameOverConfig.ApplyConfigration(m_arduinoManager);
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
