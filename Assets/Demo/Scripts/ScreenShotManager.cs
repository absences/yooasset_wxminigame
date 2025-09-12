using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// ReSharper disable once HollowTypeName
public class ScreenShotManager : Singleton<ScreenShotManager> {
    private void Start()
    {
        Debug.Log(Application.persistentDataPath);
    }

    // Update is called once per frame
	void LateUpdate ()
	{
	    if (Input.GetKeyDown(KeyCode.C))
	    {
	        StartCoroutine(TakeScreenShotCo());
	    }
	}

    private IEnumerator TakeScreenShotCo()
    {
        yield return new WaitForEndOfFrame();

        var directory = new DirectoryInfo(Application.persistentDataPath);
        // ReSharper disable once PossibleNullReferenceException
        var path = Path.Combine(directory.Parent.FullName, $"Screenshot_{DateTime.Now:yyyyMMdd_Hmmss}.png");
     
        ScreenCapture.CaptureScreenshot(path,1);
        Debug.Log($"Screen Shot Captured");
    }
}
