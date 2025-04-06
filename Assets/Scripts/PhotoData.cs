using UnityEngine;

public class PhotoAcceptationModel
{
	public bool position;
	public bool rotation;
	public bool focalLength;
	public bool aperture;
	public bool focusDistance;

	public bool IsAccepted()
	{
		return position && rotation && focalLength && aperture && focusDistance;
	}
}

[CreateAssetMenu(fileName = "PhotoData")]
public class PhotoData : ScriptableObject
{
	[SerializeField] public Sprite sprite;
	[SerializeField] Vector3 position;
	[SerializeField] Vector3 rotation;
	[SerializeField] float focalLength;
	[SerializeField] float aperture;
	[SerializeField] float focusDistance;

	[SerializeField] float positionApprox;
	[SerializeField] float rotationApprox;
	[SerializeField] float focalLengthApprox;
	[SerializeField] float apertureApprox;
	[SerializeField] float focusDistanceApprox;

	public void SetPhoto(Sprite sprite, Vector3 position, Vector3 rotation, float focalLength, float aperture, float focusDistance)
	{
		this.sprite = sprite;
		this.position = position;
		this.rotation = rotation;
		this.focalLength = focalLength;
		this.aperture = aperture;
		this.focusDistance = focusDistance;
	}

	public PhotoAcceptationModel IsPhotoAccepted(PhotoData data)
	{
		PhotoAcceptationModel model = new PhotoAcceptationModel();
		model.position = Vector2.Distance(new Vector2(data.position.x,data.position.z), new Vector2(position.x,position.z)) < positionApprox;
		model.rotation = Vector3.Distance(data.rotation, rotation) < rotationApprox;
		model.focalLength = data.focalLength - focalLength < focalLengthApprox;
		model.aperture = data.aperture - aperture < apertureApprox;
		model.focusDistance = data.focusDistance - focusDistance < focusDistanceApprox;

		return model;
	}
}
