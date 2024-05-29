using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageMove : MonoBehaviour
{
    public RectTransform image;
    public bool x = true, y = true;
    public float speed = 1f;
    public int widthTime = 100;

    private int counter = 0;
    private float move = 0.5f;

    private float tate = 0;
    private float yoko = 0;
    private void Start()
    {
        if(x)
        {
            tate = 1f;
        }
        else
        {
            tate = 0f;
        }
        if(y)

        {
            yoko = 1f;
        }
        else
        {
            yoko = 0f;
        }

        counter = widthTime / 2;
    }

    void Update()
    {
        image.position += new Vector3(tate * move * speed * Time.deltaTime, yoko * move * speed * Time.deltaTime, 0);
        counter++;
        if (counter >= widthTime)
        {
            counter = 0;
            move *= -1;
        }
    }
}