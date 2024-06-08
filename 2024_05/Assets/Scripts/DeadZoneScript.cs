using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadZoneScript : MonoBehaviour
{
	GameManageScript	game_manage;
    // Start is called before the first frame update
    void Start()
    {
        game_manage = GameObject.Find("GameManager").GetComponent<GameManageScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	private void OnTriggerEnter(Collider other)
	{
		// プレイヤーの初期位置ワープ
		if(other.CompareTag("Player"))
		{
			PlayerScript player = other.gameObject.GetComponent<PlayerScript>();
			player.MoveStartPos();

			game_manage.BreakPlayer();
		}
		else if(other.CompareTag("Enemy"))
		{
			game_manage.BreakEnemy();

			EnemyScript enemy = other.gameObject.GetComponent<EnemyScript>();
			enemy.MoveStartPos();
		}
		else
		{
			Destroy(other.gameObject);
		}
	}
}
