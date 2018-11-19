using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;
using Unity.Collections;
using UnityEngine.Experimental.Animations;



public class SMixer
{
    public AnimationScriptPlayable m_Mixer { get; protected set; }
    protected NativeArray<TransformStreamHandle> m_Handles;

    public SMixer()
    {
    }

    public SMixer(Animator animator, PlayableGraph graph)
    {
        InitHandle(animator);

        SMixerJob _Job = new SMixerJob() { handles = m_Handles, ApplyRootMotion = animator.applyRootMotion };
        m_Mixer = AnimationScriptPlayable.Create(graph, _Job, 2);
        m_Mixer.SetInputWeight(0, 1);
        m_Mixer.SetInputWeight(1, 1);
        m_Mixer.SetProcessInputs(false);
    }
    
    protected Transform[] InitHandle(Animator animator)
    {
        // Get all the transforms in the hierarchy.
        Transform[] boneList = animator.transform.GetComponentsInChildren<Transform>();
        var numTransforms = boneList.Length - 1;

        // Fill native arrays (all the bones have a weight of 1.0).
        m_Handles = new NativeArray<TransformStreamHandle>(numTransforms, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
        for (var i = 0; i < numTransforms; ++i)
            m_Handles[i] = animator.BindStreamTransform(boneList[i + 1]);

        return boneList;
    }

    public virtual void Dispose()
    {
        m_Handles.Dispose();
    }
    
    public virtual void SetWeight(float w0, float w1)
    {
        var job = m_Mixer.GetJobData<SMixerJob>();
        job.weightA = w0;
        job.weightB = w1;
        m_Mixer.SetJobData(job);
    }

    public void OverrideInputBtoA()
    {
        // reconnect to input 0
        Playable output1 = m_Mixer.GetInput(1);
        m_Mixer.DisconnectInput(1);
        m_Mixer.GetGraph().Connect(output1, 0, m_Mixer, 0);
    }

    public void Connect(Playable p, int inputIndex)
    {
        m_Mixer.GetGraph().Connect(p, 0, m_Mixer, inputIndex);
    }
}













public struct SMixerJob : IAnimationJob
{
    public NativeArray<TransformStreamHandle> handles;

    public float weightA;
    public float weightB;
    public bool ApplyRootMotion;

    public void ProcessRootMotion(AnimationStream stream)
    {
        if (ApplyRootMotion)
            SMixerUtility.ProcessRootMotion(stream, weightA, weightB);
    }


    public void ProcessAnimation(AnimationStream stream)
    {
        if (weightA == 0 && weightB == 0)
            return;

        var A = stream.GetInputStream(0);
        var B = stream.GetInputStream(1);

        if (weightB == 0)
        {
            if (weightA == 1)
                SMixerUtility.StreamSet(handles, stream, A);
            else
                SMixerUtility.StreamMix(handles, stream, stream, A, weightA);   // blend-in transition to A from stream
        }
        else
        {
            if (weightA == 1)
                SMixerUtility.StreamMix(handles, stream, A, B, weightB); // blend-in transition to B from A
            else
                SMixerUtility.StreamMixMix(handles, stream, stream, A, B, weightA, weightB);
        }
    }
}