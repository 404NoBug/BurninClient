using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class TimerNode
{
    public TimerMgr.TimerHandler callback;
    public float duration; //定时器触发的时间间隔
    public float delay; //第一次触发要个多少时间
    public int repeat; //触发次数
    public float passedTime; //这个Timer过去的时间
    public object param; //用户要穿的参数
    public int timerID; //表示这个timer的唯一id号
    public bool isRemove; //是否删除了
}

public class TimerMgr : MonoBehaviour
{
    public static TimerMgr Instance = null;

    public delegate void TimerHandler(object param);

    private int autoIncId = 1;
    private Dictionary<int, TimerNode> timers = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else //全局唯一的单例
        {
            Destroy(this);
            return;
        }
        this.Init();
    }

    //初始化入口
    void Init()
    {
        timers = new Dictionary<int, TimerNode>();
        this.autoIncId = 1;
    }
    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;
        foreach (TimerNode timer in this.timers.Values)
        {
            if (timer.isRemove)
            {
                continue;
            }
            timer.passedTime += dt;
            if (timer.passedTime >= timer.delay + timer.duration)
            {
                //做一次触发
                timer.callback(timer.param);
                timer.repeat--;
                timer.passedTime -= (timer.delay + timer.duration);
                timer.delay = 0;
                if (timer.repeat == 0)
                { //触发次数结束，删除定时器
                    // this.timers.Remove(timer.timerID);
                    timer.isRemove = true;
                }
            }
        }
    }
    public int ScheduleOnce(TimerHandler func, float delay)
    {
        return this.ScheduleOnce(func, null, delay);
    }
    public int ScheduleOnce(TimerHandler func, object param, float delay)
    {
        return this.Schedule(func, param, 1, 0, delay);
    }
    public int Schedule(TimerHandler func, int repeat, float duration, float delay = 0.0f)
    {
        return this.Schedule(func, null, repeat, duration, delay);
    }
    /// <summary>
    /// 定时器
    /// </summary>
    /// <param name="func"></param> 回调函数
    /// <param name="param"></param> 参数
    /// <param name="repeat"></param> 触发次数[repeat == 0 or repeat < 0 表示无限触发]
    /// <param name="duration"></param> 定时器触发的时间间隔
    /// <param name="delay"></param> 第一次触发要个多少时间
    /// <returns></returns>
    public int Schedule(TimerHandler func, object param, int repeat, float duration, float delay = 0.0f)
    {
        TimerNode timer = new TimerNode();
        timer.callback = func;
        timer.param = param;
        timer.repeat = repeat;
        timer.duration = duration;
        timer.delay = delay;
        timer.passedTime = timer.duration;
        timer.isRemove = false;
        timer.timerID = this.autoIncId;
        this.timers.Add(timer.timerID,timer);
        this.addAutoIncId();
        return timer.timerID;
    }
    private void addAutoIncId()
    {
        this.autoIncId++;
    }
}
