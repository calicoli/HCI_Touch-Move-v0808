using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class FileProcessor : MonoBehaviour
{
    //private string strWriteData;

    // Start is called before the first frame update
    void Start()
    {
        /*
        string filename = "filtProcessor.txt";
        string originText = readStringFromFile(filename);
        string writeText = originText + "\r\n" + originText + "\r\n";
        writeStringToFile(writeText, filename);
        string readText = readStringFromFile(filename);
        Debug.Log(readText);*/
        /*
        strWriteData = null;
        */
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void writeStringToFile(string str, string filename)
    {
#if !WEB_BUILD
        string path = pathForDocumentsFile(filename);
        FileStream file;
        if (File.Exists(path))
        {
            file = new FileStream(path, FileMode.Append, FileAccess.Write);
        } else
        {
            file = new FileStream(path, FileMode.Create, FileAccess.Write);
        }

        StreamWriter sw = new StreamWriter(file);
        sw.WriteLine(str);

        sw.Close();
        file.Close();
#endif
    }


    public string readStringFromFile(string filename)//, int lineIndex )
    {
#if !WEB_BUILD
        string path = pathForDocumentsFile(filename);

        if (File.Exists(path))
        {
            FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(file);

            string str = null;
            str = sr.ReadLine();

            sr.Close();
            file.Close();

            return str;
        }

        else
        {
            return null;
        }
#else
return null;
#endif
    }


    public string pathForDocumentsFile(string filename)
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            string path = Application.dataPath.Substring(0, Application.dataPath.Length - 5);
            path = path.Substring(0, path.LastIndexOf('/'));
            return Path.Combine(Path.Combine(path, "Documents"), filename);
        }

        else if (Application.platform == RuntimePlatform.Android)
        {
            string path = Application.persistentDataPath;
            path = path.Substring(0, path.LastIndexOf('/'));
            return Path.Combine(path, filename);
        }

        else
        {
            string path = Application.dataPath;
            path = path.Substring(0, path.LastIndexOf('/'));
            return Path.Combine(path, filename);
        }
    }

    public void setWriteData(string str)
    {
        //strWriteData = str;
    }

    public void writeNewDataToFile(string filename, string strContent)
    {
        string writeText = strContent;
        writeStringToFile(writeText, filename);
        string readText = readStringFromFile(filename);
        //Debug.Log(readText);
    }

    public void writeNewDataToFile(string filename, string strContent, out bool writeFinished)
    {
        //string originText = readStringFromFile(filename);
        //string writeText = originText + "\r\n" + strContent + "\r\n";
        string writeText = strContent;
        writeStringToFile(writeText, filename);
        string readText = readStringFromFile(filename);
        //Debug.Log(readText);

        writeFinished = true;
    }
}
