using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour {

	public Game game;

	public void Revive() {
		gameObject.SetActive(true);
	}

	void OnTriggerEnter2D (Collider2D other) {
		game.HandleCollect(this);
		gameObject.SetActive(false);
	}
}
