using UnityEngine;
using System.Collections;



/// <summary>
/// 音符ブロッククラス
/// </summary>
public class NoteBlock : MonoBehaviour {

    // 音符ブロックの状態列挙
    private enum NOTE_BLOCK_STATE {
        STANDBY,    // 待機
        DETECT,     // 検知
        PROCESSED,  // 処理済み
    }



    // メンバ変数
    [SerializeField]private uint    myNumber        = 0;    // 自分の番号

    private AudioSource             audioSource;            // オーディオソース
    private SpriteRenderer          spriteRenderer;         // スプライトレンダラ
    private NOTE_BLOCK_STATE        state;                  // 音符ブロックの状態

    private bool                    effectiveSetOfSoundFlg; // サウンド再生の有効設定フラグ( true : 有効 | false : 無効 )



	// Use this for initialization
	void Awake () {
	
        // コンポーネントを取得
        audioSource     = GetComponent<AudioSource>();
        spriteRenderer  = GetComponent<SpriteRenderer>();
        
        // リセット
        Reset();
	}


	
	// Update is called once per frame
	void LateUpdate () {
	
        // 音符ブロックの状態が検知状態なら処理済み状態にする
        if ( state.Equals( NOTE_BLOCK_STATE.DETECT ) ) { state = NOTE_BLOCK_STATE.PROCESSED; }
	}



    /// <summary>
    /// 自分の番号の取得
    /// </summary>
    /// <returns></returns>
    public int GetMyNumber() { return (int)myNumber; }



    /// <summary>
    /// 自分の番号の設定
    /// </summary>
    /// <param name="uiNumber"></param>
    public void SetMyNumber( uint uiNumber ) { myNumber = uiNumber; }



    /// <summary>
    /// 検知の瞬間か?( true : 検知の瞬間 | false : 検知してない、又は既に検知した )
    /// </summary>
    /// <returns></returns>
    public bool IsDetectNow() { return ( state.Equals( NOTE_BLOCK_STATE.DETECT ) ); }



    /// <summary>
    /// 処理済みか?( true : 処理済み | false : 処理してない )
    /// </summary>
    /// <returns></returns>
    public bool IsProcessed() { return ( state.Equals( NOTE_BLOCK_STATE.PROCESSED ) ); }



    /// <summary>
    /// サウンド再生の有効設定
    /// </summary>
    public void Activate() { effectiveSetOfSoundFlg = true; }



    /// <summary>
    /// サウンド再生の無効設定
    /// </summary>
    public void Deactivate() { effectiveSetOfSoundFlg = false; }



    /// <summary>
    /// リセット
    /// </summary>
    public void Reset() {
    
        // 音符ブロックの状態を待機状態にする
        state = NOTE_BLOCK_STATE.STANDBY;

        // サウンド再生の有効設定
        Activate();

    // カラーの変更
    ChangeColor();
    }



    void OnTriggerStay2D( Collider2D collider ) {
    
        // タグ別に処理
        if (collider.transform.tag == "Player")
        {
            // x軸方向の距離が一定距離より小さくなればサウンド再生
            if (Mathf.Abs(transform.position.x - collider.transform.position.x) <= 0.1f)
            {
                PlaySound();
            }
        }
        else if( collider.transform.tag == "Cloud" )
            PlaySound();
    }



    /// <summary>
    /// サウンド再生
    /// </summary>
    private void PlaySound() {
    
        // 音符ブロックの状態が待機状態でないなら処理しない
        if ( !state.Equals( NOTE_BLOCK_STATE.STANDBY ) ) { return; }

        // nullチェック
        if ( MyUtility.NullCheck( audioSource ) ) { return; }



        // 音符ブロックの状態を検知状態にする
        state = NOTE_BLOCK_STATE.DETECT;

        // サウンド再生の有効設定フラグが立っている場合
        if ( effectiveSetOfSoundFlg ) {

            // サウンドを再生
            audioSource.Play();

            // カラーの変更
            ChangeColor();
        }
    }


    /// <summary>
    /// カラーの変更
    /// </summary>
    private void ChangeColor() {

        // nullチェック
        if ( MyUtility.NullCheck( spriteRenderer ) ) { return; }



        // 音符ブロックの状態別にカラーを設定
        Color color = Color.white;
        switch ( state ) {

            // 待機状態
            case NOTE_BLOCK_STATE.STANDBY: color = Color.white; break;
              
  

            // 検知状態
            case NOTE_BLOCK_STATE.DETECT:

            // 処理済み状態
            case NOTE_BLOCK_STATE.PROCESSED: color = Color .gray; break;
        }

        // スプライトのカラーを設定
        spriteRenderer.color = color;
    }
}
