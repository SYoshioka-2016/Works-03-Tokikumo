using UnityEngine;
using System.Collections;



/// <summary>
/// テンプレートMonoBehaviourシングルトンクラス
/// </summary>
/// <typeparam name="T"></typeparam>
public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviourSingleton<T> {

    // メンバ変数
    protected static T instance;    // 自己インスタンス



    /// <summary>
    /// コンストラクタ
    /// </summary>
    protected MonoBehaviourSingleton() { }
    protected MonoBehaviourSingleton ( MonoBehaviourSingleton<T> r ) { }



    /// <summary>
    /// インスタンスゲッタ
    /// </summary>
    public static T Instance {

        get {

            if ( null == instance ) {

                instance = (T)FindObjectOfType( typeof(T) );
    
                MyUtility.NullCheck( instance );
            }
   
            return instance;
        }
    }



    protected void Awake() {

        // インスタンスチェック
        CheckInstance();
    }



    /// <summary>
    /// インスタンスチェック
    /// </summary>
    /// <returns></returns>
    protected bool CheckInstance() {

        // インスタンスが登録されていなければ自身を登録する
        if ( null == instance ) {

            instance = (T)this;

            return true;
        }

        // 自身が登録されていれば処理しない
        else if ( this == Instance ) { return true; }



        Destroy( this );

        return false;
    }
}
