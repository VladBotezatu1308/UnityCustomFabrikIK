# Personal IK project

<img src="docs/IKDemo.gif" height="324">

In an attempt to learn more about inverse kinematics I started this project trying to implement an IK rig using Jacobian methods.

Further along development I scratched that idea and found an article written by Dr. Andreas Aristidou regarding FABRIK, an IK solution that uses a heuristic method of moving a point on a line to find a joint's next position.

His solution can be found at http://andreasaristidou.com/FABRIK.html.

In this project I tried to understand the work behind this algortihm and implement a simple IK solution based on it in Unity, that would require little work to setup.

This soultion does not have joint constraints and it does not have production value.

To set up the IK using a simple arm:
 - create an arm using multiple transforms parented to each other

 - add the Fabrik IK component to the root of the arm

 - create a target and reference it in the "Target" field of the script

 - move the target around in play mode and see how the arm follows it