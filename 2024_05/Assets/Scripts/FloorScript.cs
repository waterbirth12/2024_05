using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorScript : MonoBehaviour
{
	enum enState
	{
		enStateNormal = 0,	// 通常
		enStateBreak,		// 破壊状態
		enStateHide			// 非表示
	}
	[SerializeField] Material[] mat_Array = new Material[5];
	enState						en_state = enState.enStateNormal;
	float						f_time = 0.0f;
	BoxCollider					box_col;
	MeshRenderer				mesh_rend;
	bool						b_on_player = false;
	bool						b_on_enemy = false;

    // Start is called before the first frame update
    void Start()
    {
        box_col = GetComponent<BoxCollider>();
		mesh_rend = GetComponent<MeshRenderer>();

		box_col.enabled = true;
		mesh_rend.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
		if(enState.enStateBreak == en_state)
		{
			f_time += Time.deltaTime;

			if(0.5f < f_time)
			{
				en_state = enState.enStateHide;
				f_time = 0.0f;

				box_col.enabled = false;
				mesh_rend.enabled = false;
			}
		}
		else if(enState.enStateHide == en_state)
		{
			f_time += Time.deltaTime;

			if(2.0f < f_time)
			{
				// 元のマテリアルに戻す
				GetComponent<MeshRenderer>().material = mat_Array[0];

				en_state = enState.enStateNormal;
				f_time = 0.0f;

				box_col.enabled = true;
				mesh_rend.enabled = true;
			}
		}
		else if(enState.enStateNormal == en_state)
		{
			// プレイヤーが乗った
			if(true == b_on_player)
			{
				// 敵オブジェクトも乗った
				if(true == b_on_enemy)
				{
					// 両方乗った時用のマテリアルに変更
					GetComponent<MeshRenderer>().material = mat_Array[3];
				}
				else
				{
					// プレイヤーのみ乗った時用のマテリアルに変更
					GetComponent<MeshRenderer>().material = mat_Array[1];
				}
			}
			// 敵オブジェクトのみ乗った
			else if(true == b_on_enemy)
			{
				// 敵のみ乗った時用のマテリアルに変更
				GetComponent<MeshRenderer>().material = mat_Array[2];
			}
			// 離れた
			else
			{
				// 元のマテリアルに戻す
				GetComponent<MeshRenderer>().material = mat_Array[0];
			}
		}
    }

	public void OnPlayerFloor(bool bOnPlayer)
	{
		b_on_player = bOnPlayer;
	}

	public void OnEnemyFloor(bool bOnEnemy)
	{
		b_on_enemy = bOnEnemy;
	}

	public void AttackFloor()
	{
		// 破壊状態に変更
		en_state = enState.enStateBreak;

		// マテリアルも変更
		GetComponent<MeshRenderer>().material = mat_Array[4];

		f_time = 0.0f;
	}

	public bool GetBreak()
	{
		if(enState.enStateNormal == en_state)
		{
			return false;
		}
		else
		{
			return true;
		}
	}
}
