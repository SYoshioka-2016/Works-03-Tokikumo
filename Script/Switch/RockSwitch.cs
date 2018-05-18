using UnityEngine;
using System.Collections;



public class RockSwitch : SwitchBase {

    // メンバ変数
    private bool switchFlg;
    private bool prevSwitchFlg;
    private Animator[] animators;



	// Use this for initialization
	void Start () {
	
        switchFlg = prevSwitchFlg = false;



        Transform parent = transform.parent;
        if ( MyUtility.NullCheck( parent ) ) { return; }

        int iRockSwitchCount = parent.childCount;
        if ( 1 > iRockSwitchCount ) { return; }
        animators = new Animator[ iRockSwitchCount ];

        for ( int i = 0; i < animators.Length; i++ ) {
        
            Transform chiledTransform = parent.GetChild( i );
            if ( MyUtility.NullCheck( chiledTransform ) ) { return; }

            animators[i] = chiledTransform.GetComponent<Animator>();
        }
	}


	
	// Update is called once per frame
	void Update () {
	
        if ( !switchFlg && IsCollided( Vector3.up ) ) { 
            
            switchFlg = true; 
            SwitchOn();



            ChangeAnimation();
        }

        if ( switchFlg && prevSwitchFlg ) {
        
            SwitchStay();
        }

        if ( switchFlg && !IsCollided( Vector3.up ) ) {
        
            switchFlg = false;
            SwitchOff();
        }

        prevSwitchFlg = switchFlg;
	}



    /// <summary>
    /// サイズの取得
    /// </summary>
    /// <returns></returns>
    private Vector2 GetSize() {
    
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if ( MyUtility.NullCheck( collider ) ) { return Vector2.zero; }

        return 
            new Vector2( 
                collider.size.x * transform.localScale.x,
                collider.size.y * transform.localScale.y
            );
    }



    private bool IsCollided( Vector3 vec3RayDirection ) {

        RaycastHit2D hit = 
            Physics2D.Raycast( 
                transform.position, 
                vec3RayDirection.normalized, 
                10.0f, 
                ~( 1 << LayerMask.NameToLayer( "BackGround" ) ) 
            );

        if ( hit.collider && hit.transform.name.Contains( "RockBlockPart" ) ) {

            float fDistance = Vector3.Distance( hit.point, transform.position );
            return ( GetSize().y / 2 >= fDistance );
        }



        return false;
    }



    private void ChangeAnimation() {

        if ( MyUtility.NullCheck( animators ) ) { return; }



        foreach ( var animator in animators ) {
        
            if ( MyUtility.NullCheck( animator ) ) { return; }



            animator.SetTrigger( "SwitchTrigger" );
        }
    }
}




/*using UnityEngine;
using System.Collections;



public class RockSwitch : SwitchBase {

    // メンバ変数



	// Use this for initialization
	void Start () {
	
	}


	
	// Update is called once per frame
	void Update () {
	
	}



    void OnTriggerEnter2D( Collider2D collider ) {
    
        // タグ別に処理
        switch ( collider.transform.tag ) {

            case "RockBlock": SwitchOn(); break;
        }
    }



    void OnTriggerStay2D( Collider2D collider ) {
    
        // タグ別に処理
        switch ( collider.transform.tag ) {

            case "RockBlock": SwitchStay(); break;
        }
    }



    void OnTriggerExit2D( Collider2D collider ) {
    
        // タグ別に処理
        switch ( collider.transform.tag ) {

            case "RockBlock": SwitchOff(); break;
        }
    }
}
*/