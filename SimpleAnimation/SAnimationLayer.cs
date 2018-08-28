using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;



public class SAnimationLayer
{
    private PlayableGraph m_Graph;
    private AnimationLayerMixerPlayable m_CrossFadeMixer;
    private SAnimation m_A;
    private SAnimation m_B;

    public float Weight { get { return Math.Max(m_A.Weight, m_B.Weight); } }
    public bool IsPlaying { get { return m_A.IsValid || m_B.IsValid; } }

    public SAnimationLayer(PlayableGraph _graph, AnimationLayerMixerPlayable layerMixer, int layerIndex)
    {
        m_Graph = _graph;
        m_CrossFadeMixer = AnimationLayerMixerPlayable.Create(m_Graph, 2);
        m_Graph.Connect(m_CrossFadeMixer, 0, layerMixer, layerIndex);

        m_A = new SAnimation() { graph = _graph };
        m_B = new SAnimation() { graph = _graph };
    }

    public void Stop()
    {
        m_A.Clear();
        m_B.Clear();
    }

    private void Create(SAnimation targetSoket, AnimationClip clip, float _blendinTime, float _blendoutTime, SAnimation.eWRAP_MODE t)
    {
        if (targetSoket.IsValid)
            targetSoket.Clear();

        AnimationClipPlayable p = AnimationClipPlayable.Create(m_Graph, clip);
        if(targetSoket == m_A)
            m_Graph.Connect(p, 0, m_CrossFadeMixer, 0);
        else
            m_Graph.Connect(p, 0, m_CrossFadeMixer, 1);
        targetSoket.Play(p, _blendinTime, _blendoutTime, t);
    }

    public void Play(AnimationClip clip, float _blendinTime, float _blendoutTime, SAnimation.eWRAP_MODE t)
    {
        Stop();
        Create(m_A, clip, _blendinTime, _blendoutTime, t);
    }

    public void CrossFade(AnimationClip clip, float _blendinTime, float _blendoutTime, SAnimation.eWRAP_MODE t)
    {
        if( m_A.IsValid )
            Create(m_B, clip, _blendinTime, _blendoutTime, t);
        else
            Create(m_A, clip, _blendinTime, _blendoutTime, t); // noting to crossfade
    }

    public void Update()
    {
        m_A.Update();
        m_CrossFadeMixer.SetInputWeight(0, m_A.Weight);

        if( m_B.IsValid )
        {
            m_B.Update();
            m_CrossFadeMixer.SetInputWeight(1, m_B.Weight);

            if (m_B.Weight == 1 )
            {
                if( m_A.IsValid )
                    m_A.Clear();
                CrossFadeComplete();
            }
        }
    }

    private void CrossFadeComplete()
    {
        // swap A and B
        SAnimation temp = m_A;
        m_A = m_B;
        m_B = temp;

        // reconnect to input 0
        Playable output1 = m_CrossFadeMixer.GetInput(1);
        m_CrossFadeMixer.DisconnectInput(1);
        m_Graph.Connect(output1, 0, m_CrossFadeMixer, 0);
    }

    public SAnimation GetAnimation()
    {
        if (m_B.IsValid)
            return m_B;
        return m_A;
    }
}