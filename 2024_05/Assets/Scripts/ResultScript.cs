using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ResultScript : MonoBehaviour
{
	public	int				i_mode = 1;
	public	int				i_purpose_count;
	public	int				i_break_count;
	public	float			f_play_time;
	public	TextMeshProUGUI	tex_game_mode;
	public	TextMeshProUGUI	tex_break_cnt;
	public  TextMeshProUGUI	tex_play_time;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<AudioSource>().PlayDelayed(0.0000001f);
    }

    // Update is called once per frame
    void Update()
    {
		// テキスト更新
		if(0 == i_mode)
		{
			// ゲームモード
			tex_game_mode.text = string.Format("{0:0}体撃破モード", i_purpose_count);

			// 撃破数テキスト
			tex_break_cnt.text = string.Format("{0} / {1}", i_break_count, i_purpose_count);

			// ゲーム時間
			tex_play_time.text = string.Format("{0:f1}", f_play_time);
		}
		else
		{
			// ゲームモード
			tex_game_mode.text = string.Format("エンドレスモード");

			// 撃破数テキスト
			tex_break_cnt.text = string.Format("{0}", i_break_count);

			// ゲーム時間
			tex_play_time.text = string.Format("{0:f1}", f_play_time);
		}

		// リトライキー(R)押下
        if (Input.GetKeyDown(KeyCode.R))
		{
			SceneManager.sceneLoaded += GameSceneLoad;

			// ゲーム本編のシーンに切り替える
			SceneManager.LoadScene("SampleScene");
		}

		// リトライキー(T)押下
		if (Input.GetKeyDown(KeyCode.T))
		{
			// ゲーム本編のシーンに切り替える
			SceneManager.LoadScene("Title");
		}
    }

	private void GameSceneLoad(Scene next, LoadSceneMode mode)
	{
		// シーン切り替え後のスクリプトを取得
		var gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManageScript>();
    
		// ゲームモードと目的数を渡す
		gameManager.i_game_mode = i_mode;
		gameManager.i_purpose_count = i_purpose_count;

		// イベントから削除
		SceneManager.sceneLoaded -= GameSceneLoad;
	}
}
