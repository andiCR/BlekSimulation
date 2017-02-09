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

	Vector3[] _recordedPositions = new Vector3[60 * 5];
	Vector3 _startPosition = new Vector3();
	Vector3 _oldPos;
	int _recordIndex = 0;
	int _replayIndex = 0;

	// Use this for initialization
	void Start () {
		_lineRenderer = GetComponent<LineRenderer>();
		_lineRenderer.enabled = false;

		for (var i = 0; i < _recordedPositions.Length; i++) {
			_recordedPositions[i] = new Vector3();
		}
	}

	void StartDrawing(Vector2 position) {
		_state = State.Drawing;
		_lineRenderer.enabled = true;
		_recordIndex = 0;

		_startPosition.x = position.x;
		_startPosition.y = position.y;
	}

	void UpdateDrawing(Vector2 position) {
		if (_recordIndex >= _recordedPositions.Length) {
			return;
		}
		_recordedPositions[_recordIndex] = Camera.main.ScreenToWorldPoint(position);
		_recordedPositions[_recordIndex].z = 1;

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
		var p = _recordedPositions[0];
		
		// Shift every point to the left
		for (var i = 0; i < _recordIndex; i++) {
			_recordedPositions[i] = _recordedPositions[i + 1];
		}

		// Save new last position (which would be the first position positioned at the end)
		_recordedPositions[_recordIndex - 1] = p - _oldPos + _startPosition;

		// Draw the line
		for (var i = 0; i < _lineRenderer.numPositions; i++) {
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
