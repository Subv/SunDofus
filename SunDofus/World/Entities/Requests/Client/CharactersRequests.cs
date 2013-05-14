using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace SunDofus.World.Entities.Requests
{
    class CharactersRequests
    {
        public static void LoadCharacters()
        {
            lock (DatabaseProvider.ConnectionLocker)
            {
                var sqlText = "SELECT * FROM dyn_characters";
                var sqlCommand = new MySqlCommand(sqlText, DatabaseProvider.Connection);

                var sqlResult = sqlCommand.ExecuteReader();

                while (sqlResult.Read())
                {
                    var character = new Game.Characters.Character()
                    {
                        ID = sqlResult.GetInt16("id"),
                        Name = sqlResult.GetString("name"),
                        Level = sqlResult.GetInt16("level"),
                        Class = sqlResult.GetInt16("class"),
                        Sex = sqlResult.GetInt16("sex"),
                        Size = 100,
                        Color = sqlResult.GetInt32("color"),
                        Color2 = sqlResult.GetInt32("color2"),
                        Color3 = sqlResult.GetInt32("color3"),

                        MapCell = int.Parse(sqlResult.GetString("mappos").Split(',')[1]),
                        MapID = int.Parse(sqlResult.GetString("mappos").Split(',')[0]),
                        Dir = int.Parse(sqlResult.GetString("mappos").Split(',')[2]),

                        Exp = sqlResult.GetInt64("experience"),

                        isNewCharacter = false,

                    };

                    character.Skin = int.Parse(string.Format("{0}{1}", character.Class, character.Sex));
                    character.ParseStats(sqlResult.GetString("stats"));

                    if (sqlResult.GetString("items") != "") 
                        character.ItemsInventary.ParseItems(sqlResult.GetString("items"));

                    if (sqlResult.GetString("spells") != "") 
                        character.SpellsInventary.ParseSpells(sqlResult.GetString("spells"));

                    var factionInfos = sqlResult.GetString("faction").Split(';');
                    character.Faction.ID = int.Parse(factionInfos[0]);
                    character.Faction.Honor = int.Parse(factionInfos[1]);
                    character.Faction.Deshonor = int.Parse(factionInfos[2]);

                    if (character.Faction.Honor > Entities.Requests.LevelsRequests.LevelsList.OrderByDescending(x => x.Alignment).ToArray()[0].Alignment)
                        character.Faction.Level = 10;
                    else
                        character.Faction.Level = Entities.Requests.LevelsRequests.LevelsList.Where(x => x.Alignment <= character.Faction.Honor).OrderByDescending(x => x.Alignment).ToArray()[0].ID;

                    lock (Game.Characters.CharactersManager.CharactersList)
                        Game.Characters.CharactersManager.CharactersList.Add(character);
                }

                sqlResult.Close();
            }

            Utilities.Loggers.StatusLogger.Write(string.Format("Loaded '{0}' characters from the database !", Game.Characters.CharactersManager.CharactersList.Count));
        }

        public static void CreateCharacter(Game.Characters.Character character)
        {
            lock (DatabaseProvider.ConnectionLocker)
            {
                var sqlText = "INSERT INTO dyn_characters VALUES(@id, @name, @level, @class, @sex, @color, @color2, @color3, @mapinfos, @stats, @items, @spells, @exp, @faction)";
                var sqlCommand = new MySqlCommand(sqlText, DatabaseProvider.Connection);

                var P = sqlCommand.Parameters;

                P.Add(new MySqlParameter("@id", character.ID));
                P.Add(new MySqlParameter("@name", character.Name));
                P.Add(new MySqlParameter("@level", character.Level));
                P.Add(new MySqlParameter("@class", character.Class));
                P.Add(new MySqlParameter("@sex", character.Sex));
                P.Add(new MySqlParameter("@color", character.Color));
                P.Add(new MySqlParameter("@color2", character.Color2));
                P.Add(new MySqlParameter("@color3", character.Color3));
                P.Add(new MySqlParameter("@mapinfos", character.MapID + "," + character.MapCell + "," + character.Dir));
                P.Add(new MySqlParameter("@stats", character.SqlStats()));
                P.Add(new MySqlParameter("@items", character.GetItemsToSave()));
                P.Add(new MySqlParameter("@spells", character.SpellsInventary.SaveSpells()));
                P.Add(new MySqlParameter("@exp", character.Exp));
                P.Add(new MySqlParameter("@faction", "0;0;0"));

                sqlCommand.ExecuteNonQuery();

                character.isNewCharacter = false;
            }
        }

        public static void SaveCharacter(Game.Characters.Character character)
        {
            if (character.isNewCharacter)
            {
                if (character.isDeletedCharacter)
                    return;

                CreateCharacter(character);
                return;
            }

            if (character.isDeletedCharacter)
            {
                DeleteCharacter(character.Name);
                return;
            }

            lock (DatabaseProvider.ConnectionLocker)
            {
                var sqlText = "UPDATE dyn_characters SET id=@id, name=@name, level=@level, class=@class, sex=@sex," +
                    " color=@color, color2=@color2, color3=@color3, mappos=@mapinfos, stats=@stats, items=@items, spells=@spells, experience=@exp, faction=@faction WHERE id=@id";
                var sqlCommand = new MySqlCommand(sqlText, DatabaseProvider.Connection);

                var P = sqlCommand.Parameters;
                P.Add(new MySqlParameter("@id", character.ID));
                P.Add(new MySqlParameter("@name", character.Name));
                P.Add(new MySqlParameter("@level", character.Level));
                P.Add(new MySqlParameter("@class", character.Class));
                P.Add(new MySqlParameter("@sex", character.Sex));
                P.Add(new MySqlParameter("@color", character.Color));
                P.Add(new MySqlParameter("@color2", character.Color2));
                P.Add(new MySqlParameter("@color3", character.Color3));
                P.Add(new MySqlParameter("@mapinfos", character.MapID + "," + character.MapCell + "," + character.Dir));
                P.Add(new MySqlParameter("@stats", character.SqlStats()));
                P.Add(new MySqlParameter("@items", character.GetItemsToSave()));
                P.Add(new MySqlParameter("@spells", character.SpellsInventary.SaveSpells()));
                P.Add(new MySqlParameter("@exp", character.Exp));
                P.Add(new MySqlParameter("@faction", string.Concat(character.Faction.ID, ";", character.Faction.Honor, ";", character.Faction.Deshonor)));

                sqlCommand.ExecuteNonQuery();
            }
        }

        public static void DeleteCharacter(string name)
        {
            lock (DatabaseProvider.ConnectionLocker)
            {
                var sqlText = "DELETE FROM dyn_characters WHERE name=@CharName";
                var sqlCommand = new MySqlCommand(sqlText, DatabaseProvider.Connection);

                sqlCommand.Parameters.Add(new MySqlParameter("@CharName", name));

                sqlCommand.ExecuteNonQuery();
            }
        }
    }
}
