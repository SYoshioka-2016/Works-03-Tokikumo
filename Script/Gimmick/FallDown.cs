using UnityEngine;
using System.Collections;



/// <summary>
/// 落下クラス
/// </summary>
public class FallDown : MonoBehaviour {

    // メンバ変数
    [SerializeField] private float  speed               = 1.0f;     // 移動の速さ

    private int                     prevCountNumber;                // 前回のカウント番号
    private Vector3                 vectorFromMyselfToOtherObj;     // 自分から真下のゲームオブジェクトへのベクトル( 自分のbottomピポッドから相手のupピポッド )
    private bool                    moveTrigger;                    // 移動開始のトリガー
    private bool                    permitFlg;                      // 落下許可フラグ( true : 許可 | false : 禁止 )



	// Use this for initialization
	void Start () {

        prevCountNumber = FrameCountor.Instance.GetCountNumber();
        moveTrigger     = false;



        // 真下のゲームオブジェクトへのベクトルの計算処理
        DirectionComputation();
	}
	


	// Update is called once per frame
	void Update () {

        // 移動の速さを調整
        speed = Mathf.Abs( speed );



        // 真下のゲームオブジェクトへのベクトルの計算処理
        DirectionComputation();

        // 移動開始のトリガー検知
        MoveTrigger();

        // カウント番号が切り替わるタイミングで下方向に移動
        if ( IsChangeCountNumber() ) { MoveDown(); }

        // カウント番号を保存
        prevCountNumber = FrameCountor.Instance.GetCountNumber();
	}



    /// <summary>
    /// 真下のゲームオブジェクトへのベクトルの計算処理
    /// </summary>
    private void DirectionComputation() {

        // レイキャストの衝突情報
        RaycastHit2D    hit;

        // 自分からレイの衝突位置へのベクトル
        Vector3         vec3RayVector   = Vector3.zero;

        // レイキャスト数
        const uint      uiRaycastCount  = 2;



        // 自分から真下のゲームオブジェクトへのベクトルを最大値で初期化
        vectorFromMyselfToOtherObj = new Vector3( float.MaxValue, float.MaxValue, float.MaxValue );



        // レイキャスト数だけレイキャストする
        for ( uint i = 1; i <= uiRaycastCount; i++ ) {

            // レイを飛ばす開始位置
            //
            // ここでは自身のゲームオブジェクトの真下より上の位置にする必要がある。
            // 自身のゲームオブジェクトの真下に合わせると、
            // 隣接する真下のオブジェクトと衝突しない事がある。
            Vector3 vec3RaycastStartPoint = transform.position - ( new Vector3( transform.localScale.x, 0, 0 ) / 2 ) + ( new Vector3( transform.localScale.x / ( uiRaycastCount + 1 ), 0, 0 ) * i );

            // 下方向に飛ばしたレイの衝突情報を取得
            hit = Physics2D.Raycast( vec3RaycastStartPoint, Vector3.down );

            // 衝突したゲームオブジェクトが有る場合
            if ( null != hit.collider ) {
            
                // 自分から真下のゲームオブジェクトへのベクトルを求める
                //
                // レイを飛ばす開始位置に合わせて始点を自身のゲームオブジェクトの真下にする。
                vec3RayVector = vec3RaycastStartPoint - new Vector3( 0, transform.localScale.y / 2, 0 ) - (Vector3)hit.point;

                // 方向ベクトルの大きさが小さければ零ベクトルとする
                if ( 0.01f >= vec3RayVector.magnitude ) { vectorFromMyselfToOtherObj = Vector3.zero; }
            }

            // 自分からレイの衝突位置へのベクトルで1番小さいものを採用する
            if ( vec3RayVector.magnitude <= vectorFromMyselfToOtherObj.magnitude ) { vectorFromMyselfToOtherObj = vec3RayVector; }
        }
    }



    /// <summary>
    /// カウント番号の切り替えの瞬間か?
    /// </summary>
    /// <returns></returns>
    private bool IsChangeCountNumber() {

        return !FrameCountor.Instance.GetCountNumber().Equals( prevCountNumber );
    }



    /// <summary>
    /// 移動開始のトリガー検知
    /// </summary>
    private void MoveTrigger() {
        
        // 既に移動開始のトリガーが発動していれば処理しない
        if ( moveTrigger ) { return; }


        
        // レイキャスト数
        const uint      uiRaycastCount  = 2;

        // レイキャストの衝突情報
        RaycastHit2D[]  hits            = new RaycastHit2D[ uiRaycastCount ];



        // レイキャスト数だけレイキャストする
        for ( uint i = 1; i <= uiRaycastCount; i++ ) {

            // レイを飛ばす開始位置
            Vector3 vec3RaycastStartPoint = transform.position - ( new Vector3( transform.localScale.x, 0, 0 ) / 2 ) + ( new Vector3( transform.localScale.x / ( uiRaycastCount + 1 ), 0, 0 ) * i );

            // 下方向に飛ばしたレイの全ての衝突情報を取得
            hits = Physics2D.RaycastAll( vec3RaycastStartPoint, Vector3.down );

            // プレイヤーと衝突したらトリガーを発動する
            foreach ( var hit in hits ) { if ( hit.transform.tag.Equals( "Player" ) ) { moveTrigger = permitFlg = true; } }
        }
    }



    /// <summary>
    /// 下方向に移動
    /// </summary>
    private void MoveDown() {

        // 落下許可フラグが降りていれば処理しない
        if ( !permitFlg ) { return; }

        // 自分から真下のゲームオブジェクトへのベクトルの大きさが0なら処理しない
        if ( 0 >= vectorFromMyselfToOtherObj.magnitude ) { return; }



        const uint  uiPrecision   = 100;                    // 移動精度
        float       fMovement     = speed / uiPrecision;    // 下方向の移動量

        // 残りの移動距離をチェックしながら少しずつ移動
        for ( uint i = 0; i < uiPrecision; i++ ) {

            // 移動量が真下のゲームオブジェクトまでの距離に収まれば一定量移動する
            if ( fMovement < vectorFromMyselfToOtherObj.y ) {

                // 下方向に移動量の分だけ移動
                transform.position += Vector3.down * fMovement;

                // 移動した分だけ真下のゲームオブジェクトまでの距離を減らす
                vectorFromMyselfToOtherObj.y -= fMovement;
            }
            // 収まらなければ差分を移動して終了
            else {

                // 差分だけ下方向に移動
                transform.position += Vector3.down * vectorFromMyselfToOtherObj.y;
                break; 
            }
        }
    }



    void OnCollisionStay2D( Collision2D col ) {
    
        // タグ別に処理
        switch ( col.transform.tag ) {

            case "Player": 

                permitFlg = false; 
                transform.position += Vector3.up * 1f * Time.deltaTime;
                break;
        }
    }



    void OnCollisionExit2D( Collision2D col ) {
    
        // タグ別に処理
        switch ( col.transform.tag ) {

            case "Player": permitFlg = moveTrigger && true; break;
        }
    }
}
