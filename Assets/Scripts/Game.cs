using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {
	public Transform[] levels;
	public LineDrawer lineDrawer;
	public UI ui;

	Transform currentLevel;
	int currentLevelIndex = 0;
	int collectableObjectsCount;
	List<Collectable> collectedObjects = new List<Collectable>();

	void Start () {
		currentLevelIndex = -1;
		StartCoroutine(NextLevel());
	}

	void LoadLevel(int index) {
		if (currentLevel != null) {
			currentLevel.gameObject.SetActive(false);
		}

		currentLevelIndex = index;
		currentLevel = levels[index];
		currentLevel.gameObject.SetActive(true);

		// Get all colliders
		var collectables = currentLevel.GetComponentsInChildren<Collectable>();
		collectableObjectsCount = collectables.Length;
		foreach (var o in collectables) {
			o.game = this;
		}

		// Get all obstacles
		var obstacles = currentLevel.GetComponentsInChildren<Obstacle>();
		foreach (var o in obstacles) {
			o.game = this;
		}
	}
	
	public void HandleCollect(Collectable collectable) {
		collectedObjects.Add(collectable);
		if (collectableObjectsCount == collectedObjects.Count) {
			StartCoroutine(NextLevel());
		}
	}

	public void HandleHitObstacle(Obstacle obstacle) {
		// Remove line
		lineDrawer.StopDrawing();
		lineDrawer.ClearDrawing();

		// Restart
		collectedObjects.Clear();

		// Revive collected objects	
		foreach (var c in collectedObjects) {
			c.Revive();
		}
	}

	IEnumerator NextLevel() {
		currentLevelIndex++;
		
		// Disable line drawer
		lineDrawer.Disable();

		// Hide old level
		if (currentLevel != null) {
			yield return new WaitForSeconds(1.5f);
			currentLevel.gameObject.SetActive(false);
			lineDrawer.gameObject.SetActive(false);
		}

		// Show ui
		if (currentLevelIndex == levels.Length) {
			// Game finished
			ui.levelNameText.text = "Congratulations!";
		} else {
			// Level name
			ui.levelNameText.text = (currentLevelIndex + 1).ToString();
		}
		ui.gameObject.SetActive(true);
		yield return new WaitForSeconds(1.5f);
		ui.gameObject.SetActive(false);

		// Load next level
		if (currentLevelIndex != levels.Length) {
			lineDrawer.gameObject.SetActive(true);
			lineDrawer.Enable();
			LoadLevel(currentLevelIndex);
		}
	}
}
