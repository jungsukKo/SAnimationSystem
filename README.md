# SAnimationSystem
Simple Animation system to control Animator without AnimatorController

# Why?
Unity change legacy animation component to animator component long time ago. Animator is much faster with no bug. However, AnimatorController is essential and there are few difficulties. 
* animatorController need lot of time to setup
* animatorController make another asset file
* animatorController has no method to control animation from code

# Functions
* Basic functions ( play, stop, pause, resume, crossfade )
* Blend-in & Blend-out
* Override / Additive layer
* easy to expand and read codes

# How-to-use
1. add SAnimationSystem component to animation GameObject
2. assign Avatar to Animator ( don't assign Animator Controller )
3. have fun!
![](https://github.com/jungsukKo/SAnimationSystem/blob/master/screenshot1.png)

# Test
```
public class SAnimationTest : MonoBehaviour {

    public SAnimationSystem system;

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

        if (Input.GetKeyDown(KeyCode.E))    // set layer 1 as additive, default is overriding
            system.SetAddive(1, true);

        if (Input.GetKeyDown(KeyCode.R))    // change loop to walk from Idle 
            system.CrossFade("walk", 0.2f);

        if (Input.GetKeyDown(KeyCode.T))    // play action layer(1) to override base layer(0)
            system.Play("roll", 1);     

        if (Input.GetKeyDown(KeyCode.Y))    // play action layer with blend-in
            system.Play("roll", 1, 0.2f);

        if (Input.GetKeyDown(KeyCode.U))    // play action layer with blend-in&out
            system.Play("roll", 1, 0.2f, 0.2f);

        if (Input.GetKeyDown(KeyCode.I))    // play action layer with blend-in&out and override wraping mode
            system.Play("roll", 1, 0.2f, 0.2f, SAnimation.eWRAP_MODE.FREEZE_AT_LAST);
    }
}
```

# Background concept
Animator is composed of a system called Playable and you can directly control this by making PlayableBehavior and Built-in Playable nodes. This system simply gives you methods to control animator like animation component. The system has 1 mixer to control animation between layers. Each layer has a mixer to crossfade A to B
* More info https://docs.unity3d.com/Manual/Playables.html

# Additional
Use this library to see the structure and flow of the Playable in the animator
* https://github.com/Unity-Technologies/graph-visualizer
