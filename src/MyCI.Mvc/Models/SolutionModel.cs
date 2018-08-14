using System;
using System.Collections.Generic;

namespace MyCI.Mvc.Models
{
    public class SolutionModel
    {
        public string Name { get; set; }
        public string SolutionPath { get; set; }
    }

    public class SolutionBuild
    {
        public DateTime StartAt { get; set; }

        public int Version { get; set; }

        public string CurrentStepName { get; set; }

        public int CurrentStep { get; set; }

        public IList<BuildStepHistory> History { get; set; }

        public int Tests { get; set; }
        public int FailedTests { get; set; }
        public int PassedTests { get; set; }


        public DateTime EndAt { get; set; }
    }

    public class BuildStepHistory
    {
        public DateTime CreatedAt { get; set; }
        public int Step { get; set; }

        public string Output { get; set; }
    }
}