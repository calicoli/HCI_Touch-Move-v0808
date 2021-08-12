using System;
using System.Collections;
using UnityEngine;
using static PublicInfo;
using static PublicLabParams;

public class PublicBlockParams : MonoBehaviour
{
    public struct BlockSequence
    {
        LabName labName;
        int lenBlock;
        public ArrayList seqPosture;        // lab 1
        public ArrayList seqOrientation;    // lab 1
        public ArrayList seqAngle;          // lab 1
        public ArrayList seqShape;

        #region Public Method
        public void setBlockLength(LabName name)
        {
            labName = name;
            switch (name)
            {
                case LabName.Lab1_move_28:
                    lenBlock = Lab1_move_28.totalBlockCount;
                    break;
            }
            seqPosture = new ArrayList();
            seqOrientation = new ArrayList();
            seqAngle = new ArrayList();
            seqShape = new ArrayList();
        }
        public void setAllSequence(int userid)
        {
            switch (labName)
            {
                case LabName.Lab1_move_28:
                    setLab1Sequance(userid);
                    break;
                default:
                    break;
            }

        }
        public void printSequence(ArrayList list)
        {
            string res = "";
            for (int i = 0; i < list.Count; i++)
            {
                res += list[i] + "-";
            }
            Debug.Log(res);
        }

        public string getAllDataWithLabName()
        {
            string res = "";
            if (labName == LabName.Lab1_move_28)
            {
                string sp = getSequenceString(seqPosture);
                string so = getSequenceString(seqOrientation);
                string sa = getSequenceString(seqAngle);
                string ss = getSequenceString(seqShape);
                res = string.Format(
                    "Posture:{0}{1}{0}Orientation: {0}{2}{0}Shape:{0}{3}{0}Angle:{0}{4}{0}",
                    Environment.NewLine, sp, so, ss, sa
                    );
            }
            return res;
        }
        #endregion

        private void setLab1Sequance(int userid)
        {
            ArrayList tmpPosture = getPostureSequenceWithUserid(userid, Enum.GetNames(typeof(Lab1_move_28.Posture)).Length),
                      tmpOrientation = getRepeatOrientationSequenceWithUserid(userid, Enum.GetNames(typeof(Lab1_move_28.Orientation)).Length),
                      tmpAngle = getFirstAngleSequenceWithUserid(userid, Lab1_move_28.AngleOfScreens.Length);
            ArrayList doubleOrientation = getPalindromeArrayList(tmpOrientation),
                      doubleAngle = getPalindromeArrayList(tmpAngle);

            for (int blockid = 0; blockid < lenBlock; blockid++)
            {
                int pid, aid, sid, oid;
                // posture id
                pid = (int)tmpPosture[blockid / (lenBlock / tmpPosture.Count)];
                seqPosture.Add(pid);
                // orientation id
                oid = (int)doubleOrientation[blockid % doubleOrientation.Count];
                seqOrientation.Add(oid);
                // angle id
                aid = (int)doubleAngle[(blockid % doubleAngle.Count)];
                seqAngle.Add(aid);
                // shape id
                if (Lab1_move_28.AngleOfScreens[aid] < 180) {
                    sid = (int)Lab1_move_28.Shape.concave;
                }
                else if (Lab1_move_28.AngleOfScreens[aid] == 180) {
                    sid = (int)Lab1_move_28.Shape.flat;
                }
                else {
                    sid = (int)Lab1_move_28.Shape.convex;
                }
                seqShape.Add(sid);
            }
        }

        private ArrayList getPostureSequenceWithUserid(int userid, int lenPosture)
        {
            ArrayList postures = new ArrayList();
            switch (userid % 6)
            {
                case 1:
                    postures = getGroupOrderedList(new ArrayList(new[] { 0, 1, 2 }), Enum.GetNames(typeof(Lab1_move_28.Orientation)).Length * Lab1_move_28.AngleOfScreens.Length);
                    break;
                case 2:
                    postures = getGroupOrderedList(new ArrayList(new[] { 1, 0, 2 }), Enum.GetNames(typeof(Lab1_move_28.Orientation)).Length * Lab1_move_28.AngleOfScreens.Length);
                    break;
                case 3:
                    postures = getGroupOrderedList(new ArrayList(new[] { 2, 0, 1 }), Enum.GetNames(typeof(Lab1_move_28.Orientation)).Length * Lab1_move_28.AngleOfScreens.Length);
                    break;
                case 4:
                    postures = getGroupOrderedList(new ArrayList(new[] { 0, 2, 1 }), Enum.GetNames(typeof(Lab1_move_28.Orientation)).Length * Lab1_move_28.AngleOfScreens.Length);
                    break;
                case 5:
                    postures = getGroupOrderedList(new ArrayList(new[] { 1, 2, 0 }), Enum.GetNames(typeof(Lab1_move_28.Orientation)).Length * Lab1_move_28.AngleOfScreens.Length);
                    break;
                case 0:
                    postures = getGroupOrderedList(new ArrayList(new[] { 2, 1, 0 }), Enum.GetNames(typeof(Lab1_move_28.Orientation)).Length * Lab1_move_28.AngleOfScreens.Length);
                    break;
                default:
                    break;
            }
            return postures;
        }

        private ArrayList getRepeatOrientationSequenceWithUserid(int userid, int lenOrientation)
        {
            ArrayList oris = new ArrayList();
            if (userid % 4 == 1 || userid % 4 == 2)
            {
                for (int i = 0; i < lenOrientation; i++)
                {
                    for (int k = 0; k < Lab1_move_28.AngleOfScreens.Length; k++)
                    {
                        oris.Add(i);
                    }
                }

            }
            else if (userid % 4 == 3 || userid % 4 == 0)
            {
                for (int i = lenOrientation - 1; i > -1; i--)
                {
                    for (int k = 0; k < Lab1_move_28.AngleOfScreens.Length; k++)
                    {
                        oris.Add(i);
                    }
                }
            }
            return oris;
        }

        private ArrayList getFirstAngleSequenceWithUserid(int userid, int lenAngle)
        {
            ArrayList angles = new ArrayList();
            if (userid % 4 == 1 || userid % 4 == 3)
            {
                for (int i = 0; i < lenAngle; i++)
                {
                    angles.Add(i); 
                }
            }
            else if (userid % 4 == 0 || userid % 4 == 2)
            {
                for (int i = lenAngle - 1; i > -1; i--)
                {
                    angles.Add(i);
                }
            }
            return angles;
        }

        private ArrayList getGroupOrderedList(ArrayList list, int repeatNum)
        {
            ArrayList res = new ArrayList();
            for (int i=0; i< list.Count; i++)
            {
                for (int k = 0; k < repeatNum; k++)
                {
                    res.Add(list[i]);
                }
            }
            return res;
        }
        
        private ArrayList getPalindromeArrayList(ArrayList list)
        {
            ArrayList res = list;
            for (int i = list.Count - 1; i >= 0; i--)
            {
                res.Add(list[i]);
            }
            return res;
        }

        /*
        private ArrayList randomSequence(int len)
        {
            ArrayList array = new ArrayList();
            for (int i = 0; i < len; i++)
            {
                array.Add(i);
            }
            System.Random rd = new System.Random();
            ArrayList result = new ArrayList();
            while (len > 0)
            {
                int rdnum = rd.Next(0, len);
                int target = (int)array[rdnum];
                array.Remove(target);
                len--;
                result.Add(target);
            }
            return result;
        }*/

        private string getSequenceString(ArrayList list)
        {
            string res = "";

            for (int i = 0; i < list.Count; i++)
            {
                res += list[i] + ";";
            }
            return res;
        }
        private string getSequenceString(float[] seq, ArrayList list)
        {
            string res = ""; ;
            for (int i = 0; i < list.Count; i++)
            {
                res += list[i] + "-" + seq[(int)list[i]] + "; ";
            }
            return res;
        }


    }

    public struct BlockCondition
    {
        string prefix;
        int blockid;
        Lab1_move_28.Posture posture;
        Lab1_move_28.Orientation orientation;
        Lab1_move_28.Shape shape;
        float angle;

        public BlockCondition(int bid, int p, int o, int a, int s)
        {
            blockid = bid;
            prefix = string.Format("B{0:D2}", bid);
            posture = (Lab1_move_28.Posture)p;
            orientation = (Lab1_move_28.Orientation)o;
            shape = (Lab1_move_28.Shape)s;
            angle = Lab1_move_28.AngleOfScreens[a];
        }

        public int getBlockid()
        {
            return blockid;
        }

        public Lab1_move_28.Posture getPosture()
        {
            return posture;
        }

        public Lab1_move_28.Orientation getOrientation()
        {
            return orientation;
        }

        public float getAngle()
        {
            return angle;
        }

        public Lab1_move_28.Shape getShape()
        {
            return shape;
        }

        public string getAllDataForSceneDisplay()
        {
            string str;
            str = "Posture: " + posture.ToString() + Environment.NewLine
                + "Orientation: " + orientation.ToString() + Environment.NewLine
                + "Shape: " + shape.ToString() + Environment.NewLine
                + "Angle: " + angle.ToString() + "°"
                ;
            return str;
        }

        public string getAllDataForFile()
        {
            string str;
            str = prefix + ";"
                + posture.ToString() + ";"
                + orientation.ToString() + ";"
                + shape.ToString() + ";"
                + angle.ToString() + ";"
                ;
            return str;
        }
    }
}
