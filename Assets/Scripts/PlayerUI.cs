using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
	[SerializeField] public Sprite checkSprite;
	[SerializeField] public Sprite crossSprite;
	[SerializeField] public Image photoFeedback;

	[SerializeField] public TextMeshProUGUI focalText;
	[SerializeField] public TextMeshProUGUI apertureText;
	[SerializeField] public TextMeshProUGUI focusText;

	[SerializeField] public GameObject photoDataUIContainer;

	[SerializeField] public PhotoDataUI photoPositionUI;
	[SerializeField] public PhotoDataUI photoRotationUI;
	[SerializeField] public PhotoDataUI photoFocalLengthUI;
	[SerializeField] public PhotoDataUI photoApertureUI;
	[SerializeField] public PhotoDataUI photoFocusUI;

	[SerializeField] public Transform objectivePhotoContainer;
	[SerializeField] public RectTransform[] objectivePhotoPositionUIs;
	[SerializeField] public GameObject[] objectivePhotoChecks;
	[SerializeField] public float[] objectivePhotoHeights;

	private int currentFirstObjectivePhoto;

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
}
