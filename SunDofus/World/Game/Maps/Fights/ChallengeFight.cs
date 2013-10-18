using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using SunDofus.World.Game.Characters;

namespace SunDofus.World.Game.Maps.Fights
{
    class ChallengeFight : Fight
    {
        public ChallengeFight(Character player1, Character player2, Map map)
            : base(FightType.CHALLENGE, map)
        {
            Fighter attacker = new CharacterFighter(player1, this);
            Fighter defender = new CharacterFighter(player2, this);

            FightInit(attacker, defender);
        }

        public override int StartTime()
        {
            return 0;
        }

        public override int TurnTime()
        {
            return 30000;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void PlayerLeave(Fighter fighter)
        {
            switch (State)
            {
                case FightState.STARTING:

                    if (fighter.IsLeader)
                    {
                        fighter.Left = true;

                        FightEnd(GetEnnemyTeam(fighter.Team), fighter.Team);
                    }
                    else
                    {
                        fighter.Team.FighterLeave(fighter);

                        fighter.Character.State.InFight = false;
                        fighter.Character.Fight = null;
                        fighter.Character.Fighter = null;

                        Send(FormatFighterDestroy(fighter));
                        Map.Send(FormatFlagFighterDestroy(fighter));
                        fighter.Character.NClient.Send(FormatLeave());
                    }

                    break;

                case FightState.PLAYING:

                    if (CurrentFighter == fighter)
                        TurnEnd();

                    fighter.Left = true;

                    if (GetWinners() != null)
                    {
                        FightEnd(GetEnnemyTeam(fighter.Team), fighter.Team);
                    }
                    else
                    {
                        fighter.Buffs.Debuff();

                        fighter.Character.State.InFight = false;
                        fighter.Character.Fight = null;
                        fighter.Character.Fighter = null;

                        Send(FormatFighterDestroy(fighter));
                        fighter.Character.NClient.Send(FormatLeave());
                    }

                    break;
            }
        }

        public override void FightEnd(FightTeam winners, FightTeam loosers)
        {
            if (State == FightState.STARTING)
                Map.Send(FormatFlagDestroy());

            StringBuilder builder = new StringBuilder("GE");

            builder.Append(EndTime).Append('|');
            builder.Append(ID).Append('|');
            builder.Append(Type == FightType.AGRESSION ? 1 : 0);

            foreach (Fighter fighter in GetFighters())
            {
                builder.Append('|').Append(fighter.Team == winners ? 2 : 0).Append(';');
                builder.Append(fighter.ID).Append(';');
                builder.Append(fighter.Name).Append(';');
                builder.Append(fighter.Level).Append(';');
                builder.Append(fighter.Dead ? 1 : 0).Append(';');

                if (Utilities.Config.GetBoolElement("EnableExpInPvPFight") && (fighter.Team == winners))
                {
                    var exp = (int)(GetFighters().ToList().FindAll(x => x.Team != fighter.Team).Count * Utilities.Formulas.ExpPvp(fighter.Character.Level, fighter.Team.GetFighters().Sum
                        (x => x.Level), GetFighters().ToList().FindAll(x => x.Team != fighter.Team).Sum(x => x.Level)));

                    fighter.Character.AddExp(exp);

                    builder.Append(Entities.Requests.LevelsRequests.ReturnLevel(fighter.Character.Level).Character).Append(";").Append(fighter.Character.Exp).Append(";");
                    builder.Append((fighter.Character.Level != Entities.Requests.LevelsRequests.MaxLevel() ? Entities.Requests.LevelsRequests.ReturnLevel(fighter.Character.Level + 1).Character.ToString() : "-1")).Append(";");
                    builder.Append(exp).Append(";;;;");
                }
                else
                    builder.Append(";;;;;;;");
            }

            Send(builder.ToString());

            State = FightState.FINISHED;

            foreach (Fighter fighter in GetFighters().OfType<CharacterFighter>())
            {
                if (fighter.Fight == this)
                {
                    fighter.Buffs.Debuff();

                    fighter.Character.State.InFight = false;
                    fighter.Character.Fight = null;
                    fighter.Character.Fighter = null;
                }
            }

            KickSpectator(true);

            Map.RemoveFight(this);
            Map.Send(Map.FormatFightCount());
        }
    }
}
