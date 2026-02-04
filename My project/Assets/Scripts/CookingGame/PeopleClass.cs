using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPerson", menuName = "People/Person")]
public class PersonData : ScriptableObject
{
    public string personNum;
    public List<Sprite> faces;
}
