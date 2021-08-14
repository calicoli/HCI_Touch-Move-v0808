using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PublicTrialParams : MonoBehaviour
{
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
        public long t1ShowupStamp, tp1SuccessStamp;     // server
        public long t2ShowupStamp, tp2SuccessStamp;     // client
        public long serverSendDataStamp, serverReceiveDataStamp;    // server
        public long clientReceiveDataStamp, clientSendDataStamp;    // client
    }

    // wait to update ↓
    public struct TrialDataWithLocalTime
    {
        // assign
        int trialid;
        int firstid, secondid;
        string prefix;
        // raw
        public int tp1Count, tp2Count;
        public Vector2 tp1SuccessPosition, tp2SuccessPosition;
        public ArrayList tp1FailPositions;
        public string tp2FailPositions;
        //public InternetTime interTime;
        public LocalTime localTime;
        // calculate
        public bool isTrialSuccessWithNoError;
        public bool isTarget1SuccessWithNoError, isTarget2SuccessWithNoError;
        public long loCompleteTime, loServerIntervalTime, loClientIntervalTime, loDataTransferForthBackTime;
        public long loTarget1CompleteTime;
        public long loTarget2CompleteTime;
        public long loTarget1ShowTime; // tp1SuccessStamp - t1ShowupStamp
        public long loTarget2ShowTime; // tp2SuccessStamp - t2ShowupStamp

        public void init(int idx, int id1, int id2)
        {
            trialid = idx;
            firstid = id1;
            secondid = id2;
            tp1Count = 0;
            tp2Count = 0;
            tp1FailPositions = new ArrayList(); tp1FailPositions.Clear();
            tp2FailPositions = null;
            localTime = new LocalTime();
        }

        public long calTimeSpan(long later, long earlier)
        {
            return later - earlier;
        }

        public void calMoreData()
        {
            // trial success
            isTarget1SuccessWithNoError = tp1Count == 1 ? true : false;
            isTarget2SuccessWithNoError = tp2Count == 1 ? true : false;
            isTrialSuccessWithNoError = isTarget1SuccessWithNoError && isTarget2SuccessWithNoError;
            // trial time
            loCompleteTime = calTimeSpan(localTime.serverReceiveDataStamp, localTime.t1ShowupStamp);
            loServerIntervalTime = calTimeSpan(localTime.serverReceiveDataStamp, localTime.serverSendDataStamp);
            loClientIntervalTime = calTimeSpan(localTime.clientSendDataStamp, localTime.clientReceiveDataStamp);
            loDataTransferForthBackTime = calTimeSpan(loServerIntervalTime, loClientIntervalTime);
            //loTarget1CompleteTime = calTimeSpan(localTime.tp1SuccessStamp, localTime.t1ShowupStamp);
            //loTarget2CompleteTime = calTimeSpan(localTime.tp2SuccessStamp, localTime.clientReceivedDataStamp);
            loTarget1ShowTime = calTimeSpan(localTime.tp1SuccessStamp, localTime.t1ShowupStamp);
            loTarget2ShowTime = calTimeSpan(localTime.tp2SuccessStamp, localTime.t2ShowupStamp);
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
            str = prefix + ";"
                + trialid.ToString() + ";" + firstid.ToString() + ";" + secondid.ToString() + ";"
                // calculate
                + isTrialSuccessWithNoError.ToString() + ";"
                + isTarget1SuccessWithNoError.ToString() + ";" + isTarget2SuccessWithNoError.ToString() + ";"
                + loCompleteTime.ToString() + ";" + loDataTransferForthBackTime.ToString() + ";"
                + loServerIntervalTime.ToString() + ";" + loClientIntervalTime.ToString() + ";"
                //+ loTarget1CompleteTime.ToString() + ";" + loTarget2CompleteTime.ToString() + ";"
                + loTarget1ShowTime.ToString() + ";" + loTarget2ShowTime.ToString() + ";"
                // raw: success position
                + tp1SuccessPosition.ToString() + ";" + tp2SuccessPosition.ToString() + ";"
                // raw: other data
                + localTime.serverSendDataStamp.ToString() + ";" + localTime.clientReceiveDataStamp.ToString() + ";"
                + localTime.clientSendDataStamp.ToString() + ";" + localTime.serverReceiveDataStamp.ToString() + ";"
                + tp1Count.ToString() + ";"
                + localTime.t1ShowupStamp.ToString() + ";" + localTime.tp1SuccessStamp.ToString() + ";"
                + tp2Count.ToString() + ";"
                + localTime.t2ShowupStamp.ToString() + ";" + localTime.tp2SuccessStamp.ToString() + ";"
                ;
            if (tp1Count > 1)
            {
                for (int i = 0; i < tp1FailPositions.Count; i++)
                {
                    str += tp1FailPositions[i].ToString() + "*";
                }
            }
            else
            {
                str += "T1NoError";
            }
            str += ";";
            str += (tp2Count > 1) ? tp2FailPositions : "T2NoError";
            str += ";";
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
