using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
	[SerializeField] public Sprite checkSprite;
	[SerializeField] public Sprite crossSprite;
	[SerializeField] public Image photoFeedback;

	[SerializeField] public GameObject photoDataUIContainer;

	[SerializeField] public PhotoDataUI photoPositionUI;
	[SerializeField] public PhotoDataUI photoRotationUI;
	[SerializeField] public PhotoDataUI photoFocalLengthUI;
	[SerializeField] public PhotoDataUI photoApertureUI;
	[SerializeField] public PhotoDataUI photoFocusUI;
}
