using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public enum SelectedSetting
{
	None,
	Aperture,
	FocalLength
}

public class CameraController : MonoBehaviour
{
	[SerializeField] private PhotoData DEBUGObjectivePhoto;
	[SerializeField] private DebugSaveScreenshot debugSave;

	[SerializeField] private PhotoData[] objectivePhotos;
	[SerializeField] private PhotoData currentPhoto;
	[SerializeField] private int currentObjectivePhoto;
	[SerializeField] private float apertureStep;
	[SerializeField] private float focalLengthStep;
	[SerializeField] private LayerMask antiPlayerLayerMask;
	[SerializeField] private Volume volume;
	[SerializeField] private CameraSettings cameraSettings;
	[SerializeField] private TakeScreenshot takeScreenshot;
	[SerializeField] private PlayerUI playerUI;

	private DepthOfField dof;
	private WaitForSeconds wfs = new WaitForSeconds(1f);

	private void Awake()
	{
		if (!volume.sharedProfile.TryGet<DepthOfField>(out dof))
		{
			Debug.LogError("COULD NOT GET DEPTH OF FIELD VOLUME PROFILE");
		}
	}

	private void Start()
	{
		dof.aperture.value = cameraSettings.aperture;
		Camera.main.aperture = cameraSettings.aperture;
		playerUI.apertureText.text = $"Aperture : {cameraSettings.aperture}";
		
		dof.focalLength.value = cameraSettings.focalLength + 40f;
		Camera.main.focalLength = cameraSettings.focalLength;
		playerUI.focalText.text = $"Focal : {cameraSettings.focalLength}";

		dof.focusDistance.value = 10f;
		playerUI.focusText.text = $"Focus : {dof.focusDistance.value.ToString("F1")}";
	}

	private SelectedSetting selectedSetting;

	public void DeselectSetting()
	{
		selectedSetting = SelectedSetting.None;
	}

	public void SelectAperture(bool selected)
	{
		selectedSetting = selected ? SelectedSetting.Aperture : SelectedSetting.None;
	}

	public void SelectFocalLength(bool selected)
	{
		selectedSetting = selected ? SelectedSetting.FocalLength : SelectedSetting.None;
	}

	public void IncreaseSetting()
	{
		switch (selectedSetting)
		{
			case SelectedSetting.None:
				break;
			case SelectedSetting.Aperture:
				cameraSettings.aperture += apertureStep;
				dof.aperture.value = cameraSettings.aperture;
				Camera.main.aperture = cameraSettings.aperture;
				playerUI.apertureText.text = $"Aperture : {cameraSettings.aperture}";
				break;
			case SelectedSetting.FocalLength:
				cameraSettings.focalLength += focalLengthStep;
				dof.focalLength.value = cameraSettings.focalLength + 40f;
				Camera.main.focalLength = cameraSettings.focalLength;
				playerUI.focalText.text = $"Focal : {cameraSettings.focalLength}";
				break;
			default:
				break;
		}
	}

	public void DecreaseSetting()
	{
		switch (selectedSetting)
		{
			case SelectedSetting.None:
				break;
			case SelectedSetting.Aperture:
				cameraSettings.aperture -= apertureStep;
				dof.aperture.value = cameraSettings.aperture;
				Camera.main.aperture = cameraSettings.aperture;
				playerUI.apertureText.text = $"Aperture : {cameraSettings.aperture}";
				break;
			case SelectedSetting.FocalLength:
				cameraSettings.focalLength -= focalLengthStep;
				dof.focalLength.value = cameraSettings.focalLength + 40f;
				Camera.main.focalLength = cameraSettings.focalLength;
				playerUI.focalText.text = $"Focal : {cameraSettings.focalLength}";
				break;
			default:
				break;
		}
	}

	public void AutoFocus()
	{
		RaycastHit hit;
		if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 500f, antiPlayerLayerMask))
		{
			dof.focusDistance.value = hit.distance;
			playerUI.focusText.text = $"Focus : {dof.focusDistance.value.ToString("00.0")}";
		}
	}

	public IEnumerator TakePhoto()
	{
		if (debugSave != null) debugSave.TakeScreenshot();
		Sprite sprite = takeScreenshot.TakePhoto();
		if(DEBUGObjectivePhoto!= null) DEBUGObjectivePhoto.SetPhoto(sprite, transform.position, transform.rotation.eulerAngles, cameraSettings.focalLength, cameraSettings.aperture, dof.focusDistance.value);
		currentPhoto.SetPhoto(sprite, transform.position, transform.rotation.eulerAngles, cameraSettings.focalLength, cameraSettings.aperture, dof.focusDistance.value);
		playerUI.photoFeedback.sprite = sprite;
		playerUI.photoFeedback.gameObject.SetActive(true);
		PhotoAcceptationModel model = objectivePhotos[currentObjectivePhoto].IsPhotoAccepted(currentPhoto);

		playerUI.photoDataUIContainer.SetActive(true);
		playerUI.photoPositionUI.gameObject.SetActive(false);
		playerUI.photoRotationUI.gameObject.SetActive(false);
		playerUI.photoFocalLengthUI.gameObject.SetActive(false);
		playerUI.photoApertureUI.gameObject.SetActive(false);
		playerUI.photoFocusUI.gameObject.SetActive(false);

		yield return wfs;
		playerUI.photoPositionUI.image.sprite = model.position ? playerUI.checkSprite : playerUI.crossSprite;
		playerUI.photoPositionUI.gameObject.SetActive(true);
		yield return wfs;
		playerUI.photoRotationUI.image.sprite = model.rotation ? playerUI.checkSprite : playerUI.crossSprite;
		playerUI.photoRotationUI.gameObject.SetActive(true);
		yield return wfs;
		playerUI.photoFocalLengthUI.image.sprite = model.focalLength ? playerUI.checkSprite : playerUI.crossSprite;
		playerUI.photoFocalLengthUI.gameObject.SetActive(true);
		yield return wfs;
		playerUI.photoApertureUI.image.sprite = model.aperture ? playerUI.checkSprite : playerUI.crossSprite;
		playerUI.photoApertureUI.gameObject.SetActive(true);
		yield return wfs;
		playerUI.photoFocusUI.image.sprite = model.focusDistance ? playerUI.checkSprite : playerUI.crossSprite;
		playerUI.photoFocusUI.gameObject.SetActive(true);
		yield return wfs;
		yield return wfs;
		yield return wfs;
		playerUI.photoDataUIContainer.SetActive(false);
		playerUI.photoFeedback.gameObject.SetActive(false);

		if (model.IsAccepted())
		{
			playerUI.CheckPhoto();
		}
	}

	public void CycleObjectives()
	{
		currentObjectivePhoto = currentObjectivePhoto + 1 >= objectivePhotos.Length ? 0 : currentObjectivePhoto + 1;
		playerUI.CycleObjectivePhotos();
	}
}
