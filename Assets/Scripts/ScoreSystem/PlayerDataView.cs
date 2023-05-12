using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDataView : MonoBehaviour
{
    [SerializeField] private Slider _hpSlider;

    [SerializeField] private TMP_Text _coinsCount;
    [SerializeField] private TMP_Text _nameText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private IEnumerator UpdateHpSlider(float oldVal, float newVal)
    {
        var time = 2f;
        var t = 0f;
        while (t < time)
        {
            _hpSlider.value = Mathf.Lerp(oldVal, newVal, t/time);
            t += Time.deltaTime;
            yield return null;
        }
    }

    public void UpdateName(string toString)
    {
        _nameText.text = toString;
    }

    public void UpdateScore(int behaviourScore)
    {
        _coinsCount.text = behaviourScore.ToString();
    }

    public void UpdateLives(int behaviourLives)
    {
        StopAllCoroutines();
        StartCoroutine(UpdateHpSlider(_hpSlider.value, behaviourLives / 10f));
    }
}
