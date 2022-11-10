using System.Collections;
using System.Collections.Generic;
using StormWarfare.Card;
using StormWarfare.Gameplay;
using StormWarfare.Interface;
using StormWarfare.Model;
using StormWarfare.Models;
using UnityEngine;
using static StormWarfare.Card.Card;

public class AttackAnimation : MonoBehaviour
{

    public GameObject attackBase, attackPointer, attackLine;
    public Sprite baseAttack, baseBuff, pointerAttack, pointerBuff;
    public Texture lineAttack, lineBuff;

    private Transform skullSource;
    private Transform skullTarget;
    private int targetIndex;

    private Camera _cam;
    private float _timer;
    private float _delay = 0.01f;

    private Commander _player;
    private AIPlayer _aiPlayer;
    public bool ApplySpecialEffect;
    public ApplySpecialEffectStruct ApplySpecialEffectData;

    int _sourceIndex;
    int _targetIndex;

    UnitCardBoard _sourceCard;
    UnitCardBoard _targetCard;
    GameManager _gm;
    bool _weaponClicked;
    bool _specialEffectClicked;
    bool _isMyTurn;
    public bool IsAttackShowing;
    LineRenderer lineRenderer;
    // Start is called before the first frame update
    UnitCardBoard _unitCard;
    void Start()
    {
        _cam = Camera.main;
        _timer = _delay;
        _gm = GameManager.Instance;
        _player = _gm.Player0;
        _aiPlayer = _gm.Player1;
        attackPointer.SetActive(false);
        attackBase.SetActive(false);
        attackLine.SetActive(false);

        EventManager.OnEndTurn += TurnFinished;
        _isMyTurn = _gm.BoardController.Myturn;
        lineRenderer = attackLine.GetComponent<LineRenderer>();
    }
    void TurnFinished(bool isMyTurn)
    {
        if (!isMyTurn)
        {
            StartCoroutine(ClearAttackAnimation());
            ClearSkull();
        }
        _isMyTurn = isMyTurn;
    }
    public void SetPositionForSpecialEffects(Vector3 cardPos, UnitCardBoard unitCardBoard, List<BaseSpecialEffect> effectData)
    {
        ApplySpecialEffect = true;
        ApplySpecialEffectData.effectData = effectData;
        attackBase.transform.position = cardPos;
        Vector3 baseDiff = Input.mousePosition - attackBase.transform.position;
        float rotationZ = Mathf.Atan2(baseDiff.y, baseDiff.x) * Mathf.Rad2Deg;
        attackBase.transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ + 90.0f);
        lineRenderer.SetPosition(0, cardPos);
        SetBuffSprites();
        for (int i = 0; i < effectData.Count; i++)
        {
            if (effectData[i] is DealDamage e && e.Target == Enums.SpecialEffectTarget.Unit)
                SetAttackSprites();
        }
        attackPointer.SetActive(true);
        attackBase.SetActive(true);
        attackLine.SetActive(true);
        IsAttackShowing = true;
        _unitCard = unitCardBoard;
    }

    public void AIAttack(Vector3 sourcePos, Vector3 targetPos, bool isAttack = true)
    {
        if (isAttack)
        {
            SetAttackSprites();
        }
        else
            SetBuffSprites();
        attackPointer.transform.position = targetPos;
        attackBase.transform.position = sourcePos;
        lineRenderer.SetPosition(0, sourcePos);
        lineRenderer.SetPosition(1, targetPos);
        Vector3 baseDiff = targetPos - attackBase.transform.position;
        float rotationZ = Mathf.Atan2(baseDiff.y, baseDiff.x) * Mathf.Rad2Deg;
        attackBase.transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ + 90.0f);
        attackPointer.SetActive(true);
        attackBase.SetActive(true);
        attackLine.SetActive(true);
        StartCoroutine(ClearAttackAnimation(true));
    }

    public void SetAttackCursorSprite(BaseCardModel sourceCard, UnitCardBoard targetCard, Commander targetCommander)
    {
        var indicatorValue = 0;
        switch (sourceCard)
        {
            case UnitCardModel c:
                indicatorValue = c.AttackPoint;
                break;
            case WeaponCardModel c:
                indicatorValue = c.AttackPoint;
                break;
            default:
                break;
        }

        if(targetCard != null)
        {
            targetCard.DamagePoint.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = "-" + indicatorValue;
            StartCoroutine(ClearAttackAnimation(targetCard.DamagePoint, true));
            targetCard.DamagePoint.SetActive(true);
            return;
        }

        if(targetCommander != null)
        {
            targetCommander.Indicator.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = "-" + indicatorValue;
            StartCoroutine(ClearAttackAnimation(targetCommander.Indicator, true));
            targetCommander.Indicator.SetActive(true);
        }

        return;
    }

    // Update is called once per frame
    void Update()
    {
        if (_gm.Player0 == null || !_gm.BoardController.BoardModel.IsPlaying)
            return;
        RaycastHit2D hit = Physics2D.GetRayIntersection(_cam.ScreenPointToRay(Input.mousePosition));
        if (hit.collider != null && !IsAttackShowing)
        {
            var colliderTag = hit.collider.tag;
            if (colliderTag == "WeaponAI")
                _gm.Player1.WeaponCardHover.gameObject.SetActive(true);
            else if (colliderTag == "Weapon")
                _gm.Player0.WeaponCardHover.gameObject.SetActive(true);
            else if (colliderTag == "SpecialEffect")
                _gm.Player0.SpecialAbilityHover.gameObject.SetActive(true);
            else if (colliderTag == "SpecialEffectAI")
                _gm.Player1.SpecialAbilityHover.gameObject.SetActive(true);
            else
            {
                _gm.Player0.WeaponCardHover.gameObject.SetActive(false);
                _gm.Player1.WeaponCardHover.gameObject.SetActive(false);
                _gm.Player0.SpecialAbilityHover.gameObject.SetActive(false);
                _gm.Player1.SpecialAbilityHover.gameObject.SetActive(false);
            }
        }
        else
        {
            _gm.Player0.WeaponCardHover.gameObject.SetActive(false);
            _gm.Player1.WeaponCardHover.gameObject.SetActive(false);
            _gm.Player0.SpecialAbilityHover.gameObject.SetActive(false);
            _gm.Player1.SpecialAbilityHover.gameObject.SetActive(false);
        }
        if (!_isMyTurn)
        {
            if (IsAttackShowing)
            {
                ClearAttackAnimation();
                ClearSkull();
            }
            if (ApplySpecialEffect)
            {
                _unitCard.ApplySpecialEffectData.effectData.Clear();
                _unitCard = null;
                ApplySpecialEffect = false;
            }
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            hit = Physics2D.GetRayIntersection(_cam.ScreenPointToRay(Input.mousePosition));
            if (hit.collider != null)
            {
                var colliderTag = hit.collider.tag;
                if (colliderTag == "BoardCard")
                {
                    _sourceCard = hit.transform.GetComponent<UnitCardBoard>();
                    if (_sourceCard == null || _sourceCard == _unitCard) return;
                    if (ApplySpecialEffect)
                    {
                        var isClickedCardOpponents = _sourceCard.GetComponentInParent<CardContainer>().IsOpponent;
                        var _boardModel = _gm.BoardController.BoardModel;
                        for (int i = 0; i < ApplySpecialEffectData.effectData.Count; i++)
                        {
                            var e = ApplySpecialEffectData.effectData[i];
                            switch (e)
                            {
                                case DealDamage u:
                                    if (!isClickedCardOpponents) continue;
                                    u.UseEffect(ref _boardModel.boardCards[_boardModel.OtherTurn], ref _boardModel.Players[_boardModel.OtherTurn], _unitCard.Model, _sourceCard.Index);
                                    ApplySpecialEffectData.effectData.RemoveAt(i--);
                                    break;
                                case GiveDefensePoint u:
                                    if (isClickedCardOpponents) continue;
                                    //_sourceCard.Model.ApplyBuff(e);
                                    u.UseEffect(ref _boardModel.boardCards[_boardModel.CurrentTurn], ref _boardModel.Players[_boardModel.CurrentTurn], _unitCard.Model, _sourceCard.Index);
                                    ApplySpecialEffectData.effectData.RemoveAt(i--);
                                    break;
                                case GiveAttackPoint u:
                                    if (isClickedCardOpponents) continue;
                                    //_sourceCard.Model.ApplyBuff(e);
                                    u.UseEffect(ref _boardModel.boardCards[_boardModel.CurrentTurn], _unitCard.Model, _sourceCard.Index);
                                    ApplySpecialEffectData.effectData.RemoveAt(i--);
                                    break;

                                default:
                                    break;
                            }
                        }
                        GameManager.Instance.BoardController.Refresh();
                        if (ApplySpecialEffectData.effectData.Count > 0) return;
                        StartCoroutine(ClearAttackAnimation());
                        ClearSkull();
                        _unitCard.ApplySpecialEffectData.effectData.Clear();
                        ApplySpecialEffectData.effectData.Clear();
                        _weaponClicked = _specialEffectClicked = ApplySpecialEffect = false;
                        _unitCard = null;
                        _sourceCard = null;
                        return;
                    }
                    if (!_sourceCard.Model.CanIPlay || _sourceCard.transform.parent.GetComponent<CardContainer>().IsOpponent)
                    {
                        _sourceCard = null;
                        return;
                    }
                    _sourceIndex = _sourceCard.Index;
                    _sourceCard.Hover.SetActive(false);
                    var selectedCard = hit.collider.gameObject;

                    attackBase.transform.position = new Vector3(selectedCard.transform.position.x, selectedCard.transform.position.y, 10);
                    lineRenderer.SetPosition(0, attackBase.transform.position);
                    SetAttackSprites();
                    attackPointer.SetActive(true);
                    attackBase.SetActive(true);
                    attackLine.SetActive(true);
                    IsAttackShowing = true;
                }
                else if (colliderTag == "Weapon" && !ApplySpecialEffect)
                {
                    var weaponGo = hit.collider.gameObject;
                    SetAttackSprites();
                    attackBase.transform.position = weaponGo.transform.position;
                    lineRenderer.SetPosition(0, attackBase.transform.position);
                    var weapon = _gm.BoardController.Player0.Model.PlayerWeapon;
                    var eligible = weapon.AmmunationPoint > 0 && !weapon.DidIUsedBefore;
                    attackPointer.SetActive(eligible);
                    attackBase.SetActive(eligible);
                    attackLine.SetActive(eligible);
                    _weaponClicked = eligible;
                    IsAttackShowing = eligible;
                    AudioManager.PlaySound("Commander-WeaponClick");
                }
                else if (colliderTag == "SpecialEffect" && !ApplySpecialEffect)
                {
                    var specialEffectGo = hit.collider.gameObject;
                    SetBuffSprites();
                    attackBase.transform.position = specialEffectGo.transform.position;
                    lineRenderer.SetPosition(0, attackBase.transform.position);
                    var player = _gm.BoardController.Player0;
                    var eligible = player.HasEnoughCPToUseSpecialEffect() && !player.Model.CommanderSpecialEffectCard.DidIUsedBefore;

                    attackPointer.SetActive(eligible);
                    attackBase.SetActive(eligible);
                    attackLine.SetActive(eligible);
                    _specialEffectClicked = eligible;
                    IsAttackShowing = eligible;
                }
            }
            _gm.Player0.WeaponCardHover.gameObject.SetActive(false);
            _gm.Player1.WeaponCardHover.gameObject.SetActive(false);
            _gm.Player0.SpecialAbilityHover.gameObject.SetActive(false);
            _gm.Player1.SpecialAbilityHover.gameObject.SetActive(false);
        }
        if (Input.GetMouseButtonUp(0) && IsAttackShowing)
        {
            hit = Physics2D.GetRayIntersection(_cam.ScreenPointToRay(Input.mousePosition));
            if (hit.collider != null && !ApplySpecialEffect)
            {
                var colliderTag = hit.collider.tag;
                if (colliderTag == "BoardCard")
                {
                    _targetCard = hit.transform.GetComponent<UnitCardBoard>();
                    var cardContainer = _targetCard.transform.parent.GetComponent<CardContainer>();
                    if (_targetCard != null && cardContainer != null)
                    {
                        _targetIndex = _targetCard.Index;
                        if (cardContainer.IsOpponent && !_specialEffectClicked)
                        {
                            if (_weaponClicked)
                            {
                                if(_gm.BoardController.Player0.Model.PlayerWeapon.AttackPoint >= _targetCard.Model.HealthPoint)
                                    _targetCard.Skull.gameObject.SetActive(true);
                                _targetCard.DamagePoint.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = "-" + _gm.BoardController.Player0.Model.PlayerWeapon.AttackPoint.ToString();
                                _gm.BoardController.AttackWeaponToCard(_targetIndex, _targetCard);
                                _gm.BoardController.SceneBlocker.SetActive(true);
                                _gm.BoardController.Player0.WeaponCover(false);
                                _gm.BoardController.Player0.DidIUsedWeaponSprite = true;
                                StartCoroutine(ClearAttackAnimation(_targetCard.DamagePoint));
                            }
                            else
                            {
                                _targetCard.Skull.gameObject.SetActive(true);
                                _targetCard.DamagePoint.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = "-" + _sourceCard.Model.AttackPoint.ToString();
                                _gm.BoardController.AttackCardToCard(_sourceIndex, _targetIndex, _sourceCard, _targetCard);
                                AudioManager.PlaySound("Cards/Attack-" + _sourceCard.Model.Sound);
                                _gm.BoardController.SceneBlocker.SetActive(true);
                                StartCoroutine(ClearAttackAnimation(_targetCard.DamagePoint));
                            }
                            _targetCard.DamagePoint.gameObject.SetActive(true);
                        }
                        else
                        {
                            if (_specialEffectClicked && !cardContainer.IsOpponent)
                            {
                                var tempPlayer = _gm.BoardController.BoardModel.IsMyTurn ? _gm.BoardController.Player0 : _gm.BoardController.Player1;
                                var card = tempPlayer.Model.CommanderSpecialEffectCard;
                                card.DidIUsedBefore = true;
                                _gm.BoardController.UpdateEventCardUsedCP(card);
                                _gm.BoardController.ApplyCardSpecialEffect(card, _targetIndex);
                                tempPlayer.SpecialAbilityCover(false);
                                tempPlayer.DidIUsedSpecialAbilitySprite = true;
                                AudioManager.PlaySound("CommanderAbility-US");
                            }
                        }
                    }
                }
                else if (colliderTag == "Opponent")
                {
                    if (_specialEffectClicked)
                        Debug.Log("SpecialEffectClicked and dropped on opponent");
                    else
                    {
                        var attackingPlayer = _gm.BoardController.BoardModel.IsMyTurn ? _gm.BoardController.Player0 : _gm.BoardController.Player1;
                        var defendingPlayer = _gm.BoardController.BoardModel.IsMyTurn ? _gm.BoardController.Player1 : _gm.BoardController.Player0;
                        if (_weaponClicked)
                        {
                            defendingPlayer.Indicator.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = "-" + attackingPlayer.Model.PlayerWeapon.AttackPoint.ToString();
                            _gm.BoardController.AttackWeaponToPlayer();
                            _gm.BoardController.Player0.WeaponCover(false);
                        }
                        else
                        {
                            defendingPlayer.Indicator.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = "-" + _sourceCard.Model.AttackPoint.ToString();
                            _gm.BoardController.AttackCardToPlayer(_sourceIndex);
                            AudioManager.PlaySound("Cards/Attack-" + _sourceCard.Model.Sound);
                        }
                        StartCoroutine(ClearAttackAnimation(defendingPlayer.Indicator));
                        defendingPlayer.Indicator.SetActive(true);
                    }
                }
            }
            if (ApplySpecialEffect)
                return;
            StartCoroutine(ClearAttackAnimation());
            ClearSkull();
            _weaponClicked = false;
            _specialEffectClicked = false;
            _gm.Player0.WeaponCardHover.gameObject.SetActive(false);
            _gm.Player1.WeaponCardHover.gameObject.SetActive(false);
            _gm.Player0.SpecialAbilityHover.gameObject.SetActive(false);
            _gm.Player1.SpecialAbilityHover.gameObject.SetActive(false);
        }
        _timer -= Time.deltaTime;

        if (IsAttackShowing || ApplySpecialEffect)
        {
            if (_timer > 0) return;
            _timer = _delay;

            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = Mathf.Abs(_cam.transform.position.z);
            Vector3 mouseWorldPosition = _cam.ScreenToWorldPoint(mousePosition);
            mouseWorldPosition.z = 0f;

            attackPointer.transform.position = mouseWorldPosition;
            lineRenderer.SetPosition(1, attackPointer.transform.position);

            Vector3 baseDiff = mouseWorldPosition - attackBase.transform.position;
            float rotationZ = Mathf.Atan2(baseDiff.y, baseDiff.x) * Mathf.Rad2Deg;
            attackBase.transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ + 90.0f);

            if (IsAttackShowing)
                if (ApplySpecialEffect || _sourceCard == null) return;
            hit = Physics2D.GetRayIntersection(_cam.ScreenPointToRay(Input.mousePosition));
            if (hit.collider != null)
            {
                var colliderTag = hit.collider.tag;
                if(colliderTag != "Opponent")
                {
                    _targetCard = hit.transform.GetComponent<UnitCardBoard>();
                    if (_targetCard == null) return;
                    var cardContainer = _targetCard.transform.parent.GetComponent<CardContainer>();
                    if (cardContainer == null) return;
                    _targetIndex = _targetCard.Index;
                    if (!cardContainer.IsOpponent)
                    {
                        SetBuffSprites();
                        return;
                    }
                    if (_targetIndex != targetIndex) ClearSkull();
                    targetIndex = _targetIndex;
                    if (_weaponClicked)
                    {
                        skullTarget = _targetCard.Skull.transform;
                        var targetModel = GameManager.Instance.BoardController.BoardModel.boardCards[GameManager.Instance.BoardController.BoardModel.OtherTurn][_targetIndex] as UnitCardModel;

                        if (_gm.Player0.Model.PlayerWeapon.AttackPoint >= targetModel.HealthPoint)
                            skullTarget.gameObject.SetActive(true);
                    }
                    else
                    {
                        skullTarget = _targetCard.Skull.transform;
                        skullSource = _sourceCard.Skull.transform;
                        var sourceModel = GameManager.Instance.BoardController.BoardModel.boardCards[GameManager.Instance.BoardController.BoardModel.CurrentTurn][_sourceIndex] as UnitCardModel;
                        var targetModel = GameManager.Instance.BoardController.BoardModel.boardCards[GameManager.Instance.BoardController.BoardModel.OtherTurn][_targetIndex] as UnitCardModel;
                        if (sourceModel.AttackPoint >= targetModel.HealthPoint)
                            skullTarget.gameObject.SetActive(true);
                        if (targetModel.AttackPoint >= sourceModel.HealthPoint)
                            skullSource.gameObject.SetActive(true);
                    }
                }
                SetAttackSprites();
            }
            else ClearSkull();
            _gm.Player0.WeaponCardHover.gameObject.SetActive(false);
            _gm.Player1.WeaponCardHover.gameObject.SetActive(false);
            _gm.Player0.SpecialAbilityHover.gameObject.SetActive(false);
            _gm.Player1.SpecialAbilityHover.gameObject.SetActive(false);
        }
    }

    private IEnumerator ClearAttackAnimation(GameObject damage, bool isAi = false)
    {
        yield return new WaitForSeconds(3);
        if(!isAi)
            _gm.BoardController.SceneBlocker.SetActive(false);
        if(damage != null)
            damage.SetActive(false);
    }
    private IEnumerator ClearAttackAnimation(bool AI = false)
    {
        IsAttackShowing = false;
        yield return new WaitForSeconds(AI ? 1 : 0.15f);
        attackPointer.SetActive(false);
        attackBase.SetActive(false);
        attackLine.SetActive(false);
    }

    private void ClearSkull()
    {
        if (skullSource != null)
        {
            skullSource.gameObject.SetActive(false);
            skullSource = null;
        }

        if (skullTarget != null)
        {
            skullTarget.gameObject.SetActive(false);
            skullTarget = null;
        }
    }
    
    public float GetDistance(Vector3 a, Vector3 b)
    {
        Vector3 vector = new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
        return Mathf.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
    }

    private void SetAttackSprites()
    {
        attackBase.GetComponent<SpriteRenderer>().sprite = baseAttack;
        attackPointer.GetComponent<SpriteRenderer>().sprite = pointerAttack;
        lineRenderer.material.SetTexture("_MainTex", lineAttack);
    }

    private void SetBuffSprites()
    {
        attackBase.GetComponent<SpriteRenderer>().sprite = baseBuff;
        attackPointer.GetComponent<SpriteRenderer>().sprite = pointerBuff;
        lineRenderer.material.SetTexture("_MainTex", lineBuff);
    }

    private void OnDestroy() => EventManager.OnEndTurn -= TurnFinished;
}