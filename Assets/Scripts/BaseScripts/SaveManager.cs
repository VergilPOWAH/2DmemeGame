﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    #region Singltone
    public static SaveManager instance { get; private set; }
    void Start()
    {
        if (instance != null)
        {
            Debug.Log("Save manager alredy exists");
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    #endregion

    public Vector2 lastPosition { get; private set; }
    public int checkPoint { get; private set; }

    public void SaveAll(int pointNum)
    {
        SavePosition(pointNum);
    }

    public void SavePosition(int pointNum)
    {
        lastPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        checkPoint = pointNum;
    }
}
