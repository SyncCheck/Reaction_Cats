﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowScript : MonoBehaviour {

	//offset from the viewport center to fix damping
	public float m_DampTime = 10f;
	//public static Transform m_Target;
	public static Vector2 GoToPosition = new Vector2(0, 0);
	public float m_XOffset = 0;
	public float m_YOffset = 0;

	private float margin = 0.1f;

	void Start () {
		//if (m_Target == null){
		//	m_Target = GameObject.FindGameObjectWithTag("Player").transform;
	    //}
	}

	void Update() {
			float targetX = GoToPosition.x + m_XOffset;
			float targetY = GoToPosition.y + m_YOffset;

			if (Mathf.Abs(transform.position.x - targetX) > margin)
				targetX = Mathf.Lerp(transform.position.x, targetX, m_DampTime * Time.deltaTime);

			if (Mathf.Abs(transform.position.y - targetY) > margin)
				targetY = Mathf.Lerp(transform.position.y, targetY, m_DampTime * Time.deltaTime);

			transform.position = new Vector3(targetX, targetY, transform.position.z);
		if (Input.GetMouseButtonDown (1)) {
			GoToPosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		}
	}
}
