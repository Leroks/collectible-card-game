using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineQueue
{
	/// <summary>
	/// Maximum number of coroutines to run at once
	/// </summary>
	readonly uint _maxActive;

	/// <summary>
	/// Delegate to start coroutines with
	/// </summary>
	readonly Func<IEnumerator, Coroutine> _coroutineStarter;

	/// <summary>
	/// Queue of coroutines waiting to start
	/// </summary>
	readonly Queue<IEnumerator> _queue;

	/// <summary>
	/// Number of currently active coroutines
	/// </summary>
	uint _numActive;

	public CoroutineQueue(uint maxActive, Func<IEnumerator, Coroutine> coroutineStarter)
	{
		if (maxActive == 0)
		{
			throw new ArgumentException("Must be at least one", "maxActive");
		}
		this._maxActive = maxActive;
		this._coroutineStarter = coroutineStarter;
		_queue = new Queue<IEnumerator>();
	}

	public void Run(IEnumerator coroutine)
	{
		if (_numActive < _maxActive)
		{
			var runner = CoroutineRunner(coroutine);
			_coroutineStarter(runner);
			return;
		}
		_queue.Enqueue(coroutine);
	}

	IEnumerator CoroutineRunner(IEnumerator coroutine)
	{
		_numActive++;
		while (coroutine.MoveNext())
			yield return coroutine.Current;
		_numActive--;
		if (_queue.Count > 0)
		{
			var next = _queue.Dequeue();
			Run(next);
		}
	}
}