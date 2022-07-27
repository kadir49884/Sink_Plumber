using UnityEngine;
using System.Collections;

public class Execute
{
	public bool isDone;
	public bool abort;
	private IEnumerator action;
	public System.Action onGUIaction = null;
	
	
	#region Run.After
	public static Execute After(float aDelay, System.Action aAction)
	{
		var tmp = new Execute();
		tmp.action = _RunAfter(tmp, aDelay, aAction);
		tmp.Start();
		return tmp;
	}
	private static IEnumerator _RunAfter(Execute aRun, float aDelay, System.Action aAction)
	{
		aRun.isDone = false;
		yield return new WaitForSeconds(aDelay);
		if (!aRun.abort && aAction != null)
			aAction();
		aRun.isDone = true;
	}
	#endregion Run.After
	
	#region Run.Lerp
	public static Execute Lerp(float aDuration, System.Action<float> aAction)
	{
		var tmp = new Execute();
		tmp.action = _RunLerp(tmp, aDuration, aAction);
		tmp.Start();
		return tmp;
	}
	private static IEnumerator _RunLerp(Execute aRun, float aDuration, System.Action<float> aAction)
	{
		aRun.isDone = false;
		float t = 0f;
		while (t < 1.0f)
		{
			t = Mathf.Clamp01(t + Time.deltaTime / aDuration);
			if (!aRun.abort && aAction != null)
				aAction(t);
			yield return null;
		}
		aRun.isDone = true;
		
	}
	#endregion Run.Lerp
	
	
	

		
	private void Start()
	{
		if (action != null)
		CoroutineHelper.Instance.StartCoroutine(action);
	}
	
	public Coroutine WaitFor
	{
		get
		{
			return CoroutineHelper.Instance.StartCoroutine(_WaitFor(null));
		}
	}
	public IEnumerator _WaitFor(System.Action aOnDone)
	{
		while(!isDone)
			yield return null;
		if (aOnDone != null)
			aOnDone();
	}
	public void Abort()
	{
		abort = true;
	}
	public Execute ExecuteWhenDone(System.Action aAction)
	{
		var tmp = new Execute();
		tmp.action = _WaitFor(aAction);
		tmp.Start();
		return tmp;
	}
}