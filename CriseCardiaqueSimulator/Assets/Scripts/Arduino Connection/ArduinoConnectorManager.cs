using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;
using UnityUtility.CustomAttributes;

public class ArduinoConnectorManager : MonoBehaviour
{
    private const int QUEUE_CAPACITY = 100;

    private const byte BUTTON_DATA_HEADER = 10;
    private const byte DMX_COMMAND_HEADER = 20;
    private const byte ERROR_HEADER = 245;

    private const byte BUTTON_0_OFFSET = 0;
    private const byte BUTTON_1_OFFSET = 1;

    public bool Button0State => m_button0State;
    public bool Button1State => m_button1State;

    [Button(nameof(GetAvailablePorts))]
    [SerializeField] private ArduinoConnector m_arduinoConnector;

    [SerializeField] private byte m_value;

    [NonSerialized] private Queue<byte> m_recievedDatas;

    [SerializeField] private bool m_button0State;
    [SerializeField] private bool m_button1State;


    private void Start()
    {
        m_recievedDatas = new Queue<byte>(QUEUE_CAPACITY);

        m_button0State = false;
        m_button1State = false;

        m_arduinoConnector.Init();

        m_arduinoConnector.OnMessageRecieved += OnArduinoMessageRecieved;
    }

    private void OnDestroy()
    {
        m_arduinoConnector.Close();
    }

    public void SendDMXCommand(byte channel, byte value)
    {
        int indexInBuffer = 0;
        Span<byte> buffer = stackalloc byte[3];

        buffer[indexInBuffer++] = DMX_COMMAND_HEADER;
        buffer[indexInBuffer++] = channel;
        buffer[indexInBuffer++] = value;

        m_arduinoConnector.Send(buffer);
    }

    private void OnArduinoMessageRecieved(byte[] buffer, int recievedBytesCount)
    {
        for (int i = 0; i < recievedBytesCount; i++)
        {
            m_recievedDatas.Enqueue(buffer[i]);
        }


        bool recievedEnoughDatas = true;
        while (m_recievedDatas.Count > 0 && recievedEnoughDatas)
        {
            byte queueHead = m_recievedDatas.Peek();
            switch (queueHead)
            {
                case BUTTON_DATA_HEADER:
                    recievedEnoughDatas &= TryProcessButtonsDatas(m_recievedDatas);
                    break;

                case ERROR_HEADER:
                    recievedEnoughDatas &= TryProcessErrorDatas(m_recievedDatas);
                    break;

                default: // The header is discarded if unknown
                    Debug.LogError($"Unknown Header ({queueHead}) Next commands might not be working properly");
                    m_recievedDatas.Dequeue();
                    break;
            }
        }
        //Debug.Log($"Ditance = {m_sensorDistance}");
    }

    private bool TryProcessButtonsDatas(Queue<byte> recievedDatas)
    {
        if (recievedDatas.Count < 2)
        {
            return false; // Should wait for more datas to arrive
        }

        recievedDatas.Dequeue(); // Dequeue the header

        byte buttonsState = recievedDatas.Dequeue();

        if (buttonsState != 0)
        {
            Debug.LogWarning($"Button values : {buttonsState}");
        }
        m_button0State = (buttonsState & (1 << BUTTON_0_OFFSET)) != 0;
        m_button1State = (buttonsState & (1 << BUTTON_1_OFFSET)) != 0;

        return true; // The command was processed and removed from the queue
    }

    private bool TryProcessErrorDatas(Queue<byte> recievedDatas)
    {
        if (recievedDatas.Count < 2)
        {
            return false; // Should wait for more datas to arrive
        }

        recievedDatas.Dequeue(); // Dequeue the header

        byte errorCode = recievedDatas.Dequeue();

        Debug.LogError($"ArduinoError recieved : {errorCode}");
        return true; // The command was processed and removed from the queue
    }

    [ContextMenu("Command COLOR")]
    private void SendCommandColor()
    {
        SendDMXCommand(DMXChannelsGlossary.LIGHT_COLOR_CHANNEL, m_value);
    }

    [ContextMenu("Command DIMMER")]
    private void SendCommandDimmer()
    {
        SendDMXCommand(DMXChannelsGlossary.LIGHT_DIMMER_CHANNEL, m_value);
    }

    private void GetAvailablePorts()
    {
        Debug.Log("Available Ports :");
        foreach (string portName in SerialPort.GetPortNames())
        {
            Debug.Log($"- {portName}");
        }
    }

}
