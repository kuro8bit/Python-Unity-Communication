using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Paint : MonoBehaviour
{
    const int width = 28;
    const int height = 28;

    Color[] drawBuffer;
    Texture2D drawTexture;
    Vector2 prevPoint;
    bool drawStartPointFlag = true;
    new Renderer renderer;

    public Text text;

    private void Start()
    {
        drawBuffer = new Color[width * height];

        drawTexture = new Texture2D(width, height, TextureFormat.RGB24, false);
        drawTexture.filterMode = FilterMode.Point;

        renderer = GetComponent<Renderer>();

        ClearCanvas();
    }

    private void Update()
    {
        bool raycastResult = false;
        var mousePos = Vector2.zero;
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            raycastResult = Physics.Raycast(ray, out hit, 100.0f);
            if(raycastResult){
                mousePos = new Vector2(hit.textureCoord.x * drawTexture.width, hit.textureCoord.y * drawTexture.height);
            }
        }

        if (raycastResult)
        {
            if (drawStartPointFlag)
            {
                DrawPoint(mousePos, Color.black);
            }
            else
            {
                DrawLine(prevPoint, mousePos, Color.black);
            }

            drawStartPointFlag = false;
            prevPoint = mousePos;
        }
        else
        {
            drawStartPointFlag = true;
        }

        drawTexture.SetPixels(drawBuffer);
        drawTexture.Apply();
        renderer.material.mainTexture = drawTexture;

        Client.sendData = new float[784];
        for(int i=0; i<784; i++)
        {
            Client.sendData[i] = drawBuffer[i] == Color.black ? 1f : 0f;
        }

        DisplayPrediction();
    }

    public void ClearCanvas()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var drawPoint = new Vector2(x, y);
                DrawPoint(drawPoint, Color.white);
            }
        }
    }

    void DrawPoint(Vector2 point, Color color, double brushSize = 1f)
    {
        point.x = (int)point.x;
        point.y = (int)point.y;

        int start_x = Mathf.Max(0, (int)(point.x - (brushSize - 1)));
        int end_x = Mathf.Min(drawTexture.width, (int)(point.x + (brushSize + 1)));
        int start_y = Mathf.Max(0, (int)(point.y - (brushSize - 1)));
        int end_y = Mathf.Min(drawTexture.height, (int)(point.y + (brushSize + 1)));

        

        for (int x = start_x; x < end_x; x++)
        {
            for (int y = start_y; y < end_y; y++)
            {
                double length = Mathf.Sqrt(Mathf.Pow(point.x - x, 2) + Mathf.Pow(point.y - y, 2));
                if (length < brushSize)
                {
                    drawBuffer.SetValue(color, x + drawTexture.width * y);
                    
                }
            }
        }
    }

    void DrawLine(Vector2 point1, Vector2 point2, Color color, int lerpNum = 10)
    {
        for (int i = 0; i < lerpNum + 1; i++)
        {
            var point = Vector2.Lerp(point1, point2, i * (1.0f / lerpNum));
            DrawPoint(point, color);
        }
    }


    /* 
        Display Prediction, which display highest prediction in red highlight
    
    */
    void DisplayPrediction()
    {
        string str = "";
        int size = Client.recvData.Length - 1;

        for (int i = 0; i < size; i++)
        {
            str += (Client.recvData[size] == i) ?
                string.Format("<color=red>{0}: {1:0.0000}</color>", i, Client.recvData[i]) :
                string.Format("<color=white>{0}: {1:0.0000}</color>", i, Client.recvData[i]);
            if (i < size) str += "\n";
        }
        text.text = str;
    }
}
