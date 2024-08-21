using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class WeaponManagement : MonoBehaviour {
    public GameObject weapon1;
    public GameObject weapon2;
    public GameObject weapon3;
    public GameObject weapon4;

    public int currentWeapon = 1;

    private Rigidbody myRigidBody;

    [SerializeField] private Animator animator = null;

    private GameObject nozzleParent;
    private Transform currWeaponTransform;
    private GameObject activeWeapon;

    private GameObject leftHandObject;
    private Transform leftHandTransform;

    void Start() {
        myRigidBody = GetComponent<Rigidbody>();
        if (animator == null) animator = GetComponent<Animator>();
         // Find the "nozzle" GameObject by name
        nozzleParent = GameObject.Find(GameObject.Find("player_nozzle").transform.parent.gameObject.name);

        GameObject currWeapon = GameObject.Find("player_nozzle");
        currWeaponTransform = currWeapon.transform;

        Debug.Log("Nozzle Parent: " + nozzleParent);

        Debug.Log("Parent Name: " + GameObject.Find("player_nozzle").transform.parent.gameObject.name);

        leftHandTransform = transform.Find("Exo Gray/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:LeftShoulder/mixamorig:LeftArm/mixamorig:LeftForeArm/mixamorig:LeftHand");

        if (leftHandTransform != null)
        {
            leftHandObject = leftHandTransform.gameObject;
        } else {
            Debug.LogError("mixamorig:LeftHand not found in this GameObject's hierarchy.");
        }
    }

    void Update() {
        HandleInput();
    }

     private void HandleInput()
    {
        // Handle weapon switching
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchWeapon(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchWeapon(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchWeapon(3);
        }
        // if (Input.GetKeyDown(KeyCode.Alpha4))
        // {
        //     SwitchWeapon(4);
        // }
    }

    void SwitchWeapon(int weaponNum) 
    {
        Debug.Log("Switching weapon to weapon " + weaponNum);
        GameObject currWeapon = GameObject.Find("player_nozzle");
        if (currWeapon != null) 
        {
            currWeapon.SetActive(false);
        }

        if (activeWeapon) {
            activeWeapon.SetActive(false);
        }

        
        
        
        if (weaponNum == 1) {
            activeWeapon = Instantiate(weapon1);
            activeWeapon.transform.SetParent(leftHandTransform); 
            activeWeapon.transform.position = currWeaponTransform.position;
            activeWeapon.transform.rotation = currWeaponTransform.rotation;
            activeWeapon.SetActive(true);
        }
        if (weaponNum == 2) {
            activeWeapon = Instantiate(weapon2);
            activeWeapon.transform.SetParent(leftHandTransform);
            activeWeapon.transform.position = currWeaponTransform.position;
            activeWeapon.transform.rotation = currWeaponTransform.rotation;
            activeWeapon.SetActive(true);
            
        }
        if (weaponNum == 3) {
            activeWeapon = Instantiate(weapon3);
            activeWeapon.transform.SetParent(leftHandTransform); 
            activeWeapon.transform.position = currWeaponTransform.position;
            activeWeapon.transform.rotation = currWeaponTransform.rotation;
            activeWeapon.SetActive(true);
        }
        // if (weaponNum == 4) {
        //     activeWeapon = Instantiate(weapon4);
        //     activeWeapon.transform.SetParent(leftHandTransform); 
        //     activeWeapon.transform.position = currWeaponTransform.position;
        //     activeWeapon.transform.rotation = currWeaponTransform.rotation;
        //     activeWeapon.SetActive(true);
        // }

        PlayerShooting playerShooting = activeWeapon.GetComponent<PlayerShooting>();
        if (playerShooting != null)
        {
            playerShooting.animator = animator; 
        }
        
        if (currentWeapon == 1) {
            // Destroy(weapon1);
            weapon1.SetActive(false);
        }
        if (currentWeapon == 2) {
            weapon2.SetActive(false);
        }
        if (currentWeapon == 3) {
            weapon3.SetActive(false);
        }
        // if (currentWeapon == 4) {
        //     weapon4.SetActive(false);
        // }

        currentWeapon = weaponNum;
    }
}