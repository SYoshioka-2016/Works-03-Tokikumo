using UnityEngine;
using System.Collections;



public class AppryForce : MonoBehaviour {

    // メンバ変数
    [SerializeField] 
    private Vector2 direction = Vector2.up;     // 力を加える方向

    [SerializeField, Range( 1, 100 )] 
    private float power = 5.0f;                 // パワー

    private Rigidbody2D rigidbody;              // Rigidbody2D



	// Use this for initialization
	void Start () {
	
        rigidbody = GetComponent<Rigidbody2D>();
	}
	


	// Update is called once per frame
	void Update () {

	}



    /// <summary>
    /// 力を加える
    /// </summary>
    public void AppryTheForce() {
    
        // nullチェック
        if ( MyUtility.NullCheck( rigidbody ) ) { return; }



        direction.Normalize();
        rigidbody.velocity = direction * power;
    }
}
