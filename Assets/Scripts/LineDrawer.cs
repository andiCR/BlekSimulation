﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDrawer : MonoBehaviour {

	//---------------------------
	// Types
	//---------------------------
	enum State {
		None,
		Drawing,
		Replaying,
		Disabled
	}
	
	//---------------------------
	// Private variables
	//---------------------------
	State _state = State.None;
	LineRenderer _lineRenderer;

	Vector3[] _recordedPositions = new Vector3[60 * 5];
	Vector3 _startPosition = new Vector3();
	Vector3 _oldPos;
	int _recordIndex = 0;
	int _replayIndex = 0;
	Vector3 _multiplier = Vector3.one;
	int _multiplierIndexX = -1;
	int _multiplierIndexY = -1;

	//---------------------------
	// Initialization
	//---------------------------
	void Start () {
		_lineRenderer = GetComponent<LineRenderer>();
		_lineRenderer.enabled = false;

		for (var i = 0; i < _recordedPositions.Length; i++) {
			_recordedPositions[i] = Vector3.zero;
		}
	}

	//---------------------------
	// Drawing
	//---------------------------
	void StartDrawing(Vector2 position) {
		_state = State.Drawing;
		_lineRenderer.enabled = true;
		_recordIndex = 0;

		_startPosition.x = position.x;
		_startPosition.y = position.y;
	}
	
	void UpdateDrawing(Vector2 position) {
		// Prevent drawing if we are out of space
		if (_recordIndex >= _recordedPositions.Length) {
			return;
		}

		// Calculate next recorded position
		_recordedPositions[_recordIndex] = Camera.main.ScreenToWorldPoint(position);
		_recordedPositions[_recordIndex].z = 1;

		// Prevent very close distances to screw up the rendering
		if (_recordIndex > 0 && Vector3.Distance(_recordedPositions[_recordIndex], _recordedPositions[_recordIndex - 1]) < 0.1f) {
			_recordedPositions[_recordIndex] = Vector3.zero;
			return;
		}

		// Set positions
		for (var i = 0; i < _lineRenderer.numPositions; i++) {
			var idx = _recordIndex - i;
			if (idx < 0) {
				// break;
				idx = 0;
			}
			_lineRenderer.SetPosition(i, _recordedPositions[idx]);
		}

		_recordIndex++;
	}
	
	public void StopDrawing() {
		_state = State.Replaying;

		_startPosition = _recordedPositions[_recordIndex - 1];
		_oldPos = _recordedPositions[0];

		transform.position = _recordedPositions[_recordIndex - 1];
	}

	public void ClearDrawing() {
		_state = State.None;
		for (var i = 0; i < _lineRenderer.numPositions; i++) {
			_lineRenderer.SetPosition(i, Vector3.zero);
		}

		for (var i = 0; i < _recordedPositions.Length; i++) {
			_recordedPositions[i] = Vector3.zero;
		}
		_recordIndex = 0;
		_replayIndex = 0;
	}

	public void HandleBorderCollision(bool isVertical) {
		if (isVertical) {
			_multiplier.y = -_multiplier.y;
			_multiplierIndexY = _replayIndex;
		} else {
			_multiplier.x = -_multiplier.x;
			_multiplierIndexX = _replayIndex;
		}
	}

	void DrawReplay() {
		if (_recordIndex <= 0) {
			for (var i = 0; i < _lineRenderer.numPositions; i++) {
				_lineRenderer.SetPosition(i, Vector3.zero);
			}
			return;
		}
		
		// Save first position
		var p = _recordedPositions[0];

		// Shift every point to the left
		for (var i = 0; i < _recordIndex - 1; i++) {
			_recordedPositions[i] = _recordedPositions[i + 1];
		}

		// Save new last position (which would be the first position positioned at the end)
		_recordedPositions[_recordIndex - 1] = Vector3.Scale (p - _oldPos, _multiplier) + _startPosition;
	
		// Draw the line
		for (var i = 0; i < _lineRenderer.numPositions; i++) {
			var idx = _recordIndex - i - 1;
			if (idx < 0) {
				idx = 0;
			}
			_lineRenderer.SetPosition(i, _recordedPositions[idx]);
		}

		// Advance replay
		_replayIndex++;
		if (_replayIndex == _recordIndex) {
			_startPosition = _recordedPositions[_replayIndex - 1];
			_oldPos = _recordedPositions[0];
			_replayIndex = 0;
		}

		// Swap multipliers
		if (_multiplierIndexX != -1 && _multiplierIndexX == _replayIndex) {
			_multiplier.x = 1;
			_multiplierIndexX = -1;
		}
		if (_multiplierIndexY != -1 && _multiplierIndexY == _replayIndex) {
			_multiplier.y = 1;
			_multiplierIndexY = -1;
		}

		transform.position = _recordedPositions[_recordIndex - 1];
	}
	//---------------------------
	// Enabling
	//---------------------------
	public void Enable() {
		_state = State.None;
	}
	
	public void Disable() {
		_state = State.Disabled;

	}
	//---------------------------
	// Update
	//---------------------------
	void FixedUpdate () {
		switch (_state) {
			case State.None:
				if (Input.GetMouseButtonDown(0)) {
					StartDrawing(Input.mousePosition);
				}
				break;
			case State.Drawing:
				if (Input.GetMouseButtonUp(0)) {
					StopDrawing();
				} else {
					UpdateDrawing(Input.mousePosition);
				}
				break;
			case State.Replaying:
				DrawReplay();

				if (Input.GetMouseButtonDown(0)) {
					ClearDrawing();
					StartDrawing(Input.mousePosition);
				}
				break;
			case State.Disabled:
				DrawReplay();
				break;
		}
	}
}
