using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
	public event Action Completed;
	public float Duration;
	public bool Looping;

	public bool Paused => m_Paused;
	public float Elapsed => m_Elapsed;
	public float Percentage => m_Elapsed / Duration;

	private bool m_Paused = true;
	private float m_Elapsed;

	public Timer(float duration = 1)
	{
		Duration = duration;
	}

	public void Start(float duration)
	{
		Duration = duration;
		Start();
	}

	public void Start()
	{
		m_Elapsed = 0;
		m_Paused = false;
	}

	public void Stop()
	{
		m_Elapsed = 0;
		m_Paused = true;
	}

	public void Reset()
	{
		m_Elapsed = 0;
	}

	public void Pause()
	{
		m_Paused = true;
	}

	public void Resume()
	{
		m_Paused = false;
	}

	public void Tick()
	{
		Tick(Time.deltaTime);
	}

	public void Tick(float deltaTime)
	{
		if (m_Paused)
		{
			return;
		}

		m_Elapsed += deltaTime;

		if (m_Elapsed < Duration)
		{
			return;
		}

		Completed?.Invoke();

		if (Looping)
		{
			Start();
		}
		else
		{
			Pause();
		}
	}
}
