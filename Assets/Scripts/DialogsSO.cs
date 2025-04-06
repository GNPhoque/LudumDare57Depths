using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Dialogs")]
public class DialogsSO : ScriptableObject
{
	[SerializeField][TextArea] public List<string> lines;
}
