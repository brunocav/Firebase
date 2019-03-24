using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecebendoDados : MonoBehaviour
{
    public static RecebendoDados Instance;

    public  Text PlayerName;
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
      
    }
}

