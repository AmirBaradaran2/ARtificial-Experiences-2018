# ARtificial-Experiences-2018
Combining past works (created under amirbaradaran2)


# CGUI_Amir
Repository for Amir Baradaran's projects

Watson Avatar
==

This project integrates Watson services with Unity in-scene Avatars (mecanim-driven). All the demos can be found in [this playlist](https://www.youtube.com/playlist?list=PLKjvzG3vJ6gocim7N6fg5sMcMnOZKnSe0).

Demo Scenes (under Assets/Scenes)
--

### *WatsonConversationScene.unity*: (for Carmine's use) 
Pick up interaction with a dumb robot. The required object will be lifted up on correct command. And there is a voice feedback such as *"Sorry I can't get you"* or *"Object picked up".* This scene is separately packed into [this package](https://www.dropbox.com/s/xu2f7ezatjhaxee/Watson_Conversation_Bootcamp.unitypackage?dl=0).

**User input**: Pick up voice command like *"Pick up the colorful box"*.

### *Watson_Droid.unity*: (for Camine's use only)
Pick up interaction with a droid robot. The droid robot will walk to the required object and the object will be lifted up on pick up command. The pick up clip needs to be further toned using other software.

**User input**: 
- Keyboard key *'F'* to let the camera focus on the robot.
- Pick up voice command like *"Pick up the colorful box"*.

### *Watson_yBot_Pickup.unity*:
Pick up interaction with a yBot robot (from Maximo). The yBot robot will walk to the required object and pick it up on pick up command. 

**User input**:
- Keyboard key *'F'* to let the camera focus on the robot.
- Pick up voice command like *"Pick up the colorful box"*.
- Put down voice command like *"Put it down"* or *"Put it back"*.

### *Watson_yBot_Poem.unity*:
Naive pick poem interaction with a yBot robot (from Maximo). The yBot is sitting in front of a collection of poems. The book will be lifted up and two text panels will appear, showing the selected poem line by line, with Watson Text-to-Speech reading the poem. 

**User input**: Keyboard key *'F'* to let the camera focus on the robot.

### *Watson_yBot_Sit_Stand_Lay.unity*:
Sit/stand/lay interaction with a yBot robot (from Mixamo). The yBot will walk to desired object and sit down / lay down / stand up on corresponding commands. 

**User input**: 
- Keyboard key *'F'* to let the camera focus on the robot.
- Sit down voice command like *"Sit on the chair"*.
- Stand up voice command like *"Stand up"*.
- Lay down voice command like *"Lay on the bed"*.

**Issue**:
Note that bugs exist when switching from sitting to standing and laying to standing; the pivot of the robot seems to change between different clips.

Project Structure (description of some general structures are omitted)
--
1. ***Assets/Animations***: animation clips for the yBot robot and Droid robot.
2. ***Assets/Animators***: 
	- *droidAnimator(for Carmine's use only)* : walk animator for the Droid robot; uses ***BlendX*** (movement on X axis) and ***BlendY*** (movement on Y axis) to blend between animation clips.
	- *yBotAnimator*: walk/pickup/sit/stand/lay animator for the yBot robot; uses ***BlendX*** (movement on X axis) and ***BlendY*** (movement on Y axis) to blend between animation clips.
	- *yBotPickPoemAnimator*: pick poem animator for the yBot robot; uses ***BlendX*** (movement on X axis) and ***BlendY*** (movement on Y axis) to blend between animation clips.
	- **yBotTurnForwardAnimator*: walk animator for the yBot robot; uses ***Turn*** (turn amount) and ***Forward*** (movement amount) to blend between animation clips.
3. ***Assets/ButterflyParticles***: butterfly particle system [[unitypackage]](https://assetstore.unity.com/packages/vfx/particles/butterfly-particle-system-4000).
4. ***Assets/CielaSpike***: threaded coroutine scripts [[unitypackage]](https://assetstore.unity.com/packages/tools/thread-ninja-multithread-coroutine-15717).
5. ***Assets/Classic Skybox***: skybox textures [[unitypackage]](https://assetstore.unity.com/packages/2d/textures-materials/sky/classic-skybox-24923).
6. ***Assets/Models/3D Font Art Pack***: 3d text models [[unitypackage]](https://assetstore.unity.com/packages/3d/props/3d-font-art-pack-65635).
7. ***Assets/Models/BigFurniture Pack***: bed and chair models [[unitypackage]](https://assetstore.unity.com/packages/3d/props/furniture/big-furniture-pack-7717)
8. ***Assets/Models/Books***: book models [[unitypackage]](https://assetstore.unity.com/packages/3d/props/interior/books-3356).
9. ***Assets/Models/Bot***: Droid robot model from Carmine.
10. ***Assets/Models/Flatscreen TV***: tv model [[unitypackage]](https://assetstore.unity.com/packages/3d/props/electronics/flatscreen-tv-9721).
11. ***Assets/Models/HQ Old Books***: book models [[unitypackage]](https://assetstore.unity.com/packages/3d/props/hq-old-books-96841).
12. ***Assets/Models/TV Furniture***: TV desk models [[unitypackage]](https://assetstore.unity.com/packages/3d/props/electronics/tv-furniture-60122).
13. ***Assets/Models/_Creepy_Cat***: ceeling light model [[unitypackage]](https://assetstore.unity.com/packages/3d/environments/3d-scifi-kit-starter-kit-92152).
14. ***Assets/Models/voxel_scifi_free***: low-poly domb robot model [[unitypackage]](https://assetstore.unity.com/packages/3d/environments/sci-fi/voxel-scifi-environment-free-version-101492).
15. ***Assets/Models/PostProcessing***: postprocessing camera filters [[unitypackage]](https://assetstore.unity.com/packages/essentials/post-processing-stack-83912).
16. ***Assets/Models/PrimitivePlus***: pyramid models [[unitypackage]](https://www.assetstore.unity3d.com/en/#!/content/25542).
17. ***Assets/Models/Profile***: light profile used for the post processing filter.
18. ***Assets/Watson***: Watson unity sdk 1.0.0.
19. ***Assets/Scripts/IG Downloader***: scripts from Julie [[doc]](https://docs.google.com/document/d/1dYlU322MgREKFApCiEhhtbiiFRyzV7XFlqVQy2ifLU0/edit#) to download IG images and visualize on particle systems.
20. ***Assets/Scripts/Pick Poem***: scripts related to pick poem interaction.
	- *PickPoemGameManager.cs*: game manager in this interaction; added to the camera.
	- *PoemBehavior.cs*: behavior of the poem on lifted up; added to the book model.
21. ***Assets/Scripts/Robot***: scripts related to robot mecanim.
	- *MyCameraController.cs*: camera controller that makes it focusing on the robot when a key input 'F' is given.
	- *MyDroidRobotController.cs*: droid robot controller; differred from MyRobotController.cs in that the object is lifted up instead of playing pick up clip.
	- *MyRobotCharacter.cs*: general robot character script that defines internally how to update the variables (in this case ***BlendX*** and ***BlendY***) of the animator and how to apply turn amount on the robot.
		**Important func** :  *Move()*
	- *MyRobotCharacter.cs*: general robot controller that defines when to update the animator; user inputs are triggered from this script.
		**Important func**: *FixedUpdate()*
	- *MyRobotTurnForwardCharacter.cs*: general robot character script that defines internally how to update the variables (in this case ***Turn*** and ***Forward***) of the animator and how to apply turn amount on the robot.
		**Important func** :  *Move()*
	- *MyRobotCharacter.cs*: general robot controller that defines when to update the animator; user inputs are triggered from this script.
		**Important func**: *FixedUpdate()*
22. ***Assets/Scripts/Watson Conversation***: scripts related to Watson services.
	- *GameManager.cs*: game manager. 
	- *MyConversation.cs*: script that shows how to use Conversation service.
		**Important func**: *OnMessage()*
	- *MySpeechToText.cs*: script that shows how to use Speech to Text service using realtime voice input; sends recognized text data directly to Conversation script via *Widget.Output*.
	- *MyTextToSpeech.cs*: script that shows how to use Text to Speech service.
		**Important func**: *textToSpeech(string text, string gender, bool goodNews)*
23. ***Assets/Scripts/Utilities***: other scripts.
	- *CanvasLookAt.cs*: keep the world space canvas looking at the camera.
	- *TypeWord.cs*: type effect; make the text appear letter by letter.

How to Change the Avatar while Reserving all the Features
--
1. Create a new avatar from the new model.
2. Create an animator for the new model, tone animation clips, use the avatar created in 1. for all clips and add them to corresponding states in the animator.
3. Add *MyCameraController.cs* to camera and throw the robot object into the script input.
4. Add *MyRobotCharater.cs* and *MyRobotController.cs* to the robot object.
5. Add *GameManager.cs*, *MyTextToSpeech.cs*, *MyConversation.cs*, *MySpeechToText.cs* to either camera or the showroom object and fill in the inputs as prompted. Throw the *MyRobotController.cs* on the robot object to the input *"RobotController"* of *GameManager.cs*. Should be all set then.

Lighting
--
Global illumination is applied and the ambient lighting source is gradient color. Post processing filter (*Assets/PostProcessing/Runtime/PostProcessingBehavior.cs*) is added to the camera and a light profile (under *Assets/Profile/ShowroomProfie.asset*) is toned.

=======
# GalleryDesign

