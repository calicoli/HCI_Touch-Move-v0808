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
        // assgin
        int trialid, secondid;
        public int tp2Count;

        public Vector2 tp2SuccessPosition;
        public ArrayList tp2FailPositions;
        public LocalTime localTime;

        public void init(int idx, int id2)
        {
            trialid = idx;
            secondid = id2;
            tp2Count = 0;
            tp2FailPositions = new ArrayList();
            tp2FailPositions.Clear();
            localTime = new LocalTime();
        }

        public string getAllData()
        {
            string str;
            str = trialid.ToString() + "#" + secondid.ToString() + "#"
                + localTime.clientReceiveDataStamp + "#"
                + localTime.clientSendDataStamp + "#"
                + tp2Count.ToString() + "#"
                + localTime.t2ShowupStamp.ToString() + "#"
                + localTime.tp2SuccessStamp.ToString() + "#"
                + tp2SuccessPosition.x.ToString() + "#"
                + tp2SuccessPosition.y.ToString() + "#"
                ;
            if (tp2Count > 1)
            {
                for (int i = 0; i < tp2FailPositions.Count; i++)
                {
                    str += tp2FailPositions[i].ToString() + "*";
                }
            }
            else
            {
                str += " ";
            }
            str += "#";
            Debug.Log("cTrialData: " + str);
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
