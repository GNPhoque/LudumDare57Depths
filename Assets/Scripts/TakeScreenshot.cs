using UnityEngine;

// Screen Recorder will save individual images of active scene in any resolution and of a specific image format
// including raw, jpg, png, and ppm.  Raw and PPM are the fastest image formats for saving.
//
// You can compile these images into a video using ffmpeg:
// ffmpeg -i screen_3840x2160_%d.ppm -y test.avi

public class TakeScreenshot : MonoBehaviour
{
	// 4k = 3840 x 2160   1080p = 1920 x 1080
	public int captureWidth = 1920;
	public int captureHeight = 1080;

	// optional game object to hide during screenshots (usually your scene canvas hud)
	public GameObject hideGameObject;

	// private vars for screenshot
	private Rect rect;
	private RenderTexture renderTexture;
	private Texture2D screenShot;

	public Sprite TakePhoto()
	{
		// hide optional game object if set
		if (hideGameObject != null) hideGameObject.SetActive(false);

		// create screenshot objects if needed
		if (renderTexture == null)
		{
			// creates off-screen render texture that can rendered into
			rect = new Rect(0, 0, captureWidth, captureHeight);
			renderTexture = new RenderTexture(captureWidth, captureHeight, 24);
			screenShot = new Texture2D(captureWidth, captureHeight, TextureFormat.RGB24, false);
		}

		// get main camera and manually render scene into rt
		Camera camera = this.GetComponent<Camera>(); // NOTE: added because there was no reference to camera in original script; must add this script to Camera
		camera.targetTexture = renderTexture;
		camera.Render();

		// read pixels will read from the currently active render texture so make our offscreen 
		// render texture active and then read the pixels
		RenderTexture.active = renderTexture;
		screenShot.ReadPixels(rect, 0, 0);
		screenShot.Apply();

		// reset active camera texture and render texture
		camera.targetTexture = null;
		RenderTexture.active = null;

		Sprite sprite = Sprite.Create(screenShot, rect, Vector2.zero);

		if (hideGameObject != null) hideGameObject.SetActive(true);
		return sprite;
	}
}