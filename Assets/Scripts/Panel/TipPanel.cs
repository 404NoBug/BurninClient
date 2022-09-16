using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TipPanel : BasePanel
{
    //tip提示内容
    private TMP_Text _tipInput;
    //关闭按钮
    private Button _okBtn;
    //初始化
    public override void OnInit()
    {
        skinPath = "TipPanel";
        layer = PanelManager.Layer.Tip;
    }
    //显示
    public override void OnShow(params object[] para)
    {
        //寻找组件
        _tipInput = skin.transform.Find("Text").GetComponent<TMP_Text>();
        _okBtn = skin.transform.Find("OkBtn").GetComponent<Button>();
        //添加按钮监听
        _okBtn.onClick.AddListener(OnCloseClick);
        Debug.Log("OnShow = " + para.Length);
        //提示语言
        if (para.Length == 1)
        {
            _tipInput.text = para[0].ToString();
        }
    }
    //关闭
    public override void OnClose()
    {
        
    }
    public void OnCloseClick()
    {
        Close();
    }
}
