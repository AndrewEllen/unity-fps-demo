using UnityEngine;

[CreateAssetMenu(fileName = "BulletData", menuName = "ScriptableObjects/CreateBullet", order = 1)]
public class BulletScriptableObject : ScriptableObject
{
    public int bob;


    public BulletScriptableObject()
    {
        bob = 1;
    }
}
