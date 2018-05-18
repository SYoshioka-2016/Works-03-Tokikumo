using UnityEngine;
using System.Collections;
using System.Collections.Generic;



/// <summary>
/// ユーティリティクラス
/// </summary>
public class MyUtility {

    // メンバ変数



    /// <summary>
    /// コンストラクタ
    /// </summary>
    private MyUtility() { }
    private MyUtility( MyUtility inst ) { }



    /// <summary>
    /// 変数のnullチェック
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="bIsMessageOn"></param>
    /// <returns></returns>
    public static bool NullCheck( System.Object obj, bool bIsMessageOn = true ) { 
        
        // null許容型の変数がnullの場合
        if ( null == obj ) {
        
            // エラーログを表示
            if ( bIsMessageOn ) { Debug.LogWarning( "null値の変数です。" ); }

            return true;
        }



        return false; 
    }
}
