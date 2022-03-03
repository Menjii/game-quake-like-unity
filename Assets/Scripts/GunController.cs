using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    Axe,
    Shotgun,
    SuperShotgun,
    Nailgun,
    SuperNailgun,
    GrenadeLancher,
    RocketLauncher,
    Lighting
}

public class GunController : MonoBehaviour
{
    static readonly KeyCode[] kWeaponKeys = {
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5,
        KeyCode.Alpha6,
        KeyCode.Alpha7,
        KeyCode.Alpha8,
    };

    [SerializeField]
    WeaponInfo[] m_weapons;

    [SerializeField]
    WeaponType m_weaponType;

    WeaponInfo m_weaponInfo;
    public float damage = 10f;
    public float range = 100f;

    public Camera fpsCam;
    //public ParticleSystem muzzleflash;
    Animator m_animator;

    float m_shootElapsed;
    float m_shootDelayed;
    bool m_fire;
    // Start is called before the first frame update
    void Start()
    {
        m_animator = GetComponent<Animator>();
        ChangeWeapon(m_weaponType);
    }

    // Update is called once per frame
    void Update()
    {
        m_shootElapsed += Time.deltaTime;

        for (int i = 0; i < kWeaponKeys.Length; ++i) {
            
            if (Input.GetKeyDown(kWeaponKeys[i])) {
                ChangeWeapon((WeaponType) i);
            }
        }

        m_fire = Input.GetButtonDown("Fire1");

        if (m_fire) {

            if (m_shootElapsed > m_shootDelayed) {
                m_shootElapsed = 0.0f;
                Shoot();
            }
        }
            
    }

    void Shoot(){
        //muzzleflash.Play();

        RaycastHit hit;
        //muzzleFlash.Play();
        m_animator.SetTrigger("Shoot");
        
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range)) {
            Debug.Log(hit.transform.name);

            Target target = hit.transform.GetComponent<Target>();
            if (target != null) {
                target.TakeDamage(damage);
            }
        }
    }

    public void ChangeWeapon(WeaponType weaponType) {
        m_weaponType = weaponType;
        m_weaponInfo = m_weapons[(int) weaponType];

        m_shootDelayed = 60.0f / m_weaponInfo.shotsPerMinute;

        
    }

    public WeaponType weaponType
    {
        get { return m_weaponType; }
    }

    public WeaponInfo[] weapons
    {
        get { return m_weapons; }
    }
}
