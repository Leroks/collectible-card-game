using System;
using System.Collections;
using System.Collections.Generic;
using StormWarfare.Card;
using StormWarfare.Core;
using StormWarfare.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StormWarfare.Gameplay {

    [System.Serializable]
    public class ParticleGroup {
        public List<ParticleSystem> particleSystems = new List<ParticleSystem> ();
    }

    public class BoardController : MonoBehaviour {
        public enum PlayerActions {
            Pass,
            PickCard,
            AttackToOpponent,
            AttackToCard
        }

        public Transform SpawnPoint;
        public GameObject Myturn;
        public GameObject EnemyTurn;
        public GameObject TimerSprite;

        public Countdown TurnTimer;
        public AttackAnimation AttackCursor;
        public Mulligan Mulligan;
        public Finish Finish;

        public Commander Player0;
        public AIPlayer Player1;
        public GameObject MockPlayer0;
        public GameObject MockPlayer1;
        public GameObject SceneBlocker;

        private BoardModel _boardModel;
        public BoardModel BoardModel {
            get {
                if (_boardModel == null) {
                    _boardModel = new BoardModel ();
                }
                return _boardModel;
            }
        }

        public GameObject StartGameButton;
        public GameObject EndMulliganButton;

        public static Command baseMechanism, baseMechanism2, usDeckLogo, deDeckLogo;
        [SerializeField] Animator animator;
        [SerializeField] string animationString;

        [SerializeField] Animator P02_animator;
        [SerializeField] string P02_animationString;
        public TMPro.TextMeshPro TurnText;

        public Animator deAnimator;
        public Animator usAnimator;
        public string deString;
        public string usString;
        public GameObject turnNotification;

        public List<ParticleGroup> baseWeopenDamageGroupsOpponent;
        public List<ParticleGroup> baseWeopenDamageGroupsPlayer;

        public Command baseWeopenOpponent, baseWeopenPlayer;

        public GameObject[] PlayerDecks;
        public GameObject[] PlayerDeckLogos;

        void Start () {
            _boardModel = null;
            BoardModel.TotalRoundCount = 2;
            CreatePlayers ();
            CreatePlayerDecks ();
            baseMechanism = new BaseMechanism (animator, animationString);
            baseMechanism2 = new BaseMechanism (P02_animator, P02_animationString);
            usDeckLogo = new BaseMechanism (usAnimator, usString);
            deDeckLogo = new BaseMechanism (deAnimator, deString);

            baseWeopenOpponent = new BaseWeopen (baseWeopenDamageGroupsOpponent, this);
            baseWeopenPlayer = new BaseWeopen (baseWeopenDamageGroupsPlayer, this);
            //history gorsellestirmesi icin demo veri yarat
            //GameManager.Instance.BattleLog?.CreateTempHistoryData();
        }

        // called first
        void OnEnable () {
            SceneBlocker.GetComponent<BoxCollider2D>().size = new Vector2(Screen.width, Screen.height);
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneBlocker.SetActive(true);
        }

        // called second
        public void OnSceneLoaded (Scene scene, LoadSceneMode mode) {
            StartCoroutine (StartGame ());
        }

        public void RestartGame () {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Scene scene = SceneManager.GetActiveScene ();
            SceneManager.LoadScene (scene.name);
        }

        //todo: menuden startgame butonuna baglanabilir.
        public IEnumerator StartGame () {
            yield return new WaitForSeconds (2);
            BoardModel.EndGameData = null;
            BoardModel.BoardState = BoardModel.BoardStates.Mulligan;
            Debug.Log ($"BOARDSTATE: {BoardModel.BoardState}");
            TurnTimer.gameObject.SetActive(true);
            TurnTimer.StartCountdown (BoardModel.MulliganTurnTimer, EndMulligan);
            Mulligan.gameObject.SetActive (true);
            SceneBlocker.SetActive(false);
            AttackCursor.GetComponent<AttackAnimation> ().enabled = BoardModel.IsMyTurn;

            //todo: sonrasinda kaldir!
            //StartGameButton.SetActive(false);
        }

        public void EndMulligan () {
            if (BoardModel.BoardState != BoardModel.BoardStates.Mulligan) return;
            if (Mulligan.isActiveAndEnabled)
                Mulligan.GetComponent<Mulligan> ().CompleteMulligan ();
            //AI 4 icin eklendi 4 kartla baslasin diye
            BoardModel.CurrentTurn = 1;
            /*for (int i = 0; i < 4; i++)
            {
                PickCard();
            }*/
            Player1.Play ();
            BoardModel.BoardState = BoardModel.BoardStates.Playing;
            Debug.Log ($"BOARDSTATE: {BoardModel.BoardState}");
            BoardModel.CurrentTurn = 0;
            BoardModel.ShuffleDecks ();
            TurnTimer.StartCountdown (BoardModel.RoundTurnTimer, EndTurn);

            //todo: sonrasinda kaldir !
            EndMulliganButton.SetActive (false);
            baseMechanism.Execute ();
            baseMechanism2.Execute ();
            usDeckLogo.Execute ();
            deDeckLogo.Execute ();
            // TimerSprite.SetActive(true);
        }

        /// <summary>
        /// oyuncu sirasini karsiya gecirir ayni zamanda timeri resetler ve gorselini de gunceller
        /// </summary>
        public void EndTurn () {
            if (BoardModel.BoardState == BoardModel.BoardStates.Finish || Mulligan.isActiveAndEnabled) return;
            //todo: timeout oldugunda sira gecmesi disinda herhangi effekt olucak mi kart atma vs gibi
            var myTurn = BoardModel.IsMyTurn;
            StartCoroutine (ShowTurnNotification (myTurn));
            Myturn.SetActive (!myTurn);
            EnemyTurn.SetActive (myTurn);
            BoardModel.AdvanceNextPlayer ();
            SceneBlocker.SetActive(BoardModel.IsMyTurn);
            PickCard ();
            Refresh ();
            BoardModel.UpdateCommandingPoint ();
            if (!BoardModel.IsMyTurn) {
                if (Player1.DidIUsedWeaponSprite && Player1.Model.PlayerWeapon.AmmunationPoint > 0)
                {
                    Player1.WeaponCover(true);
                    Player1.DidIUsedWeaponSprite = false;
                }
                if (Player1.DidIUsedSpecialAbilitySprite && Player1.Model.CommanderSpecialEffectCard.CommandingPoint <= Player1.Model.CommandingPoint)
                {
                    Player1.SpecialAbilityCover(true);
                    Player1.DidIUsedSpecialAbilitySprite = false;
                }
                Player1.Play ();
            }
            else
            {
                if (Player0.DidIUsedWeaponSprite && Player0.Model.PlayerWeapon.AmmunationPoint > 0)
                {
                    Player0.WeaponCover(true);
                    Player0.DidIUsedWeaponSprite = false;
                }
                if (Player0.DidIUsedSpecialAbilitySprite && Player0.Model.CommanderSpecialEffectCard.CommandingPoint <= Player0.Model.CommandingPoint)
                {
                    Player0.SpecialAbilityCover(true);
                    Player0.DidIUsedSpecialAbilitySprite = false;
                }
            }
            EventManager.EndTurned (BoardModel.IsMyTurn);
            BoardModel.ResetCardAttack ();
            Refresh ();
            TurnTimer.StartCountdown (BoardModel.RoundTurnTimer, EndTurn);
            if (TurnTimer.Time < 20) {
                TurnTimer.turnTimer.SetActive (!myTurn);
            }
            TurnTimer.turnTimer.SetActive (false);
            AudioManager.PlaySound("EndTurn");
        }

        IEnumerator EndGame (EndGameModel model) {
            BoardModel.BoardState = BoardModel.BoardStates.Finish;
            yield return new WaitForSeconds (3f);
            TurnTimer.gameObject.SetActive(false);
            StopAllCoroutines ();
            TurnTimer.StopAllCoroutines ();
            Finish.gameObject.SetActive (true);
            Finish.InitModels (Player0.Model, model);
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        void CreatePlayers () {
            TextAsset txt = (TextAsset) Resources.Load ("Data/players", typeof (TextAsset));
            List<CommanderModel> playersData = JsonModel.Deserialize<List<CommanderModel>> (Json.parse (txt.text));
            BoardModel.Players[0] = playersData[0];
            BoardModel.Players[1] = playersData[1];
            GameManager.Instance.Player0 = Player0 = Instantiate (Resources.Load ("Prefabs/2D/PlayerNew", typeof (Commander)) as Commander, new Vector3 (32, -443, 0), Quaternion.identity);
            Player0.InitPlayer (playersData[0]);
            GameManager.Instance.Player1 = Player1 = Instantiate (Resources.Load ("Prefabs/2D/AIPlayerNew", typeof (AIPlayer)) as AIPlayer, new Vector3 (32, 428, 0), Quaternion.identity);
            Player1.InitPlayer (playersData[1]);
            MockPlayer0.SetActive (false);
            MockPlayer1.SetActive (false);
        }

        void CreatePlayerDecks () {
            BoardModel.MyDeck.Clear ();
            BoardModel.OppDeck.Clear ();
            TextAsset usdeckfile = (TextAsset) Resources.Load ("Data/usdeck", typeof (TextAsset));
            TextAsset dedeckfile = (TextAsset) Resources.Load ("Data/dedeck", typeof (TextAsset));
            var usdeckparsed = (IList) Json.parse (usdeckfile.text);
            var dedeckparsed = (IList) Json.parse (dedeckfile.text);

            for (int i = 0; i < usdeckparsed.Count; i++) {
                var item = usdeckparsed[i];
                switch ((Enums.CardType) item.ToInt ("type")) {
                    case Enums.CardType.Unit:
                        BoardModel.MyDeck.Add (JsonModel.Deserialize<UnitCardModel> (item));
                        break;
                    case Enums.CardType.Hero:
                        break;
                    case Enums.CardType.Event:
                        BoardModel.MyDeck.Add (JsonModel.Deserialize<EventCardModel> (item));
                        break;
                    case Enums.CardType.Weapon:
                        break;
                    case Enums.CardType.Commander:
                        break;
                    default:
                        break;
                }
            }

            for (int i = 0; i < dedeckparsed.Count; i++) {
                var item = dedeckparsed[i];
                switch ((Enums.CardType) item.ToInt ("type")) {
                    case Enums.CardType.Unit:
                        BoardModel.OppDeck.Add (JsonModel.Deserialize<UnitCardModel> (item));
                        break;
                    case Enums.CardType.Hero:
                        break;
                    case Enums.CardType.Event:
                        BoardModel.OppDeck.Add (JsonModel.Deserialize<EventCardModel> (item));
                        break;
                    case Enums.CardType.Weapon:
                        break;
                    case Enums.CardType.Commander:
                        break;
                    default:
                        break;
                }
            }

        }

        /// <summary>
        /// todo: kontrol et gerekmiyorsa kaldir
        /// butun sahnenin gorsel duzeninden sorumlu oyuncularin ve masanin gorselini modele gore duzelt
        /// </summary>
        public void Refresh (bool refreshBase = false) {
            Player0.CardContainer.UpdateChildDatas ();
            Player1.CardContainer.UpdateChildDatas ();
            if (refreshBase) {
                Player0.BaseContainer.SetChildPositions ();
                Player1.BaseContainer.SetChildPositions ();
            }
            Player0.CardContainer.SetChildPositions (-1);
            Player1.CardContainer.SetChildPositions (-1);
            RefreshPlayers ();
        }

        void RefreshPlayers () {
            if (Player0.UpdatePlayerData (BoardModel.Players[0])) {
                BoardModel.EndGameData = new EndGameModel {
                    CurrentRank = 100,
                    GainedETH = 3000,
                    GainedXP = 20,
                    MaxRank = 2000,
                    WinnerFaction = Player1.Model.Faction
                };
            } else if (Player1.UpdatePlayerData (BoardModel.Players[1])) {
                BoardModel.EndGameData = new EndGameModel {
                    CurrentRank = 100,
                    GainedETH = 3000,
                    GainedXP = 20,
                    MaxRank = 2000,
                    WinnerFaction = Player0.Model.Faction
                };
            }

            if (BoardModel.EndGameData != null) {
                StartCoroutine (EndGame (BoardModel.EndGameData));
            }
        }

        public void AttackCardToCard (int sourceIndex, int targetIndex, UnitCardBoard sourceCard, UnitCardBoard targetCard) {
            //todo: history log card attacked

            var sourceModel = BoardModel.boardCards[BoardModel.CurrentTurn][sourceIndex] as UnitCardModel;
            var targetModel = BoardModel.boardCards[BoardModel.OtherTurn][targetIndex] as UnitCardModel;
            sourceModel.DidIAttackBefore = true;
            BoardModel.AttackToCard (sourceIndex, targetIndex);
            GameManager.Instance.BattleLog?.LogAttackToCard (sourceModel, targetModel);
            if (sourceModel != null && sourceCard.UpdateCardData (sourceModel)) {
                //BoardModel.boardCards[BoardModel.CurrentTurn].RemoveAt(sourceIndex);
                StartCoroutine (RefreshBoard ());
            }
            if (targetModel != null && targetCard.UpdateCardData (targetModel)) {
                //BoardModel.boardCards[BoardModel.OtherTurn].RemoveAt(targetIndex);
                StartCoroutine (RefreshBoard ());
            } else {
                targetCard.ExecuteAnimation ("damage");
                Refresh ();
            }
        }
        private IEnumerator RefreshBoard () {
            yield return new WaitForSeconds (1.5f);
            Refresh ();
        }

        public void AttackCardToPlayer (int sourceIndex) {
            var sourceModel = BoardModel.boardCards[BoardModel.CurrentTurn][sourceIndex] as UnitCardModel;
            BoardModel.AttackCardToPlayer (sourceIndex);
            sourceModel.DidIAttackBefore = true;
            GameManager.Instance.BattleLog?.LogAttackToCommander (sourceModel, BoardModel.Players[BoardModel.OtherTurn]);
            Refresh ();
        }

        public void AttackWeaponToPlayer () {
            var sourceModel = BoardModel.Players[BoardModel.CurrentTurn].PlayerWeapon;
            sourceModel.DidIUsedBefore = true;
            BoardModel.AttackWeaponToPlayer ();
            var tempPlayer = BoardModel.CurrentTurn == 0 ? Player0 : Player1;
            tempPlayer.DidIUsedWeaponSprite = true;
            tempPlayer.WeaponCardHover.UpdateCardData (sourceModel);
            GameManager.Instance.BattleLog?.LogAttackToCommander (sourceModel, BoardModel.Players[BoardModel.OtherTurn]);
            RefreshPlayers ();
            AudioManager.PlaySound("CommanderWeapon");
            if (tempPlayer == Player0)
                baseWeopenOpponent.Execute ();
            else
                baseWeopenPlayer.Execute ();
        }

        public void AttackWeaponToCard (int targetIndex, UnitCardBoard targetCard) {
            var sourceModel = BoardModel.Players[BoardModel.CurrentTurn].PlayerWeapon;
            sourceModel.DidIUsedBefore = true;
            var targetModel = BoardModel.boardCards[BoardModel.OtherTurn][targetIndex] as UnitCardModel;
            BoardModel.AttackWeaponToCard (targetIndex);
            var tempPlayer = BoardModel.CurrentTurn == 0 ? Player0 : Player1;
            tempPlayer.WeaponCardHover.UpdateCardData (sourceModel);
            tempPlayer.DidIUsedWeaponSprite = true;
            GameManager.Instance.BattleLog?.LogAttackToCard (sourceModel, targetModel);
            AudioManager.PlaySound("CommanderWeapon");
            targetCard.ExecuteAnimation ("weopen");
            if (targetCard.UpdateCardData(targetModel))
            {
                StartCoroutine(RefreshBoard());
                return;
            }
            Refresh ();
        }

        /// <summary>
        /// modeldeki yere acilan kartlari guncelle sadece siranin o anki oyuncuda oldugu oyuncu kart atabilir
        /// bu yuzden model icerisinde sira kimdeyse onun verisini guncelle
        /// </summary>
        /// <param name="card"></param>
        /// <param name="index"></param>
        public void PlaceCardToBattleGround (UnitCardModel card, int index) {
            //todo: historylog card oynandi
            BoardModel.PlaceCardToBattleGround (card, index);
            card.DeploymentRound = BoardModel.TotalRoundCount + 1;
            if (card.SpecialEffects.Count > 0) {
                ApplyCardSpecialEffect (card, index);
            } else {
                //todo:eger efekti olan kart ortaya atildiysa ve onceden buff veren kart varsa bu karta buff vermiyor duzeltilmeli !!
                var tempPlayer = BoardModel.CurrentTurn == 0 ? Player0 : Player1;
                tempPlayer.CardContainer.ApplySpecialEffectBuffs (card);
            }
            GameManager.Instance.BattleLog?.LogPlaceCardToPlayGround (card);

            Refresh ();
        }

        /// <summary>
        /// event kart ortaya oynandiysa efektini uygular
        /// </summary>
        /// <param name="cardData"></param>
        public void ApplyCardSpecialEffect (EventCardModel cardData, int targetIndex = 0) {

            var resetBase = true;
            for (int i = 0; i < cardData.SpecialEffects.Count; i++) {
                var effect = cardData.SpecialEffects[i];
                for (int j = 0; j < effect.Conditions.Count; j++) {
                    var condition = effect.Conditions[j];
                    switch (condition.Type) {
                        case Enums.SpecialEffectCondition.HaveTypeOf:
                            break;
                        default:
                            break;
                    }
                }

                switch (effect) {
                    case DealDamage e:
                        e.UseEffect (ref _boardModel.boardCards[_boardModel.OtherTurn], ref _boardModel.Players[_boardModel.OtherTurn], cardData);
                        break;
                    case GiveDefensePoint e:
                        e.UseEffect (ref _boardModel.boardCards[_boardModel.CurrentTurn], ref _boardModel.Players[(_boardModel.CurrentTurn)], cardData);
                        break;
                    case GiveAttackPoint e:
                        e.UseEffect (ref _boardModel.boardCards[_boardModel.CurrentTurn], cardData, targetIndex);
                        break;
                    case DrawCard e:
                        resetBase = false;
                        for (int j = 0; j < e.Count; j++)
                            PickCard (e.Count);
                        break;
                    case GainCommandingPoint e:
                        e.UseEffect (ref _boardModel.Players[(_boardModel.CurrentTurn)]);
                        break;
                    case Retreat e:
                        RetreatCards (e.UseEffect (ref _boardModel));
                        break;

                    default:
                        break;
                }
            }

            //BoardModel.UseEventCard (cardData);
            GameManager.Instance.BattleLog?.LogEventCardEffect (cardData);
            Refresh (resetBase);
        }

        public void UpdateEventCardUsedCP(EventCardModel cardData)
        {
            BoardModel.UseEventCard(cardData);
            RefreshPlayers();
        }

        /// <summary>
        /// unitcardlarin specialeffectlerini varsa kullandirtir
        /// </summary>
        /// <param name="cardData"></param>
        void ApplyCardSpecialEffect (UnitCardModel cardData, int targetIndex = 0) {

            for (int i = 0; i < cardData.SpecialEffects.Count; i++) {
                var effect = cardData.SpecialEffects[i];
                for (int j = 0; j < effect.Conditions.Count; j++) {
                    var condition = effect.Conditions[j];
                    switch (condition.Type) {
                        case Enums.SpecialEffectCondition.HaveTypeOf:
                            break;
                        default:
                            break;
                    }
                }

                switch (effect) {
                    case DealDamage e:
                        if (e.Target == Enums.SpecialEffectTarget.Unit) {
                            if (_boardModel.IsMyTurn) {
                                if (Player1.CardContainer.Cards.Count >= 1)
                                    Player0.CardContainer.Cards[targetIndex].ApplySpecialEffectData.effectData.Add (e);
                            } else {
                                if (Player0.CardContainer.Cards.Count >= 1)
                                    Player1.CardContainer.Cards[targetIndex].ApplySpecialEffectData.effectData.Add (e);
                            }
                        } else
                            e.UseEffect (ref _boardModel.boardCards[_boardModel.OtherTurn], ref _boardModel.Players[_boardModel.OtherTurn], cardData);
                        break;
                    case GiveDefensePoint e:
                        if (e.Target == Enums.SpecialEffectTarget.Unit) {
                            if (_boardModel.IsMyTurn) {
                                if (Player0.CardContainer.Cards.Count > 1)
                                    Player0.CardContainer.Cards[targetIndex].ApplySpecialEffectData.effectData.Add (e);
                            } else {
                                if (Player1.CardContainer.Cards.Count > 1)
                                    Player1.CardContainer.Cards[targetIndex].ApplySpecialEffectData.effectData.Add (e);
                            }
                        } else
                            e.UseEffect (ref _boardModel.boardCards[_boardModel.CurrentTurn], ref _boardModel.Players[_boardModel.CurrentTurn], cardData);
                        break;
                    case GiveAttackPoint e:
                        if (e.Target == Enums.SpecialEffectTarget.Unit) {
                            if (_boardModel.IsMyTurn) {
                                if (Player0.CardContainer.Cards.Count > 1)
                                    Player0.CardContainer.Cards[targetIndex].ApplySpecialEffectData.effectData.Add (e);
                            } else {
                                if (Player1.CardContainer.Cards.Count > 1)
                                    Player1.CardContainer.Cards[targetIndex].ApplySpecialEffectData.effectData.Add (e);
                            }
                        } else
                            e.UseEffect (ref _boardModel.boardCards[_boardModel.CurrentTurn], cardData);
                        break;
                    case DrawCard e:
                        for (int j = 0; j < e.Count; j++)
                            PickCard ();
                        break;
                    case GainCommandingPoint e:
                        e.UseEffect (ref _boardModel.Players[(_boardModel.CurrentTurn)]);
                        break;
                    case Retreat e:
                        RetreatCards (e.UseEffect (ref _boardModel));
                        break;
                    default:
                        break;
                }
            }
        }
        public void RetreatCards (List<int> removedCardIndexes) {
            Commander tempPlayer = Player0;
            if (BoardModel.IsMyTurn)
                tempPlayer = Player1;

            for (int i = 0; i < removedCardIndexes.Count; i++) {
                var card = tempPlayer.CardContainer.Cards[removedCardIndexes[i]];
                card.IsRetreat = true;
                var unitCard = Instantiate (Resources.Load ("Prefabs/2D/UnitCardHand", typeof (UnitCardHand)) as UnitCardHand, card.transform.localPosition, Quaternion.identity, tempPlayer.BaseContainer.transform);
                unitCard.InitCard ((card as UnitCardBoard).Model);
                if (tempPlayer.Model.Faction == Enums.Faction.DE) {
                    unitCard.transform.GetChild (0).gameObject.SetActive (false);
                    unitCard.transform.GetChild (1).gameObject.SetActive (true);
                }
                tempPlayer.BaseContainer.CardRetreat (unitCard);
            }
            for (int i = 0; i < tempPlayer.CardContainer.Cards.Count; i++) {
                var card = tempPlayer.CardContainer.Cards[i];
                if (!card.IsRetreat) continue;
                tempPlayer.CardContainer.Cards.Remove (card);
                Destroy (card.gameObject);
                tempPlayer.CardContainer.SetChildPositions (-1);
                i--;
            }
        }

        private IEnumerator ClearAttackAnimation(GameObject damage)
        {
            yield return new WaitForSeconds(3);
            if(damage != null)
                damage.SetActive(false);
        }

        public void PickCard (int drawCount = 0)
        {
            if (BoardModel.PunishOutOfDeckPlayer())
            {
                SceneBlocker.SetActive(false);
                var currPlayer = BoardModel.IsMyTurn ? Player0 : Player1;
                currPlayer.Indicator.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = "" + -1 * BoardModel.PunishCounter[BoardModel.CurrentTurn]++;
                StartCoroutine(ClearAttackAnimation(currPlayer.Indicator));
                currPlayer.Indicator.SetActive(true);
                return;
            }
            Commander tempPlayer = Player1;
            string prefabPostFix = "";
            if (BoardModel.IsMyTurn) {
                tempPlayer = Player0;
                prefabPostFix = "";
                if(BoardModel.MyDeck.Count > 0)
                {
                    baseMechanism.Execute ();
                    usDeckLogo.Execute ();
                }
            } else {
                if(BoardModel.OppDeck.Count > 0)
                {
                    baseMechanism2.Execute ();
                    deDeckLogo.Execute ();
                }
            }
            if (tempPlayer.BaseContainer.Hand.Count > 9) {
                //todo 9 dan fazla ise pick card yapma notification vs goster?
                return;
            }
            var nextCardModel = BoardModel.GetNextCard (BoardModel.CurrentTurn);
            BoardModel.PickCard ();
            // baseMechanism.Execute();
            // baseMechanism2.Execute();
            switch (nextCardModel) {
                case UnitCardModel:
                    var unitCard = Instantiate (Resources.Load ("Prefabs/2D/UnitCardHand" + prefabPostFix, typeof (UnitCardHand)) as UnitCardHand, tempPlayer.SpawnPoint.localPosition, Quaternion.identity, tempPlayer.BaseContainer.transform);
                    unitCard.InitCard (nextCardModel as UnitCardModel);
                    if (tempPlayer.Model.Faction == Enums.Faction.DE) {
                        unitCard.transform.GetChild (0).gameObject.SetActive (false);
                        unitCard.transform.GetChild (1).gameObject.SetActive (true);
                    }
                    unitCard.gameObject.SetActive(false);
                    tempPlayer.BaseContainer.NewCardPicked(unitCard, drawCount);
                    break;
                case EventCardModel:
                    var eventCard = Instantiate (Resources.Load ("Prefabs/2D/EventCardHand" + prefabPostFix, typeof (EventCard)) as EventCard, tempPlayer.SpawnPoint.localPosition, Quaternion.identity, tempPlayer.BaseContainer.transform);
                    eventCard.InitCard (nextCardModel as EventCardModel);
                    if (tempPlayer.Model.Faction == Enums.Faction.DE) {
                        eventCard.transform.GetChild (0).gameObject.SetActive (false);
                        eventCard.transform.GetChild (1).gameObject.SetActive (true);
                    }
                    eventCard.gameObject.SetActive(false);
                    tempPlayer.BaseContainer.NewCardPicked(eventCard, drawCount);
                    break;
            }

            StartCoroutine(DelayDeckFinished(BoardModel.CurrentTurn));
        }

        IEnumerator DelayDeckFinished(int currTurn)
        {
            yield return new WaitForSeconds (currTurn == 0 ? 3f : .7f);
            var tempDeck = currTurn == Enums.Faction.US.ToInt() ? BoardModel.MyDeck : BoardModel.OppDeck;
            if(tempDeck.Count <= 0)
            {
                PlayerDecks[currTurn].SetActive(false);
                PlayerDeckLogos[currTurn].SetActive(false);
            }
        }

        IEnumerator ShowTurnNotification (bool isEnemyTurn) {
            if (isEnemyTurn) {
                turnNotification.transform.GetChild (0).gameObject.SetActive (true);
                yield return new WaitForSeconds (2);
                turnNotification.transform.GetChild (0).gameObject.SetActive (false);
            }
            yield return new WaitForSeconds (0.5f);
            if (!isEnemyTurn) {
                turnNotification.transform.GetChild (1).gameObject.SetActive (true);
                yield return new WaitForSeconds (2);
                turnNotification.transform.GetChild (1).gameObject.SetActive (false);
            }
        }
    }
}