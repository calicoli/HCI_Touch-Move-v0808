using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PublicLabFactors : MonoBehaviour
{
    public enum LabScene
    {
        Entry = 0,
        Demo = 1,
    }

    public enum ServerCommand
    {
        no_server_command = 0,
        server_say_enter_lab = 4,
        server_say_exit_lab = 5,
        server_say_end_lab = 6,
        server_say_reset_drag_type_and_position = 7,
    }
    public enum ClientCommand
    {
        no_client_command = 0,
        client_enter_lab = 4,
    }

    public enum MessageType
    {
        Command = 0,
        Scene   = 1,
        DragMode = 2,
        DirectDragInfo = 3,
        HoldTapInfo = 4,
        ThrowCatchInfo = 5,
    }

    public enum DragType
    {
        direct_drag = 0,
        hold_tap = 1,
        throw_catch = 2,
    }
}
