using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour {
	public Game game;

	void OnTriggerEnter2D (Collider2D other) {
		game.HandleHitObstacle(this);
	}
}
