using System;
using System.IO;
using System.IO.Ports;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class ArduinoConnector
{
    private const int BUFFER_SIZE = 32;

    public event Action<byte[], int> OnMessageRecieved;

    [SerializeField] private string m_port;
    [SerializeField] private int m_baud;

    [NonSerialized] private SerialPort m_serialPort;
    [NonSerialized] private byte[] m_buffer;

    public void Init()
    {
        m_buffer = new byte[BUFFER_SIZE];

        m_serialPort = new SerialPort(m_port, m_baud);
        m_serialPort.ReadTimeout = 50;

        m_serialPort.RtsEnable = true;
        m_serialPort.DtrEnable = true;

        Debug.Log(m_serialPort.IsOpen);
        m_serialPort.Open();

        Task.Factory.StartNew(AwaitDatas);
    }

    public void Close()
    {
        m_serialPort.Close();
    }

    public void Send(Span<byte> buffer)
    {
        m_serialPort.BaseStream.Write(buffer);
        //m_serialPort.WriteLine(message);
        m_serialPort.BaseStream.Flush();
        Debug.Log($"Sent {buffer.Length} bytes");
    }

    private async Task AwaitDatas()
    {
        while(true)
        {
            int readBytesCount = 0;
            try
            {
                readBytesCount = m_serialPort.Read(m_buffer, 0, BUFFER_SIZE);
            }
            catch (TimeoutException)
            {
                readBytesCount = 0;
            }

            if (readBytesCount != 0)
            {
                //Debug.Log($"Recieved {readBytesCount} bytes");
                OnMessageRecieved.Invoke(m_buffer, readBytesCount);
            }
            await Task.Delay(20);
        }
    }
}

