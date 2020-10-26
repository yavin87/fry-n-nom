using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadScoreScript : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Text>().text = StaticScoreClass.score.ToString();
    }
}
