using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurninTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //界面
        PanelManager.Init();
        PanelManager.Open<LoginPanel>();
    }
}
