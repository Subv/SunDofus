﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace SunDofus.World.Entities.Requests
{
    class NoPlayerCharacterRequests
    {
        public static List<Game.Characters.NPC.NPCMap> NpcsList = new List<Game.Characters.NPC.NPCMap>();
        public static List<Models.NPC.NPCsQuestion> QuestionsList = new List<Models.NPC.NPCsQuestion>();
        public static List<Models.NPC.NPCsAnswer> AnswersList = new List<Models.NPC.NPCsAnswer>();

        public static void LoadNPCs()
        {
            lock (DatabaseProvider.Locker)
            {
                var sqlText = "SELECT * FROM npcs";
                var sqlCommand = new MySqlCommand(sqlText, DatabaseProvider.Connection);

                var sqlReader = sqlCommand.ExecuteReader();

                while (sqlReader.Read())
                {
                    var npcModel = new Models.NPC.NoPlayerCharacterModel()
                    {
                        ID = sqlReader.GetInt32("ID"),
                        GfxID = sqlReader.GetInt32("Gfx"),
                        Size = sqlReader.GetInt32("Size"),
                        Sex = sqlReader.GetInt32("Sex"),
                        Color = sqlReader.GetInt32("Color1"),
                        Color2 = sqlReader.GetInt32("Color2"),
                        Color3 = sqlReader.GetInt32("Color3"),

                        Name = sqlReader.GetString("Name"),
                        Items = sqlReader.GetString("Items"),

                        Question = (sqlReader.GetInt32("initQuestion") != -1 ? QuestionsList.First
                            (x => x.QuestionID == sqlReader.GetInt32("initQuestion")) : null),
                    };

                    foreach (var itemToSell in sqlReader.GetString("SellingList").Split(','))
                    {
                        if (itemToSell == "")
                            continue;

                        lock (npcModel.SellingList)
                            npcModel.SellingList.Add(int.Parse(itemToSell));
                    }

                    var infosMap = sqlReader.GetString("Mapinfos").Split(';');

                    var npc = new Game.Characters.NPC.NPCMap(npcModel)
                    {

                        MapID = int.Parse(infosMap[0]),
                        MapCell = int.Parse(infosMap[1]),
                        Dir = int.Parse(infosMap[2]),
                        mustMove = bool.Parse(infosMap[3]),
                    };

                    if (MapsRequests.MapsList.Any(x => x.Model.ID == npc.MapID))
                    {
                        var map = MapsRequests.MapsList.First(x => x.Model.ID == npc.MapID);
                        npc.ID = MapsRequests.MapsList.First(x => x.Model.ID == npc.MapID).NextNpcID();

                        lock (map.Npcs)
                            map.Npcs.Add(npc);

                        npc.StartMove();
                    }

                    lock (NpcsList)
                        NpcsList.Add(npc);
                }

                sqlReader.Close();
            }

            Utilities.Loggers.Status.Write(string.Format("Loaded '{0}' npcs from the database !", NpcsList.Count));
        }

        public static void LoadNPCsQuestions()
        {
            lock (DatabaseProvider.Locker)
            {
                var sqlText = "SELECT * FROM npcs_questions";
                var sqlCommand = new MySqlCommand(sqlText, DatabaseProvider.Connection);

                var sqlReader = sqlCommand.ExecuteReader();

                while (sqlReader.Read())
                {
                    var question = new Models.NPC.NPCsQuestion()
                    {
                        QuestionID = sqlReader.GetInt32("questionID"),
                        RescueQuestionID = sqlReader.GetInt32("rescueQuestion"),
                    };

                    foreach (var answer in sqlReader.GetString("answers").Split(';'))
                    {
                        if (answer == "")
                            continue;

                        lock(question.Answers)
                            question.Answers.Add(AnswersList.First(x => x.AnswerID == int.Parse(answer)));
                    }

                    foreach (var condi in sqlReader.GetString("conditions").Split('&'))
                    {
                        if (condi == "")
                            continue;

                        var condiInfos = condi.Split(';');
                        var condiObject = new Game.World.Conditions.NPCConditions();

                        condiObject.CondiID = int.Parse(condiInfos[0]);
                        condiObject.Args = condiInfos[1];

                        lock(question.Conditions)
                            question.Conditions.Add(condiObject);
                    }

                    question.Params = sqlReader.GetString("params").Split(',').ToList();

                    lock(QuestionsList)
                        QuestionsList.Add(question);
                }

                sqlReader.Close();
            }

            foreach (var question in QuestionsList.Where(x => x.RescueQuestionID != -1))
                question.RescueQuestion = QuestionsList.First(x => x.QuestionID == question.RescueQuestionID);

            Utilities.Loggers.Status.Write(string.Format("Loaded '{0}' npcsQuestions from the database !", QuestionsList.Count));
        }

        public static void LoadNPCsAnswers()
        {
            lock (DatabaseProvider.Locker)
            {
                var sqlText = "SELECT * FROM npcs_answers";
                var sqlCommand = new MySqlCommand(sqlText, DatabaseProvider.Connection);

                var sqlReader = sqlCommand.ExecuteReader();

                while (sqlReader.Read())
                {
                    var answer = new Models.NPC.NPCsAnswer()
                    {
                        AnswerID = sqlReader.GetInt32("answerID"),
                        Effects = sqlReader.GetString("effects"),
                    };

                    foreach (var condi in sqlReader.GetString("conditions").Split('&'))
                    {
                        if (condi == "")
                            continue;

                        var condiInfos = condi.Split(';');
                        var condiObject = new Game.World.Conditions.NPCConditions();

                        condiObject.CondiID = int.Parse(condiInfos[0]);
                        condiObject.Args = condiInfos[1];

                        lock(answer.Conditions)
                            answer.Conditions.Add(condiObject);
                    }

                    lock(AnswersList)
                        AnswersList.Add(answer);
                }

                sqlReader.Close();
            }

            Utilities.Loggers.Status.Write(string.Format("Loaded '{0}' npcsAnswers from the database !", AnswersList.Count));
        }
    }
}
