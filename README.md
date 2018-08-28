# SAnimationSystem
Simple Animation system to control Animator without AnimatorController

# Why I make this?
Unity change legacy animation component to animator component long time ago. To use animator, animatorController are necessary but there are few difficulties to use it.
* animatorController need lot of time to setup
* make another asset file
* no method to control from code
However, animator is much faster and no bug like animation componenet

# Background concept
Animator is composed of a system called Playable and you can directly controll this by making PlayableBehavior and Built-in Playerable nodes. This system is simply give you methode to control animator like animation component.

# Set-up
1. add SAnimationSystem component
2. assign Avatar to Animator ( don't assign Animator Controller )
3. have fun!
