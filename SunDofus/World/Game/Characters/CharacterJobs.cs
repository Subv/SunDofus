using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Game.Characters
{
    class CharacterJobs
    {
        public List<Jobs.Job> Jobs;
        private Character Client;
        public JobOptions Options;

        public CharacterJobs(Character client)
        {
            Client = client;
            Options = new JobOptions(0, 0, 0);
            Jobs = new List<Jobs.Job>();
        }

        public void Parse(string data)
        {
            foreach (var j in data.Split('|'))
            {
                var info = data.Split(';');
                var job = new Jobs.Job(int.Parse(info[0]), int.Parse(info[1]), int.Parse(info[2]));
                job.ParseSkills(info[3]);
                Jobs.Add(job);
            }
        }

        public string Save()
        {
            string data = "";
            foreach (var job in Jobs)
                data += string.Format("{0}|{1};{2};{3};{4}", data, job.Id, job.Level, job.Experience, job.SaveSkills());
            return data.Length > 0 ? data.Substring(1) : "";
        }

        public void AddNewJob(int id)
        {
            if (Jobs.Any(x => x.Id == id))
                return;
            Jobs.Add(new Jobs.Job(id, 1, 0));
            SendJobs();
            SendJobsXP();
            SendJobOptions();
        }

        public void SendJobOptions()
        {
            Client.NetworkClient.Send(string.Format("JO{0}", Options.ToString()));
        }

        public void SendJobsXP()
        {
            string data = string.Empty;
            foreach (var job in Jobs)
                data = string.Format("{0}|{1};{2};{3};{4};{5}",data, job.Id, job.Level, job.GetMinExperience(), job.Experience, job.GetMaxExperience());
            
            Client.NetworkClient.Send(string.Format("JX{0}", data));
        }

        public void SendJobs()
        {
            string data = string.Empty;
            foreach (var job in Jobs)
                data = string.Format("{0}|{1};{2}",data, job.Id, job.GetSkills());

            Client.NetworkClient.Send(String.Format("JS{0}", data));
        }

        public bool HasJob(int id)
        {
            return Jobs.Any(x => x.Id == id);
        }
    }

    class JobOptions
    {
        public JobOptionParams Params;
        public int MinSlots;
        public int MaxSlots;

        public JobOptions(JobOptionParams p, int min, int max)
        {
            Params = p;
            MinSlots = min;
            MaxSlots = max;
        }

        public override string ToString()
        {
            return string.Format("{0}|{1}|{2}", (int)Params, MinSlots, MaxSlots);
        }
    }

    enum JobOptionParams
    {
        PaidService = 1,
        FreeIfFailed = 2,
        ResourcesNeeded = 4
    }
}
