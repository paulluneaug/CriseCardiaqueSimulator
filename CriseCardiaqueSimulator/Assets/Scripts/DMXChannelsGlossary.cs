using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DMXChannelsGlossary
{
    public const byte LIGHT_ADRESS = 1 - 1;

    // Light Channels
    public const byte LIGHT_PAN_CHANNEL = LIGHT_ADRESS + 1;
    public const byte LIGHT_TILT_CHANNEL = LIGHT_ADRESS + 3;
    public const byte LIGHT_COLOR_CHANNEL = LIGHT_ADRESS + 5;
    public const byte LIGHT_GOBO_CHANNEL = LIGHT_ADRESS + 6;
    public const byte LIGHT_STROBE_CHANNEL = LIGHT_ADRESS + 7;
    public const byte LIGHT_DIMMER_CHANNEL = LIGHT_ADRESS + 8;
}
