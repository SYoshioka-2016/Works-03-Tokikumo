using UnityEngine;
using System.Collections;



/// <summary>
/// ウィジェットカラー点滅クラス
/// </summary>
public class FlashingWidget : MonoBehaviour {

    /// <summary>
    /// アルファ値の増減の方向
    /// </summary>
    private enum FadeDirection {

        FADE_IN,    // 増加方向
        FADE_OUT,   // 減少方向
    }



    // メンバ変数
    [SerializeField] private float      TIME_INTERVAL               = 1.0f;     // アルファ値折り返しの時間間隔(秒)
    [SerializeField] private bool       playFlash                   = true;     // 点滅再生フラグ(true : 点滅する | false : 点滅しない)

    private UIWidget                    widget;                                 // ウィジェット
    private float                       currentAlpha;                           // アルファ値
    private FadeDirection               fadeDirection;                          // アルファ値の増減の方向



	// Use this for initialization
	void Start () {

        TIME_INTERVAL           = Mathf.Abs( TIME_INTERVAL );

        if ( !widget ) { widget = GetComponent<UIWidget>(); }



        SetWidgetColorAlpha( 1.0f );

        fadeDirection = FadeDirection.FADE_IN;
	}


	
	// Update is called once per frame
	void Update () {
	
        // 点滅
        Flashing();
	}


/*
    /// <summary>
    /// 点滅再生
    /// </summary>
    /// <param name="color"></param>
    public void PlayFlash( Color color ) {

        playFlash = true;

        SetWidgetColor( color );
    }
*/
    /// <summary>
    /// 点滅再生
    /// </summary>
    /// <param name="color"></param>
    public void PlayFlash( float fAlpha ) {

        playFlash = true;

        SetWidgetColorAlpha( fAlpha );
    }


    /// <summary>
    /// 点滅停止
    /// </summary>
    public void StopFlash() {

        playFlash = false;
    }



    /// <summary>
    /// 点滅
    /// </summary>
    private void Flashing() { 

        // nullチェック
        if ( MyUtility.NullCheck( widget ) ) { return; }
        if ( MyUtility.NullCheck( widget.color ) ) { return; }

        // 点滅再生フラグが降りていれば処理しない
        if ( !playFlash ) { return; }
    


        // 点滅
        switch ( fadeDirection ) {

            // 増加方向
            case FadeDirection.FADE_IN:

                // アルファ値を増加
                currentAlpha += Time.deltaTime / TIME_INTERVAL;

                // アルファ値が最大以上ならアルファ値の増減を減少方向にする
                if ( 1.0f <= currentAlpha ) { fadeDirection = FadeDirection.FADE_OUT; }

                break;



            // 減少方向
            case FadeDirection.FADE_OUT:

                // アルファ値を減少
                currentAlpha -= Time.deltaTime / TIME_INTERVAL;

                // アルファ値が最小以下ならアルファ値の増減を増加方向にする
                if ( 0.0f >= currentAlpha ) { fadeDirection = FadeDirection.FADE_IN; }

                break;
        }

        // ウィジェットカラーのアルファ値を変更
        SetWidgetColorAlpha( currentAlpha );
    }



    /// <summary>
    /// ウィジェットカラーの設定
    /// </summary>
    /// <param name="color"></param>
    private void SetWidgetColor( Color color ) { 
    
        // nullチェック
        if ( MyUtility.NullCheck( color ) ) { return; }
        if ( MyUtility.NullCheck( widget ) ) { return; }
        if ( MyUtility.NullCheck( widget.color ) ) { return; }



        // ウィジェットのカラーを設定
        widget.color  = color;

        // 現在のアルファ値を取得
        currentAlpha = widget.color.a;
    }



    /// <summary>
    /// ウィジェットカラーのアルファ値の設定
    /// </summary>
    /// <param name="alpha"></param>
    private void SetWidgetColorAlpha( float fAlpha ) { 
    
        // アルファ値を調整
        fAlpha = Mathf.Clamp( fAlpha, 0.0f, 1.0f );

        // ウィジェットのカラーを設定
        Color color     = widget.color;
        color.a         = fAlpha;
        SetWidgetColor( color );
    }
}
