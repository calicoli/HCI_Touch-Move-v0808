using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PublicTrialParams : MonoBehaviour
{
    private static char paramSeperators = ';';
    private static char trialSeperators = '#';
    public static (int firstid, int secondid)[] TrialPositionPairs = new[] {
        (100, 200), (101, 201), (102, 202), (103, 203), (104, 204),
        (105, 205), (106, 206), (107, 207), (108, 208), (109, 209),
        (110, 210), (111, 211), (110, 211), (111, 210),

        (200, 100), (201, 101), (202, 102), (203, 103), (204, 104),
        (205, 105), (206, 106), (207, 107), (208, 108), (209, 109),
        (210, 110), (211, 111), (211, 110), (210, 111),
    };

    // wait to update ↓
    public enum TrialPhase
    {
        inactive_phase = 0,
        block_start = 1,
        repeatition_start = 2,
        repeatition_scheduling = 3,
        a_trial_set_params = 4,
        s_sent_trial_params = 5,
        c_received_trial_params = 6,
        a_trial_start_from_1 = 71,
        a_trial_start_from_2 = 72,
        a_trial_ongoing = 8,
        a_successful_trial = 91,
        a_failed_trial = 92,
        a_trial_output_data = 10,
        a_trial_end = 11,
        block_end = 12,
    } 

    public enum TrialResult
    {
        not_complete = 0,
        success = 1,
        fail = 2,
    }

    // wait to update ↓
    public struct InternetTime
    {
        public long t1ShowupStamp, t2ShowupStamp;
        public long tp1SuccessStamp, tp2SuccessStamp;
        public long serverSendDataStamp, clientReceivedDataStamp;
        public long clientSendDataStamp, serverReceivedDataStamp;
    }

    // wait to update ↓
    public struct LocalTime
    {
        // time stamp
        //public long targetShowupStamp;
        public long touch1StartStamp, touch1EndStamp;
        public long touch2StartStamp, touch2EndStamp;
        public long targetReachMidpointStamp, targetReachEndStamp;
        //public long serverSendDataStamp, serverReceiveDataStamp;    // server
        //public long clientReceiveDataStamp, clientSendDataStamp;    // client
    }

    // wait to update ↓
    public struct TrialDataWithLocalTime
    {
        // assign
        int trialid;
        int firstid, secondid;
        string prefix;

        // two-phase
        Vector2 moveStartPos, moveDestination;
        TrialPhase1RawData phase1Data;
        TrialPhase2RawData phase2Data;
        
        // final-accuracy
        bool isTrialSuccess, isPhase1Success, isPhase2Success;
        public string techResult;
        // final-time
        long loTargetMoveSpan, loMovePhase1Span, loMovePhase2Span;
        long loTouch1Span, loTouch2Span;
        long loDevice1IntervalSpan, loDevice2IntervalSpan, loDataFetchSpan;
        // final-pos
        Vector2 touch1StartPos, touch1EndPos;
        Vector2 touch2StartPos, touch2EndPos;
        Vector2 targetPhase1StartPos, targetPhase1EndPos, targetPhase2StartPos, targetPhase2EndPos;
        // final-tech specific data
        public string techData;

        public void init(int idx, int id1, int id2)
        {
            trialid = idx;
            firstid = id1;
            secondid = id2;
            phase1Data = new TrialPhase1RawData();
            phase1Data.init(idx, id1, id2);
            phase2Data = new TrialPhase2RawData();
            phase2Data.init(idx, id1, id2);
            moveStartPos = moveDestination = Vector2.zero;
            isTrialSuccess = isPhase1Success = isPhase2Success = false;
            touch1StartPos = touch1EndPos = Vector2.zero;
            touch2StartPos = touch2EndPos = Vector2.zero;
            targetPhase1StartPos = targetPhase1EndPos = targetPhase2StartPos = targetPhase2EndPos = Vector2.zero;
            techData = "None";
        }

        public long calTimeSpan(long later, long earlier)
        {
            return later - earlier;
        }

        public void setTrialAccuracy(bool p1success, bool p2success)
        {
            // trial success
            isPhase1Success = p1success;
            isPhase2Success = p2success;
            isTrialSuccess = isPhase1Success && isPhase2Success;
        }

        public void conveyPhase1Data(TrialPhase1RawData rawdata1)
        {
            phase1Data = rawdata1;
            moveStartPos = phase1Data.moveStartPos;
            touch1StartPos = phase1Data.touch1StartPos;
            touch1EndPos = phase1Data.touch1EndPos;
            targetPhase1StartPos = phase1Data.movePhase1StartPos;
            targetPhase1EndPos = phase1Data.movePhase1EndPos;
    }

        public void conveyPhase2Data(TrialPhase2RawData rawdata2)
        {
            phase2Data = rawdata2;
            moveDestination = phase2Data.moveDestination;
            touch2StartPos = phase2Data.touch2StartPos;
            touch2EndPos = phase2Data.touch2EndPos;
            targetPhase2StartPos = phase2Data.movePhase2StartPos;
            targetPhase2EndPos = phase2Data.movePhase2EndPos;
        }

        public void calMoreData()
        {
            // trial time
            loTargetMoveSpan = calTimeSpan(phase1Data.targetReachEndpointInfoReceivedStamp, phase1Data.touch1StartStamp);
            loMovePhase1Span = calTimeSpan(phase1Data.targetReachMidpointStamp, phase1Data.touch1StartStamp);
            loMovePhase2Span = calTimeSpan(phase2Data.targetReachEndpointStamp, phase2Data.touch2StartStamp);
            loTouch1Span = calTimeSpan(phase1Data.touch1EndStamp, phase1Data.touch1StartStamp);
            loTouch2Span = calTimeSpan(phase2Data.touch2EndStamp, phase2Data.touch2StartStamp);
            loDevice1IntervalSpan = calTimeSpan(phase1Data.targetReachEndpointInfoReceivedStamp, phase1Data.targetReachMidpointStamp);
            loDevice2IntervalSpan = calTimeSpan(phase2Data.targetReachEndpointStamp, phase2Data.targetReachMidpointInfoReceivedStamp);
            loDataFetchSpan = calTimeSpan(loDevice1IntervalSpan, loDevice2IntervalSpan);
        }

        public void setPrefix(string pre)
        {
            prefix = pre;
        }

        public string getAllDataForFile()
        {
            calMoreData();
            string str;
            // assign
            str = prefix + paramSeperators
                + trialid.ToString() + paramSeperators + firstid.ToString() + paramSeperators + secondid.ToString() + paramSeperators
                + isTrialSuccess.ToString() + paramSeperators + isPhase1Success.ToString() + paramSeperators + isPhase2Success.ToString() + paramSeperators
                + techResult.ToString() + paramSeperators
                // final-time
                + loTargetMoveSpan.ToString() + paramSeperators
                + loMovePhase1Span.ToString() + paramSeperators
                + loMovePhase2Span.ToString() + paramSeperators
                + loTouch1Span.ToString() + paramSeperators
                + loTouch2Span.ToString() + paramSeperators
                + loDevice1IntervalSpan.ToString() + paramSeperators
                + loDevice2IntervalSpan.ToString() + paramSeperators
                + loDataFetchSpan.ToString() + paramSeperators
                // final-pos
                + moveStartPos.ToString() + paramSeperators
                + moveDestination.ToString() + paramSeperators
                + touch1StartPos.ToString() + paramSeperators
                + touch1EndPos.ToString() + paramSeperators
                + touch2StartPos.ToString() + paramSeperators
                + touch2EndPos.ToString() + paramSeperators
                + targetPhase1StartPos.ToString() + paramSeperators
                + targetPhase1EndPos.ToString() + paramSeperators
                + targetPhase2StartPos.ToString() + paramSeperators
                + targetPhase2EndPos.ToString() + paramSeperators
                // final-tech specific data
                + techData.ToString() + paramSeperators
                + phase1Data.getAllData() + paramSeperators
                + phase2Data.getAllData() + paramSeperators;

            return str;
        }
    }

    public struct TrialPhase1RawData
    {
        int trialid;
        int firstid, secondid;
        public Vector2 moveStartPos;
        // time
        public long touch1StartStamp, touch1EndStamp;
        public long targetReachMidpointStamp, targetReachEndpointInfoReceivedStamp;
        // final-pos
        public Vector2 touch1StartPos, touch1EndPos;
        public Vector2 movePhase1StartPos, movePhase1EndPos;

        public void init (int tid, int id1, int id2)
        {
            trialid = tid;
            firstid = id1;
            secondid = id2;
            moveStartPos = Vector2.zero;
            touch1StartStamp = touch1EndStamp = targetReachMidpointStamp = targetReachEndpointInfoReceivedStamp = 0;
            touch1StartPos = touch1EndPos = movePhase1StartPos = movePhase1EndPos = Vector2.zero;
        }

        public string getAllData()
        {
            string str;
            str = trialid.ToString() + trialSeperators
                + firstid.ToString() + trialSeperators + secondid.ToString() + trialSeperators
                + moveStartPos.x.ToString() + trialSeperators + moveStartPos.y.ToString() + trialSeperators
                + touch1StartStamp.ToString() + trialSeperators
                + touch1EndStamp.ToString() + trialSeperators
                + targetReachMidpointStamp.ToString() + trialSeperators
                + targetReachEndpointInfoReceivedStamp.ToString() + trialSeperators
                + touch1StartPos.x.ToString() + trialSeperators + touch1StartPos.y.ToString() + trialSeperators
                + touch1EndPos.x.ToString() + trialSeperators + touch1EndPos.y.ToString() + trialSeperators
                + movePhase1StartPos.x.ToString() + trialSeperators + movePhase1StartPos.y.ToString() + trialSeperators
                + movePhase1EndPos.x.ToString() + trialSeperators + movePhase1EndPos.y.ToString() + trialSeperators;
            return str;
        }
    }

    public struct TrialPhase2RawData
    {
        int trialid;
        int firstid, secondid;
        public Vector2 moveDestination;
        // time
        public long touch2StartStamp, touch2EndStamp;
        public long targetReachMidpointInfoReceivedStamp, targetReachEndpointStamp;
        // final-pos
        public Vector2 touch2StartPos, touch2EndPos;
        public Vector2 movePhase2StartPos, movePhase2EndPos;

        public void init(int tid, int id1, int id2)
        {
            trialid = tid;
            firstid = id1;
            secondid = id2;
            moveDestination = Vector2.zero;
            touch2StartStamp = touch2EndStamp = targetReachMidpointInfoReceivedStamp = targetReachEndpointStamp = 0;
            touch2StartPos = touch2EndPos = movePhase2StartPos = movePhase2EndPos = Vector2.zero;
        }

        public string getAllData()
        {
            string str;
            str = trialid.ToString() + trialSeperators
                + firstid.ToString() + trialSeperators + secondid.ToString() + trialSeperators
                + moveDestination.x.ToString() + trialSeperators + moveDestination.y.ToString() + trialSeperators
                + touch2StartStamp.ToString() + trialSeperators
                + touch2EndStamp.ToString() + trialSeperators
                + targetReachMidpointInfoReceivedStamp.ToString() + trialSeperators
                + targetReachEndpointStamp.ToString() + trialSeperators
                + touch2StartPos.x.ToString() + trialSeperators + touch2StartPos.y.ToString() + trialSeperators
                + touch2EndPos.x.ToString() + trialSeperators + touch2EndPos.y.ToString() + trialSeperators
                + movePhase2StartPos.x.ToString() + trialSeperators + movePhase2StartPos.y.ToString() + trialSeperators
                + movePhase2EndPos.x.ToString() + trialSeperators + movePhase2EndPos.y.ToString() + trialSeperators;
            return str;
        }
    }

    public struct TrialSequence
    {
        int lenTrial;
        string prefix;
        //PublicLabFactors.Posture posture;
        public List<int> seqRamdon;
        public List<int> seqTarget1;
        public List<int> seqTarget2;
        public List<(int firstid, int secondid)> seqAllTargets;

        public void setTrialLength(int len)
        {
            lenTrial = len;
            seqRamdon = new List<int>();
            seqTarget1 = new List<int>();
            seqTarget2 = new List<int>();
            seqAllTargets = new List<(int firstid, int secondid)>();
        }
        public void setPrefix(string pre)
        {
            prefix = pre + ";";
        }

        public void setAllQuence(PublicLabParams.Lab1_move_28.Posture p)
        {
            var positions = new (int firstid, int secondid)[TrialPositionPairs.Length];
            positions = TrialPositionPairs;
            Debug.Log(p.ToString());
            Debug.Log(positions.Length);
            seqRamdon = randomSequence(positions.Length);
            for (int i = 0; i < lenTrial; i++)
            {
                int pid = seqRamdon[i];
                seqTarget1.Add(positions[pid].firstid);
                seqTarget2.Add(positions[pid].secondid);
                seqAllTargets.Add(positions[pid]);
            }
        }

        public void printSequence(List<int> list)
        {
            string res = "";
            for (int i = 0; i < list.Count; i++)
            {
                res += list[i] + "-";
            }
            Debug.Log(res);
        }

        public string getAllDataForFile()
        {
            string res = "";
            string str = getSequenceString(seqRamdon);
            string st1 = getSequenceString(seqTarget1);
            string st2 = getSequenceString(seqTarget2);
            string sall = getSequenceString(seqAllTargets);
            res = string.Format(
                "{0}Target1;{2}{1}" +
                "{0}Target2;{3}{1}" +
                "{0}AllTargets;{4}{1}" +
                "{0}RamdonSeq;{5}{1}",
                prefix, Environment.NewLine, st1, st2, sall, str
                );
            return res;
        }

        private string getSequenceString(List<int> list)
        {
            string res = "";
            for (int i = 0; i < list.Count; i++)
            {
                res += list[i] + ";";
            }
            return res;
        }
        private string getSequenceString(List<(int firstid, int secondid)> list)
        {
            string res = "";
            for (int i = 0; i < list.Count; i++)
            {
                res += list[i].ToString() + ";";
            }
            return res;
        }

        private List<int> randomSequence(int len)
        {
            List<int> array = new List<int>();
            for (int i = 0; i < len; i++)
            {
                array.Add(i);
            }
            System.Random rd = new System.Random();
            List<int> result = new List<int>();
            while (len > 0)
            {
                int rdnum = rd.Next(0, len);
                int target = (int)array[rdnum];
                array.Remove(target);
                len--;
                result.Add(target);
            }
            return result;
        }
    }

    public struct Trial
    {
        public int index;
        public int firstid, secondid;

        public void setParams(int idx, int id1, int id2)
        {
            index = idx;
            firstid = id1;
            secondid = id2;
        }

        public void printParams()
        {
            Debug.Log("Trial #" + index + ": target1- " + firstid + "; target2-" + secondid);
        }
    }
}
