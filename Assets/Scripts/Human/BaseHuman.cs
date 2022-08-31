using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;
using PlayerMsg;

public class BaseHuman : MonoBehaviour
{
    //�Ƿ�����ƶ�
    protected bool isMoving = false;
    //�ƶ�Ŀ���
    private Vector3 targetPostion;
    //�ƶ��ٶ�
    public float speed = 1.2f;
    //�������
    private Animator animator;
    //����
    public string desc = "";
    //�ƶ���ĳ��
    public void MoveTo(Vector3 pos)
    {
        targetPostion = pos;
        isMoving = true;
        animator.SetBool("isMoving", true);
    }
    //�ƶ�Update
    public void MoveUpdate(string desc)
    {
        if (isMoving == false)
        {
            return;
        }
        Vector3 pos = transform.position;
        transform.position = Vector3.MoveTowards(pos, targetPostion, speed * Time.deltaTime);
        transform.LookAt(targetPostion);
        if (Vector3.Distance(pos, targetPostion) < 0.05f)
        {
            Vector3 eul = transform.eulerAngles;
            C2GS_PlayerStopMove c2GS_PlayerStopMove = new C2GS_PlayerStopMove(){
                Dir = eul.y
            };
            byte[] msgBytes = c2GS_PlayerStopMove.ToByteArray();
            NetManager.Send(MessageId.MessageId.C2GsPlayerStopMove, msgBytes);
            isMoving = false;
            animator.SetBool("isMoving", false);
        }
    }

    public void StopMove(float dir)
    {
        this.isMoving = false;
        transform.eulerAngles = new Vector3(0, dir, 0);
        this.animator.SetBool("isMoving", false);
    }
    // Start is called before the first frame update
    public void Start()
    {
        animator = GetComponent<Animator>(); 
        animator.SetBool("isMoving", false);
    }

    // Update is called once per frame
    public void Update()
    {
        MoveUpdate(this.desc);
    }
}
