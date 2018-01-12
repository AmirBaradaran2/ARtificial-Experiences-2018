Watson Avatar
============

This project integrates Watson services with Unity in-scene Avatars (mecanim-driven). All the demos can be found in [this playlist](https://www.youtube.com/playlist?list=PLKjvzG3vJ6gocim7N6fg5sMcMnOZKnSe0).

[TOC]

Demo Scenes (under Assets/Scenes)
-------------------

#### *WatsonConversationScene.unity*: (for Carmine's use) 
Pick up interaction with a dumb robot. The required object will be lifted up on correct command. And there is a voice feedback such as *"Sorry I can't get you"* or *"Object picked up".* This scene is separately packed into [this package](https://www.dropbox.com/s/xu2f7ezatjhaxee/Watson_Conversation_Bootcamp.unitypackage?dl=0).

**User input**: Pick up voice command like *"Pick up the colorful box"*.

#### *Watson_Droid.unity*: (for Camine's use only)
Pick up interaction with a droid robot. The droid robot will walk to the required object and the object will be lifted up on pick up command. The pick up clip needs to be further toned using other software.

**User input**: 
- Keyboard key *'F'* to let the camera focus on the robot.
- Pick up voice command like *"Pick up the colorful box"*.

#### *Watson_yBot_Pickup.unity*:
Pick up interaction with a yBot robot (from Maximo). The yBot robot will walk to the required object and pick it up on pick up command. 

**User input**:
- Keyboard key *'F'* to let the camera focus on the robot.
- Pick up voice command like *"Pick up the colorful box"*.
- Put down voice command like *"Put it down"* or *"Put it back"*.

#### *Watson_yBot_Poem.unity*:
Naive pick poem interaction with a yBot robot (from Maximo). The yBot is sitting in front of a collection of poems. The book will be lifted up and two text panels will appear, showing the selected poem line by line, with Watson Text-to-Speech reading the poem. 

**User input**: Keyboard key *'F'* to let the camera focus on the robot.

#### *Watson_yBot_Sit_Stand_Lay.unity*:
Sit/stand/lay interaction with a yBot robot (from Mixamo). The yBot will walk to desired object and sit down / lay down / stand up on corresponding commands. 

**User input**: 
- Keyboard key *'F'* to let the camera focus on the robot.
- Sit down voice command like *"Sit on the chair"*.
- Stand up voice command like *"Stand up"*.
- Lay down voice command like *"Lay on the bed"*.

**Issue**:
Note that bugs exist when switching from sitting to standing and laying to standing; the pivot of the robot seems to change between different clips.

Project Structure
------------------------

