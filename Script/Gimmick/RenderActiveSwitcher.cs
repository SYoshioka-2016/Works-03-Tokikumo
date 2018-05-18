using UnityEngine;
using System.Collections;



/// <summary>
/// 描画有効スイッチクラス
/// </summary>
public class RenderActiveSwitcher : MonoBehaviour {

    // メンバ変数
    [SerializeField] private int    triggerCountNumber = 1;     // 描画を無効にする番号

    private bool                    isEnebledTrigger;           // トリガーを有効にしていたか( true : 有効にしていた | false : 無効にしていた )
    private Renderer                render;                     // 描画コンポーネント
    private Collider2D              collider;                   // コライダコンポーネント



	// Use this for initialization
	void Start () {

        isEnebledTrigger = false;

        // コンポーネントを取得
        render      = GetComponent<SpriteRenderer>();
        collider    = GetComponent<BoxCollider2D>();
	}
	


	// Update is called once per frame
	void LateUpdate () {

        // nullチェック
        if ( MyUtility.NullCheck( render ) ) { return; }
        if ( MyUtility.NullCheck( collider ) ) { return; }


/*
        // 現在のカウントが指定の番号でないときに描画を有効にする
        render.enabled      = !FrameCountor.Instance.GetCountNumber().Equals( triggerCountNumber );

        // 描画が無効か、又はトリガーを有効にしていたらトリガーを有効にする
        collider.isTrigger = ( !render.enabled || isEnebledTrigger );
*/



        render.enabled      = !FrameCountor.Instance.GetCountNumber().Equals( triggerCountNumber );
        collider.enabled    = render.enabled;
	}


/*
    void OnCollisionStay2D( Collision2D col ) {
    
        // nullチェック
        if ( MyUtility.NullCheck( collider ) ) { return; }



        // タグ別に処理
        switch ( col.transform.tag ) {

            // プレイヤーならトリガーの有効設定を保存
            case "Player": isEnebledTrigger = collider.isTrigger; break;
        }
    }



    void OnTriggerStay2D( Collider2D col ) {

        // タグ別に処理
        switch ( col.tag ) {

            // プレイヤーならトリガーを有効にする
            case "Player": isEnebledTrigger = true; break;
        }
    }



    void OnTriggerExit2D( Collider2D col ) {

        // タグ別に処理
        switch ( col.tag ) {

            // プレイヤーならトリガーを無効にする
            case "Player":  isEnebledTrigger = false; break;
        }
    }
*/
}
