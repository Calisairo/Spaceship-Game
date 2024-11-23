using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    

    [Header("Variables")]
    [SerializeField] int sensitivity = 80;
    [SerializeField] float rotationSpeed = 10f;
    [SerializeField] float accelerationSpeed = 10f;
    [Space]
    [SerializeField] Vector3 velocity;
    [Space]
    [SerializeField] GameObject laser;
    [SerializeField] bool firingLaser;
    [SerializeField] int laserDamage = 30;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        bool engine = Input.GetKey(KeyCode.Space);
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        float mx = Input.GetAxis("Mouse X") * sensitivity * 10 * Time.deltaTime;
        float my = Input.GetAxis("Mouse Y") * sensitivity * 10 * Time.deltaTime;

        cameraController.transform.RotateAround(cameraController.transform.position, cameraController.transform.right, my);
        cameraController.transform.RotateAround(cameraController.transform.position, cameraController.transform.up, mx);

        ship.transform.rotation = Quaternion.Lerp(ship.transform.rotation, cameraController.transform.rotation, Time.deltaTime * rotationSpeed);

        engineAudio.mute = true;
        if (engine)
            Accelerate(1);

        transform.position += velocity * Time.deltaTime;


        if (!firingLaser && Input.GetMouseButtonDown(0))
            StartCoroutine(FireLaser());

        if (x < 0) ChangeCamera(1);
        else if (y > 0) ChangeCamera(2);
        else if (y < 0) ChangeCamera(3);
        else if (x > 0) ChangeCamera(4);
        else if (Input.GetKey(KeyCode.X)) ChangeCamera(5);
        else ChangeCamera(0);

        EngineColor(engine, mx, my);
    }

    void EngineColor(bool mEngine, float mx, float my)
    {
        if (mEngine)
            mainEngine.material = engineActive;
        else
            mainEngine.material = engineInactive;

        Vector2 subEngineVector = new Vector2(mx, my);
        Vector2 topEngine = new Vector2(0, -1);
        Vector2 leftEngine = new Vector2(-3, 1.5f).normalized;
        Vector2 rightEngine = new Vector2(3, 1.5f).normalized;

        if (Vector2.Dot(subEngineVector, topEngine) > 0)
        {
            topThruster.material = engineActive;
            topThrusterAudio.mute = false;
        }
        else
        {
            topThruster.material = engineInactive;
            topThrusterAudio.mute = true;
        }

        if (Vector2.Dot(subEngineVector, leftEngine) > 0)
        {
            leftThruster.material = engineActive;
            leftThrusterAudio.mute = false;
        }
        else
        {
            leftThruster.material = engineInactive;
            leftThrusterAudio.mute = true;
        }

        if (Vector2.Dot(subEngineVector, rightEngine) > 0)
        {
            rightThruster.material = engineActive;
            rightThrusterAudio.mute = false;
        }
        else
        {
            rightThruster.material = engineInactive;
            rightThrusterAudio.mute = true;
        }
    }

    void Accelerate(float acc)
    {
        velocity += ship.transform.forward * acc * Time.deltaTime * accelerationSpeed;
        engineAudio.mute = false;

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
