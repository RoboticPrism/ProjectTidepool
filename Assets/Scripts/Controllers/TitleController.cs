using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TitleController : MonoBehaviour {
	public void GotoPlay()
    {
		SceneManager.LoadScene (1);
	}

    public void GotoWatch()
    {
        SceneManager.LoadScene(2);
    }
}
