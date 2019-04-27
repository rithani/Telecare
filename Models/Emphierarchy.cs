using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SrEngineerCodingExercise.Models
{
    public class Emphierarchy
    {
        public List<Emphierarchy> Reportees { get; set; }
        public string NameAndTitle { get; set; }

        //[System.Xml.Serialization.XmlIgnore]
         int Level { get; set; }

        public void SetLevel(int level)
        {
            Level = level;
        }

        public int GetLevel()
        {
            return Level;
        }
    }
}