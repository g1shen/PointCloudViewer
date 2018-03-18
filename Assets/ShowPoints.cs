using UnityEngine;
using System.Collections;
using System.IO;

public class ShowPoints : MonoBehaviour
{
    int numPoints = 60000;
    void Start()
    {
        // read data
        // store the point cloud in Assert/StreamingAssets, each line contains x，y，z，r，g，b  (float)
        string fileAddress = (Application.streamingAssetsPath + "/" + "pointsData.txt");
        FileInfo f0 = new FileInfo(fileAddress);

        string s = "";
        StreamReader reader;
        ArrayList arrayXYZ = new ArrayList();
        ArrayList arrayRGB = new ArrayList();

        if (f0.Exists)
        {
            FileStream temp = new FileStream(fileAddress, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            reader = new StreamReader(temp);
        }
        else
        {
            Debug.Log("File Not Exist!");
            return;
        }
        //store points in file to the two arrays
        while ((s = reader.ReadLine()) != null)
        {
            string[] words = s.Split(' '); //assume each number in the file is separated by ' '
            Vector3 position = new Vector3(float.Parse(words[0]), -float.Parse(words[1]), float.Parse(words[2]));
            arrayXYZ.Add(position);
            Color color = new Color(float.Parse(words[3]) / 255.0f, float.Parse(words[4]) / 255.0f, float.Parse(words[5]) / 255.0f);
            arrayRGB.Add(color);
        }
        reader.Close();
        // draw
        int num = arrayXYZ.Count;

        int meshs = num / numPoints; //how many mesh required
        int pointsleft = num % numPoints; //left out points
        int i = 0;

        for (; i < meshs; i++)
        {
            GameObject obj = new GameObject();
            obj.name = i.ToString();
            obj.AddComponent<MeshFilter>();
            obj.AddComponent<MeshRenderer>();
            Mesh tempMesh = new Mesh();
            SetMesh(ref tempMesh, ref arrayXYZ, ref arrayRGB, i * numPoints, numPoints);
            Material material = new Material(Shader.Find("Custom/VertexColor"));
            obj.GetComponent<MeshFilter>().mesh = tempMesh;
            obj.GetComponent<MeshRenderer>().material = material;
            obj.transform.localScale += new Vector3(10, 10, 10);
        }
        GameObject ThingsLeft = new GameObject();
        ThingsLeft.name = i.ToString();
        ThingsLeft.AddComponent<MeshFilter>();
        ThingsLeft.AddComponent<MeshRenderer>();
        Mesh tempMeshLeft = new Mesh();
        SetMesh(ref tempMeshLeft, ref arrayXYZ, ref arrayRGB, i * numPoints, pointsleft);
        Material materialLeft = new Material(Shader.Find("Custom/VertexColor"));
        ThingsLeft.GetComponent<MeshFilter>().mesh = tempMeshLeft;
        ThingsLeft.GetComponent<MeshRenderer>().material = materialLeft;
        ThingsLeft.transform.localScale += new Vector3(10, 10, 10);
        
    }

    void SetMesh(ref Mesh mesh, ref ArrayList arrayListXYZ, ref ArrayList arrayListRGB, int beginIndex, int pointsNum)
    {
        Vector3[] points = new Vector3[pointsNum];
        Color[] colors = new Color[pointsNum];
        int[] indecies = new int[pointsNum];
        for (int i = 0; i < pointsNum; ++i)
        {
            points[i] = (Vector3)arrayListXYZ[beginIndex + i];
            indecies[i] = i;
            colors[i] = (Color)arrayListRGB[beginIndex + i];
        }

        mesh.vertices = points;
        mesh.colors = colors;
        mesh.SetIndices(indecies, MeshTopology.Points, 0);

    }
}

