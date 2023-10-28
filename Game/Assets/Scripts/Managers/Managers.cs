using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Managers : MonoBehaviour
{
    static Managers s_instance;
    static Managers Instance { get { Init(); return s_instance; } }

    DataManager _data  = new DataManager();
    FieldManager _field = new FieldManager();
    InputManager _input = new InputManager();
    InventoryManager _inventory = new InventoryManager();
    ResourceManager _resource = new ResourceManager();
    SoundManager _sound = new SoundManager();
    TimeManager _time = new TimeManager();
    UIManager _ui = new UIManager();
    EnergyManager _energy = new EnergyManager();
    GoldManager _gold = new GoldManager();

    public static DataManager Data { get { return Instance._data; } }
    public static FieldManager Field { get { return Instance._field; } }
    public static InputManager Input { get { return Instance._input; } }
    public static InventoryManager Inventory { get { return Instance._inventory; } }
    public static ResourceManager Resource { get { return Instance._resource; } }
    public static SoundManager Sound { get { return Instance._sound; } }
    public static TimeManager Time { get { return Instance._time; } }
    public static UIManager UI { get { return Instance._ui; } }
    public static EnergyManager Energy { get { return Instance._energy; } }

    public static GoldManager Gold { get { return Instance._gold;} }

    public GameObject LoadingUI;
    public GameObject nameUI;


    void Awake()
    {
        Init();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        UI.Start();
        Data.Start();
        Energy.Start();
        Sound.Start();

        //LoadingUI = GameObject.Find("UI_Loading");
        //nameUI = GameObject.Find("UI_Name");
    }

    private void Start()
    {
        LoadingUI.SetActive(true);
        StartCoroutine(RecoverTimeRun());
        UnityEngine.Time.timeScale = 50f;
    }

    void Update()
    {
        Input.OnUpdate();
        Time.Update();
        Energy.Update();
        UI.Update();
        Gold.Update();
        Resources.UnloadUnusedAssets();

        if(UnityEngine.Input.GetKeyDown(KeyCode.Delete))
        {
            Application.Quit();
        }
          
    }

    static void Init()
    {
        if(s_instance == null)
        {
            GameObject go = GameObject.Find("@Managers"); 
            if(go == null) //@Managers 오브젝트가 없으면 추가
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }
            DontDestroyOnLoad(go);  //사라지지 않게 함
            s_instance = go.GetComponent<Managers>();   
        }

    }

    IEnumerator RecoverTimeRun()
    {
        yield return new WaitUntil(() => Time.GetHour() == 8);
        UnityEngine.Time.timeScale = 1;
        LoadingUI.SetActive(false);

        //Name
        nameUI.SetActive(true);
        UI.ReleaseCursor();
    }
}
