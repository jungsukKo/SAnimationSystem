using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;
using Unity.Collections;
using UnityEngine.Experimental.Animations;



public struct SPlayDesc
{
    public enum eWRAP_MODE
    {
        AUTO,   // follow clip setting
        LOOP,
        ONCE,
        FREEZE_AT_LAST  // don't kill animation
    }


    public float blendinTime;
    public float blendoutTime;
    public eWRAP_MODE WrapMode;
    public AnimationClip clip;
    public bool blendOutAfterEnd;
    
    public bool IsValid { get { return clip != null; } }
    public float length { get { return clip.length; } }
    public float lifeTime
    {
        get
        {
            if (blendOutAfterEnd)
                return length + blendoutTime;
            else
                return length;
        }
    }

    public float blendOutStartTime
    {
        get {
            if (blendOutAfterEnd)
                return length;
            else
                return Math.Max(0, length - blendoutTime);
        }
    }

    public SPlayDesc(AnimationClip _clip, float _blendinTime, SPlayDesc.eWRAP_MODE wrap)
    {
        blendinTime = _blendinTime;
        blendoutTime = 0;
        blendOutAfterEnd = false;
        clip = _clip;
        
        WrapMode = wrap;
        if (wrap == SPlayDesc.eWRAP_MODE.AUTO)
            WrapMode = _clip.isLooping ? SPlayDesc.eWRAP_MODE.LOOP : SPlayDesc.eWRAP_MODE.ONCE;
    }

    public SPlayDesc(AnimationClip _clip, float _blendinTime, float _blendoutTime, bool _blendOutAfterEnd)
    {
        blendinTime = _blendinTime;
        blendoutTime = _blendoutTime;
        blendOutAfterEnd = _blendOutAfterEnd;
        clip = _clip;

        WrapMode = eWRAP_MODE.ONCE;
    }
}










/*
 * this component is giving example how to control animation and make custom system for animation
 * 
 * this simple animation system has two layers
 * 1st layer is for base motion like "idle", "walk", which is looping in common case
 * 2nd layer is for action motion like "attack", hit", it is normally one time play with blend-in and out
 * 2nd layer support masking so that part of hierarchy play action ( ex: lower part is running but upper part play hit motion )
 */
[RequireComponent(typeof(Animator))]
public class SAnimationSystem : MonoBehaviour
{
    [Serializable]
    public class EditorState
    {
        public AnimationClip clip;
        public string state;
    }

    [Serializable]
    public class LayerMaskBone
    {
        public Transform transform;
        [Range(0.0f, 1.0f)]
        public float weight;
    }

    [SerializeField]
    private int m_StartAnimationIndex = 0;
    [SerializeField]
    private AnimatorCullingMode m_CullingMode = AnimatorCullingMode.CullUpdateTransforms;
    [SerializeField]
    private AnimatorUpdateMode m_AnimatorUpdateMode = AnimatorUpdateMode.Normal;
    [SerializeField]
    private EditorState[] m_States;


    private SAnimationLayer[] m_Layer;
    private SLayerMixer m_LayerManager;
    private PlayableGraph m_Graph;

    private const int m_LayerCount = 2;


    private void Awake()
    {
        Animator animator = GetComponent<Animator>();
        animator.updateMode = m_AnimatorUpdateMode;
        animator.cullingMode = m_CullingMode;

#if !UNITY_2018_2_OR_NEWER
        Debug.LogError("Only support 2018.2 or newer!");
#else
#if UNITY_2018_2
        m_Graph = animator.playableGraph;
#else
        m_Graph = PlayableGraph.Create(this.name);
        AnimationPlayableOutput.Create(m_Graph, "ani", animator);
        m_Graph.Play();
#endif
#endif
        m_LayerManager = new SLayerMixer(animator, m_Graph);
        m_Graph.GetOutput(0).SetSourcePlayable(m_LayerManager.m_Mixer);

        m_Layer = new SAnimationLayer[m_LayerCount];
        for (int i = 0; i < m_LayerCount; i++)
            m_Layer[i] = new SAnimationLayer(animator, m_LayerManager.m_Mixer, i);
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

    public void Play(string state, uint layer = 0, float _blendinTime = 0, SPlayDesc.eWRAP_MODE wrap = SPlayDesc.eWRAP_MODE.AUTO)
    {
        AnimationClip _clip = GetClip(state);
        if (_clip == null)
            return;

        SPlayDesc info = new SPlayDesc(_clip, _blendinTime, wrap);
        m_Layer[layer].Play(info);
    }

    public void PlayBlendOut(string state, uint layer = 0, float _blendinTime = 0, float _blendoutTime = 0, bool _blendOutAfterEnd = true)
    {
        AnimationClip _clip = GetClip(state);
        if (_clip == null)
            return;

        SPlayDesc info = new SPlayDesc(_clip, _blendinTime, _blendoutTime, _blendOutAfterEnd);
        m_Layer[layer].Play(info);
    }
    
    public void SetBlendingMask(Transform boneMask, float weight)
    {
        m_LayerManager.SetBlendingMask(boneMask, weight);
    }

    public void ClearBlendingMask()
    {
        m_LayerManager.ClearBlendingMask();
    }

    const int STOP_ALL = 999999;
    public void Stop(uint layer = STOP_ALL)
    {
        if (layer == STOP_ALL)
        {
            for (int i = 0; i < m_LayerCount; i++)
                m_Layer[i].Stop();
        }
        else
        {
            m_Layer[layer].Stop();
        }
    }

    public void Update()
    {
        foreach (SAnimationLayer l in m_Layer)
            l.Update();

        m_LayerManager.SetWeight(m_Layer[0].Weight, m_Layer[1].Weight);
    }

    private void Dispose()
    {
        foreach (SAnimationLayer l in m_Layer)
            l.Dispose();

        m_LayerManager.Dispose();
        m_Graph.Destroy();
    }

#if UNITY_EDITOR
    public void OnDisable()
    {
        Dispose();
    }
#else
    public void OnDestroy()
    {
        Dispose();
    }
#endif
}
