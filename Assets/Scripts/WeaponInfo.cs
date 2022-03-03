using UnityEngine;
using System.Collections;

[CreateAssetMenu]
public class WeaponInfo : ScriptableObject
{
    [SerializeField]
    public MDL model;
    
    [SerializeField]
    public int damage;

    [SerializeField]
    public int shotsPerMinute;

    [SerializeField]
    public int maxAmmo;
}
