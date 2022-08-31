using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MessageId;
using Google.Protobuf;
using PlayerMsg;
using UnityEngine.EventSystems;

public class CtrlHuman : BaseHuman
{
    // Use this for initialization
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
        if (CheckGuiRaycastObjects()) return;
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);
            if (hit.collider.tag == "Terrain")
            {
                MoveTo(hit.point);
                pos_info pos_Info = new pos_info()
                {
                    X = hit.point.x,
                    Y = hit.point.z
                };
                C2GS_PlayerMove c2GS_PlayerMove = new C2GS_PlayerMove()
                {
                    Pos = pos_Info,
                };
                byte[] msgBytes = c2GS_PlayerMove.ToByteArray();
                NetManager.Send(MessageId.MessageId.C2GsPlayerMove, msgBytes);
            }
        }
    }
    bool CheckGuiRaycastObjects()
    {
        // PointerEventData eventData = new PointerEventData(Main.Instance.eventSystem);

        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.pressPosition = Input.mousePosition;
        eventData.position = Input.mousePosition;

        List<RaycastResult> list = new List<RaycastResult>();
        // Main.Instance.graphicRaycaster.Raycast(eventData, list);
        EventSystem.current.RaycastAll(eventData, list);
        //Debug.Log(list.Count);
        return list.Count > 0;
    }

}
