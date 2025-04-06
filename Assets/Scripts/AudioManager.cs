using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
	[SerializeField] private AudioSource audioSourceSFX;
	[SerializeField] private AudioSource audioSourceBlablabla;

	[SerializeField] private AudioClip[] clics;
	[SerializeField] private AudioClip[] swipes;
	[SerializeField] private AudioClip focus;
	[SerializeField] private AudioClip trigger;
	[SerializeField] private AudioClip[] blablablas;

	public static AudioManager instance;

	private void Awake()
	{
		if(instance != null)
		{
			Destroy(gameObject);
			return;
		}

		instance = this;
	}

	private float GetRandomPitch()
	{
		return Random.Range(.8f, 1.2f);
	}

	public void PlayClic()
	{
		audioSourceSFX.pitch = GetRandomPitch();
		audioSourceSFX.PlayOneShot(clics[Random.Range(0, clics.Length)]);
	}

	public void PlaySwipe()
	{
		audioSourceSFX.pitch = GetRandomPitch();
		audioSourceSFX.PlayOneShot(swipes[Random.Range(0, swipes.Length)]);
	}

	public void PlayFocus()
	{
		audioSourceSFX.pitch = 1f;
		audioSourceSFX.PlayOneShot(focus);
	}

	public void PlayTrigger()
	{
		audioSourceSFX.pitch = GetRandomPitch();
		audioSourceSFX.PlayOneShot(trigger);
	}

	public void PlayBlablabla()
	{
		audioSourceBlablabla.Stop();
		audioSourceBlablabla.pitch = GetRandomPitch();
		audioSourceBlablabla.clip = blablablas[Random.Range(0, blablablas.Length)];
		audioSourceBlablabla.Play();
	}
}
