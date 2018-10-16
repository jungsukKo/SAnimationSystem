using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;
using Unity.Collections;
using UnityEngine.Experimental.Animations;



public class SLayerMixer : SMixer
{
    public NativeArray<float> m_BoneWeights;
    public Transform[] m_BoneList;

    public SLayerMixer(Animator animator, PlayableGraph graph)
        : base()
    {
        m_BoneList = InitHandle(animator);
        var numTransforms = m_BoneList.Length - 1;

        // Create job.
        SMixerLayerJob _Job = new SMixerLayerJob() { handles = m_Handles, ApplyRootMotion = animator.applyRootMotion };
        m_BoneWeights = new NativeArray<float>(numTransforms, Allocator.Persistent, NativeArrayOptions.ClearMemory);
        for (var i = 0; i < numTransforms; ++i)
            m_BoneWeights[i] = 1.0f;
        _Job.boneWeights = m_BoneWeights;

        m_Mixer = AnimationScriptPlayable.Create(graph, _Job, 2);
        m_Mixer.SetProcessInputs(false);
    }

    public override void Dispose()
    {
        base.Dispose();

        m_BoneList = null;
        if (m_BoneWeights.IsCreated)
            m_BoneWeights.Dispose();
    }
    
    public override void SetWeight(float w0, float w1)
    {
        var job = m_Mixer.GetJobData<SMixerLayerJob>();
        job.weightA = w0;
        job.weightB = w1;
        m_Mixer.SetJobData(job);
    }

    public void SetBlendingMask(Transform maskBone, float weight)
    {
        var numTransforms = m_BoneList.Length - 1;
        for (var i = 0; i < numTransforms; ++i)
            m_BoneWeights[i] = 0;

        // Set bone weights for selected transforms and their hierarchy.
        var childList = maskBone.transform.GetComponentsInChildren<Transform>();
        foreach (var child in childList)
        {
            var boneIndex = Array.IndexOf(m_BoneList, child);
            Debug.Assert(boneIndex > 0, "Index can't be less or equal to 0");
            m_BoneWeights[boneIndex - 1] = weight;
        }
    }

    public void ClearBlendingMask()
    {
        var numTransforms = m_BoneList.Length - 1;
        for (var i = 0; i < numTransforms; ++i)
            m_BoneWeights[i] = 1.0f;
    }
}

















public struct SMixerLayerJob : IAnimationJob
{
    public NativeArray<TransformStreamHandle> handles;
    public NativeArray<float> boneWeights;

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
                SMixerUtility.StreamMix(handles, stream, stream, A, weightA, boneWeights); // blend-in transition to A from stream
        }
        else
        {
            if (weightA == 1)
                SMixerUtility.StreamMix(handles, stream, A, B, weightB, boneWeights); // blend-in transition to B from A
            else
                SMixerUtility.StreamMixMix(handles, stream, stream, A, B, weightA, weightB, boneWeights); 
        }
    }
}