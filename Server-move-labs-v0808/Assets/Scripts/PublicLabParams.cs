using UnityEngine;
using static PublicInfo;
using static PublicBlockParams;
using System;

public class PublicLabParams : MonoBehaviour
{
    public const int BLOCK_START_INDEX = 1;
    public const int TRIAL_START_INDEX = 1;

    public enum LabPhase
    {
        in_lab_scene = 0,
        check_connection = 1,
        check_client_scene = 2,
        in_experiment = 3,
        end_experiment = 4,
        wait_to_back_to_entry = 5,
        out_lab_scene = 6,
    }

    public struct LabInfos
    {
        public LabName labName;
        public LabMode labMode;
        public int totalBlockCount, totalTrialCount, repetitionCount,
                   s1PositionCount, s2PositionCount;

        public int postureCount, orientationCount, angleCount;

        public void setLabInfoWithName(LabName name)
        {
            labName = name;
            switch (name)
            {
                case LabName.Lab1_move_28:
                    setParams(Lab1_move_28.totalBlockCount, 
                        Lab1_move_28.fullTrialCount, Lab1_move_28.fullRepetitionCount, 
                        Lab1_move_28.s1PositionCount, Lab1_move_28.s2PositionCount,
                        Enum.GetNames(typeof(Lab1_move_28.Posture)).Length,
                        Enum.GetNames(typeof(Lab1_move_28.Orientation)).Length,
                        Lab1_move_28.AngleOfScreens.Length);
                    break;
                default:
                    break;
            }
        }

        private void setParams(int cntBlock, int cntTrial, int cntRepetition, int cntS1Pos, int cntS2Pos,
            int cntPosture, int cntOrientation, int cntAngle)
        {
            totalBlockCount = cntBlock;
            totalTrialCount = cntTrial;
            labMode = LabMode.Full;
            repetitionCount = cntRepetition;
            s1PositionCount = cntS1Pos;
            s2PositionCount = cntS2Pos;

            postureCount = cntPosture;
            orientationCount = cntOrientation;
            angleCount = cntAngle;
        }

        public void setTestModeParams()
        {
            switch (labName)
            {
                case LabName.Lab1_move_28:
                    labMode = LabMode.Test;
                    totalTrialCount = Lab1_move_28.testTrialCount;
                    repetitionCount = Lab1_move_28.testRepetitionCount;
                    break;
            }
        }
    }

    enum Factors
    {
        Technique = 0,
        Posture = 1,
        Orientation = 2,
        Shape = 3,
        AngleOfScreens = 4,
    }

    public class Lab1_move_28
    {
        public const int
            totalBlockCount = 10, // for one technique
            fullTrialCount = 14 * 2,
            testTrialCount = 14 * 2,
            fullRepetitionCount = 3,
            testRepetitionCount = 1,
            s1PositionCount = 12,
            s2PositionCount = 12;

        public enum Technique
        {
            direct_drag = 0,
            hold_and_tap = 1,
            throw_and_catch = 2,
        }

        public enum Posture
        {
            //NDH_hi_DH_hi = 0,
            //NDH_hi_DH_0i = 1,
            //NDH_h0_DH_0i = 2,
            hold_and_interact = 0,
        }
        public enum Orientation
        {
            protrait = 0,
            landscape = 1
        }

        public enum Shape
        {
            concave = 0,
            flat = 1,
            convex = 2,
        }

        public static float[] AngleOfScreens = { 90, 135, 180, 225, 270 };
    }
}
