using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class SimpleController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject cameraController;
    [SerializeField] GameObject cameraController2;
    [SerializeField] GameObject ship;
    [Space]
    [SerializeField] MeshRenderer mainEngine;
    [SerializeField] MeshRenderer topThruster;
    [SerializeField] MeshRenderer leftThruster;
    [SerializeField] MeshRenderer rightThruster;
    [SerializeField] Material engineActive;
    [SerializeField] Material engineInactive;
    [Space]
    [SerializeField] AudioSource engineAudio;
    [SerializeField] AudioSource laserAudio;
    [SerializeField] AudioSource topThrusterAudio;
    [SerializeField] AudioSource leftThrusterAudio;
    [SerializeField] AudioSource rightThrusterAudio;

    [Header("Score")]
    [SerializeField] public int score = 0;
    [SerializeField] int ammo = 1000;
    [SerializeField] float fuel = 5000;
    

    [Header("Variables")]
    [SerializeField] int sensitivity = 3;
    [SerializeField] float rotationSpeed = 10f;
    [SerializeField] float accelerationSpeed = 10f;
    [Space]
    [SerializeField] Vector3 velocity;
    [Space]
    [SerializeField] GameObject laser;
    [SerializeField] bool firingLaser;
    [SerializeField] int laserDamage = 30;

    [Header("Internal")]
    float rx;
    float ry;
    float engineInactiveTime;
    float topThrusterInactiveTime;
    float leftThrusterInactiveTime;
    float rightThrusterInactiveTime;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        bool engine = Keyboard.current.spaceKey.isPressed;
        float x = Keyboard.current.aKey.isPressed ? -1 : Keyboard.current.dKey.isPressed ? 1 : 0;
        float y = Keyboard.current.wKey.isPressed ? 1 : Keyboard.current.sKey.isPressed ? -1 : 0;
        float mx = Mouse.current.delta.ReadValue().x * sensitivity * 10 * Time.deltaTime;
        float my = Mouse.current.delta.ReadValue().y * sensitivity * 10 * Time.deltaTime;

        cameraController.transform.RotateAround(cameraController.transform.position, cameraController.transform.right, my);
        cameraController.transform.RotateAround(cameraController.transform.position, cameraController.transform.up, mx);

        if (fuel > 0)
            ship.transform.rotation = Quaternion.Lerp(ship.transform.rotation, cameraController.transform.rotation, Time.deltaTime * rotationSpeed);

        rx = Mathf.Abs(mx) > Mathf.Abs(rx) ? mx : rx * 0.5f;
        ry = Mathf.Abs(my) > Mathf.Abs(ry) ? my : ry * 0.5f;

        engineInactiveTime += Time.deltaTime;
        if (engine && fuel > 0)
            Accelerate(1);
        if (engineInactiveTime > 0.02f)
            engineAudio.mute = true;

        transform.position += velocity * Time.deltaTime;


        if (!firingLaser && Mouse.current.leftButton.wasPressedThisFrame)
            StartCoroutine(FireLaser());

        if (x < 0) ChangeCamera(1);
        else if (y > 0) ChangeCamera(2);
        else if (y < 0) ChangeCamera(3);
        else if (x > 0) ChangeCamera(4);
        else if (Keyboard.current.xKey.isPressed) ChangeCamera(5);
        else ChangeCamera(0);

        topThrusterInactiveTime += Time.deltaTime;
        rightThrusterInactiveTime += Time.deltaTime;
        leftThrusterInactiveTime += Time.deltaTime;
        if (fuel > 0)
            EngineColor(engine, rx, ry);
    }

    void EngineColor(bool mEngine, float rx, float ry)
    {
        if (mEngine)
            mainEngine.material = engineActive;
        else if (engineInactiveTime > 0.02f)
            mainEngine.material = engineInactive;

        Vector2 subEngineVector = new Vector2(rx, ry).normalized;
        Vector2 topEngine = new Vector2(0, 2.24f).normalized;
        Vector2 leftEngine = new Vector2(2, -1f).normalized;
        Vector2 rightEngine = new Vector2(-2, -1f).normalized;

        if (Vector2.Dot(subEngineVector, topEngine) > 0)
        {
            topThruster.material = engineActive;
            topThrusterAudio.mute = false;
            topThrusterInactiveTime = 0;
        }
        else if (topThrusterInactiveTime > 0.1f)
        {
            topThruster.material = engineInactive;
            topThrusterAudio.mute = true;
        }

        if (Vector2.Dot(subEngineVector, leftEngine) > 0)
        {
            leftThruster.material = engineActive;
            leftThrusterAudio.mute = false;
            leftThrusterInactiveTime = 0;
        }
        else if (leftThrusterInactiveTime > 0.01f)
        {
            leftThruster.material = engineInactive;
            leftThrusterAudio.mute = true;
        }

        if (Vector2.Dot(subEngineVector, rightEngine) > 0)
        {
            rightThruster.material = engineActive;
            rightThrusterAudio.mute = false;
            rightThrusterInactiveTime = 0;
        }
        else if (rightThrusterInactiveTime > 0.01f)
        {
            rightThruster.material = engineInactive;
            rightThrusterAudio.mute = true;
        }
    }

    void Accelerate(float acc)
    {
        velocity += ship.transform.forward * acc * Time.deltaTime * accelerationSpeed;
        engineAudio.mute = false;
        engineInactiveTime = 0;

    }

    IEnumerator FireLaser()
    {
        //laserAudio.pitch = Random.Range(1.4f, 1.8f);

        if (ammo > 0)
        {
            laserAudio.pitch = Random.Range(1.4f, 1.8f);
            laserAudio.Play();
            firingLaser = true;
            laser.SetActive(true);
            ammo--;

            if (Physics.Raycast(ship.transform.position, ship.transform.forward, out RaycastHit hit, 100))
            {
                hit.transform.SendMessageUpwards("TakeDamage", laserDamage);
            }

            yield return new WaitForSeconds(0.1f);
            laser.SetActive(false);
            firingLaser = false;
        }
        else
        {
            //Play fail to shoot audio
            laserAudio.Play();
            yield return new WaitForSeconds(0.04f);
            laserAudio.Stop();
        }
    }

    void ChangeCamera(int dir) // 0 Front, 1 Right, 2 Up, 3 Back, 4 Left, 5 Down
    {
        switch (dir)
        {
            case 0:
                cameraController2.transform.localRotation = Quaternion.Lerp(
                    cameraController2.transform.localRotation,
                    Quaternion.Euler(Vector3.zero),
                    Time.deltaTime * rotationSpeed * 3);
                break;
            case 1:
                cameraController2.transform.localRotation = Quaternion.Lerp(
                    cameraController2.transform.localRotation,
                    Quaternion.Euler(new Vector3(0, 90, 0)),
                    Time.deltaTime * rotationSpeed * 3);
                break;
            case 2:
                cameraController2.transform.localRotation = Quaternion.Lerp(
                    cameraController2.transform.localRotation,
                    Quaternion.Euler(new Vector3(270, 0, 0)),
                    Time.deltaTime * rotationSpeed * 3);
                break;
            case 3:
                cameraController2.transform.localRotation = Quaternion.Lerp(
                    cameraController2.transform.localRotation,
                    Quaternion.Euler(new Vector3(0, 180, 0)),
                    Time.deltaTime * rotationSpeed * 3);
                break;
            case 4:
                cameraController2.transform.localRotation = Quaternion.Lerp(
                    cameraController2.transform.localRotation,
                    Quaternion.Euler(new Vector3(0, 270, 0)),
                    Time.deltaTime * rotationSpeed * 3);
                break;
            case 5:
                cameraController2.transform.localRotation = Quaternion.Lerp(
                    cameraController2.transform.localRotation,
                    Quaternion.Euler(new Vector3(90, 0, 0)),
                    Time.deltaTime * rotationSpeed * 3);
                break;
        }
    }
}
