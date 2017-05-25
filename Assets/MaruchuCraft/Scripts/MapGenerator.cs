//	=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//
//	地形を生成するジェネレータ
//
//	Copyright(C)2017 Maruchu
//	http://maruchu.nobody.jp/
//
//
//	地形生成の手法はコチラを参考にしました。
//	https://docs.unity3d.com/ScriptReference/Mathf.PerlinNoise.html
//	http://kan-kikuchi.hatenablog.com/entry/PerlinNoise
//
//	=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;
using System.Collections.Generic;



/// <summary>
/// 地形を生成するジェネレータ
/// </summary>
public class		MapGenerator		: MonoBehaviour {



	//キューブの種類
	public	enum	CubeKind {
		 Green		//土
		,Sand		//砂
		,Stone		//石
	}




	//マップのグリッドサイズ(キューブの個数)
	public		static readonly		int				m_fieldGrid_MaxX		= 64;		//横幅
	public		static readonly		int				m_fieldGrid_MaxZ		= 64;		//奥行き

	public		static readonly		int				m_fieldGrid_MaxY		= 16;		//高さ


	private		static readonly		int				m_fieldGrid_FillY		= 3;		//キューブをいくつ積むか


	public							Transform		m_waterPlaneObject		= null;		//水面のオブジェクト
	public							float			m_waterPlane_PosY		= 5.0f;		//水面の高さ

	public							GameObject[]	m_makeCubePrefabArray	= null;		//生成する地形のキューブ




	/// <summary>
	/// 初期化処理
	/// </summary>
	private		void		Awake() {

		//地形
		{
			//乱数シード(ここが同じなら、まったく同じフィールドになる)
			float		seedX			= (Random.value	*100f);
			float		seedZ			= (Random.value	*100f);
			//地形のなめらかさ(数字が大きいと起伏が大きい)
			float		undulationDiv	= 16f;

			//キューブを配置する場所
			Vector3		makePos;
			//キューブを配置する高さ
			int			cubeY;

			//生成するキューブの種類
			CubeKind	cubeKind;
			//生成したキューブのオブジェクト
			GameObject	cubeObj;

			//XZ平面にキューブを生成していく
			for( int gridX = 0;	gridX < m_fieldGrid_MaxX;	gridX++) {
				for( int gridZ = 0;	gridZ < m_fieldGrid_MaxZ;	gridZ++) {

					//生成する位置はここ
					makePos		= new Vector3(	gridX, 0, gridZ);
					//高さはここ
					{
						//サンプリング点情報
						float	sampleX		= ((gridX	+seedX)		/undulationDiv);
						float	sampleZ		= ((gridZ	+seedZ)		/undulationDiv);
						//生成する高さを取得・設定
						cubeY				= (int)(Mathf.PerlinNoise( sampleX, sampleZ)	*m_fieldGrid_MaxY);
						makePos.y			= cubeY;
					}


					//配置したキューブの下を少し埋める
					for( int gridY = 0;	gridY < m_fieldGrid_FillY;	gridY++) {
						//どのキューブを作るか
						cubeKind	= CubeKind.Green;
						{
							//もし高さが水より低いなら
							if( makePos.y < m_waterPlane_PosY) {
								//砂
								cubeKind	= CubeKind.Sand;
							}

							//もし (X座標 + Z座標) が32以下なら
							if( (makePos.x + makePos.z) < 32) {
								//石
								cubeKind	= CubeKind.Stone;
							}
						}

						//新しいキューブ作成して配置
						cubeObj		= Instantiate( m_makeCubePrefabArray[ (int)cubeKind], makePos, Quaternion.identity)		as GameObject;
						//ジェネレータの子供にする
						cubeObj.transform.parent	= transform;

						//生成する高さを -1
						makePos.y	-= 1;
					}
				}
			}
		}

		//水面
		{
			//位置を設定(グリッドの中心に置く)
			m_waterPlaneObject.transform.position	= new Vector3( (0.5f *m_fieldGrid_MaxX),	m_waterPlane_PosY,	(0.5f *m_fieldGrid_MaxZ));

			//スケール(広さ)はグリッドサイズ
			m_waterPlaneObject.transform.localScale	= new Vector3( m_fieldGrid_MaxX, m_fieldGrid_MaxZ, 1);
		}
	}




}

