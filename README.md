# VNyan Leap Motion Fixer
Removes stuttering and noise from your leap motion tracking! 

![image of plugin window](https://github.com/Lunazera/VNyan-Leap-Motion-Fixer/blob/master/example.png)

## Installation
1. Download the latest zip file from [releases]([https://github.com/Lunazera/VNyan-Tracking-Detection/releases/](https://github.com/Lunazera/VNyan-Eye-Smoothing/releases))
2. Unzip the contents in your VNyan folder under `Items/Assemblies`.
3. The plugin should be present when you load VNyan! (you should see it in the plugin menu)

## How it works
This lets you apply smoothing to your leap motion tracking, and monitors for when there are many on/off events under a short timespan (which happens when the tracking flickers on and off). When that occurs, it enters an unstable state and freezes your arm's pose until it stabilizes again.
Make sure you set your Mirror On setting to match what you have in your Leap Motion settings!

### Smoothing
Amount of smoothing to apply to your arms

### Smooth Boost
Makes up for the slowdown caused by smoothing by turning it down automatically for making bigger rotations.

### Unstable Smoothing
Amount of smoothing to apply to your arms when either in transition states or when tracking is unstable.

## Graph
The accompanying graph manages the Leap Motion on/off events to tell the plugin the status of the left/right arms. Every on/off signal adds an event, while the timer loop below subtracts events every time window. If the Number of events goes past your Sensitivity setting, then it'll enter the "unstable" state and pause your arm until it stabilizes.

### Parameters
You can monitor the state of either arm under LZ_LeapFixer_LeftStateManager and LZ_LeapFixer_RightStateManager
For convenience, this also saves a RightMotionDetected and LeftMotionDetected parameter that'll switch to 1 whenever the models left or right arm is in motion from tracking, while allowing some buffer time to transition automatically.
(I got left/right mixed up so it's your model's left/right arm from the user's perspective :P )

### Extra 
There's also a currently unused "Freeze State", that you can access by setting "LZ_LeapFixer_FreezeL" or "LZ_LeapFixer_FreezeR" to 1 to activate. This will put your arm back to it's previous rotations before leap motion turned on.
