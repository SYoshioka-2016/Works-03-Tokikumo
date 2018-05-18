using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;



public class RockBlockPart : MonoBehaviour {
    
    // メンバ変数
    private Animator animator;
    private RockBlockState prevState;



	// Use this for initialization
	void Start () {
	
        animator = GetComponent<Animator>();
	}


	
	// Update is called once per frame
	void FixedUpdate () {

        // タグの変更
        ChangeTag();
	}



    /// <summary>
    /// 接触チェック
    /// </summary>
    public void CheckTouch() {

        Transform parent = transform.parent;
        if ( MyUtility.NullCheck( parent ) ) { return; }

        RockBlock rockBlock = parent.GetComponent<RockBlock>();
        if ( MyUtility.NullCheck( rockBlock ) ) { return; }

        List<RockBlockPart> rockBlockParts = rockBlock.rockBlockParts;
        if ( MyUtility.NullCheck( rockBlockParts ) ) { return; }

        Transform[] rotationPoints = rockBlock.rotationPoints;
        if ( MyUtility.NullCheck( rotationPoints ) ) { return; }

        int iRotationPointCount = rotationPoints.Length;

        RaycastHit2D[][] hitAlls = 
        {
            Physics2D.RaycastAll( transform.position, Vector3.right ),
            Physics2D.RaycastAll( transform.position, Vector3.left ),
            Physics2D.RaycastAll( transform.position, Vector3.up ),
            Physics2D.RaycastAll( transform.position, Vector3.down ),
        };

        float fReach = GetSize().x / 2;



        foreach ( var hitAll in hitAlls ) {

            foreach ( var hit in hitAll ) {
            
                if ( hit.collider && hit.transform.name.Contains( "RockBlockPart" ) ) {

                    bool bCheck = false;
                    foreach ( var part in rockBlockParts ) {
                    
                        if ( hit.transform.GetComponent<RockBlockPart>() == part ) { bCheck = true; }
                    }
                    if ( bCheck ) { continue; }
     


                    float fDistance = Vector3.Distance( hit.point, transform.position );

                    if ( Mathf.Abs( fDistance - fReach ) <= 0.01f ) {

                        Transform otherParent = hit.transform.parent;
                        if ( MyUtility.NullCheck( otherParent ) ) { return; }

                        RockBlock otherRockBlock = otherParent.GetComponent<RockBlock>();
                        if ( MyUtility.NullCheck( otherRockBlock ) ) { return; }

                        List<RockBlockPart> otherRockBlockParts = otherRockBlock.rockBlockParts;
                        if ( MyUtility.NullCheck( otherRockBlockParts ) ) { return; }

                        Transform[] otherRotationPoints = otherRockBlock.rotationPoints;
                        if ( MyUtility.NullCheck( otherRotationPoints ) ) { return; }

                        int iOtherRotationPointCount = otherRotationPoints.Length;



                        foreach ( var otherPart in otherRockBlockParts ) {
                        
                            rockBlockParts.Add( otherPart );
                            otherPart.transform.parent = parent;
                        }



                        Transform[] points = 
                            new Transform[ iRotationPointCount + iOtherRotationPointCount ];

                        for ( int i = 0; i < iRotationPointCount; i++ ) { 
                            
                            points[i] = rotationPoints[i]; 
                        }

                        for ( int i = iRotationPointCount; i < iRotationPointCount + iOtherRotationPointCount; i++ ) { 
                            
                            points[i] = otherRotationPoints[i - iRotationPointCount]; 
                        }



                        Vector3 vec3RotationPointLD = new Vector3( 1, 1, 0 ) * float.MaxValue;
                        Vector3 vec3RotationPointRU = new Vector3( 1, 1, 0 ) * float.MinValue;

                        for ( int i = 0; i < points.Length; i++ ) { 
          
                            Vector3 vec3 = points[i].position;

                            if ( vec3.x < vec3RotationPointLD.x ) { vec3RotationPointLD.x = vec3.x; }
                            if ( vec3.y < vec3RotationPointLD.y ) { vec3RotationPointLD.y = vec3.y; }

                            if ( vec3.x > vec3RotationPointRU.x ) { vec3RotationPointRU.x = vec3.x; }
                            if ( vec3.y > vec3RotationPointRU.y ) { vec3RotationPointRU.y = vec3.y; }          
                        }

                        rockBlock.rotationPoints[0].position = vec3RotationPointLD;
                        rockBlock.rotationPoints[1].position = new Vector3( vec3RotationPointLD.x, vec3RotationPointRU.y );
                        rockBlock.rotationPoints[2].position = vec3RotationPointRU;
                        rockBlock.rotationPoints[3].position = new Vector3( vec3RotationPointRU.x, vec3RotationPointLD.y );



                        Destroy( otherParent.gameObject );



                        rockBlock.rotationPoint = 
                            ( MyDirection.Left == rockBlock.rotateDirection ) ? rockBlock.rotationPoints[1] : rockBlock.rotationPoints[3];



                        rockBlock.state = RockBlockState.STANDBY;
                    }
                }
            }
        }
    }



    /// <summary>
    /// 左右に衝突したか?( true : 衝突した | false : 衝突してない )
    /// </summary>
    public bool IsCollidedLeftOrRight() {
    
        Transform parent = transform.parent;
        if ( MyUtility.NullCheck( parent ) ) { return false; }

        RockBlock rockBlock = parent.GetComponent<RockBlock>();
        if ( MyUtility.NullCheck( rockBlock ) ) { return false; }



        int iLayerMask = ( 1 << LayerMask.NameToLayer( "BackGround" ) ) | ( 1 << LayerMask.NameToLayer( "Cloud" ) );
        iLayerMask = ~iLayerMask;
        RaycastHit2D[] hits = 
        {
            Physics2D.Raycast( transform.position, Vector3.right, 10.0f, iLayerMask ),
            Physics2D.Raycast( transform.position, Vector3.left, 10.0f, iLayerMask ),
        };

        RaycastHit2D hit = ( rockBlock.rotateDirection.Equals( MyDirection.Right ) ) ? hits[0] : hits[1]; 

        if ( hit.collider && !hit.transform.name.Contains( "RockBlockPart" ) ) {

            float fDistance = Vector3.Distance( hit.point, transform.position );
            return ( GetSize().x / 2 + 0.2f >= fDistance );
        }



        return false;
    }



    public bool IsCollidedLeftAndRight() {

        int iLayerMask = ( 1 << LayerMask.NameToLayer( "BackGround" ) ) | ( 1 << LayerMask.NameToLayer( "Cloud" ) );
        iLayerMask = ~iLayerMask;

        RaycastHit2D[] hits = 
        {
            Physics2D.Raycast( transform.position, Vector3.right, 10.0f, iLayerMask ),
            Physics2D.Raycast( transform.position, Vector3.left, 10.0f, iLayerMask ),
        };

        bool bCheck1 = false, bCheck2 = false;


        if ( hits[0].collider ) {

            if ( hits[0].transform.name.Contains( "RockBlockPart" ) ) {
            
                hits[0] = Physics2D.Raycast( hits[0].transform.position, Vector3.right, 1.0f, iLayerMask );
                bCheck1 = hits[0].collider && !hits[0].transform.name.Contains( "RockBlockPart" );
            }
            else {

                float fDistance = Vector3.Distance( hits[0].point, transform.position );
                bCheck1 = ( GetSize().x / 2 + 0.1f >= fDistance );
            }
        }

        if ( hits[1].collider ) {

            if ( hits[1].transform.name.Contains( "RockBlockPart" ) ) {
            
                hits[1] = Physics2D.Raycast( hits[1].transform.position, Vector3.left, 1.0f, iLayerMask );
                bCheck2 = hits[1].collider && !hits[1].transform.name.Contains( "RockBlockPart" );
            }
            else {

                float fDistance = Vector3.Distance( hits[1].point, transform.position );
                bCheck2 = ( GetSize().x / 2 + 0.1f >= fDistance );
            }
        }



        return bCheck1 && bCheck2;
    }



    public bool IsCollidedDown() {

        Transform parent = transform.parent;
        if ( MyUtility.NullCheck( parent ) ) { return false; }

        RockBlock rockBlock = parent.GetComponent<RockBlock>();
        if ( MyUtility.NullCheck( rockBlock ) ) { return false; }

        List<RockBlockPart> rockBlockParts = rockBlock.rockBlockParts;
        if ( MyUtility.NullCheck( rockBlockParts ) ) { return false; }
        


        int iLayerMask = ( 1 << LayerMask.NameToLayer( "BackGround" ) ) | ( 1 << LayerMask.NameToLayer( "Cloud" ) );
        iLayerMask = ~iLayerMask;

        RaycastHit2D hit = 
            Physics2D.Raycast( 
                transform.position, 
                Vector3.down, 
                1.0f, 
                iLayerMask
            );

        if ( hit.collider && !hit.transform.tag.Contains( "Player" ) ) {

            bool bCheck = false;
            foreach ( var part in rockBlockParts ) {
                    
                if ( hit.transform.GetComponent<RockBlockPart>() == part ) { bCheck = true; }
            }
            if ( bCheck ) { return false; }



            float fDistance = Vector3.Distance( hit.point, transform.position );
            return ( GetSize().y / 2 + 0.1f >= fDistance );
        }



        return false;
    }



    /// <summary>
    /// トランスフォームの調整
    /// </summary>
    public void AdjustTransform() {
    
        Vector3 vec3Position = transform.position;

        transform.position = 
            new Vector3(
                (float)Math.Round( vec3Position.x, MidpointRounding.AwayFromZero ),
                (float)Math.Round( vec3Position.y, MidpointRounding.AwayFromZero ),
                (float)Math.Round( vec3Position.z, MidpointRounding.AwayFromZero )
            );



        Vector3 vec3Rotation = transform.eulerAngles;

        transform.rotation = 
            Quaternion.Euler(
                (float)Math.Round( vec3Rotation.x, MidpointRounding.AwayFromZero ),
                (float)Math.Round( vec3Rotation.y, MidpointRounding.AwayFromZero ),
                (float)Math.Round( vec3Rotation.z, MidpointRounding.AwayFromZero )
            );
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



    /// <summary>
    /// タグの変更
    /// </summary>
    private void ChangeTag() {
    
        Transform parent = transform.parent;
        if ( MyUtility.NullCheck( parent ) ) { return; }

        RockBlock rockBlock = parent.GetComponent<RockBlock>();
        if ( MyUtility.NullCheck( rockBlock ) ) { return; }



        if ( Mathf.Abs( rockBlock.moveVector.y ) > 0 ) {
        
            transform.tag = "DangerObject";
            return;
        }

        if ( rockBlock.state.Equals( RockBlockState.MOVE ) ) {
        
            transform.tag = "DangerObject";
            return;
        }

        if ( 
            ( GameObject.FindGameObjectWithTag( "Player" ).transform.position - transform.position ).magnitude <= 0.2f 
        ) {
        
            transform.tag = "DangerObject";
            return;
        }
        
            transform.tag = "Untagged";
    }



    public void ChangeAnimation( bool moveFlg ) {
    
        if ( MyUtility.NullCheck( animator ) ) { return; }



        animator.SetBool( "MoveFlg", moveFlg );
    }
}