using Unity.Collections;
using UnityEngine;
using UnityEngine.Experimental.Animations;






public class SMixerUtility
{
    public static void ProcessRootMotion(AnimationStream stream, float weightA, float weightB)
    {
        if (weightA == 0 && weightB == 0)
            return;

        var A = stream.GetInputStream(0);
        var B = stream.GetInputStream(1);

        if (weightB != 0)
        {
            if (weightB == 1)
            {
                stream.velocity = B.velocity;
                stream.angularVelocity = B.angularVelocity;
            }
            else if (weightA == 1)
            {
                // crossfade
                stream.velocity = Vector3.Lerp(A.velocity, B.velocity, weightB);
                stream.angularVelocity = Vector3.Lerp(A.angularVelocity, B.angularVelocity, weightB);
            }
            else
            {
                // blend-in
                var velocity = Vector3.Lerp(stream.velocity, A.velocity, weightA);
                var angularVelocity = Vector3.Lerp(stream.angularVelocity, A.angularVelocity, weightA);
                // crossfade
                stream.velocity = Vector3.Lerp(velocity, B.velocity, weightB);
                stream.angularVelocity = Vector3.Lerp(angularVelocity, B.angularVelocity, weightB);
            }
        }
        else
        {
            if (weightA == 1)
            {
                stream.velocity = A.velocity;
                stream.angularVelocity = A.angularVelocity;
            }
            else
            {
                // blend-in
                stream.velocity = Vector3.Lerp(stream.velocity, A.velocity, weightA);
                stream.angularVelocity = Vector3.Lerp(stream.angularVelocity, A.angularVelocity, weightA);
            }
        }
    }

    public static void StreamSet(NativeArray<TransformStreamHandle> handles, AnimationStream result, AnimationStream A)
    {
        var numHandles = handles.Length;
        for (var i = 0; i < numHandles; ++i)
        {
            var handle = handles[i];
            handle.SetLocalPosition(result, handle.GetLocalPosition(A));
            handle.SetLocalRotation(result, handle.GetLocalRotation(A));
        }
    }

    public static void StreamMix(NativeArray<TransformStreamHandle> handles, AnimationStream stream, AnimationStream A, AnimationStream B, float w)
    {
        for (var i = 0; i < handles.Length; ++i)
            UpdateStream(handles[i], stream, A, B, w);
    }

    public static void StreamMixMix(NativeArray<TransformStreamHandle> handles, AnimationStream stream, AnimationStream A, AnimationStream B, AnimationStream C, float w1, float w2)
    {
        for (var i = 0; i < handles.Length; ++i)
            UpdateStream(handles[i], stream, A, B, C, w1, w2);
    }
        
    public static void StreamMix(NativeArray<TransformStreamHandle> handles, AnimationStream stream, AnimationStream A, AnimationStream B, float w, NativeArray<float> boneWeights)
    {
        for (var i = 0; i < handles.Length; ++i)
            UpdateStream(handles[i], stream, A, B, w * boneWeights[i]);
    }

    public static void StreamMixMix(NativeArray<TransformStreamHandle> handles, AnimationStream stream, AnimationStream A, AnimationStream B, AnimationStream C, float w1, float w2, NativeArray<float> boneWeights)
    {
        for (var i = 0; i < handles.Length; ++i)
            UpdateStream(handles[i], stream, A, B, C, w1, w2 * boneWeights[i]);
    }

    public static void UpdateStream(TransformStreamHandle handle, AnimationStream stream, AnimationStream A, AnimationStream B, float w)
    {
        var posA = handle.GetLocalPosition(A);
        var posB = handle.GetLocalPosition(B);
        var pos = Vector3.Lerp(posA, posB, w);

        var rotA = handle.GetLocalRotation(A);
        var rotB = handle.GetLocalRotation(B);
        var rot = Quaternion.Slerp(rotA, rotB, w);

        handle.SetLocalPosition(stream, pos);
        handle.SetLocalRotation(stream, rot);
    }

    public static void UpdateStream(TransformStreamHandle handle, AnimationStream stream, AnimationStream A, AnimationStream B, AnimationStream C, float w1, float w2)
    {
        var posA = handle.GetLocalPosition(A);
        var posB = handle.GetLocalPosition(B);
        var posC = handle.GetLocalPosition(C);
        var posAB = Vector3.Lerp(posA, posB, w1);
        var pos = Vector3.Lerp(posAB, posC, w2);

        var rotA = handle.GetLocalRotation(A);
        var rotB = handle.GetLocalRotation(B);
        var rotC = handle.GetLocalRotation(C);
        var rotAB = Quaternion.Slerp(rotA, rotB, w1);
        var rot = Quaternion.Slerp(rotAB, rotC, w2);

        handle.SetLocalPosition(stream, pos);
        handle.SetLocalRotation(stream, rot);
    }
}

