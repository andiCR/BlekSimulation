using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDrawer : MonoBehaviour {

	enum State {
		None,
		Drawing,
		Replaying

	}

	State _state = State.None;
	LineRenderer _lineRenderer;

	List<Vector3> _recordedPositions = new List<Vector3>();
	Vector3 _startPosition = new Vector3();
	Vector3 _oldPos;
	int _recordIndex = 0;
	int _replayIndex = 0;

	// Use this for initialization
	void Start () {
		_lineRenderer = GetComponent<LineRenderer>();
		_lineRenderer.enabled = false;


	}

	void StartDrawing(Vector2 position) {
		_state = State.Drawing;
		_lineRenderer.enabled = true;
		_recordIndex = 0;

		_startPosition.x = position.x;
		_startPosition.y = position.y;
	}

	void UpdateDrawing(Vector2 position) {
		if (_recordIndex > _recordedPositions.Count) {
			return;
		}

		_recordedPositions.Add( new Vector3(Camera.main.ScreenToWorldPoint(position).x, 
											Camera.main.ScreenToWorldPoint(position).y, 
											1));

		for (var i = 0; i < _lineRenderer.numPositions; i++) {
			var idx = _recordIndex - i;
			if (idx < 0) {
				break;
			}
			_lineRenderer.SetPosition(i, _recordedPositions[idx]);
		}

		_recordIndex++;
	}
	void StopDrawing(Vector2 position) {
		_state = State.Replaying;

		_startPosition = Camera.main.ScreenToWorldPoint(position);
		_startPosition.z = 1;
		_oldPos = _recordedPositions[0];
	}

	void DrawReplay() {
		// Save first position
//		Debug.Log (_recordedPositions[0]);
		var p = _recordedPositions[0];
//		Debug.Log (p);
		// Shift every point to the left
		for (var i = 0; i < _recordedPositions.Count - 1; i++) {
//			Debug.Log (_recordedPositions[i]);
			_recordedPositions[i] = _recordedPositions[i + 1];
		}

		// Save new last position (which would be the first position positioned at the end)
		_recordedPositions[_recordIndex - 1] = p - _oldPos + _startPosition;

		// Draw the line
		for (var i = 0; i < _recordedPositions.Count; i++) {
			_lineRenderer.SetPosition(i, _recordedPositions[i]);
		}

		// Advance replay
		_replayIndex++;
		if (_replayIndex == _recordIndex) {
			_startPosition = _recordedPositions[_replayIndex - 1];
			_oldPos = _recordedPositions[0];
			_replayIndex = 0;
		}
	}

	// Update is called once per frame
	void FixedUpdate () {
		switch (_state) {
			case State.None:
				if (Input.GetMouseButtonDown(0)) {
					StartDrawing(Input.mousePosition);
				}
				break;
			case State.Drawing:
				if (Input.GetMouseButtonUp(0)) {
					StopDrawing(Input.mousePosition);
				} else {
					UpdateDrawing(Input.mousePosition);
				}
			break;
			case State.Replaying:
				DrawReplay();
			break;
		}
	}
}
