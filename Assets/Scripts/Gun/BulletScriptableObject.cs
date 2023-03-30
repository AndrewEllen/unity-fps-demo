using UnityEngine;

[CreateAssetMenu(fileName = "BulletData", menuName = "ScriptableObjects/CreateBullet", order = 1)]
public class BulletScriptableObject : ScriptableObject
{
    public GameObject myBullet;


    public BulletScriptableObject()
    {
        myBullet = null;
    }
}
