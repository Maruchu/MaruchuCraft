﻿//	=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//
//	プレーヤークラス
//
//	Copyright(C)2017 Maruchu
//	http://maruchu.nobody.jp/
//
//	=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;
using System.Collections.Generic;



/// <summary>
/// プレーヤークラス
/// </summary>
public class		CraftPlayer			: MonoBehaviour {




	private							float		m_nowRotY				= 0.0f;		//プレーヤーの現在のY軸の角度


	private		static readonly		float		m_movePosZ_Front		=  5.0f;	//前進の速度
	private		static readonly		float		m_movePosZ_Back			= -2.0f;	//後退の速度
	private		static readonly		float		m_movePosX				=  4.0f;	//左右移動の速度

	private		static readonly		float		m_moveRotY_Mouse		= 360.0f;	//回転の速度(マウス)


	public							Transform	m_armRoot				= null;		//腕のオブジェクト

	private							float		m_armRot_Now			= 0.0f;		//現在の腕の角度
	private		static readonly		float		m_armRot_Base			= 25.0f;	//腕の角度の基本
	private		static readonly		float		m_armRot_Max			= 50.0f;	//腕の角度の上限

	private		static readonly		float		m_armRot_Speed			= 300.0f;	//腕を振る速度


	private							Rigidbody	m_rigidbody				= null;		//ジャンプの時に力を加える対象のリジッドボディ



	/// <summary>
	/// 初期化
	/// </summary>
	private		void	Awake() {
		//リジッドボディを取得
		m_rigidbody		= gameObject.GetComponent<Rigidbody>();
	}
	/// <summary>
	/// 毎フレーム呼び出される処理
	/// </summary>
	private		void	Update() {

		//回転
		{
			//マウスの移動量による回転
			float	addRotY		= (Input.GetAxis( "Mouse X")	*m_moveRotY_Mouse);

			//現在の角度に加算
			m_nowRotY			+= (addRotY	*Time.deltaTime);				//移動量、回転量には Time.deltaTime をかけて実行環境(フレーム数の差)による違いが出ないようにします

			//オイラー角で入れる
			transform.rotation	= Quaternion.Euler( 0, m_nowRotY, 0);		//Y軸回転でキャラの向きを横に動かせます
		}

		//移動
		{
			Vector3	addPos		= Vector3.zero;		//移動量
			/*
				Vector3.zero は new Vector3( 0f, 0f, 0f) と同じ

				他にも色々あるので↓のページを参照してみてください
				http://docs.unity3d.com/ScriptReference/Vector3.html
			 */

			//キー操作から移動する量を取得
			Vector3	nowInput		= new Vector3( Input.GetAxisRaw( "Horizontal"), 0, Input.GetAxisRaw( "Vertical"));
			//Xに左右の、Zに前後の入力を入れます(W、A、S、Dキー、ゲームパッドの入力など)

			//Z に何か値が入っている？
			if( nowInput.z > 0) {
				//前進
				addPos.z		= m_movePosZ_Front;
			} else
			if( nowInput.z < 0) {
				//後退
				addPos.z		= m_movePosZ_Back;
			}

			//X に何か値が入っている？
			if( nowInput.x > 0) {
				//右
				addPos.x		= m_movePosX;
			} else
			if( nowInput.x < 0) {
				//左
				addPos.x		= -m_movePosX;
			}

			//移動量を Transform に渡して移動させる
			transform.position	+= ((transform.rotation	 	*addPos)		*Time.deltaTime);
			/*
				Vector3 に transform.rotation をかけると、その方向へ曲げてくれます
				この時、Vector3 は Z+ の方向を正面として考えます
			 */
		}

		//ジャンプ
		{
			//地面の方向にコリジョンチェック
			if( Physics.Raycast( (transform.position +new Vector3( 0, 0.1f, 0)), Vector3.down, 0.4f)) {
				/*
					ちょっとだけ上からチェック
					Vector3.down は new Vector3( 0f, -1f, 0f) と同じ
				 */

				//スペースを押していたら
				if( Input.GetKeyDown( KeyCode.Space)) {
					//物理に力を加える
					m_rigidbody.AddForce( 0, 300, 0, ForceMode.Force);
				}
			}
		}


		//腕を振る
		{
			//マウスの左クリックを押していたら
			if( Input.GetKey( KeyCode.Mouse0)) {
				//腕を振る
				m_armRot_Now	= ((m_armRot_Now		+(m_armRot_Speed	*Time.deltaTime))		%m_armRot_Max);
			} else {
				//腕を止める
				m_armRot_Now	= 0;
			}

			//腕の角度を反映
			m_armRoot.transform.localRotation	= Quaternion.Euler( (m_armRot_Base +m_armRot_Now), 0, 0);
		}
	}




}
