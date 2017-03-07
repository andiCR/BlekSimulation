using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderBounce : MonoBehaviour {

	public bool isVertical;

	void OnTriggerEnter2D (Collider2D other) {
		var line = other.GetComponent<LineDrawer> ();
		if (line != null) {
			line.HandleBorderCollision (isVertical);
		}
		//game.HandleCollect(this);
		//gameObject.SetActive(false);
	}
}
