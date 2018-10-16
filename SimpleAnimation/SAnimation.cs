using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;


public class SAnimation
{
    public SPlayDesc PlayInfo { get; private set; }
    public float Weight { get; private set; }
    public bool IsValid { get { return PlayInfo.IsValid; } }

    private AnimationClipPlayable m_Playable;



    public void Play(AnimationClipPlayable playable, SPlayDesc info)
    {
        PlayInfo = info;
        m_Playable = playable;

        if (PlayInfo.blendinTime == 0)
            Weight = 1;
        else
            Weight = 0;
    }

    public void Clear()
    {
        if (IsValid == false)
            return;

        if(m_Playable.IsValid())
        {
            m_Playable.GetGraph().DestroyPlayable(m_Playable);
            m_Playable = new AnimationClipPlayable { };
        }

        PlayInfo = new SPlayDesc { };
        Weight = 0;
    }

    public void Update()
    {
        if (PlayInfo.clip == null)
            return;

        UpdateBlendIn();

        if (PlayInfo.WrapMode == SPlayDesc.eWRAP_MODE.ONCE)
        {
            float currentTime = (float)m_Playable.GetTime();
            if (currentTime >= PlayInfo.lifeTime)
            {
                Clear();
                return;
            }

            if (PlayInfo.blendoutTime != 0)
            {
                float dt = currentTime - PlayInfo.blendOutStartTime;
                if (dt > 0)
                    Weight = 1 - dt / PlayInfo.blendoutTime;
            }
        }
    }

    private void UpdateBlendIn()
    {
        if (PlayInfo.blendinTime == 0 || Weight == 1)
            return;

        float currentTime = (float)m_Playable.GetTime();
        if (currentTime < PlayInfo.blendinTime)
            Weight = currentTime / PlayInfo.blendinTime;
        else
            Weight = 1;
    }
    
    public float GetTime()
    {
        return (float)m_Playable.GetTime();
    }

    public void SetTime(float time)
    {
        m_Playable.SetTime(time);
    }

    public void SetSpeed(float speed)
    {
        m_Playable.SetSpeed(speed);
    }
}