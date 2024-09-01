using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeaponManagement : MonoBehaviour
{
    public GameObject weapon1;
    public GameObject weapon2;
    public GameObject weapon3;

    public int currentWeapon = 1;

    private Rigidbody myRigidBody;
    [SerializeField] private Animator animator = null;

    private GameObject nozzleParent;
    private Transform currWeaponTransform;
    private GameObject activeWeapon;

    private GameObject leftHandObject;
    private Transform leftHandTransform;

    void Start()
    {
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
        }
        else
        {
            Debug.LogError("mixamorig:LeftHand not found in this GameObject's hierarchy.");
        }

        // Initialize the weapon text
        UpdateWeaponText(currentWeapon);
    }

    void Update()
    {
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
    }

    void SwitchWeapon(int weaponNum)
    {
        Debug.Log("Switching weapon to weapon " + weaponNum);
        GameObject currWeapon = GameObject.Find("player_nozzle");
        if (currWeapon != null)
        {
            currWeapon.SetActive(false);
        }

        if (activeWeapon)
        {
            activeWeapon.SetActive(false);
        }

        switch (weaponNum)
        {
            case 1:
                activeWeapon = Instantiate(weapon1);
                break;
            case 2:
                activeWeapon = Instantiate(weapon2);
                break;
            case 3:
                activeWeapon = Instantiate(weapon3);
                break;
        }

        if (activeWeapon != null)
        {
            activeWeapon.transform.SetParent(leftHandTransform);
            activeWeapon.transform.position = currWeaponTransform.position;
            activeWeapon.transform.rotation = currWeaponTransform.rotation;
            activeWeapon.SetActive(true);

            PlayerShooting playerShooting = activeWeapon.GetComponent<PlayerShooting>();
            if (playerShooting != null)
            {
                playerShooting.animator = animator;
            }
        }

        // Update the weapon text on the screen
        UpdateWeaponText(weaponNum);

        currentWeapon = weaponNum;
    }

    private void UpdateWeaponText(int weaponNum)
    {
        string weaponName = "Unknown";

        switch (weaponNum)
        {
            case 1:
                weaponName = "Rifle";
                break;
            case 2:
                weaponName = "Kalashnikova";
                break;
            case 3:
                weaponName = "Laser Gun";
                break;
        }

        // Update the weapon text via GameManager
        GameManager.Instance.UpdateWeaponText(weaponName);
    }
}