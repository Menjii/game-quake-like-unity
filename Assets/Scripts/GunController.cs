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

[RequireComponent(typeof(MDLAnimator))]
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
    public float range = 100f;

    [SerializeField]
    Camera fpsCam;
    //public ParticleSystem muzzleflash;
    MDLAnimator m_animator;
    AudioSource m_audioSource;

    float m_shootElapsed;
    float m_shootDelayed;
    bool m_fire;

    [SerializeField]
    GameObject m_hit;

    [SerializeField]
    LayerMask m_shootingMask;

    [SerializeField]
    Transform m_shootingOrigin;
    // Start is called before the first frame update
    void Awake()
    {
        m_animator = GetComponent<MDLAnimator>();
        m_audioSource = GetComponent<AudioSource>();
    }
    void Start()
    {
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

        m_fire = Input.GetButton("Fire1");

        if (m_fire) {

            if (m_shootElapsed > m_shootDelayed) {
                m_shootElapsed = 0.0f;
                Shoot();
            }
        }
            
    }

    void Shoot(){
        //muzzleflash.Play();
        if (!m_animator.isAnimationPlaying)
        {
            m_animator.PlayAnimation(m_weaponInfo.shotAnimation, ShootAnimationFinished);
        }

        m_audioSource.Play();

        Ray ray = fpsCam.ViewportPointToRay(new Vector2(0.5f, 0.5f));
        if (m_weaponInfo.projectile != null) {
            var rotation = Quaternion.LookRotation(ray.direction);
            Instantiate(m_weaponInfo.projectile, m_shootingOrigin.transform.position, rotation);
        } else {
        RaycastHit hit;
        //muzzleFlash.Play();
        
            if (Physics.Raycast(ray, out hit, range, m_shootingMask)) {
                Debug.Log(hit.transform.name);

                var target = hit.collider.GetComponent<EnemyAiTutorial>();
                if (target != null) {
                    target.TakeDamage(m_weaponInfo.damage);
                }

                Instantiate(m_hit, hit.point, Quaternion.LookRotation(-ray.direction));
            }
        }
    }

    public void ChangeWeapon(WeaponType weaponType) {
        m_weaponType = weaponType;
        m_weaponInfo = m_weapons[(int) weaponType];

        #if UNITY_EDITOR
        m_animator = GetComponent<MDLAnimator>();
        #endif
        
        m_animator.model = m_weaponInfo.model; 
        m_audioSource.clip = m_weaponInfo.shotSound;
        m_shootDelayed = 60.0f / m_weaponInfo.shotsPerMinute;
        
    }

    void ShootAnimationFinished()
    {
        if (m_fire)
        {
            m_animator.PlayAnimation(m_weaponInfo.shotAnimation, ShootAnimationFinished);
        }
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
