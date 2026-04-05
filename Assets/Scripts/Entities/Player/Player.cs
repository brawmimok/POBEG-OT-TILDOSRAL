using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(CharacterController), typeof(AudioSource))]
public class Player : PepusBehaviour
{
    [Header("MoveController")]
    //[SerializeField] private float jumpPower = 7f;
    public Camera playerCamera;
    public Canvas canvas;
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float gravity = 9.8f;

    [SerializeField] private float runSpeedMultiplier = 2f;
    [SerializeField] private float crouchSpeedMultiplier = 0.5f;

    [SerializeField] private float defaultHeight = 2f;
    [SerializeField] private float crouchHeight = 1f;

    private Vector3 moveDirection = Vector3.zero;
    private float movementDirectionY;
    private float currentSpeed;
    private float rotationX = 0;

    public bool canMove = true;
    public bool isMoving;
    private bool isRunning;


#if UNITY_EDITOR
    [Header("Noclip")]
    [SerializeField] private TMPro.TextMeshProUGUI noclipUI;
    private string sourceNoclipText;

    [SerializeField] private GameObject allUI;
    private bool inNoclip;
    private float noclipSpeedMultiplier = 5f;
    private KeyCode noclipKey = KeyCode.V;
#endif

    [Space]
    [Header("LookController")]
    [SerializeField] private float lookXLimit = 70f;
    public float lookSpeed = 2f;

    [Space]
    [Header("StepSFX")]
    [SerializeField] private AudioClip[] footstepsSound;
    private float baseStepRate = 0.64f;
    //private float runStepRate = 0.4f;
    private float stepTimer = 0f;

    [Space]
    [Header("Slava BOB")]
    [SerializeField] private float shakeAmplitude = 0.3f;
    [SerializeField] private float shakeFrequency = 5f;

    private float shakeTimer = 0f;
    float yOffset = 1.0f;
    float yPosition;

    [Space]
    [Header("Stamina")]
    [SerializeField] private GameObject[] staminaStrips;
    [SerializeField] private float timeForFullStamina = 3f;
    [SerializeField] private float timeForFullEndedStamina = 2.3f;
    [SerializeField] private KeyCode runKeyCode = KeyCode.LeftShift;

    private float staminaStrip;
    private float curStaminaStrips;

    private float curStamina;
    private bool staminaEnded;
    private float staminaTimer;

    [Space]
    [Header("Blink")]
    [SerializeField] private GameObject[] blinkStrips;
    [SerializeField] private GameObject blinkScreen;
    [SerializeField] private float timeForBlinkReturn = 10f;
    [SerializeField] private float blinkTime = 0.3f;
    [SerializeField] private KeyCode blinkKeyCode = KeyCode.Space;
    [NonSerialized] public bool isBlinking;
    private float blinkTimer;
    private float inBlinkTimer;
    private float blinkStrip;

    [Space]
    [Header("DeathAnimation")]
    [SerializeField] private float transitionSpeed = 1.5f;

    [Space]
    [Header("Interacting")]
    public Dictionary<Collider, Usable> useList = new();
    public Transform useSourceDistance;
    [NonSerialized] public Usable object_to_use;

    [SerializeField] private Image handIcon;
    private Vector2 minHandIconOnScreenCord;
    private Vector2 maxHandIconOnScreenCord;

    [SerializeField] private float handInvisCanvasIconScale = 3f;

    [Space]
    [Header("Inventory")]
    public InventoryItem[] inventory = new InventoryItem[5];
    public int keyCardLevel = 0;

    [Space]
    [Header("Other")]
    [NonSerialized] public bool inPause = false;
    public AudioClip DeathSound;
    [NonSerialized] public CharacterController characterController;
    [NonSerialized] public bool alive = true;

    //[SerializeField] private GameObject playerModel;

    protected override void Awake()
    {
        base.Awake();
        characterController = GetComponent<CharacterController>();
    }
    private void Start()
    {
#if UNITY_EDITOR
        sourceNoclipText = noclipUI.text;
#endif
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        minHandIconOnScreenCord = handIcon.rectTransform.sizeDelta * handInvisCanvasIconScale;
        maxHandIconOnScreenCord = new(Screen.width - handIcon.rectTransform.sizeDelta.x * handInvisCanvasIconScale, Screen.height - handIcon.rectTransform.sizeDelta.y * handInvisCanvasIconScale);
        // SNR 
        curStamina = timeForFullStamina;
        staminaStrip = timeForFullStamina / staminaStrips.Length;
        // BNR
        blinkTimer = timeForBlinkReturn;
        blinkStrip = timeForBlinkReturn / blinkStrips.Length;
    }

    private void Update()
    {
        if (!alive)
        {
            playerCamera.transform.SetLocalPositionAndRotation(Vector3.Lerp(playerCamera.transform.localPosition, Vector3.zero, Time.deltaTime * transitionSpeed),
                Quaternion.Slerp(playerCamera.transform.localRotation, Quaternion.Euler(-90, playerCamera.transform.localRotation.y, playerCamera.transform.localRotation.z), Time.deltaTime * transitionSpeed));
            movementDirectionY = moveDirection.y;
            moveDirection = Vector3.zero;
            if (!characterController.isGrounded) movementDirectionY -= gravity * Time.deltaTime;
            else movementDirectionY = -gravity;

            characterController.Move(moveDirection * Time.deltaTime);

            moveDirection.y = movementDirectionY;

        }
        else
        {
            //
            // МУВМЕНТ ИЗИ
            //
            isRunning = Input.GetKey(runKeyCode);
#if UNITY_EDITOR
            if (inNoclip)
            {
                noclipUI.text = string.Format(sourceNoclipText, noclipSpeedMultiplier.ToString("F2"));
                if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.H))
                    noclipUI.gameObject.SetActive(!noclipUI.gameObject.activeSelf);
                else if (Input.GetKeyDown(KeyCode.H))
                    allUI.SetActive(!allUI.activeSelf);
                noclipSpeedMultiplier += Input.GetAxis("Mouse ScrollWheel") * 2f;
                transform.position += Time.deltaTime * noclipSpeedMultiplier *
                    Vector3.ClampMagnitude(playerCamera.transform.right * Input.GetAxis("Horizontal") +
                    playerCamera.transform.forward * Input.GetAxis("Vertical") +
                    playerCamera.transform.up * Input.GetAxis("UpDown")
                    , 1f);
                //Мышка
                rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
                rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
                playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
                transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
            }
            else
#endif
            {
                movementDirectionY = moveDirection.y;

                if (canMove)
                {
                    // Просчёт скорости
                    if (Input.GetKey(KeyCode.LeftControl))
                    {
                        isRunning = false;
                        characterController.height = crouchHeight;
                        currentSpeed = walkSpeed * crouchSpeedMultiplier;
                    }
                    else if (isRunning && !staminaEnded)
                        currentSpeed = walkSpeed * runSpeedMultiplier;
                    else
                    {
                        characterController.height = defaultHeight;
                        currentSpeed = walkSpeed;
                    }
                    //
                    moveDirection =
                            Vector3.ClampMagnitude(
                            transform.forward * Input.GetAxis("Vertical")
                            + transform.right * Input.GetAxis("Horizontal")
                            , 1f) * currentSpeed;

                    if (!characterController.isGrounded) movementDirectionY -= gravity * Time.deltaTime;
                    else movementDirectionY = -gravity;
                    //else if (Input.GetButton("Jump")) moveDirection.y = jumpPower; // Для прыжка
                    moveDirection.y = movementDirectionY;

                    //Мышка
                    rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
                    rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
                    playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
                    transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);

                    isMoving = characterController.isGrounded && characterController.velocity.magnitude > 0.1f;

                    if (isMoving)
                    {
                        shakeTimer += Time.deltaTime * shakeFrequency;
                        yPosition = yOffset + Mathf.Sin(shakeTimer) * shakeAmplitude;

                        var tiltAngle = Mathf.Sin(shakeTimer) * 0.5f;

                        var currentRotation = playerCamera.transform.localEulerAngles;
                        playerCamera.transform.localEulerAngles = new Vector3(currentRotation.x, currentRotation.y, tiltAngle);

                        playerCamera.transform.localPosition = new Vector3(0, yPosition, 0);

                        stepTimer -= Time.deltaTime;
                        var currentStepRate = Mathf.Clamp(baseStepRate * (walkSpeed / currentSpeed), 0.4f, 1.2f);

                        if (stepTimer <= 0f)
                        {
                            //PlayFootStep();
                            MainMechanics.instance.sfxAudio.PlayOneShot(footstepsSound[Random.Range(0, footstepsSound.Length)]);
                            stepTimer = currentStepRate;
                        }
                    }
                    else
                    {
                        var currentRotation = playerCamera.transform.localEulerAngles;
                        playerCamera.transform.localEulerAngles = new Vector3(currentRotation.x, currentRotation.y, 0);
                        stepTimer = Mathf.Clamp(baseStepRate * (walkSpeed / currentSpeed), 0.4f, 1.2f);
                    }
                }
                else
                {
                    moveDirection = Vector3.zero;
                    moveDirection.y = movementDirectionY;
                    if (!characterController.isGrounded) movementDirectionY -= gravity * Time.deltaTime;
                    else movementDirectionY = -gravity;
                }

                characterController.Move(moveDirection * Time.deltaTime);
            }

            //
            // ВЗАИМОДЕЙСТВИЕ, ИСПОЛЬЗОВАНИЕ
            //
            UsableDetect(); // Отрисовка руки, нахождение из нескольких объектов того который можно использовать
            if (Input.GetMouseButtonDown(0) && !GameUIManager.instance._isInventoryOpen && !GameUIManager.instance._isPaused) Use();
            //Input.GetKeyDown(KeyCode.E) ||
            //
            // NOCLIP
            //
#if UNITY_EDITOR
            if (Input.GetKeyDown(noclipKey))
            {
                movementDirectionY = -gravity;

                characterController.enabled = inNoclip;
                allUI.SetActive(inNoclip);

                inNoclip = !inNoclip;
                noclipUI.gameObject.SetActive(inNoclip);
            }
#endif
        }
        //
        //                               МАТЕРИНСКАЯ СТАМИНА ХАИЛЬ
        //
        if (!staminaEnded)
        {
            if (isMoving && isRunning)
            {
                curStamina -= Time.deltaTime;
                if (curStamina <= 0f)
                {
                    curStamina = 0f;
                    staminaEnded = true;
                }
            }
            else
            {
                if (curStamina > timeForFullStamina)
                    curStamina = timeForFullStamina;
                if (curStamina < timeForFullStamina)
                    curStamina += Time.deltaTime;
            }
        }
        else
        {
            staminaTimer += Time.deltaTime;
            if (staminaTimer >= timeForFullEndedStamina)
            {
                staminaTimer = 0f;
                staminaEnded = false;
            }
        }
        curStaminaStrips = Mathf.Ceil(curStamina / staminaStrip);
        for (int i = staminaStrips.Length; i > 0; i--)
            staminaStrips[i - 1].SetActive(i <= curStaminaStrips);
        //
        //                               МАТЕРИНСКАЯ МОРГАЛКА ЗИГ
        //
        if (!isBlinking && !Input.GetKey(blinkKeyCode))
        {
            blinkTimer -= Time.deltaTime;
            if (blinkTimer <= 0f)
            {
                inBlinkTimer = 0f;
                isBlinking = true;
            }
            for (var i = blinkStrips.Length; i > 0; i--)
                blinkStrips[i - 1].SetActive(i <= Mathf.Ceil(blinkTimer / blinkStrip));
        }
        else
        {
            isBlinking = true;
            blinkScreen.SetActive(true);
            if (inBlinkTimer >= blinkTime)
            {
                if (Input.GetKey(blinkKeyCode))
                    isBlinking = true;
                else
                {
                    inBlinkTimer = 0f;
                    blinkTimer = 10f;
                    isBlinking = false;
                    blinkScreen.SetActive(false);
                }
            }
            else inBlinkTimer += Time.deltaTime;
            for (var i = blinkStrips.Length; i > 0; i--)
                blinkStrips[i - 1].SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {
            GameUIManager.instance.ShowText("Игра сохранена", 3f);
            SaveManager.instance.Save();
        }
        if (Input.GetKeyDown(KeyCode.F7))
        {
            GameUIManager.instance.ShowText("Игра Загружена", 3f);
            SaveManager.instance.LoadSave();
        }
    }
    #region USING_CODE

    [SerializeField] private LayerMask layerMask;
    [SerializeField] private LayerMask layerMaskNpcRaycast;
    private void Use()
    {
        if (object_to_use != null)
        {
            object_to_use.Use(this);
            return;
        }
    }
    private void UsableDetect()
    {
        if (useList.Count > 1)
        {
            float distance = float.MaxValue;
            float dist;
            object_to_use = null;
            handIcon.gameObject.SetActive(false);
            foreach (var entity in useList)
            {
                if (Physics.Raycast(playerCamera.transform.position, entity.Key.ClosestPoint(playerCamera.transform.position) - playerCamera.transform.position, out var hitInfo, float.MaxValue, layerMask.value))
                {
                    dist = Vector3.Distance(entity.Key.transform.position, useSourceDistance.position);
                    if (dist < distance)
                    {
                        if (hitInfo.collider == entity.Key)
                        {
                            distance = dist;
                            object_to_use = entity.Value;

                            var screenPos3 = playerCamera.WorldToScreenPoint(entity.Key.transform.position);
                            var screenPos = (Vector2)screenPos3;
                            if (screenPos3.z < 0f) screenPos = -screenPos;

                            screenPos = screenPos.Clamp(minHandIconOnScreenCord, maxHandIconOnScreenCord);

                            handIcon.rectTransform.position = screenPos;
                            handIcon.gameObject.SetActive(true);
                        }
                    }
                }
            }
            return;
        }
        else if (useList.Count == 1)
        {
            foreach (var entity in useList)
            {
                if (Physics.Raycast(new Ray(playerCamera.transform.position, entity.Key.ClosestPoint(playerCamera.transform.position) - playerCamera.transform.position), out var hitInfo, float.MaxValue, layerMask.value))
                {
                    //print(hitInfo.collider.gameObject.name);
                    //Debug.DrawLine(playerCamera.transform.position, hitInfo.point, Color.red);
                    //Debug.DrawLine(playerCamera.transform.position, entity.Key.ClosestPoint(playerCamera.transform.position), Color.green);
                    if (hitInfo.collider == entity.Key)
                    {
                        object_to_use = entity.Value;

                        var screenPos3 = playerCamera.WorldToScreenPoint(entity.Key.transform.position);
                        var screenPos = (Vector2)screenPos3;
                        if (screenPos3.z < 0f) screenPos = -screenPos;

                        screenPos = screenPos.Clamp(minHandIconOnScreenCord, maxHandIconOnScreenCord);

                        handIcon.rectTransform.position = screenPos;
                        handIcon.gameObject.SetActive(true);
                    }
                    else
                    {
                        handIcon.gameObject.SetActive(false);
                        object_to_use = null;
                    }
                }
                else
                {
                    handIcon.gameObject.SetActive(false);
                    object_to_use = null;
                }
            }
        }
        else
        {
            handIcon.gameObject.SetActive(false);
            object_to_use = null;
        }
    }
    #endregion
    //PLAYER API
    public void Death(string reason = "")
    {
        if (alive)
        {
            alive = false;
            canMove = false;
#if UNITY_EDITOR
            inNoclip = false;
            allUI.SetActive(true);
            noclipUI.gameObject.SetActive(false);
#endif
            characterController.enabled = true;
            GameUIManager.instance.DeathMessage.text = reason;
            MainMechanics.instance.sfxAudio.PlayOneShot(DeathSound);
        }
    }
    public void TeleportTo(Transform target)
    {
        characterController.enabled = false;
        transform.position = target.position;
        characterController.enabled = true;
    }



    public bool PlayerIsWatching(Collider collider, Collider meshcollider)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(playerCamera);
        print(1);
        if (!GeometryUtility.TestPlanesAABB(planes, meshcollider.bounds)) return false;
        print(2);
        Bounds b = collider.bounds;
        Vector3[] pointsToCheck = {
            b.center,
            // --- Углы (8 точек) ---
            new (b.min.x, b.min.y, b.min.z), new (b.max.x, b.min.y, b.min.z),
            new (b.min.x, b.max.y, b.min.z), new (b.max.x, b.max.y, b.min.z),
            new (b.min.x, b.min.y, b.max.z), new (b.max.x, b.min.y, b.max.z),
            new (b.min.x, b.max.y, b.max.z), new (b.max.x, b.max.y, b.max.z),
            
            // --- Точки на 0.25 от центра к углам (8 точек) ---
            Vector3.Lerp(b.center, new (b.min.x, b.min.y, b.min.z), 0.25f),
            Vector3.Lerp(b.center, new (b.max.x, b.min.y, b.min.z), 0.25f),
            Vector3.Lerp(b.center, new (b.min.x, b.max.y, b.min.z), 0.25f),
            Vector3.Lerp(b.center, new (b.max.x, b.max.y, b.min.z), 0.25f),
            Vector3.Lerp(b.center, new (b.min.x, b.min.y, b.max.z), 0.25f),
            Vector3.Lerp(b.center, new (b.max.x, b.min.y, b.max.z), 0.25f),
            Vector3.Lerp(b.center, new (b.min.x, b.max.y, b.max.z), 0.25f),
            Vector3.Lerp(b.center, new (b.max.x, b.max.y, b.max.z), 0.25f),

            // --- Точки на 0.50 от центра к углам (8 точек) ---
            Vector3.Lerp(b.center, new (b.min.x, b.min.y, b.min.z), 0.5f),
            Vector3.Lerp(b.center, new (b.max.x, b.min.y, b.min.z), 0.5f),
            Vector3.Lerp(b.center, new (b.min.x, b.max.y, b.min.z), 0.5f),
            Vector3.Lerp(b.center, new (b.max.x, b.max.y, b.min.z), 0.5f),
            Vector3.Lerp(b.center, new (b.min.x, b.min.y, b.max.z), 0.5f),
            Vector3.Lerp(b.center, new (b.max.x, b.min.y, b.max.z), 0.5f),
            Vector3.Lerp(b.center, new (b.min.x, b.max.y, b.max.z), 0.5f),
            Vector3.Lerp(b.center, new (b.max.x, b.max.y, b.max.z), 0.5f),

            // --- Точки на 0.75 от центра к углам (8 точек) ---
            Vector3.Lerp(b.center, new (b.min.x, b.min.y, b.min.z), 0.75f),
            Vector3.Lerp(b.center, new (b.max.x, b.min.y, b.min.z), 0.75f),
            Vector3.Lerp(b.center, new (b.min.x, b.max.y, b.min.z), 0.75f),
            Vector3.Lerp(b.center, new (b.max.x, b.max.y, b.min.z), 0.75f),
            Vector3.Lerp(b.center, new (b.min.x, b.min.y, b.max.z), 0.75f),
            Vector3.Lerp(b.center, new (b.max.x, b.min.y, b.max.z), 0.75f),
            Vector3.Lerp(b.center, new (b.min.x, b.max.y, b.max.z), 0.75f),
            Vector3.Lerp(b.center, new (b.max.x, b.max.y, b.max.z), 0.75f)
        };
        if (b.Contains(playerCamera.transform.position))
            return true;
        //var a = false;
        //foreach (Vector3 point in pointsToCheck)
        //    if (Physics.Linecast(playerCamera.transform.position, point, out RaycastHit hit, layerMask.value))
        //        if (hit.collider == collider)
        //        {
        //            Debug.DrawLine(point, point + Vector3.up * 0.2f, Color.green);
        //            a = true;
        //        }
        //        else
        //            Debug.DrawLine(point, point + Vector3.up * 0.2f, Color.red);
        //return a;
        foreach (Vector3 point in pointsToCheck)
            if (Physics.Linecast(playerCamera.transform.position, point, out RaycastHit hit, layerMaskNpcRaycast.value))
                if (hit.collider == collider)
                    return true;
        return false;
    }
    // Сохранения
    [Serializable]
    public class InventorySaveDataWrap
    {
        public ulong[] inv_item_id;
        //public InventorySaveDataWrap(int[] arr)
        //{
        //    inv_item_id = new int[arr.Length];
        //    for (int i = 0; i < arr.Length; i++)
        //        inv_item_id[i] = arr[i];
        //}
        public InventorySaveDataWrap(InventoryItem[] arr)
        {
            inv_item_id = new ulong[arr.Length];
            for (int i = 0; i < arr.Length; i++)
                inv_item_id[i] = arr[i] == null ? 0 : arr[i].Id;
        }
    }
    [Serializable]
    public class PlayerSaveData : SaveData
    {
        public float x, y, z;

        public float wRot, xRot, yRot, zRot;

        public InventorySaveDataWrap inventory;

        public float currentStamina;
        public float staminaTimer;
        public bool staminaEnded;

        public float blinkTimer;
        public float inBlinkTimer;
        public float blinkStrip;
        public bool isBlinking;
    }
    public override void OnSave()
    {
        saveData = new PlayerSaveData()
        {
            x = transform.position.x,
            y = transform.position.y,
            z = transform.position.z,

            wRot = transform.rotation.w,
            xRot = transform.rotation.x,
            yRot = transform.rotation.y,
            zRot = transform.rotation.z,

            inventory = new(inventory),

            currentStamina = curStamina,
            staminaTimer = staminaTimer,
            staminaEnded = staminaEnded,

            blinkTimer = blinkTimer,
            blinkStrip = blinkStrip,
            isBlinking = isBlinking,
            inBlinkTimer = inBlinkTimer,

            active = gameObject.activeInHierarchy,
            id = Id
        };
    }
    public override void OnLoad(SaveData saveData)
    {
        base.OnLoad(saveData);
        PlayerSaveData saveData1 = (PlayerSaveData)saveData;

        GameUIManager.instance.UndisplayItemOnScreen();

        characterController.enabled = false;
        transform.SetPositionAndRotation(
            new Vector3(saveData1.x, saveData1.y, saveData1.z),
            new Quaternion(saveData1.xRot, saveData1.yRot, saveData1.zRot, saveData1.wRot));
        characterController.enabled = true;

        curStamina = saveData1.currentStamina;
        staminaTimer = saveData1.staminaTimer;
        staminaEnded = saveData1.staminaEnded;

        blinkTimer = saveData1.blinkTimer;
        blinkStrip = saveData1.blinkStrip;
        isBlinking = saveData1.isBlinking;
        inBlinkTimer = saveData1.blinkTimer;

        for (int i = 0; i < saveData1.inventory.inv_item_id.Length; i++)
            inventory[i] = saveData1.inventory.inv_item_id[i] == 0 ? null : (InventoryItem)SaveManager.instance.behaviours[saveData1.inventory.inv_item_id[i]];
    }
}