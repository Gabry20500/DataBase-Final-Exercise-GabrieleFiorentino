using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogInError : MonoBehaviour
{
    public GameObject registerPanel;
    public GameObject loginPanel;
    
    public void Yes()
    {
        this.gameObject.SetActive(false);
        loginPanel.SetActive(false);
        registerPanel.SetActive(true);
    }
    
    public void No()
    {
        this.gameObject.SetActive(false);
    }
}
