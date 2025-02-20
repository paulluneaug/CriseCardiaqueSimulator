using System;
using System.IO;
using UnityEngine;

public class BPMFileReader : MonoBehaviour
{

    [SerializeField] private string m_bpmFilePath;
    [SerializeField] private string m_bpmFileContent;

    [NonSerialized] private FileStream m_bpmFileStream;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        m_bpmFileContent = File.ReadAllText(m_bpmFilePath);
    }
}
