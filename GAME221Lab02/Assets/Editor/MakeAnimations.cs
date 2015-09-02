﻿using UnityEngine;
using System.Collections;
using UnityEditor;

public class MakeAnimations : EditorWindow
{
	
	//Will hold the object that the user has selected when the script is run
	public static Object selectedObject;
	
	//Will store how many animations will be created
	int numAnimations;
	//Name of the controller to be created
	string controllerName;
	//name of each of the animations to be created
	string[] animationNames = new string[100];
	
	//The frame rate for each animation
	float[] clipFrameRate = new float[100];
	//The time between each animation
	float[] clipTimeBetween = new float[100];
	//What frame each animation starts at
	int[] startFrames = new int[100];
	//What frame each animation ends at
	int[] endFrames = new int[100];
	//If each animation should pingpong
	bool[] pingPong = new bool[100];
	//if each animation should loop
	bool[] loop = new bool[100];
	
	
	[MenuItem("Project Tools/2D Animations Master")]
	static void Init()
	{
		//Grab the active object
		selectedObject = Selection.activeObject;
		
		//If the object doesn't exist, do nothing
		if (selectedObject == null)
			return;
		
		//Otherwise, create a new window
		MakeAnimations window = (MakeAnimations)EditorWindow.GetWindow(typeof(MakeAnimations));
		
		//Show the window
		window.Show();
	}
	
	void OnGUI()
	{
		if (selectedObject != null)
		{
			//Display the objects name that the animations will be created from
			EditorGUILayout.LabelField("Animations for " + selectedObject.name);
			//Create a space
			EditorGUILayout.Separator();
			//Get the name for the animation controller
			controllerName = EditorGUILayout.TextField("Controller Name", controllerName);
			//Determine how many animations there will be
			numAnimations = EditorGUILayout.IntField("How many animations?", numAnimations);
			//Loop through each theoretical animation
			for (int i = 0; i < numAnimations; i++)
			{
				//Determine a name for the animation
				animationNames[i] = EditorGUILayout.TextField("Animation Name", animationNames[i]);
				
				//Start a section where the following items will be displayed horizontally instead of vertically
				EditorGUILayout.BeginHorizontal();
				//Determine the start frame for the animation
				startFrames[i] = EditorGUILayout.IntField("Start Frame", startFrames[i]);
				//Determine the end frame for the animation
				endFrames[i] = EditorGUILayout.IntField("End Frame", endFrames[i]);
				//End the section where the previous items are displayed horitontally instead of vertically
				EditorGUILayout.EndHorizontal();
				
				//Start a section where the following items will be displayed horizontally instead of vertically
				EditorGUILayout.BeginHorizontal();
				//Determine the frame rate for the animation
				clipFrameRate[i] = EditorGUILayout.FloatField("Frame Rate", clipFrameRate[i]);
				//Determine the space between each keyframe
				clipTimeBetween[i] = EditorGUILayout.FloatField("Frame Spacing", clipTimeBetween[i]);
				//End the section where the previous items are displayed horitontally instead of vertically
				EditorGUILayout.EndHorizontal();
				
				
				//Start a section where the following items will be displayed horizontally instead of vertically
				EditorGUILayout.BeginHorizontal();
				//Create a checkbox to determine if this animation should loop
				loop[i] = EditorGUILayout.Toggle("Loop", loop[i]);
				//Create a checkbox to determine if this animation should pingpong
				pingPong[i] = EditorGUILayout.Toggle("Ping Pong", pingPong[i]);
				//End the section where the previous items are displayed horitontally instead of vertically
				EditorGUILayout.EndHorizontal();
				
				//Create a space
				EditorGUILayout.Separator();
				
			}
			
			//Create a button with the label "Create"
			if (GUILayout.Button("Create"))
			{
				//Create an animator controller
				UnityEditor.Animations.AnimatorController controller = 
					UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath(("Assets/" + 
					                                                                          controllerName + ".controller"));
				
				for (int i = 0; i < numAnimations; i++)
				{
					//Create animation clip
					AnimationClip tempClip = CreateClip(selectedObject, animationNames[i], startFrames[i], endFrames[i],
					                                    clipFrameRate[i], clipTimeBetween[i], pingPong[i]);
					
					//Detemine if the clip should loop
					if (loop[i])
					{
						//If so, capture the settings of the clip
						AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(tempClip);
						
						//Set the looping to true
						settings.loopTime = true;
						settings.loopBlend = true;
						
						//Apply the settings to the clip
						AnimationUtility.SetAnimationClipSettings(tempClip, settings);
					}
					
					//Add the clip to the Animator Controller
					controller.AddMotion(tempClip);
				}

				GameObject controllerObj = PrefabUtility.CreatePrefab("Assets/testing.prefab", new GameObject());
				controllerObj.AddComponent(typeof(SpriteRenderer));
				Animator controllerAnimator = (Animator)controllerObj.AddComponent(typeof(Animator));
				controllerAnimator.runtimeAnimatorController = controller;
			}
		}
	}
	
	public AnimationClip CreateClip(Object obj, string clipName, int startFrame, int endFrame, float frameRate, float timeBetween, bool pingPong)
	{
		//Get the path to the object
		string path = AssetDatabase.GetAssetPath(obj);
		
		//Extract the sprites
		Object[] sprites = AssetDatabase.LoadAllAssetsAtPath(path);
		
		//Determine how many frames, and the length of each frame
		int frameCount = endFrame - startFrame + 1;
		float frameLength = 1f / timeBetween;
		
		//Create a new (empty) animation clip
		AnimationClip clip = new AnimationClip();
		
		//Set the framerate for the clip
		clip.frameRate = frameRate;
		
		//Create the new (empty) curve binding
		EditorCurveBinding curveBinding = new EditorCurveBinding();
		//Assign it to change the sprite renderer
		curveBinding.type = typeof(SpriteRenderer);
		//Assign it to alter the sprite of the sprite renderer
		curveBinding.propertyName = "m_Sprite";
		
		//Create a container for all of the keyframes
		ObjectReferenceKeyframe[] keyFrames;
		
		//Determine how many frames there will be if we are or are not pingponging
		if (!pingPong)
			keyFrames = new ObjectReferenceKeyframe[frameCount + 1];
		else
			keyFrames = new ObjectReferenceKeyframe[frameCount*2 + 1];
		
		//Keep track of what frame number we are on
		int frameNumber = 0;
		
		//Loop from start to end, incrementing frameNumber as we go
		for (int i = startFrame; i < endFrame + 1; i++, frameNumber++)
		{
			//Create an empty keyframe
			ObjectReferenceKeyframe tempKeyFrame = new ObjectReferenceKeyframe();
			//Assign it a time to appear in the animation
			tempKeyFrame.time = frameNumber * frameLength;
			//Assign it a sprite
			tempKeyFrame.value = sprites[i];
			//Place it into the container for all the keyframes
			keyFrames[frameNumber] = tempKeyFrame;
		}
		
		//If we are pingponging this animation
		if (pingPong)
		{
			//Create keyframes starting at the end and going backwards
			//Continue to keep track of the frame number
			for(int i = endFrame; i >= startFrame; i--, frameNumber++)
			{
				ObjectReferenceKeyframe tempKeyFrame = new ObjectReferenceKeyframe();
				tempKeyFrame.time = frameNumber * frameLength;
				tempKeyFrame.value = sprites[i];
				keyFrames[frameNumber] = tempKeyFrame;
			}
		}
		
		//Create the last sprite to stop it from switching quickly from the last frame to the first one
		ObjectReferenceKeyframe lastSprite = new ObjectReferenceKeyframe();
		lastSprite.time = frameNumber * frameLength;
		lastSprite.value = sprites[startFrame];
		keyFrames[frameNumber] = lastSprite;
		
		//Assign the name
		clip.name = clipName;
		
		//Apply the curve
		AnimationUtility.SetObjectReferenceCurve(clip, curveBinding, keyFrames);
		
		//Creat the clip
		AssetDatabase.CreateAsset(clip, ("Assets/" + clipName + ".anim"));
		
		//return the clip
		return clip;
		
	}
}
