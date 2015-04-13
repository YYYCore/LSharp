using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;


namespace YTurnAround
{

    public class Champ
    {
        public string name { get; set; }
        public string skill { get; set; }
        public string dspname { get; set; }
        public int range { get; set; }
        public int delay { get; set; }
        public int direction { get; set; }
        public int speed { get; set; }

    }


    internal class Turn
    {

        public static Orbwalking.Orbwalker Orbwalker;

        public static Menu config;
        private static Obj_AI_Hero player;
        public static Spell TryndW, ShacoE, CassR;
        public static Champ Tryndamere, Shaco, Cassiopeia;
        public static Vector3 pos;

        public int direct = 0;


        public static DateTime timestamp;
        public static TimeSpan time;







        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }


        static double getDistance(Vector3 vec1, Vector3 vec2)
        {
            double dist = (Math.Pow(((Math.Pow(vec2.X - vec1.X, 2)) + Math.Pow(vec2.Y - vec1.Y, 2) + Math.Pow(vec2.Z - vec1.Z, 2)), 0.5));
            return dist;
        }


        static int getArrivalTime(Vector3 startpos, Vector3 endpos, int speed)
        {
            double dist = getDistance(startpos, endpos);
            int time = Convert.ToInt32(dist) / speed;
            return time;
        }


        static void Game_OnGameLoad(EventArgs args)
        {


            Tryndamere = new Champ
            {
                name = "Tryndamere",
                skill = "MockingShout",
                dspname = "Trynda W",
                range = 850,
                delay = 10,
                direction = 1,
                speed = 500,
            };

            Shaco = new Champ
            {
                name = "Shaco",
                skill = "TwoShivPoison",
                dspname = "Shaco E",
                range = 625,
                delay = 100,
                direction = 1,
                speed = 1500,
            };

            Cassiopeia = new Champ
            {
                name = "Cassiopeia",
                skill = "CassiopeiaPetrifyingGaze",
                dspname = "Cassio R",
                range = 750,
                delay = 50,
                direction = -1,
                speed = 0
            };





            Game.PrintChat("test1");
            //
            player = ObjectManager.Player;
            config = new Menu("YTurnAround", "YTurnAround", true);
            config.AddSubMenu(new Menu("Spells", "Spells"));
            config.SubMenu("Spells").AddItem(new MenuItem(Tryndamere.dspname, Tryndamere.dspname).SetValue(true));
            config.SubMenu("Spells").AddItem(new MenuItem(Shaco.dspname, Shaco.dspname).SetValue(true));
            config.SubMenu("Spells").AddItem(new MenuItem(Cassiopeia.dspname, Cassiopeia.dspname).SetValue(true));
            config.AddToMainMenu();


            



            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;

        }


        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs spell)
        {

            if ((spell.SData.Name == Tryndamere.skill
               || spell.SData.Name == Shaco.skill
               || spell.SData.Name == Cassiopeia.skill))
            {

                Game.PrintChat("yolo");
                timestamp = DateTime.Now;

                double delay = 0;
                int direct = 0;



                if (sender.BaseSkinName == "Tryndamere")
                {
                    if (getDistance(player.Position, sender.Position) <= Tryndamere.range) {
                        if (!player.IsFacing(sender)) {

                            delay = Tryndamere.delay + getArrivalTime(player.Position, sender.Position, Tryndamere.speed);
                            direct = Tryndamere.direction;

                            pos = new Vector3(player.Position.X + (direct * sender.Position.X) / 5
                             , player.Position.Y + (direct * sender.Position.Y) / 5
                             , 0);

                        }
                    
                    }
                }



                if (sender.BaseSkinName == "Shaco")
                {
                    delay = Shaco.delay + getArrivalTime(player.Position, sender.Position, Shaco.speed);
                    foreach (var missile in ObjectManager.Get<Obj_SpellMissile>())
                    {
                        if (missile.SData.Name == "ShivPoison" && missile.Target.Name == player.Name)
                        {
                            if (!player.IsFacing(sender))
                            {
                                //Game.PrintChat(missile.SData.Name + missile.SpellCaster + missile.Target.Name + missile.Position);
                                pos = new Vector3((player.Position.X + missile.Position.X) / 2, (player.Position.Y + missile.Position.Y) / 2, 0);                  
                            }
                        }
                    }
                }



                if (sender.BaseSkinName == "Cassiopeia")
                {
                    delay = Cassiopeia.delay;
                    direct = Cassiopeia.direction;

                    if (player.IsFacing(sender))
                    {
                        pos = new Vector3(player.Position.X + (direct * 10)
                      , player.Position.Y + (direct * 10)
                      , 0);
                    }
                }

                Vector3 lastpos = player.Position;

                time = timestamp - DateTime.Now;
                Utility.DelayAction.Add(Convert.ToInt32(delay - time.TotalMilliseconds), () =>
                {
                    player.IssueOrder(GameObjectOrder.MoveTo, pos);
                });

                Utility.DelayAction.Add(100, () =>
                {
                    player.IssueOrder(GameObjectOrder.MoveTo, lastpos);
                });


            }
        }
    }
}