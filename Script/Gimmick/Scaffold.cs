using UnityEngine;
using System.Collections;



/// <summary>
/// 足場クラス
/// </summary>
public class Scaffold : MonoBehaviour {

    // メンバ変数
    private bool        isTrigger;  // コライダのisTrigger
    private Collider2D  collider;   // コライダ



	// Use this for initialization
	void Start () {
	
        collider = GetComponent<BoxCollider2D>();
        if ( !MyUtility.NullCheck( collider ) ) { isTrigger = collider.isTrigger; }
	}


	
	// Update is called once per frame
	void Update () {
	
        // トリガーの切り替え
        SwitchingTrigger();
	}



    /// <summary>
    /// トリガーの切り替え
    /// </summary>
    private void SwitchingTrigger() {
    
        // nullチェック
        if ( MyUtility.NullCheck( collider ) ) { return; }



        collider.isTrigger = isTrigger;
    }



    void OnTriggerExit2D( Collider2D collider ) {
    
        // タグ別に処理
        switch ( collider.transform.tag ) {

            case "Player": 
                
                isTrigger = false;
                break;
        }
    }
}
