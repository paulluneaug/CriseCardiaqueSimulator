using System;
using UnityEngine;
using UnityUtility.Utils;

[Serializable]
public class DMXSpotConfiguration
{
    private enum DMXSpotColor : byte
    {
        None = 255,
        White = 0,
        Red = 10, 
        Yellow = 20,
        Blue = 30,
        Green = 40,
        Orange = 50,
        Pink = 60,
        Cyan = 70,
        CyanPink = 80,
        PinkOrange = 90,
        OrangeGreen = 100,
        GreenBlue = 110,
        BlueYellow = 120,
        YellowRed = 130,
        RedWhite = 140,
    }

    [SerializeField, Range(-1, 255)] private int m_pan = -1;
    [SerializeField, Range(-1, 255)] private int m_tilt = -1;
    [SerializeField, Range(-1, 255)] private int m_dimmer = -1;
    [SerializeField] private DMXSpotColor m_color = DMXSpotColor.None;

    public void ApplyConfigration(ArduinoConnectorManager arduinoConnectorManager)
    {
        if (GetByteValue(m_pan, out byte panValue))
        {
            arduinoConnectorManager.SendDMXCommand(DMXChannelsGlossary.LIGHT_PAN_CHANNEL, panValue);
        }
        if (GetByteValue(m_tilt, out byte tiltValue))
        {
            arduinoConnectorManager.SendDMXCommand(DMXChannelsGlossary.LIGHT_TILT_CHANNEL, tiltValue);
        }
        if (GetByteValue(m_dimmer, out byte dimmerValue))
        {
            arduinoConnectorManager.SendDMXCommand(DMXChannelsGlossary.LIGHT_DIMMER_CHANNEL, dimmerValue);
        }
        if (GetByteValue(m_color, out byte colorValue))
        {
            arduinoConnectorManager.SendDMXCommand(DMXChannelsGlossary.LIGHT_COLOR_CHANNEL, colorValue);
        }
    }

    private bool GetByteValue(int intValue, out byte byteValue)
    {
        if (!intValue.Between(0, 255))
        {
            byteValue = 0;
            return false;
        }

        byteValue = (byte)intValue; 
        return true;
    }

    private bool GetByteValue(DMXSpotColor color, out byte byteValue)
    {
        if (m_color == DMXSpotColor.None)
        {
            byteValue = 0;
            return false;
        }
        byteValue = (byte)color;
        return true;
    }
}
