using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCounter : Singletion<FPSCounter>
{
    public const int MAX_FRAME_RATE = 30;

    private float m_LastUpdateShowTime = 0f;    //上一次更新帧率的时间;
    private float m_UpdateShowDeltaTime = 0.01f;//更新帧率的时间间隔;
    private int m_FrameUpdate = 0;//帧数;
    private float m_FPS = 0;

    private bool iStart = false;

    public void Init()
    {
        iStart = false;
        Application.targetFrameRate = MAX_FRAME_RATE;
        m_LastUpdateShowTime = Time.realtimeSinceStartup;
    }

    public void StartCounting()
    {
        iStart = true;
        StartCoroutine(IUpdate());
    }

    public void StopCounting()
    {
        iStart = false;
        StopCoroutine(IUpdate());
    }
    IEnumerator IUpdate()
    {
        while (iStart)
        {
            m_FrameUpdate++;
            if (Time.realtimeSinceStartup - m_LastUpdateShowTime >= m_UpdateShowDeltaTime)
            {
                m_FPS = m_FrameUpdate / (Time.realtimeSinceStartup - m_LastUpdateShowTime);
                m_FrameUpdate = 0;
                m_LastUpdateShowTime = Time.realtimeSinceStartup;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    void OnGUI()
    {
        if(iStart)
            GUI.Label(new Rect(Screen.width / 2, 0, 100, 100), "FPS: " + m_FPS);
    }
}
