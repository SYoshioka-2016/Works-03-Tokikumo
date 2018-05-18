using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;



//http://ftvoid.com/blog/post/662
//http://notargs.com/blog/?p=313



/// <summary>
/// ポーズクラス
/// </summary>
public class Pauser : MonoBehaviour {

    // メンバ変数
    [SerializeField] private GameObject[]   ignoreGameObjects;                                      // ポーズ無視リスト(このPauserコンポーネントを持つゲームオブジェクト以下の階層にあり、ポーズさせないゲームオブジェクト)
 
	private static List<Pauser>             pauseTargets                    = new List<Pauser>();	// ポーズ対象のスクリプト(ゲームオブジェクト)
 
	private Behaviour[]                     pauseBehaviours                 = null;                 // ポーズ対象のコンポーネント
 
	private Rigidbody[]                     rigidbodies                     = null;                 // ポーズ対象のRigidbody
	private Vector3[]                       rigidbodyVelocities             = null;                 // Rigidbodyの速度保存用
	private Vector3[]                       rigidbodyAngularVelocities      = null;                 // Rigidbodyの角速度保存用
 
	private Rigidbody2D[]                   rigidbody2Ds                    = null;                 // ポーズ対象のRigidbody2D
	private Vector2[]                       rigidbody2DVelocities           = null;                 // Rigidbody2Dの速度保存用
	private float[]                         rigidbody2DAngularVelocities    = null;                 // Rigidbody2Dの角速度保存用



	// Use this for initialization
	void Start () {

        // 自身をポーズ対象に追加する
        pauseTargets.Add( this );
	}


	
    /// <summary>
    /// ポーズ
    /// </summary>
    public static void Pause() {

        foreach ( var obj in pauseTargets ) { obj.OnPause(); }
    }



    /// <summary>
    /// ポーズ再開
    /// </summary>
    public static void Resume() { 
    
        foreach ( var obj in pauseTargets ) { obj.OnResume(); }
    }



	/// <summary>
    /// ポーズ処理
    /// </summary>
	private void OnPause() {
        
        // 自身が無いなら処理しない
        if ( null == this ) { return; }
		
        // ポーズ対象のコンポーネントが有れば処理しない
        //
        // この後にポーズ対象のコンポーネントを取得してポーズの処理を行う為、
        // 既にポーズ対象のコンポーネントが有るなら処理済みである。
        if ( null != pauseBehaviours ) { return; }



        // Behaviour用Predicateデリゲート
        Predicate<Behaviour> behaviourPredicate;

        // ポーズを無視するゲームオブジェクトが有る場合
        if ( null != ignoreGameObjects && 0 < ignoreGameObjects.Length ) {

            // アクティブが有効なBehaviourコンポーネントで、
            // このPauserコンポーネントでなく、
            // ポーズを無視するゲームオブジェクトでないデリゲートを設定
            behaviourPredicate = 
			    obj =>
                    obj.enabled     && 
				    obj != this     && 
				    Array.FindIndex( ignoreGameObjects, gameObject => gameObject == obj.gameObject ) < 0;
        }
        // ポーズを無視するゲームオブジェクトが無い場合
        else {
        
            // アクティブが有効なコンポーネントで、
            // このPauserコンポーネントでないデリゲートを設定
            behaviourPredicate = 
			    obj => 
                    obj.enabled     && 
                    obj != this;
        }

        // 有効なBehaviourコンポーネントを取得
        //
        // このPauserコンポーネントを持つオブジェクトとそれ以下の下層のオブジェクトから、
        // Predicateデリゲートによってポーズ対象となるコンポーネントを全て取得する。
		pauseBehaviours = Array.FindAll( GetComponentsInChildren<Behaviour>(), behaviourPredicate );

        // ポーズ対象のコンポーネントのアクティブを切る
        //
        // コンポーネントのアクティブを切る事でUpdate関数が呼び出されなくなり、動作を停止させる。
		foreach ( var com in pauseBehaviours ) { com.enabled = false; }
		


        // Rigidbodyのポーズ処理
        
        // Rigidbody用Predicateデリゲート
        Predicate<Rigidbody> rigidbodyPredicate;
        
        // ポーズを無視するゲームオブジェクトが有る場合
        if ( null != ignoreGameObjects ) {

            // スリープモードでないRigidbodyで、
            // ポーズを無視するゲームオブジェクトでないデリゲートを設定
            rigidbodyPredicate = 
			    obj => !obj.IsSleeping() && Array.FindIndex( ignoreGameObjects, gameObject => gameObject == obj.gameObject) < 0;
        }
        // ポーズを無視するゲームオブジェクトが無い場合
        else {
        
            // スリープモードでないRigidbodyのデリゲートを設定
            rigidbodyPredicate = 
			    obj => !obj.IsSleeping();
        }

        // 有効なRigidbodyコンポーネントを取得
		rigidbodies    = Array.FindAll( GetComponentsInChildren<Rigidbody>(), rigidbodyPredicate );

        // 配列を生成
		rigidbodyVelocities         = new Vector3[ rigidbodies.Length ];
		rigidbodyAngularVelocities  = new Vector3[ rigidbodies.Length ];

        // Rigidbodyを停止する
		for ( var i = 0 ; i < rigidbodies.Length ; ++i ) {

            // ポーズ前のRigidbodyの速度を保存
			rigidbodyVelocities[i]          = rigidbodies[i].velocity;
            
            // ポーズ前のRigidbodyの角速度を保存
			rigidbodyAngularVelocities[i]   = rigidbodies[i].angularVelocity;
            
            // Rigidbodyをスリープ
			rigidbodies[i].Sleep();
		}



        // Rigidbody2Dのポーズ処理

        // Rigidbody2D用Predicateデリゲート
        Predicate<Rigidbody2D> rigidbody2DPredicate;

        // ポーズを無視するゲームオブジェクトが有る場合
        if ( null != ignoreGameObjects ) {

            // スリープモードでないRigidbody2Dで、
            // ポーズを無視するゲームオブジェクトでないデリゲートを設定
            rigidbody2DPredicate = 
			    obj => !obj.IsSleeping() && Array.FindIndex( ignoreGameObjects, gameObject => gameObject == obj.gameObject) < 0;
        }
        // ポーズを無視するゲームオブジェクトが無い場合
        else {
        
            // スリープモードでないRigidbody2Dのデリゲートを設定
            rigidbody2DPredicate = 
			    obj => !obj.IsSleeping();
        }
        
        // 有効なRigidbody2Dコンポーネントを取得
		rigidbody2Ds = Array.FindAll( GetComponentsInChildren<Rigidbody2D>(), rigidbody2DPredicate );
        
        // 配列を生成
		rigidbody2DVelocities           = new Vector2[ rigidbody2Ds.Length ];
		rigidbody2DAngularVelocities    = new float[ rigidbody2Ds.Length ];
        
        // Rigidbody2Dを停止する
		for ( var i = 0 ; i < rigidbody2Ds.Length ; ++i ) {
            
            // ポーズ前のRigidbody2Dの速度を保存
			rigidbody2DVelocities[i]        = rigidbody2Ds[i].velocity;
            
            // ポーズ前のRigidbody2Dの角速度を保存
			rigidbody2DAngularVelocities[i] = rigidbody2Ds[i].angularVelocity;
            
            // Rigidbody2Dをスリープ
			rigidbody2Ds[i].Sleep();
		}
	}



    /// <summary>
    /// ポーズ再開処理
    /// </summary>
	private void OnResume() {

        // ポーズ対象のコンポーネントが無ければ処理しない
        //
        // この後にポーズ対象のコンポーネントを全てクリアする為、
        // 既にポーズ対象のコンポーネントが無いなら処理済みである。
		if ( null == pauseBehaviours ) { return; }


 
		// 全てのポーズ対象のコンポーネントのアクティブを有効にする
		foreach ( var com in pauseBehaviours ) { com.enabled = true; }

		

        // Rigidbodyを再開する
		for ( var i = 0 ; i < rigidbodies.Length ; ++i ) {

            // Rigidbodyをウェイクアップ
			rigidbodies[i].WakeUp();

            // Rigidbodyの速度をポーズ前に設定
			rigidbodies[i].velocity         = rigidbodyVelocities[i];
            
            // Rigidbodyの角速度をポーズ前に設定
			rigidbodies[i].angularVelocity  = rigidbodyAngularVelocities[i];
		}


		
        // Rigidbody2Dを再開する
		for ( var i = 0 ; i < rigidbody2Ds.Length ; ++i ) {
            
            // Rigidbody2Dをウェイクアップ
			rigidbody2Ds[i].WakeUp();
            
            // Rigidbody2Dの速度をポーズ前に設定
			rigidbody2Ds[i].velocity        = rigidbody2DVelocities[i];
            
            // Rigidbody2Dの角速度をポーズ前に設定
			rigidbody2Ds[i].angularVelocity = rigidbody2DAngularVelocities[i];
		}



        // クリア処理
		pauseBehaviours                 = null;
 
		rigidbodies                     = null;
		rigidbodyVelocities             = null;
		rigidbodyAngularVelocities      = null;
		
		rigidbody2Ds                    = null;
		rigidbody2DVelocities           = null;
		rigidbody2DAngularVelocities    = null;
	}
}