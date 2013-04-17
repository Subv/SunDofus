using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace SunDofus.World.Entities.Cache
{
    class CharactersCache
    {
        public static void LoadCharacters()
        {
            lock (DatabaseHandler.ConnectionLocker)
            {
                var sqlText = "SELECT * FROM dyn_characters";
                var sqlCommand = new MySqlCommand(sqlText, DatabaseHandler.Connection);

                var sqlResult = sqlCommand.ExecuteReader();

                while (sqlResult.Read())
                {
                    var character = new Realm.Characters.Character();

                    character.ID = sqlResult.GetInt16("id");
                    character.Name = sqlResult.GetString("name");
                    character.Level = sqlResult.GetInt16("level");
                    character.Class = sqlResult.GetInt16("class");
                    character.Sex = sqlResult.GetInt16("sex");
                    character.Skin = int.Parse(character.Class + "" + character.Sex);
                    character.Size = 100;
                    character.Color = sqlResult.GetInt32("color");
                    character.Color2 = sqlResult.GetInt32("color2");
                    character.Color3 = sqlResult.GetInt32("color3");

                    character.MapCell = int.Parse(sqlResult.GetString("mappos").Split(',')[1]);
                    character.MapID = int.Parse(sqlResult.GetString("mappos").Split(',')[0]);
                    character.Dir = int.Parse(sqlResult.GetString("mappos").Split(',')[2]);

                    character.Exp = sqlResult.GetInt64("experience");

                    character.ParseStats(sqlResult.GetString("stats"));

                    if (sqlResult.GetString("items") != "") 
                        character.ItemsInventary.ParseItems(sqlResult.GetString("items"));

                    if (sqlResult.GetString("spells") != "") 
                        character.SpellsInventary.ParseSpells(sqlResult.GetString("spells"));

                    character.isNewCharacter = false;

                    lock(Realm.Characters.CharactersManager.CharactersList)
                        Realm.Characters.CharactersManager.CharactersList.Add(character);
                }

                sqlResult.Close();
            }

            Utilities.Loggers.StatusLogger.Write(string.Format("Loaded @'{0}' characters@ from the database !",Realm.Characters.CharactersManager.CharactersList.Count));
        }

        public static void CreateCharacter(Realm.Characters.Character character)
        {
            lock (DatabaseHandler.ConnectionLocker)
            {
                var sqlText = "INSERT INTO dyn_characters VALUES(@id, @name, @level, @class, @sex, @color, @color2, @color3, @mapinfos, @stats, @items, @spells, @exp)";
                var sqlCommand = new MySqlCommand(sqlText, DatabaseHandler.Connection);

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
                P.Add(new MySqlParameter("@items", ""));
                P.Add(new MySqlParameter("@spells", ""));
                P.Add(new MySqlParameter("@exp", "0"));

                sqlCommand.ExecuteNonQuery();

                character.isNewCharacter = false;
            }
        }

        public static void SaveCharacter(Realm.Characters.Character character)
        {
            lock (DatabaseHandler.ConnectionLocker)
            {
                var sqlText = "UPDATE dyn_characters SET id=@id, name=@name, level=@level, class=@class, sex=@sex," +
                    " color=@color, color2=@color2, color3=@color3, mappos=@mapinfos, stats=@stats, items=@items, spells=@spells, experience=@exp WHERE id=@id";
                var sqlCommand = new MySqlCommand(sqlText, DatabaseHandler.Connection);

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

                sqlCommand.ExecuteNonQuery();
            }
        }

        public static void DeleteCharacter(string name)
        {
            lock (DatabaseHandler.ConnectionLocker)
            {
                var sqlText = "DELETE FROM dyn_characters WHERE name=@CharName";
                var sqlCommand = new MySqlCommand(sqlText, DatabaseHandler.Connection);

                sqlCommand.Parameters.Add(new MySqlParameter("@CharName", name));

                sqlCommand.ExecuteNonQuery();
            }
        }

        private static int _lastID = -1;

        public static int GetNewID()
        {
            lock (DatabaseHandler.ConnectionLocker)
            {
                if (_lastID == -1)
                {
                    var sqlText = "SELECT id FROM dyn_characters ORDER BY id DESC LIMIT 0,1";
                    var sqlCommand = new MySqlCommand(sqlText, DatabaseHandler.Connection);

                    var sqlResult = sqlCommand.ExecuteReader();

                    _lastID = 0;

                    if (sqlResult.Read())
                    {
                        _lastID = sqlResult.GetInt32("id");
                    }

                    sqlResult.Close();

                    return ++_lastID;
                }
                else
                {
                    return ++_lastID;
                }
            }
        }
    }
}
