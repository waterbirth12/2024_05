using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

public class TitleScript : MonoBehaviour
{
	public EventSystem		event_system;
	public TextMeshProUGUI	tex_purpose;
	int						i_mode;
	int						i_purpose_count;

    // Start is called before the first frame update
    void Start()
    {
		GetComponent<AudioSource>().PlayDelayed(0.0000001f);
    }

    // Update is called once per frame
    void Update()
    {
		if(null != event_system.currentSelectedGameObject)
		{
			// 詳細説明のテキスト更新
			tex_purpose.text = event_system.currentSelectedGameObject.gameObject.transform.Find("DispText").GetComponent<TextMeshProUGUI>().text;

			if (Input.GetKeyDown(KeyCode.Space))
			{
				switch(event_system.currentSelectedGameObject.gameObject.name)
				{
					case "Btn_Break10":
						i_mode = 0;
						i_purpose_count = 10;

						SceneManager.sceneLoaded += GameSceneLoad;

						// SampleSceneに切り替える
						SceneManager.LoadScene("SampleScene");
						break;
					case "Btn_Break50":
						i_mode = 0;
						i_purpose_count = 50;

						SceneManager.sceneLoaded += GameSceneLoad;

						// SampleSceneに切り替える
						SceneManager.LoadScene("SampleScene");
						break;
					case "Btn_Break100":
						i_mode = 0;
						i_purpose_count = 100;

						SceneManager.sceneLoaded += GameSceneLoad;

						// SampleSceneに切り替える
						SceneManager.LoadScene("SampleScene");
						break;
					case "Btn_Endless":
						i_mode = 1;
						i_purpose_count = 100;

						SceneManager.sceneLoaded += GameSceneLoad;

						// SampleSceneに切り替える
						SceneManager.LoadScene("SampleScene");
						break;
					case "Btn_Cregit":
						break;
					case "Btn_GameEnd":
#if UNITY_EDITOR
						//ゲームプレイ終了
						UnityEditor.EditorApplication.isPlaying = false;
#else
						//ゲームプレイ終了
						Application.Quit();
#endif
						break;
					default:
						break;
				}
			}
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

	void OnApplicationFocus(bool hasFocus)
    {
        if(false == hasFocus)
		{
			GetComponent<AudioSource>().Pause();
		}
		else
		{
			GetComponent<AudioSource>().PlayDelayed(0.0000001f);
		}
    }
}
