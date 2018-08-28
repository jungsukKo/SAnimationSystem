# SAnimationSystem
Simple Animation system to control Animator without AnimatorController

# Why I make this?
Unity change legacy animation component to animator component long time ago. To use animator, animatorController are necessary but there are few difficulties to use it. However, animator is much faster and no bug like animation componenet
* animatorController need lot of time to setup
* animatorController make another asset file
* animatorController no method to control from code

# Background concept
Animator is composed of a system called Playable and you can directly controll this by making PlayableBehavior and Built-in Playerable nodes. This system is simply give you methode to control animator like animation component.

# Functions
* Basic functions ( play, stop, pause, resume, crossfade )
* Support Override / Additive layer
* easy to expend and read

# Set-up
1. add SAnimationSystem component
2. assign Avatar to Animator ( don't assign Animator Controller )
3. have fun!

# Test Monobehaviour
```
public class SAnimationTest : MonoBehaviour {

    public SAnimationSystem system;

    public void Update()
    {
        // assume that system has 2 layers ( 0 for looping, 1 for action ) and 3 animation states( "idle", "walk", "roll" )

        if (Input.GetKeyDown(KeyCode.Q))    // pause
            system[0].SetSpeed(0);

        if (Input.GetKeyDown(KeyCode.W))    // resume
            system[0].SetSpeed(1);

        if (Input.GetKeyDown(KeyCode.E))    // set layer 1 as additive, default is overriding
            system.SetAddive(1, true);

        if (Input.GetKeyDown(KeyCode.R))    // play loop from Idle to walk
            system.CrossFade("walk", 0.2f);

        if (Input.GetKeyDown(KeyCode.T))    // play action layer
            system.Play("roll", 1);     

        if (Input.GetKeyDown(KeyCode.Y))    // play action layer with blend-in
            system.Play("roll", 1, 0.2f);

        if (Input.GetKeyDown(KeyCode.U))    // play action layer with blend-in and blend-out
            system.Play("roll", 1, 0.2f, 0.2f);

        if (Input.GetKeyDown(KeyCode.I))    // play action layer with blend-in and blend-out and override wraping mode
            system.Play("roll", 1, 0.2f, 0.2f, SAnimation.eWRAP_MODE.FREEZE_AT_LAST);
    }
}
```