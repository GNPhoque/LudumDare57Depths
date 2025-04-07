using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

	[Space(10)]
	[SerializeField] private DialogsSO introDialog;
	[SerializeField] private DialogsSO correctPhotoLines;
	[SerializeField] private DialogsSO failedPhotoLines;
	[SerializeField] private DialogsSO failedPhotoSettingLines;
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

	private bool isPhotoOk;
	public bool isCheckingPhoto;
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
		playerUI.SetObjectivePhotos(objectivePhotos);

		cameraSettings.aperture = 4f;
		dof.aperture.value = cameraSettings.aperture;
		Camera.main.aperture = cameraSettings.aperture;
		playerUI.apertureText.text = $"Aperture : {cameraSettings.aperture}";

		cameraSettings.focalLength = 20f;
		dof.focalLength.value = cameraSettings.focalLength + 40f;
		Camera.main.focalLength = cameraSettings.focalLength;
		playerUI.focalText.text = $"Focal : {cameraSettings.focalLength}";

		dof.focusDistance.value = 10f;
		playerUI.focusText.text = $"Focus : {dof.focusDistance.value.ToString("F1")}";

		playerUI.SetDialogs(new List<string>(introDialog.lines));
		playerUI.OnDialogCompleted += PlayerUI_OnDialogCompleted;
	}

	private void PlayerUI_OnDialogCompleted()
	{
		playerUI.OnDialogCompleted -= PlayerUI_OnDialogCompleted;
		//Show pictures
		playerUI.ShowObjectivePhotos();
		//Show tutorial
		playerUI.ShowTutorial();
	}

	private SelectedSetting selectedSetting;

	public void DeselectSetting()
	{
		selectedSetting = SelectedSetting.None;
	}

	public void SelectAperture()
	{
		if (selectedSetting != SelectedSetting.Aperture)
		{
			selectedSetting = SelectedSetting.Aperture;
			playerUI.ShowSelectedAperture();
		}
		else
		{
			selectedSetting = SelectedSetting.None;
			playerUI.ShowSelectedNone();
		}
	}

	public void SelectFocalLength()
	{
		if (selectedSetting != SelectedSetting.FocalLength)
		{
			selectedSetting = SelectedSetting.FocalLength;
			playerUI.ShowSelectedFocal();
		}
		else
		{
			selectedSetting = SelectedSetting.None;
			playerUI.ShowSelectedNone();
		}
	}

	public void IncreaseSetting()
	{
		switch (selectedSetting)
		{
			case SelectedSetting.None:
				break;
			case SelectedSetting.Aperture:
				AudioManager.instance.PlayClic();
				cameraSettings.aperture += apertureStep;
				dof.aperture.value = cameraSettings.aperture;
				Camera.main.aperture = cameraSettings.aperture;
				playerUI.apertureText.text = $"Aperture : {cameraSettings.aperture}";
				break;
			case SelectedSetting.FocalLength:
				AudioManager.instance.PlayClic();
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
				AudioManager.instance.PlayClic();
				cameraSettings.aperture -= apertureStep;
				dof.aperture.value = cameraSettings.aperture;
				Camera.main.aperture = cameraSettings.aperture;
				playerUI.apertureText.text = $"Aperture : {cameraSettings.aperture}";
				break;
			case SelectedSetting.FocalLength:
				AudioManager.instance.PlayClic();
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
			AudioManager.instance.PlayFocus();
			dof.focusDistance.value = hit.distance;
			playerUI.focusText.text = $"Focus : {dof.focusDistance.value.ToString("00.0")}";

			playerUI.CollimatorAnimation();
		}
	}

	public IEnumerator TakePhoto()
	{
		isCheckingPhoto = true;
		AudioManager.instance.PlayTrigger();
		if (debugSave != null) debugSave.TakeScreenshot();
		Sprite sprite = takeScreenshot.TakePhoto();
		if (DEBUGObjectivePhoto != null) DEBUGObjectivePhoto.SetPhoto(sprite, transform.position, transform.rotation.eulerAngles, cameraSettings.focalLength, cameraSettings.aperture, dof.focusDistance.value);
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

		isPhotoOk = model.IsAccepted();
		List<string> dialogStrings = new List<string>();

		if (isPhotoOk)
		{
			dialogStrings.Add(correctPhotoLines.lines[Random.Range(0, correctPhotoLines.lines.Count())]);
		}
		else
		{
			dialogStrings.AddRange(failedPhotoLines.lines);

			if (model.position == false)
			{
				dialogStrings.Add(failedPhotoSettingLines.lines[0]);
			}
			if (model.rotation == false)
			{
				dialogStrings.Add(failedPhotoSettingLines.lines[1]);
			}
			if (model.focalLength == false)
			{
				dialogStrings.Add(failedPhotoSettingLines.lines[2]);
			}
			if (model.aperture == false)
			{
				dialogStrings.Add(failedPhotoSettingLines.lines[3]);
			}
			if (model.focusDistance == false)
			{
				dialogStrings.Add(failedPhotoSettingLines.lines[4]);
			}

			string selectedDialog = dialogStrings[Random.Range(0, dialogStrings.Count)];
			dialogStrings = new List<string>() { selectedDialog };
		}

		playerUI.SetDialogs(dialogStrings);
		playerUI.OnDialogCompleted += ClosePhotoReport;
		isCheckingPhoto = false;
	}

	private void ClosePhotoReport()
	{
		playerUI.OnDialogCompleted -= ClosePhotoReport;
		playerUI.photoDataUIContainer.SetActive(false);
		playerUI.photoFeedback.gameObject.SetActive(false);

		if (isPhotoOk)
		{
			playerUI.CheckPhoto();
		}
	}

	public void CycleObjectives()
	{
		if (playerUI.isCycling)
		{
			return;
		}
		currentObjectivePhoto = currentObjectivePhoto + 1 >= objectivePhotos.Length ? 0 : currentObjectivePhoto + 1;
		playerUI.CycleObjectivePhotos();
	}
}
