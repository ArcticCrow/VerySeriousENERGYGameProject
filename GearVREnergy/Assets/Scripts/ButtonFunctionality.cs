using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctionality : MonoBehaviour {

	public void GameScene(int gameScene) 
	{
		SceneManager.LoadScene (gameScene);
	}
	
	public void Quit()
	{
		UnityEditor.EditorApplication.isPlaying = false;
	}
}
