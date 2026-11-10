using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class chapter2 : MonoBehaviour
{
    public School school_script;
    walk playermovement;
    cam_rotation fps_Cam;
    SimpleOpenClose OpenClose;
    public RayCast Raycast;
    bool hidden;
    [SerializeField]
    private AudioClip[] PainEffects;
    private bool alive, levelCheck = false, ClosedDoorCheck = false, ExitTime = false;
    private bool codeGenerated = false, roomSwitched = false, exitItemMoved = false;
    private float jumpsave;
    private int PassCode;
    private string PassCodeText = "----";
    public float Level2jump = 30f, jumpspeed = 0.8f;
    public GameObject Chaser, NormalRoom, ScaryRoom;
    public TextMesh codepaperText;
    public TextMeshPro digitalInputText;
    public UIDocument uIDocument;
    private Label PaperPanel, PaperText;
    private Button ExitButton;
    private int CodeNumber, codeindex = 0;
    private int[] codes = new int[4];
    public Animator DoorAnimator;
    public Material Redlight, GreenLight;
    public AudioSource KeyPadLockAudioSource, DoorAudioSource, KeyAudioSource;
    public AudioClip correctpass, WrongPass, beep;
    private Animator KeysAnimator;
    public GameObject[] parts;
    void Start()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
        var root = uIDocument.rootVisualElement;
        playermovement = GetComponent<walk>();
        fps_Cam = GetComponentInChildren<cam_rotation>();
        Debug.Log("playermovement = " + playermovement);
        Debug.Log("fps_Cam = " + fps_Cam);
        alive = true;
        jumpsave = playermovement.jumpForce;
        PaperPanel = root.Q<Label>("PaperPanel");
        PaperText = root.Q<Label>("PaperText");
        ExitButton = root.Q<Button>("ExitButton");
        ExitButton.clicked += PaperExit;
        PaperPanel.style.display = DisplayStyle.None;
        SetEmission(Redlight, Color.red);
        SetEmission(GreenLight, Color.black);
        DoorAnimator.SetBool("Close", true);
        DoorAudioSource.enabled = false;
    }

    void Update()
    {
        RayCastOutput("");
    }

    private void OnDestroy()
    {
        if (ExitButton != null) ExitButton.clicked -= PaperExit;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("level1"))
        {
            Level1();
        }
        if (other.CompareTag("look"))
        {
            hidden = false;
            school_script.HiddenPlayer(hidden);
        }
        if (other.CompareTag("level2"))
        {
            Level2();
            Debug.Log(other.name);
        }
        if (other.CompareTag("level3"))
        {
            Level3();
            levelCheck = false;
            Debug.Log(other.name);
        }
        if (other.CompareTag("level4"))
        {
            Level4();
            other.gameObject.SetActive(false);
        }
        if (other.CompareTag("Close"))
        {
            if (!ClosedDoorCheck)
            {
                CloseDoor();
                ClosedDoorCheck = true;
            }
            other.gameObject.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("look"))
        {
            hidden = true;
            school_script.HiddenPlayer(hidden);
        }
        if (other.CompareTag("level2"))
        {
            if (transform.position.z > other.transform.position.z && !levelCheck)
            {
                Collider c = other.GetComponent<Collider>();
                c.isTrigger = false;
                levelCheck = true;
            }
            if (transform.position.z < other.transform.position.z)
            {
                school_script.enabled = true;
            }
        }
        if (other.CompareTag("level3"))
        {
            if (transform.position.z > other.transform.position.z && !levelCheck)
            {
                Collider c = other.GetComponent<Collider>();
                c.isTrigger = false;
                levelCheck = true;
            }
            if (transform.position.z < other.transform.position.z)
            {
                school_script.enabled = true;
            }
        }
    }

    public void Level1()
    {
        school_script.enabled = true;
    }

    public void Level2()
    {
        if (!Chaser.activeSelf)
        {
            Chaser.SetActive(true);
        }
        school_script.enabled = false;
        playermovement.jumpForce = Level2jump;
        playermovement.character_ani.SetFloat("jumpspeed", jumpspeed);
    }

    public void Level3()
    {
        playermovement.jumpForce = jumpsave;
        playermovement.thirdperson = false;
        playermovement._first_person();
        fps_Cam.enabled = true;
        playermovement.character_ani.SetFloat("jumpspeed", 1f);
        GenerateCode();
        parts[0].SetActive(false);
        parts[3].SetActive(true);
    }
    private void Level4()
    {
        parts[3].SetActive(false);
         for(int x = 6; x<= 10; x++)
        {
             parts[x].SetActive(true);
        }
        playermovement.thirdperson = true;
        playermovement._3rd_person();
    }
    private void GenerateCode()
    {
        if (codeGenerated) return;
        codeGenerated = true;

        PassCode = UnityEngine.Random.Range(0, 10000);   
        PassCodeText = PassCode.ToString("D4");          

        if (codepaperText != null)
            codepaperText.text += PassCodeText;

        Debug.Log("PassCode = " + PassCodeText);
    }

    public void GameOver()
    {
        playermovement.character_ani.enabled = false;
        playermovement.WalkAudioSource.enabled = false;
        playermovement.RunAudioSource.enabled = false;
        playermovement.enabled = false;
        fps_Cam.enabled = false;
        if (alive)
        {
            SoundFxManeger.instance.playsoundFX(PainEffects, transform, 1f);
            Debug.Log("playing sound");
            alive = false;
        }
        enabled = false;
    }

    public void RayCastOutput(string Name)
    {
        if (Raycast.HitName != null)
            Name = Raycast.HitName;

        Collider hitCollider = Raycast.hit.collider;

        switch (Name)
        {
            case "Paper1":
                PaperPanel.style.display = DisplayStyle.Flex;
                PaperText.text = "Code: " + PassCodeText;
                UnityEngine.Cursor.lockState = CursorLockMode.None;
                UnityEngine.Cursor.visible = true;
                playermovement.ani.SetBool("walk", false);
                playermovement.WalkAudioSource.enabled = false;
                playermovement.RunAudioSource.enabled = false;
                playermovement.enabled = false;
                fps_Cam.enabled = false;
                Raycast.HitName = null;
                break;

            case "Drawer":
                ToggleOpenClose(hitCollider);
                Raycast.HitName = null;
                break;

            case "gun":
                
                Raycast.HitName = null;
                if (!ExitTime)
                {
                    ExitTime = true;
                    StartCoroutine(Switchroom());
                }
                break;

            case "ExitDoor":
                Raycast.HitName = null;
                if (ExitTime && hitCollider != null)
                {
                    ToggleOpenClose(hitCollider);
                    MoveScaryItemTo(hitCollider.transform);
                    parts[4].SetActive(true);
                    parts[5].SetActive(true);
                }
                break;
        }

        if (Name.StartsWith("Key") && Raycast.HitName != null)
        {
            if (hitCollider != null)
            {
                KeysAnimator = hitCollider.GetComponent<Animator>();
            }
            if (KeysAnimator != null)
            {
                KeysAnimator.speed = 2f;
                KeysAnimator.Play("KeyClicked");
            }
            KeyAudioSource.clip = beep;
            KeyAudioSource.Play();

            if (Name == "Key_Enter")
            {
                if (codeGenerated && codeindex == codes.Length && CodeNumber == PassCode)
                {
                    OpenTheDoor();
                }
                else
                {
                    KeyPadLockAudioSource.clip = WrongPass;
                    KeyPadLockAudioSource.Play();
                    ClearInput();
                    SetEmission(Redlight, Color.red);
                    SetEmission(GreenLight, Color.black);
                    CloseDoor();
                }
                Raycast.HitName = null;
                return;
            }

            if (Name == "Key_x")
            {
                ClearInput();
                Raycast.HitName = null;
                return;
            }

            if (Name.Length > 4 && int.TryParse(Name.Substring(4), out int x))
            {
                if (codeindex < codes.Length)
                {
                    codes[codeindex] = x;
                    if (digitalInputText != null) digitalInputText.text += x;
                    codeindex++;

                    CodeNumber = 0;
                    for (int i = 0; i < codeindex; i++)
                    {
                        CodeNumber = CodeNumber * 10 + codes[i];
                    }

                    Debug.Log("Entered digits: " + CodeNumber.ToString("D" + codeindex));
                }
            }
            Raycast.HitName = null;
        }
    }

    private void ClearInput()
    {
        CodeNumber = 0;
        codeindex = 0;
        System.Array.Clear(codes, 0, codes.Length);
        if (digitalInputText != null) digitalInputText.text = "";
    }

    private void ToggleOpenClose(Collider target)
    {
        if (target == null) return;
        OpenClose = target.GetComponent<SimpleOpenClose>();
        if (OpenClose != null) OpenClose.ObjectClicked();
    }

    
    private void MoveScaryItemTo(Transform newParent)
    {
        if (exitItemMoved || newParent == null) return;
        exitItemMoved = true;

        if (ScaryRoom == null)
        {
            Debug.LogError("ScaryRoom is not assigned.", this);
            return;
        }

        Transform room = ScaryRoom.transform;
        if (room.childCount == 0 || room.GetChild(0).childCount == 0)
        {
            Debug.LogError(
                "ScaryRoom hierarchy is not what the code expects. " +
                "ScaryRoom.childCount = " + room.childCount + ", " +
                "first child childCount = " + (room.childCount > 0 ? room.GetChild(0).childCount : 0),
                this);
            return;
        }

        room.GetChild(0).GetChild(0).SetParent(newParent);
    }

    private static void SetEmission(Material mat, Color color)
    {
        if (mat != null && mat.HasProperty("_EmissionColor"))
            mat.SetColor("_EmissionColor", color);
    }

    public void OpenTheDoor()
    {
        DoorAudioSource.enabled = true;
        DoorAnimator.SetBool("Close", false);
        SetEmission(Redlight, Color.black);
        SetEmission(GreenLight, Color.green);
        KeyPadLockAudioSource.clip = correctpass;
        KeyPadLockAudioSource.Play();
    }

    public void CloseDoor()
    {
        DoorAnimator.SetBool("Close", true);
        if (DoorAudioSource.isActiveAndEnabled)
        {
            DoorAudioSource.Play();
        }
        parts[2].SetActive(false);
        parts[1].SetActive(false);
    }

    public void PaperExit()
    {
        PaperPanel.style.display = DisplayStyle.None;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
        playermovement.enabled = true;
        fps_Cam.enabled = true;
        Debug.Log("Exit");
    }

    public IEnumerator Switchroom()
    {
        if (roomSwitched) yield break;
        roomSwitched = true;

        if (NormalRoom != null) NormalRoom.SetActive(false);
        if (ScaryRoom != null) ScaryRoom.SetActive(true);
        yield return null;
    }
}
