using System;
using System.IO;
using UnityEngine;
using UnityUtility.CustomAttributes;
using UnityUtility.Timer;

public class BPMFileReader : MonoBehaviour
{
    public int CurrentBPM => m_currentBPM;

    [SerializeField] private string m_bpmFilePath;
    [SerializeField] private Timer m_reloadFrequency;
    [SerializeField, Disable] private int m_currentBPM;

    private void Start()
    {
        m_reloadFrequency.Start();
    }
    // Update is called once per frame
    void Update()
    {
        if (m_reloadFrequency.Update(Time.deltaTime))
        {
            string bpmFileContent = File.ReadAllText(m_bpmFilePath);
            if (!int.TryParse(bpmFileContent, out m_currentBPM))
            {
                Debug.LogError("Failed to parse the BPM file \nFile content : {bpmFileContent}");
            }
        }
    }
}
