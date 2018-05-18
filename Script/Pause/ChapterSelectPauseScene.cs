using UnityEngine;
using System.Collections;



/// <summary>
/// ポーズ画面クラス
/// </summary>
public class ChapterSelectPauseScene : MonoBehaviour
{

    // メンバ変数
    [SerializeField]
    private UIWidget[] pauseGraphicWidgetArray;   // ポーズ中の画像ウィジェット配列
    [SerializeField]
    private UIWidget[] pauseMenuWidgetArray;      // ポーズ中のメニューウィジェット配列
    //   [SerializeField] string testname;
    private static bool isPause;                // ポーズ中か?( true : ポーズしている | false : ポーズしてない )
    private static bool wasPause;               // 前回ポーズしていたか?( true : ポーズしていた | false : ポーズしてなかった )
    private int currentMenuIndex;       // 現在のメニューインデックス
    private float prevTimeScale;          // 前回のタイムスケール
    private string currentSceneName;       //現在のシーン名
   


    /// <summary>
    /// ポーズ中(前回ポーズしていた)か?
    /// </summary>
    /// <returns></returns>
    public static bool IsPause() { return wasPause; }



    // Use this for initialization
    void Start()
    {


        // ウィジェットを無効にする
        InitializeWidgetEnable(false);


        currentSceneName = Application.loadedLevelName;
       


        isPause = false;
        wasPause = false;
        currentMenuIndex = 0;
        prevTimeScale = 0;
    }



    // Update is called once per frame
    void Update()
    {

        // ポーズのフラグを保存
        wasPause = isPause;



        // ポーズ処理
        Pause();

        // ポーズメニューの実行
        ExecutePauseMenu();

        // 現在のメニューインデックスの切り替え
        SwitchCurrentMenuIndex();
    }



    /// <summary>
    /// ウィジェットのアクティブ設定
    /// </summary>
    private void InitializeWidgetEnable(bool bEnable)
    {

        // 各種ウィジェットのアクティブを設定する

        // ポーズ中の画像ウィジェット配列
        if (null != pauseGraphicWidgetArray)
        {

            for (int i = 0; i < pauseGraphicWidgetArray.Length; i++)
            {

                if (pauseGraphicWidgetArray[i]) { pauseGraphicWidgetArray[i].enabled = bEnable; }
            }
        }

        // ポーズ中のメニューウィジェット配列
        if (null != pauseMenuWidgetArray)
        {

            for (int i = 0; i < pauseMenuWidgetArray.Length; i++)
            {

                if (pauseMenuWidgetArray[i]) { pauseMenuWidgetArray[i].enabled = bEnable; }
            }
        }
    }



    /// <summary>
    /// 現在のメニューインデックスの切り替え
    /// </summary>
    private void SwitchCurrentMenuIndex()
    {

        // ポーズ中でなければ処理しない
        if (!isPause) { return; }

        // nullチェック
        if (MyUtility.NullCheck(pauseMenuWidgetArray)) { return; }

        // 現在のメニューインデックスを保存
        int prevIndex = currentMenuIndex;



        // 現在のメニューインデックスを前方に戻す
        if (Input.GetAxis("Vertical") > 0) { currentMenuIndex--; }

        // 現在のメニューインデックスを後方に進める
        if (Input.GetAxis("Vertical") < 0) { currentMenuIndex++; }

        // 現在のメニューインデックスを調整
        currentMenuIndex = Mathf.Clamp(currentMenuIndex, 0, pauseMenuWidgetArray.Length - 1);



        // メニューインデックスが変化したら(一致しなかったら)メニューウィジェットの初期化
        if (!prevIndex.Equals(currentMenuIndex)) { InitializePauseMenuWidgetByCurrentIndex(); }
    }



    /// <summary>
    /// ポーズ処理
    /// </summary>
    private void Pause()
    {

        // ポーズするキーが押されたらポーズ
        if(Input.GetKeyDown(KeyCode.Escape))
         //if (Input.GetKeyDown(KeyCode.Joystick1Button7))
        {

            // ポーズしてなければポーズする
            if (!isPause)
            {

                // ポーズ中にする
                isPause = true;

                // ウィジェットを有効にする
                InitializeWidgetEnable(true);

                // 現在のメニューインデックスを初期化
                currentMenuIndex = 0;

                // メニューウィジェットの初期化
                InitializePauseMenuWidgetByCurrentIndex();

                // ポーズ直前のタイムスケールを保存
                prevTimeScale = Time.timeScale;

                // タイムスケールを0にする
                //
                // ここでタイムスケールの影響を受けるゲームオブジェクトは停止する
                //Time.timeScale  = 0;

                // ポーズ
                Pauser.Pause();
            }
        }
    }



    /// <summary>
    /// ポーズから再開
    /// </summary>
    private void Resume()
    {

        // ポーズをやめる
        isPause = false;

        // ウィジェットを無効にする
        InitializeWidgetEnable(false);

        // タイムスケールをポーズ直前に戻す
        //Time.timeScale = prevTimeScale;

        // ポーズ再開
        Pauser.Resume();
    }



    /// <summary>
    /// メニューウィジェットの初期化
    /// </summary>
    private void InitializePauseMenuWidgetByCurrentIndex()
    {

        // ポーズ中でなければ処理しない
        if (!isPause) { return; }

        // nullチェック
        if (MyUtility.NullCheck(pauseMenuWidgetArray)) { return; }



        // 非選択メニューのカラー
        float colorElement = (1.0f / 255) * 70;
        Color grayColor = new Color(colorElement, colorElement, colorElement, 1.0f);

        // メニューウィジェットのカラー設定
        for (int i = 0; i < pauseMenuWidgetArray.Length; i++)
        {

            // 現在のメニューインデックスでなければ非選択メニューのカラーに設定
            if (!currentMenuIndex.Equals(i))
            {

                pauseMenuWidgetArray[i].color = grayColor;
            }

            // 現在のメニューインデックスなら選択メニューのカラーに設定
            else
            {

                pauseMenuWidgetArray[i].color = Color.white;
            }
        }
    }



    /// <summary>
    /// ポーズメニューの実行
    /// </summary>
    private void ExecutePauseMenu()
    {

        // ポーズ中でなければ処理しない
        if (!isPause) { return; }

        // ポーズメニューを実行するキーが押されなければ処理しない
        if (!Input.GetKeyDown(KeyCode.Joystick1Button1)) { return; }

        // nullチェック
        if (MyUtility.NullCheck(pauseMenuWidgetArray)) { return; }

        if (0 >= pauseMenuWidgetArray.Length) { return; }

        currentMenuIndex = Mathf.Clamp(currentMenuIndex, 0, pauseMenuWidgetArray.Length - 1);
        UIWidget pauseMenuWidget = pauseMenuWidgetArray[currentMenuIndex];
        if (MyUtility.NullCheck(pauseMenuWidget)) { return; }



        // ポーズメニューウィジェットの名前別に処理
        string name = pauseMenuWidget.name;
        switch (name)
        {

            case "Pause_Label_Menu_Return":

                // ポーズから再開
                Resume();
                break;

            case "Pause_Label_Menu_Title":



                // ポーズから再開
                Resume();
                Debug.Log("title enter");

                //タイトルへ移動
                Application.LoadLevel("Title");
                break;

            // 該当無し
            default:

                // ポーズから再開
                Resume();
                break;
        }
    }
}
