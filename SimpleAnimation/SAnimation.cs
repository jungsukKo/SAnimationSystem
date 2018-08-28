using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;


public class SAnimation
{
    public enum eWRAP_MODE
    {
        AUTO, // follow clip setting
        LOOP,
        ONCE,
        FREEZE_AT_LAST
    }

    public PlayableGraph graph;

    private float m_BlendInTime;
    private float m_BlendOutTime;
    private float m_BlendOutStartTime;
    private AnimationClip m_Clip;
    private AnimationClipPlayable m_Playable;
    public eWRAP_MODE m_WrapMode;
    public float Weight { get; private set; }


    public bool IsValid { get { return m_Clip != null; } }
    public AnimationClip Clip { get { return m_Clip; } }

    public void Play(AnimationClipPlayable playable, float _blendinTime, float _blendoutTime, eWRAP_MODE t)
    {
        m_Clip = playable.GetAnimationClip();
        m_Playable = playable;
        m_BlendInTime = _blendinTime;
        m_BlendOutTime = _blendoutTime;
        m_BlendOutStartTime = Math.Max(0, m_Clip.length - m_BlendOutTime);

        if (t == eWRAP_MODE.AUTO)
            m_WrapMode = Clip.isLooping ? SAnimation.eWRAP_MODE.LOOP : SAnimation.eWRAP_MODE.ONCE;
        else
            m_WrapMode = t;

        if (m_BlendInTime == 0)
            Weight = 1;
        else
            Weight = 0;
    }

    public void Clear()
    {
        if (IsValid == false)
            return;

        graph.DestroyPlayable(m_Playable);
        m_BlendInTime = 0;
        m_BlendOutTime = 0;
        m_BlendOutStartTime = 0;
        m_Clip = null;
        Weight = 0;
    }

    public void Update()
    {
        if (m_Clip == null)
            return;

        UpdateBlendIn();

        switch (m_WrapMode)
        {
            case eWRAP_MODE.ONCE:
                if (m_Playable.GetTime() >= m_Clip.length)
                    Clear();
                else
                    UpdateBlendOut();
                break;

            case eWRAP_MODE.FREEZE_AT_LAST:
                if (m_Playable.GetTime() >= m_Clip.length)
                    m_Playable.Pause();
                break;
        }
    }

    private void UpdateBlendIn()
    {
        if (m_BlendInTime == 0 || Weight == 1)
            return;

        float currentTime = (float)m_Playable.GetTime();
        if (currentTime < m_BlendInTime)
            Weight = currentTime / m_BlendInTime;
        else
            Weight = 1;
    }

    private void UpdateBlendOut()
    {
        if (m_BlendOutTime == 0)
            return;

        float currentTime = (float)m_Playable.GetTime();
        float passedTimeFromBlendOut = currentTime - m_BlendOutStartTime;
        if (passedTimeFromBlendOut < 0)
            return;

        Weight = 1 - passedTimeFromBlendOut / m_BlendOutTime;
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