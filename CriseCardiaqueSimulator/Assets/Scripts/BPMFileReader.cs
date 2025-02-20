using System;
using System.IO;
using UnityEngine;
using UnityUtility.CustomAttributes;

public class BPMFileReader : MonoBehaviour
{
    public int CurrentBPM => m_currentBPM;

    [SerializeField] private string m_bpmFilePath;
    [SerializeField, Disable] private int m_currentBPM;


    // Update is called once per frame
    void Update()
    {
        string bpmFileContent = File.ReadAllText(m_bpmFilePath);
        if (!int.TryParse(bpmFileContent, out m_currentBPM))
        {
            Debug.LogError("Failed to parse the BPM file \nFile content : {bpmFileContent}");
        }
    }
}
