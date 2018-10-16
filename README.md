Need 2018.02 or Newer version of Unity  

# Why?
Unity changed legacy animation component to animator component long time ago. Animator is much faster with no bug. However, AnimatorController is essential and it has few difficulties. This simple code is starting point of user custom animation system, easy to expand.  

# Functions
* Basic functions ( play, stop, pause, resume, crossfade )
* Blend-in & Blend-out
* two Layers
* Bone Mask

# How-to-use
1. add SAnimationSystem component to animation GameObject
2. assign Avatar to Animator ( don't assign Animator Controller )
3. have fun!  

# Background concept
Animator is composed of a system called Playable and you can directly control this by making PlayableBehavior / IAnimationJob and Built-in Playable nodes. This system simply gives you methods to control animator like animation component. 
* More info https://docs.unity3d.com/Manual/Playables.html  

# Additional
Use this library to see the structure and flow of the Playable in the animator
* https://github.com/Unity-Technologies/graph-visualizer

# Test
```
public class SAnimationTest : MonoBehaviour {

    public SAnimationSystem system;
    public Transform blendingMaskBone;

    public void Update()
    {
        // assume that system has 2 layers and 3 animation states("idle","walk","roll")
        // layer 0 is for base motion like idle and walk
        // layer 1 is for one time action to override base animation

        // test asset is from assetstore ( RPG Character Mecanim Animation Pack FREE )
        // https://assetstore.unity.com/packages/3d/animations/rpg-character-mecanim-animation-pack-free-65284

        if (Input.GetKeyDown(KeyCode.Q))    // pause
            system[0].SetSpeed(0);

        if (Input.GetKeyDown(KeyCode.W))    // resume
            system[0].SetSpeed(1);

        if (Input.GetKeyDown(KeyCode.E))    // Blend action layer only upper part of body
            system.SetBlendingMask(blendingMaskBone, 1);

        if (Input.GetKeyDown(KeyCode.R))    // change loop to walk from Idle 
            system.CrossFade("walk", 0.2f);

        if (Input.GetKeyDown(KeyCode.T))    // play action layer(1) to override base layer(0)
            system.Play("roll", 1);     

        if (Input.GetKeyDown(KeyCode.Y))    // play action layer with blend-in
            system.Play("roll", 1, 0.2f);

        if (Input.GetKeyDown(KeyCode.U))    // play action layer with blend-in&out
            system.PlayBlendOut("roll", 1, 0.2f, 0.2f);
    }
}
```
