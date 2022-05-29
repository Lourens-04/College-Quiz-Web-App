using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizWebApp.Models
{
    public class QuestionLayout
    {
        //Declaring variables that will be used to set questions a lecture creates
        //------------------------------------------------------------------------
        string question;
        string qA;
        string qB;
        string qC;
        string qAns;
        int qMark;
        //------------------------------------------------------------------------

        //Setting the data that is comming from the create a test window to the variables in this class
        //------------------------------------------------------------------------
        public QuestionLayout(string question, string qA, string qB, string qC, string qAns, int qMark)
        {
            this.question = question;
            this.qA = qA;
            this.qB = qB;
            this.qC = qC;
            this.qAns = qAns;
            this.qMark = qMark;
        }
        //------------------------------------------------------------------------

        //Geeters and setters to set and get the variables in this class in other windows
        //------------------------------------------------------------------------
        public string Question { get => question; set => question = value; }
        public string QA { get => qA; set => qA = value; }
        public string QB { get => qB; set => qB = value; }
        public string QC { get => qC; set => qC = value; }
        public string QAns { get => qAns; set => qAns = value; }
        public int QMark { get => qMark; set => qMark = value; }
        //------------------------------------------------------------------------
    }
}