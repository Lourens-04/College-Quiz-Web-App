using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizWebApp.Models
{
    public class TestMemo
    {
        //Declareing Question information variables that will be set in a list to display a test to a student
        //--------------------------------------------------------------------------------------------
        string question;
        string qA;
        string qB;
        string qC;
        string qAns;
        string qMark;
        string tTitle;
        string tTotal;
        string tDesc;
        //--------------------------------------------------------------------------------------------

        //setting the questions details from another class to the variables in this class
        //--------------------------------------------------------------------------------------------
        public TestMemo(string question, string qA, string qB, string qC, string qAns, string qMark, string tTitle, string tDesc, string tTotal)
        {
            this.question = question;
            this.qA = qA;
            this.qB = qB;
            this.qC = qC;
            this.qAns = qAns;
            this.qMark = qMark;
            this.tTitle = tTitle;
            this.tDesc = tDesc;
            this.tTotal = tTotal;
        }
        //--------------------------------------------------------------------------------------------

        //Getters and setters for the question details so they can be called in another 
        //--------------------------------------------------------------------------------------------
        public string Question { get => question; set => question = value; }
        public string QA { get => qA; set => qA = value; }
        public string QB { get => qB; set => qB = value; }
        public string QC { get => qC; set => qC = value; }
        public string QAns { get => qAns; set => qAns = value; }
        public string QMark { get => qMark; set => qMark = value; }
        public string TTotal { get => tTotal; set => tTotal = value; }
        public string TTitle { get => tTitle; set => tTitle = value; }
        public string TDesc { get => tDesc; set => tDesc = value; }
        //--------------------------------------------------------------------------------------------
    }
}