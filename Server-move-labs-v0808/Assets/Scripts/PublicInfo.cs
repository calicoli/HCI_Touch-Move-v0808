using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PublicInfo : MonoBehaviour
{
    public enum LabScene
    {
        Index = 0,
        Lab1_move_28 = 1,
    }

    public enum LabMode
    {
        Full = 0,
        Test = 1,
    }

    // WelcomePhase for server
    public enum WelcomePhase
    {
        in_entry_scene = 0,
        wait_for_input_information = 1,
        check_client_scene = 2,
        set_target_lab = 3,
        assign_block_conditions = 4,
        accept_acc_from_now = 5,
        adjust_block_conditions = 6,
        confirm_block_conditions = 7,
        ready_to_enter_lab = 8,
        in_lab_scene = 9,
    }

    public enum ServerCommand
    {
        no_server_command = 0,
        server_set_target_lab = 1,
        server_begin_to_receive_acc = 2,
        server_confirm_block_conditions = 3,
        server_say_enter_lab = 4,
        server_say_exit_lab = 5,
        server_say_end_lab = 6,
        server_say_reset_drag_type_and_position = 7,
    }
    public enum ClientCommand
    {
        no_client_command = 0,
        client_finish_lab_set = 1,
        client_begin_to_deliver_acc = 2,
        client_confirm_block_conditions = 3,
        client_enter_lab = 4,
    }

    public enum MessageType
    {
        Command = 0,
        Scene = 1,
        Block = 2,
        Angle = 3,
        Trial = 4,
        DragMode = 5,
        DirectDragInfo = 6,
        HoldTapInfo = 7,
        ThrowCatchInfo = 8,
    }

    public enum DragType
    {
        direct_drag = 0,
        hold_tap = 1,
        throw_catch = 2,
    }
}