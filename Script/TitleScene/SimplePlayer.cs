using UnityEngine;
using System.Collections;



public class SimplePlayer : MonoBehaviour {

    // メンバ変数

    // 移動の速さ
    [ SerializeField, Range( 0, 10 ) ]
    private float       speed = 1;
    
    // 移動量
    [ SerializeField, Range( 0, 10 ) ]
    private float       movement = 1;

    // 接地しているか?( true : 接地している | false : 接地してない )
    private bool        isGrounded;

    // 移動方向
    private Vector3     direction;

    // 移動距離( 移動ベクトルを蓄積するバッファ )
    private Vector3     migrationLength;

    // 前回の座標
    private Vector3     prevPosition;

    // アニメータ
    private Animator    animator;



	// Use this for initialization
	void Start () {
	
        isGrounded      = false;
        direction       = Vector3.right;
        migrationLength = Vector3.zero;
        prevPosition    = transform.position;
        animator        = GetComponent<Animator>();
	}
	


	// Update is called once per frame
	void Update () {

        // 移動処理
        Move();

        // アニメータのパラメータの設定
        SetAnimatorParameter();



if ( transform.position.magnitude >= 1000 ) { Debug.Log( "Destroy " + transform.name + " !!!" ); Destroy( gameObject ); }



        // 座標を保存
        prevPosition = transform.position;
	}



    void OnCollisionStay2D( Collision2D collision ) {
    
        string name = collision.transform.name;
        switch ( name ) {
        
            case "Cloud":

                isGrounded = ( Mathf.Abs( Movement().y ) <= 0 );

                break;
        }
    }



    /// <summary>
    /// 1フレームの移動量( ベクトル )の取得
    /// </summary>
    /// <returns></returns>
    private Vector3 Movement() { return transform.position - prevPosition; }



    /// <summary>
    /// アニメータのパラメータの設定
    /// </summary>
    private void SetAnimatorParameter() {
    
        // nullチェック
        if ( MyUtility.NullCheck( animator ) ) { return; }



        animator.SetFloat( "MoveVectorX", Mathf.Abs( Movement().x ) );

        animator.SetFloat( "MoveVectorY", Movement().y );
    }



    /// <summary>
    /// 移動処理
    /// </summary>
    private void Move() {
    
        // 接地していなければ処理しない
        if ( !isGrounded ) { return; }



        // 移動ベクトル
        Vector3 vec3MoveVector = direction.normalized * speed * Time.deltaTime;

        // 移動
        transform.position += vec3MoveVector;

        // 移動距離として移動ベクトルを加算
        migrationLength += vec3MoveVector;

        // 移動距離が移動量を超えた場合
        if ( migrationLength.magnitude > movement ) {
        
            // 超えた分の逆向きの移動ベクトル
            vec3MoveVector = -direction.normalized * ( migrationLength.magnitude - movement );

            // 移動
            transform.position += vec3MoveVector;



            // 移動距離を初期化
            migrationLength = Vector3.zero;

            // 移動方向を逆向きにする
            direction *= -1;

            // Y軸に反転する
            transform.Rotate( Vector3.up, 180 );
        }
    }
}
