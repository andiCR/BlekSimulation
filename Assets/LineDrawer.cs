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
	}

	void DrawReplay() {
		// var i = 0;//_replayIndex % _lineRenderer.numPositions;
		// for (; i < _replayIndex; i++) {
		// 	var idx = i % _lineRenderer.numPositions; // Indice del line renderer que ocupo
		// 	_lineRenderer.SetPosition(idx, _recordedPositions[i] - _recordedPositions[0] + _startPosition);
		// }
		for (var i = 0; i < _lineRenderer.numPositions; i++) {
			var idx = _replayIndex - i;
			Vector3 pos;
			if (idx < 0) {
				idx = _recordIndex + idx;
				pos = _recordedPositions[idx];
			} else {
				pos = _recordedPositions[i] -_recordedPositions[0] + _startPosition;
			}
			_lineRenderer.SetPosition(i, pos);
		}

		_replayIndex++;
		if (_replayIndex == _recordIndex) {
			_startPosition = _recordedPositions[_replayIndex - 1] - _recordedPositions[0] + _startPosition;
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
