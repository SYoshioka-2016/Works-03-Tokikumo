using UnityEngine;
using System;
using System.Collections;



/// <summary>
/// 音符ブロック昇順ソート用Comparerクラス
/// </summary>
public class NoteBlockNumberComparer : IComparer {

    public int Compare( object x, object y ) {

        NoteBlock noteBlock1 = (NoteBlock)x;
        NoteBlock noteBlock2 = (NoteBlock)y;

        return noteBlock1.GetMyNumber() - noteBlock2.GetMyNumber();
    }
}



/// <summary>
/// 音符ブロックシャッタークラス
/// </summary>
public class NoteBlockShutter : MonoBehaviour {

    /// <summary>
    /// シャッターの状態列挙
    /// </summary>
    private enum SHUTTER_STATE {
    
        STANDBY,    // 待機
        LIFT_UP,    // リフトアップ
        STAY,       // 滞在
    }



    // メンバ変数
    [SerializeField] 
    private NoteBlock[]     noteBlocks;                 // 音符ブロック配列

    [SerializeField, Range( 0, 0.5f )] 
    private float           speed           = 0.1f;     // 移動の速さ

    [SerializeField, Range( 1, 10 )] 
    private float           movement        = 3;        // 移動量

    private Vector3         moveDirection;              // 移動方向
    private Vector3         startPoint;                 // 移動開始位置
    private Vector3         distance;                   // 移動距離

    private SHUTTER_STATE   state;                      // シャッターの状態

    private uint            currentNumber;              // 現在の番号



	// Use this for initialization
	void Start () {
	
        moveDirection       = Vector3.zero;
        startPoint          = transform.position;
        distance            = Vector3.zero;
        state               = SHUTTER_STATE.STANDBY;
        currentNumber       = 0;



        // 音符ブロック配列の要素を昇順にソート
        IComparer comparer  = new NoteBlockNumberComparer();
        Array.Sort( noteBlocks, comparer );

        // 音符ブロックの初期化
        InitializeNoteBlocks();
	}
	


	// Update is called once per frame
	void Update () {

        // 状態別処理
        SwitchingProcess();
	}



    /// <summary>
    /// 音符ブロックの初期化
    /// </summary>
    private void InitializeNoteBlocks() {
    
        // nullチェック
        if ( MyUtility.NullCheck( noteBlocks ) ) { return; }



        // 音符ブロックの数だけループ
        for ( int i = 0; i < noteBlocks.Length; i++ ) {
        
            if ( !MyUtility.NullCheck( noteBlocks[i] ) ) { 
                
                // 音符ブロックをリセット
                noteBlocks[i].Reset();

                // 要素の格納順に音符ブロックの番号を振り分ける
                noteBlocks[i].SetMyNumber( (uint)i );

                // 音符ブロックのサウンド再生の設定を無効にする
                noteBlocks[i].Deactivate();
            }
        }
        
        // 最初の音符ブロックのサウンド再生の設定を有効にする
        if ( !MyUtility.NullCheck( noteBlocks[0] ) ) { noteBlocks[0].Activate(); }
    }



    /// <summary>
    /// 移動
    /// </summary>
    private void Move() {
    
        // シャッターの状態がリフトアップ状態でなければ処理しない
        if ( !state.Equals( SHUTTER_STATE.LIFT_UP ) ) { return; }



        // 移動ベクトルを設定
        Vector3 vec3MoveVector = moveDirection.normalized * speed;

        // 移動距離を設定
        distance = transform.position - startPoint;

        // 移動距離の大きさが移動量よりも大きくなった場合
        if ( distance.magnitude > movement ) {
        
            // 移動ベクトルを設定
            vec3MoveVector = -vec3MoveVector.normalized * ( distance.magnitude - movement );

            // 移動方向を初期化
            moveDirection = Vector3.zero;

            // シャッターの状態を滞在状態にする
            state = SHUTTER_STATE.STAY;
        }

        // 移動
        transform.position += vec3MoveVector;
    }



    /// <summary>
    /// 待機
    /// </summary>
    private void Standby() {
    
        // シャッターの状態が待機状態でなければ処理しない
        if ( !state.Equals( SHUTTER_STATE.STANDBY ) ) { return; }

        // nullチェック
        if ( MyUtility.NullCheck( noteBlocks ) ) { return; }
	


        // 音符ブロックの数だけループ
        for ( int i = 0; i < noteBlocks.Length; i++ ) { 
            
            // 音符ブロックが検知した場合
            if ( noteBlocks[i].IsDetectNow() ) { 
                
                // 音符ブロックの番号が現在の番号と一致した場合
                if ( noteBlocks[i].GetMyNumber() == currentNumber ) {
                
                    // 音符ブロックのサウンド再生の設定を無効にする
                    noteBlocks[i].Deactivate();

                    // 次の音符ブロックのサウンド再生の設定を有効にする
                    if ( i < noteBlocks.Length - 1 ) { noteBlocks[i + 1].Activate(); }

                    // 現在の番号を更新
                    currentNumber++;
                }
                // 音符ブロックの番号が現在の番号と一致しなかった場合
                else {
            
                    // 現在の番号を初期化
                    currentNumber = 0;

                    // 音符ブロックの初期化
                    InitializeNoteBlocks();
                }
            }
        }

        // 現在の番号が音符ブロックの個数以上になったらリフトアップする
        if ( currentNumber >= noteBlocks.Length ) { 
            
            // 移動方向を上向きにする
            moveDirection   = Vector3.up; 

            // シャッターの状態をリフトアップ状態にする
            state           = SHUTTER_STATE.LIFT_UP;

            // 現在の番号を初期化
            currentNumber   = 0;
        }
    }



    /// <summary>
    /// 状態別処理
    /// </summary>
    private void SwitchingProcess() {
    
        // シャッターの状態別処理
        switch ( state ) {
        
            // 待機状態
            case SHUTTER_STATE.STANDBY: Standby(); break;



            // リフトアップ状態
            case SHUTTER_STATE.LIFT_UP: Move(); break;
        }
    }
}
