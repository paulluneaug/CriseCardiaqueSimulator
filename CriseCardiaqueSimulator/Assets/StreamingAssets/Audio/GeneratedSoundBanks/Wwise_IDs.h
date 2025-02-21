/////////////////////////////////////////////////////////////////////////////////////////////////////
//
// Audiokinetic Wwise generated include file. Do not edit.
//
/////////////////////////////////////////////////////////////////////////////////////////////////////

#ifndef __WWISE_IDS_H__
#define __WWISE_IDS_H__

#include <AK/SoundEngine/Common/AkTypes.h>

namespace AK
{
    namespace EVENTS
    {
        static const AkUniqueID PLAY_BUTTON_GOOD = 3676383806U;
        static const AkUniqueID PLAY_BUTTON_TOO_EARLY = 3025487729U;
        static const AkUniqueID PLAY_BUTTON_TOO_LATE = 3781966448U;
        static const AkUniqueID PLAY_GAMEREADY = 1010471913U;
        static const AkUniqueID PLAY_INIT_GAME = 3291290705U;
        static const AkUniqueID PLAY_MUTE_MUSIC = 1340738297U;
        static const AkUniqueID PLAY_SET_GAME = 1376856651U;
        static const AkUniqueID PLAY_SET_GAMEOVER = 842556545U;
        static const AkUniqueID PLAY_SET_INTRO = 3245586965U;
        static const AkUniqueID PLAY_STOP_ALL = 2904946146U;
        static const AkUniqueID PLAY_UNMUTE_MUSIC = 1426335544U;
    } // namespace EVENTS

    namespace STATES
    {
        namespace MUSIC_MUTED
        {
            static const AkUniqueID GROUP = 2798074986U;

            namespace STATE
            {
                static const AkUniqueID MUTED = 3791155954U;
                static const AkUniqueID NONE = 748895195U;
                static const AkUniqueID UNMUTED = 3763503965U;
            } // namespace STATE
        } // namespace MUSIC_MUTED

    } // namespace STATES

    namespace SWITCHES
    {
        namespace GAMESTATE
        {
            static const AkUniqueID GROUP = 4091656514U;

            namespace SWITCH
            {
                static const AkUniqueID GAME = 702482391U;
                static const AkUniqueID GAMEOVER = 4158285989U;
                static const AkUniqueID INTRO = 1125500713U;
            } // namespace SWITCH
        } // namespace GAMESTATE

    } // namespace SWITCHES

    namespace GAME_PARAMETERS
    {
        static const AkUniqueID RTPC_BUTTON_METER_DUCK = 2951868199U;
        static const AkUniqueID RTPC_HEARTBEAT_BPM = 1409130199U;
    } // namespace GAME_PARAMETERS

    namespace BANKS
    {
        static const AkUniqueID INIT = 1355168291U;
        static const AkUniqueID MAIN = 3161908922U;
    } // namespace BANKS

    namespace BUSSES
    {
        static const AkUniqueID MASTER_AUDIO_BUS = 3803692087U;
    } // namespace BUSSES

    namespace AUDIO_DEVICES
    {
        static const AkUniqueID NO_OUTPUT = 2317455096U;
        static const AkUniqueID SYSTEM = 3859886410U;
    } // namespace AUDIO_DEVICES

}// namespace AK

#endif // __WWISE_IDS_H__
