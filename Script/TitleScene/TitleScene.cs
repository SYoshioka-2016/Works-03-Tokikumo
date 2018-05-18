using UnityEngine;
using System.Collections;



/// <summary>
/// タイトルシーン管理クラス
/// </summary>
public class TitleScene : MonoBehaviour {

    // メンバ変数
    private static bool                     playerJumpFlg = false;  // プレイヤーがジャンプする演出の有効設定フラグ( true : 有効 | false : 無効 )
//    private static bool                     playerJumpFlg = true;

    [SerializeField] private GameObject     player;                 // プレイヤー
    [SerializeField] private GameObject     cloud;                  // 雲

    private bool                            isEndScene;             // シーン終了か?( true : 終了 | false : 継続 )
    private bool                            fadeFlg;                // フェード中の情報フラグ( true : フェードしていた | false : フェードしてなかった )



	// Use this for initialization
	void Start () {
	
        isEndScene  = false;
        fadeFlg     = false;


        
        // プレイヤーがジャンプする演出によってゲームオブジェクトのアクティブを設定

        // プレイヤー
        if ( !MyUtility.NullCheck( player ) ) { player.SetActive( playerJumpFlg ); }

        // 雲
        if ( !MyUtility.NullCheck( cloud ) ) { cloud.SetActive( playerJumpFlg ); }



//        if ( playerJumpFlg ) { playerJumpFlg = false; }
	}

	

	// Update is called once per frame
	void Update () {

        // プレイヤーの破棄
        DestroyPlayer();

        // シーンが終了していれば処理しない
        if ( isEndScene ) { return; }


	
        // プレイヤーのジャンプ
        JumpPlayer();



        // シーン切り替え
        if ( (!FadeScript.Instance.isFade && !GameObject.Find("ResetManager").GetComponent<ResetManager>().reset)
		    &&(Input.GetKeyDown( KeyCode.Space )
		    || Input.GetKeyDown(KeyCode.JoystickButton0)|| Input.GetKeyDown(KeyCode.JoystickButton1)
		    || Input.GetKeyDown(KeyCode.JoystickButton2)|| Input.GetKeyDown(KeyCode.JoystickButton3)
		    || Input.GetKeyDown(KeyCode.JoystickButton7)) )
		{ if ( playerJumpFlg ) { playerJumpFlg = false; }
			FadeScript.Instance.FadeOutStart( "ChapterSelect" );
            isEndScene = true;
            AudioManager.Instance.BGMFade();
        }


        // フェードの情報を保存
        fadeFlg = FadeScript.Instance.isFade;
	}



    /// <summary>
    /// プレイヤーがジャンプする演出の有効設定
    /// </summary>
    public static void ActivatePlayerJump() { playerJumpFlg = true; }



    /// <summary>
    /// プレイヤーがジャンプする演出の無効設定
    /// </summary>
    public static void DeactivatePlayerJump() { playerJumpFlg = false; }


    
    /// <summary>
    /// フェード終了の瞬間( true : 終了の瞬間 | false : 終了してない、又は終了している )
    /// </summary>
    /// <returns></returns>
    private bool IsJustEndFade() { return ( fadeFlg && !FadeScript.Instance.isFade ); }



    /// <summary>
    /// プレイヤーのジャンプ
    /// </summary>
    private void JumpPlayer() {
        
        // プレイヤーがジャンプする演出の有効設定フラグが降りていれば処理しない
        if ( !playerJumpFlg ) { return; }
    
        // nullチェック
        if ( MyUtility.NullCheck( player ) ) { return; }



        // フェードが終了したらプレイヤーをジャンプさせる
        if ( IsJustEndFade() ) {

            // プレイヤーに上向きの力を加える
            AppryForce playersAppryForce = player.GetComponent<AppryForce>();
            if ( !MyUtility.NullCheck( playersAppryForce ) ) { playersAppryForce.AppryTheForce(); }

            // プレイヤーがジャンプする演出の有効設定フラグを降ろす
            playerJumpFlg = false;
        }
    }



    /// <summary>
    /// プレイヤーの破棄
    /// </summary>
    private void DestroyPlayer() {
    
        // プレイヤーが無ければ処理しない
        if ( null == player ) { return; }



        // プレイヤーの原点からの距離を取得
        Vector3 vec3Distance = 
            new Vector3( 
                Mathf.Abs( player.transform.position.x ), 
                Mathf.Abs( player.transform.position.y ), 
                Mathf.Abs( player.transform.position.z ) 
            );

        // 原点から一定距離離れていれば破棄
        if ( 
            500 <= vec3Distance.x || 
            500 <= vec3Distance.y || 
            500 <= vec3Distance.z  
        ) {
            Destroy( player );
        }
    }
}
