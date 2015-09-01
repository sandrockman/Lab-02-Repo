using UnityEngine;
using System.Collections;
using UnityEditor;

public class AnimationGeneration : EditorWindow {

	public static Object selectedObject;

	int numAnimations;
	string controllerName;
	string[] animationNames = new string[100];
	string[] clipFrameRate = new string[100];
	int[] clipTimeBetween = new int[100];
	int[] startFrames = new int[100];
	int[] endFrames = new int[100];
	bool[] pingPong = new bool[100];
	bool[] loop = new bool[100];

	[MenuItem("Project Tools/2DAnimations")]
	public static void Init()
	{
		selectedObject = Selection.activeObject;

		if (selectedObject == null)
			return;

		AnimationGeneration window = (AnimationGeneration)EditorWindow.GetWindow (typeof(AnimationGeneration));
		window.Show ();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
