using System.Xml.Serialization;
using System.Collections.Generic;
using Standings.Data.Models;
using System;

namespace Standings.Parser.XmlModels
{

    [XmlRoot(ElementName = "problem")]
    public class Problem
    {
        [XmlAttribute(AttributeName = "alias")]
        public string Alias { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlElement(ElementName = "run")]
        public List<Run> Runs { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "solved")]
        public string Solved { get; set; }
        [XmlAttribute(AttributeName = "penalty")]
        public string Penalty { get; set; }
        [XmlAttribute(AttributeName = "time")]
        public string Time { get; set; }
        [XmlAttribute(AttributeName = "accepted")]
        public string Accepted { get; set; }
        [XmlAttribute(AttributeName = "attempts")]
        public string Attempts { get; set; }
        [XmlAttribute(AttributeName = "score")]
        public string Score { get; set; }
    }

    [XmlRoot(ElementName = "challenge")]
    public class Challenge
    {
        [XmlElement(ElementName = "problem")]
        public List<Problem> Problems { get; set; }
    }

    [XmlRoot(ElementName = "run")]
    public class Run
    {
        [XmlAttribute(AttributeName = "accepted")]
        public string Accepted { get; set; }
        [XmlAttribute(AttributeName = "time")]
        public string Time { get; set; }
    }

    [XmlRoot(ElementName = "session")]
    public class Session
    {
        [XmlElement(ElementName = "problem")]
        public List<Problem> Problems { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "party")]
        public string Party { get; set; }
        [XmlAttribute(AttributeName = "alias")]
        public string Alias { get; set; }
        [XmlAttribute(AttributeName = "solved")]
        public string Solved { get; set; }
        [XmlAttribute(AttributeName = "penalty")]
        public string Penalty { get; set; }
        [XmlAttribute(AttributeName = "time")]
        public string Time { get; set; }
        [XmlAttribute(AttributeName = "accepted")]
        public string Accepted { get; set; }
        [XmlAttribute(AttributeName = "attempts")]
        public string Attempts { get; set; }
        [XmlAttribute(AttributeName = "score")]
        public string Score { get; set; }
    }

    [XmlRoot(ElementName = "contest")]
    public class Contest
    {
        [XmlElement(ElementName = "challenge")]
        public Challenge Challenge { get; set; }
        [XmlElement(ElementName = "session")]
        public List<Session> Sessions { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "time")]
        public string Time { get; set; }
        [XmlAttribute(AttributeName = "length")]
        public string Length { get; set; }
        [XmlAttribute(AttributeName = "status")]
        public string Status { get; set; }
        [XmlAttribute(AttributeName = "frozen")]
        public string Frozen { get; set; }
    }

    [XmlRoot(ElementName = "standings")]
    public class Standing
    {
        [XmlElement(ElementName = "contest")]
        public Contest Contest { get; set; }
    }

}