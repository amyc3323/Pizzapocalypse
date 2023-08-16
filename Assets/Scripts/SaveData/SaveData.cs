using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface SaveData 
{
    public string ToJson();
    public bool LoadJson(string json);

}
