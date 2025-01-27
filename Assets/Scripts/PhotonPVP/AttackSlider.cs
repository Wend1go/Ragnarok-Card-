using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

[System.Serializable]
public enum SliderAttack
{
    nun,
    LightAttack,
    MediumAttack,
    HeavyAttack,
}

public class AttackSlider : MonoBehaviour
{
    public static AttackSlider instance;

    public Slider _slider;
    [SerializeField] private Text _attackText;
    [SerializeField] private Image _fillImage;
    [SerializeField] private Text _attackValueText;

    public SliderAttack _sliderAttack;
    public GameObject UseSpeedObj;
    public RectTransform SpeedFill;
    public RectTransform AttackFill;
    float ExtraSpeedAmt;
    public TextMeshProUGUI speedTx;
    public TextMeshProUGUI UsedSpeedTx;
    public GameObject UsedSpeedButton;
    public PlayerAction action;

    float difference = 0;

    bool SpeedCanceled, SpeedFixed;
    private void Awake()
    {
        if (instance==null)
        {
            instance = this;
        }
        instance.gameObject.transform.parent.gameObject.SetActive(false);
    }
     void OnEnable()
    {

        //Allow Maxminum attack value which is Minimum from both player's health

        // _slider.maxValue = Mathf.Min(PVPManager.manager.P1HealthBar.value,PVPManager.manager.P2HealthBar.value);
        Debug.Log("P2 LATEST REMAINING HEALTH " + PVPManager.manager.P2RemainingHandHealth);
        _slider.maxValue = Mathf.Min(PVPManager.manager.P1StaVal * 10f,Mathf.Min(PVPManager.manager.P1RemainingHandHealth,PVPManager.manager.P2RemainingHandHealth));
        // if(Game.Get().turn == 2 || Game.Get().turn == 4 || Game.Get().turn == 6 || Game.Get().turn == 8)
        // {
        //     _slider.minValue = Game.Get().lastAction == PlayerAction.counterAttack ? Game.Get().BetAmount : Game.Get().BetAmount;
        // }
        // else
        // {
        //     _slider.minValue = Game.Get().lastAction == PlayerAction.counterAttack ? Game.Get().BetAmount : Game.Get().BetAmount; //change 20-4
        // }
        Debug.LogError("Bet amt = "+ PVPManager.manager.AttackFor +" - "+ (Game.Get().lastAction == PlayerAction.counterAttack));
        //_slider.minValue = (Game.Get().lastAction == PlayerAction.counterAttack) ? PVPManager.manager.AttackFor : 1;
        //_slider.minValue = Game.Get().lastAction == PlayerAction.counterAttack? Game.Get().BetAmount * 2:Game.Get().BetAmount;
        difference = _slider.maxValue - _slider.minValue;
        //_slider.minValue = 1;
        _slider.value = _slider.minValue;
        _attackValueText.text = _slider.minValue.ToString();
        SpeedCanceled = false;
        SpeedFixed = false;
        ExtraSpeedAmt = 0;

        SpeedFill.anchorMax = Vector2.zero;
        UsedSpeedButton.SetActive(false);
        _slider.onValueChanged.AddListener(delegate {UpdateAttackValueText(); });
    }
    
    void OnDisable()
    {
        _slider.onValueChanged.RemoveListener(delegate {UpdateAttackValueText(); });
    }
    private void Start()
    {
        _slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {

        //if (_slider.value<=(_slider.maxValue*.33))
        //{
        //    _attackText.text = "Light Attack ";
        //    _attackText.color = Color.green;
        //    _fillImage.color = Color.green;
        //    _sliderAttack = SliderAttack.LightAttack;
        //}
        //else if (_slider.value>= (_slider.maxValue * .34) && _slider.value <= (_slider.maxValue * .66))
        //{
        //    _attackText.text = "Medium Attack";
        //    _attackText.color = Color.yellow;
        //    _fillImage.color = Color.yellow;
        //    _sliderAttack = SliderAttack.MediumAttack;
        //}
        //else if (_slider.value>= (_slider.maxValue * .67))
        //{
        //    _attackText.text = "Heavy Attack";
        //    _attackText.color = Color.red;
        //    _fillImage.color = Color.red;
        //    _sliderAttack = SliderAttack.HeavyAttack;
        //}
        //else
        //{
        //    _attackText.text = "nun";
        //    _sliderAttack = SliderAttack.nun;

        //}      


        if(_slider.value <= (_slider.minValue+(difference * .33)))
        {
            _attackText.text = "Light Attack ";
            _attackText.color = Color.green;
            _fillImage.color = Color.green;
            _sliderAttack = SliderAttack.LightAttack;
        }
        else if(_slider.value >= (_slider.minValue + (difference * .34)) && _slider.value <= (_slider.minValue + (difference * .66)))
        {
            _attackText.text = "Medium Attack";
            _attackText.color = Color.yellow;
            _fillImage.color = Color.yellow;
            _sliderAttack = SliderAttack.MediumAttack;
        }
        else if(_slider.value >= (_slider.minValue + (difference * .67)))
        {
            _attackText.text = "Heavy Attack";
            _attackText.color = Color.red;
            _fillImage.color = Color.red;
            _sliderAttack = SliderAttack.HeavyAttack;
        }

        
        
        //else
        //{
        //    _attackText.text = "nun";
        //    _sliderAttack = SliderAttack.nun;

        //}
    }

    public void UpdateUI(int value){
        
    }

    public void SpeedDecision(string ans){
        ExtraSpeedAmt = ans == "Use" ? MathF.Min(PVPManager.Get().p1Speed * 10f, (int)_slider.value) : 0; 
        if(ExtraSpeedAmt <= 0){
            SpeedFill.anchorMax = Vector2.zero;
            _slider.fillRect = AttackFill;
            SpeedCanceled = true;
            UsedSpeedButton.SetActive(false);
        }else{
            UsedSpeedButton.SetActive(true);
            UsedSpeedTx.text = ExtraSpeedAmt+" Speed \nUsed";
            SpeedFill.anchorMax = new Vector2(ExtraSpeedAmt / _slider.maxValue,1f);
            SpeedFixed = true;
        }
        
        UseSpeedObj.SetActive(false);
    }

    

    public void btn_SliderComplete()
    {
        Game.Get().lastAction = action;
        Game.Get().UpdateLastAction(action);
        PVPManager.Get().sliderAttackbuttonClick(_slider.value, MathF.Round(((int)_slider.value - ExtraSpeedAmt) / 10f,1));
        PVPManager.Get().UpdateRemainingHandHealth((int)(_slider.value - ExtraSpeedAmt));
        PVPManager.Get().DeductSpeed(MathF.Round(ExtraSpeedAmt / 10f,1));
        PVPManager.Get().AttackChoices.SetActive(false);
        PVPManager.Get().speedAttackChoices.SetActive(false);
    }
    public void UpdateAttackValueText()
    {
        
        float speedVal = PVPManager.Get().p1Speed * 10f;
        if(PVPManager.Get().p1Speed > 0 && !SpeedCanceled && !SpeedFixed){
            
            if(_slider.value <= speedVal){
                _slider.value = Mathf.RoundToInt(_slider.value);
                _slider.fillRect = SpeedFill;
                AttackFill.anchorMax = Vector2.zero;
                ExtraSpeedAmt = (int)_slider.value;
                speedTx.text = "Use "+ExtraSpeedAmt+"\nFree Speed?";
                UseSpeedObj.SetActive(true);
            }else{
                _slider.fillRect = AttackFill;
            }
        }else{
            if(SpeedFixed){
                _slider.value = Mathf.Clamp((int)_slider.value,ExtraSpeedAmt, _slider.maxValue);
                UsedSpeedButton.SetActive(true);
                UsedSpeedTx.text = ExtraSpeedAmt+" Speed \nUsed";
            }else{
                SpeedFill.anchorMax = Vector2.zero;
            }
            _slider.fillRect = AttackFill;
            UseSpeedObj.SetActive(false);
        }

        if(_slider.value > speedVal && speedVal > 0 && !SpeedCanceled && !SpeedFixed){
            ExtraSpeedAmt = speedVal;
            speedTx.text = "Use "+ExtraSpeedAmt+"\nFree Speed?";
            UseSpeedObj.SetActive(true);
            SpeedFill.anchorMax = new Vector2(speedVal / _slider.maxValue,1f);
        }

        _attackValueText.text = ((int)_slider.value).ToString();

        if(_slider.value == _slider.maxValue)
        {
            _attackValueText.text = "All In";
        }
    }
}
