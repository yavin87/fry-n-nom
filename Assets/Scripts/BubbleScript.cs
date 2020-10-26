using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleScript : MonoBehaviour
{
    void OnEnable()
    {
        CancelInvoke();
        Invoke("disableObject", 3);
    }

    private void disableObject()
    {
        this.gameObject.SetActive(false);
    }
}
