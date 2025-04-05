using UnityEngine;
using System;

[Serializable]
public class CameraSettings
{
	[SerializeField] private float minFocalLength;
	[SerializeField] private float maxFocalLength;
	[SerializeField] private float minAperture;
	[SerializeField] private float maxAperture;

	[SerializeField] private float _focalLength;

	public float focalLength
	{
		get { return _focalLength; }
		set { _focalLength = Mathf.Clamp(value, minFocalLength, maxFocalLength) ; }
	}

	[SerializeField] private float _aperture;
	public float aperture
	{
		get { return _aperture; }
		set { _aperture = Mathf.Clamp(value, minAperture, maxAperture) ; }
	}

}
