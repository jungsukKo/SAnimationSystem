using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;


public class SAnimationLayer
{
    private SAnimation m_A;
    private SAnimation m_B;
    private SMixer m_MixerManager;


    public float Weight { get { return Math.Max(m_A.Weight, m_B.Weight); } }
    public bool IsPlaying { get { return m_A.IsValid || m_B.IsValid; } }



    public SAnimationLayer(Animator animator, Playable output, int layerIndex)
    {
        PlayableGraph graph = output.GetGraph();
        m_MixerManager = new SMixer(animator, graph);
        graph.Connect(m_MixerManager.m_Mixer, 0, output, layerIndex);

        m_A = new SAnimation();
        m_B = new SAnimation();
    }

    public void Stop()
    {
        m_A.Clear();
        m_B.Clear();
    }

    private void Create(SAnimation targetSoket, SPlayDesc info)
    {
        if (targetSoket.IsValid)
            targetSoket.Clear();

        AnimationClipPlayable p = AnimationClipPlayable.Create(m_MixerManager.m_Mixer.GetGraph(), info.clip);
        if(targetSoket == m_A)
            m_MixerManager.Connect(p, 0);
        else
            m_MixerManager.Connect(p, 1);

        targetSoket.Play(p, info);
    }

    public void Play(SPlayDesc info)
    {
        if (m_A.IsValid && info.blendinTime > 0)
        { 
            // crossfade current animation
            Create(m_B, info); 
        }
        else
        {
            m_B.Clear();
            Create(m_A, info);
        }
    }

    public void Update()
    {
        m_A.Update();

        if (m_B.IsValid)
        {
            m_B.Update();
            if (m_B.Weight == 1)
            {
                if (m_A.IsValid)
                    m_A.Clear();
                OverrideBtoA();
            }
        }

        m_MixerManager.SetWeight(m_A.Weight, m_B.Weight);
    }
    
    private void OverrideBtoA()
    {
        // swap A and B
        SAnimation temp = m_A;
        m_A = m_B;
        m_B = temp;

        m_MixerManager.OverrideInputBtoA();
    }

    public SAnimation GetAnimation()
    {
        if (m_B.IsValid)
            return m_B;
        return m_A;
    }

    public void Dispose()
    {
        m_MixerManager.Dispose();
    }
}