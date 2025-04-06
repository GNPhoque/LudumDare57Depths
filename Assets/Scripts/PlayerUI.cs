using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
	[SerializeField] public Sprite checkSprite;
	[SerializeField] public Sprite crossSprite;
	[SerializeField] public Image photoFeedback;

	[SerializeField] public GameObject dialogueBox;
	[SerializeField] public TextMeshProUGUI dialogueText;

	[SerializeField] public TextMeshProUGUI focalText;
	[SerializeField] public TextMeshProUGUI apertureText;
	[SerializeField] public TextMeshProUGUI focusText;

	[SerializeField] public GameObject photoDataUIContainer;

	[SerializeField] public PhotoDataUI photoPositionUI;
	[SerializeField] public PhotoDataUI photoRotationUI;
	[SerializeField] public PhotoDataUI photoFocalLengthUI;
	[SerializeField] public PhotoDataUI photoApertureUI;
	[SerializeField] public PhotoDataUI photoFocusUI;

	[SerializeField] public RectTransform objectivePhotoContainer;
	[SerializeField] public RectTransform[] objectivePhotoPositionUIs;
	[SerializeField] public GameObject[] objectivePhotoChecks;
	[SerializeField] public float[] objectivePhotoHeights;

	[SerializeField] GameObject focalSettingSelected;
	[SerializeField] GameObject apertureSettingSelected;
	[SerializeField] GameObject focusSettingSelected;

	[SerializeField] private GameObject tutorial;

	public bool isWaitingInput => isDialogOpen || isTutorialOpen;

	private bool isDialogOpen;
	private bool isTutorialOpen;
	private int currentFirstObjectivePhoto;
	public List<string> dialogs = new List<string>();

	public event Action OnDialogCompleted;

	public void ContinueUI()
	{
		if (isDialogOpen)
		{
			ContinueDialog();
		}
		else if (isTutorialOpen)
		{
			ContinueTutorial();
		}
	}

	public void SetDialogs(List<string> newDialogs)
	{
		dialogs = newDialogs;
		ContinueDialog();
	}

	private void ContinueDialog()
	{
		if (dialogs.Count != 0)
		{
			isDialogOpen = true;
			dialogueBox.SetActive(true);
			dialogueText.text = dialogs[0];
			dialogs.RemoveAt(0);
		}
		else		
		{
			isDialogOpen = false;
			dialogueBox.SetActive(false);
			OnDialogCompleted?.Invoke();
		}
	}

	public void ShowTutorial()
	{
		isTutorialOpen = true;
		tutorial.SetActive(true);
	}

	private void ContinueTutorial()
	{
		isTutorialOpen = false;
		tutorial.SetActive(false);
	}

	public void ShowObjectivePhotos()
	{
		objectivePhotoContainer.anchoredPosition = new Vector2(objectivePhotoContainer.anchoredPosition.x, 0f);
	}

	[ContextMenu("Cycle")]
	public void CycleObjectivePhotos()
	{
		objectivePhotoPositionUIs[currentFirstObjectivePhoto].anchoredPosition = new Vector2(0f, 0f);
		objectivePhotoPositionUIs[currentFirstObjectivePhoto].SetSiblingIndex(0);

		currentFirstObjectivePhoto = currentFirstObjectivePhoto + 1 >= objectivePhotoPositionUIs.Length ? 0 : currentFirstObjectivePhoto + 1;
		for (int i = 0; i < objectivePhotoPositionUIs.Length; i++)
		{
			RectTransform rect =objectivePhotoContainer.GetChild(i).GetComponent<RectTransform>();
			Vector2 anchorPos = rect.anchoredPosition;
			rect.anchoredPosition = new Vector2(anchorPos.x, objectivePhotoHeights[i]);
		}
	}

	public void CheckPhoto()
	{
		objectivePhotoChecks[currentFirstObjectivePhoto].SetActive(true);
	}

	public void ShowSelectedFocal()
	{
		focalSettingSelected.SetActive(true);
		apertureSettingSelected.SetActive(false);
		focusSettingSelected.SetActive(false);
	}

	public void ShowSelectedAperture()
	{
		focalSettingSelected.SetActive(false);
		apertureSettingSelected.SetActive(true);
		focusSettingSelected.SetActive(false);
	}

	public void ShowSelectedFocus()
	{
		focalSettingSelected.SetActive(false);
		apertureSettingSelected.SetActive(false);
		focusSettingSelected.SetActive(true);
	}

	public void ShowSelectedNone()
	{
		focalSettingSelected.SetActive(false);
		apertureSettingSelected.SetActive(false);
		focusSettingSelected.SetActive(false);
	}
}
