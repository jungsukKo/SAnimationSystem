using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;





[RequireComponent(typeof(Animator))]
public class SAnimationSystem : MonoBehaviour
{
    public class SAnimationBehaviour : PlayableBehaviour
    {
        public SAnimationSystem m_System;

        override public void PrepareFrame(Playable owner, FrameData info)
        {
            for (int i = 0; i < m_System.m_LayerCount; i++)
            {
                m_System.m_Layer[i].Update();
                m_System.m_LayerMixer.SetInputWeight(i, m_System.m_Layer[i].Weight);
            }
        }
    }




    [System.Serializable]
    public class EditorState
    {
        public AnimationClip clip;
        public string state;
    }

    [SerializeField]
    private int m_StartAnimationIndex = 0;
    [SerializeField]
    private AnimatorCullingMode m_CullingMode = AnimatorCullingMode.CullUpdateTransforms;
    [SerializeField]
    private AnimatorUpdateMode m_AnimatorUpdateMode = AnimatorUpdateMode.Normal;
    [SerializeField]
    [Range(1, 4)]
    private int m_LayerCount = 2;
    [SerializeField]
    private EditorState[] m_States;


    private SAnimationLayer[] m_Layer;
    private AnimationLayerMixerPlayable m_LayerMixer;


    private void Awake()
    {
        Animator animator = GetComponent<Animator>();
        animator.updateMode = m_AnimatorUpdateMode;
        animator.cullingMode = m_CullingMode;
        
        // 1) set behaviour to control updating
        var script = ScriptPlayable<SAnimationBehaviour>.Create(animator.playableGraph, 1);
        script.GetBehaviour().m_System = this;
        animator.playableGraph.GetOutput(0).SetSourcePlayable(script);

        // 2) set layer mixer
        m_LayerMixer = AnimationLayerMixerPlayable.Create(animator.playableGraph, 2);
        animator.playableGraph.Connect(m_LayerMixer, 0, script, 0);

        // 3) create layers 
        m_Layer = new SAnimationLayer[m_LayerCount];
        for (int i = 0; i < m_LayerCount; i++)
            m_Layer[i] = new SAnimationLayer(animator.playableGraph, m_LayerMixer, i);
    }

    private void Start()
    {
        if (m_States.Length > m_StartAnimationIndex)
            Play(m_States[m_StartAnimationIndex].state);
    }
    
    public SAnimation this[uint layer]
    {
        get { return m_Layer[layer].GetAnimation(); }
    }

    public AnimationClip GetClip(string state)
    {
        foreach (EditorState s in m_States)
        {
            if (s.state == state)
                return s.clip;
        }
        Debug.LogError("Can't find state");
        return null;
    }

    public void Play(string state, uint layer = 0, float _blendinTime = 0, float _blendoutTime = 0, SAnimation.eWRAP_MODE t = SAnimation.eWRAP_MODE.AUTO)
    {
        m_Layer[layer].Play(GetClip(state), _blendinTime, _blendoutTime, t);
    }

    public void CrossFade(string state, float _blendinTime, float _blendoutTime = 0, uint layer = 0, SAnimation.eWRAP_MODE t = SAnimation.eWRAP_MODE.AUTO)
    {
        m_Layer[layer].CrossFade(GetClip(state), _blendinTime, _blendoutTime, t);
    }

    public void Stop(uint layer = 999)
    {
        if (layer == 999)// stop all layers
        {
            for (int i = 0; i < m_LayerCount; i++)
                m_Layer[i].Stop();
        }
        else
        {
            m_Layer[layer].Stop();
        }
    }

    public void SetAddive(uint layer, bool value)
    {
        m_LayerMixer.SetLayerAdditive(layer, value);
    }
}
