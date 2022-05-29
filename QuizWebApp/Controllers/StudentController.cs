using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuizWebApp.Models;
using System.Collections;
using System.Data;
using System.Data.Entity;
using System.Net;

namespace QuizWebApp.Controllers
{
    public class StudentController : Controller
    {
        //Calling in the model of the database
        private SchoolApplicationAppEntities db = new SchoolApplicationAppEntities();

        // GET: Student
        public ActionResult Index()
        {
            return View();
        }

        //ActionResult for Student test Results 
        //--------------------------------------------------------------------------
        public ActionResult AllTests()
        {
            //Used if the view is opened and a user is not loged in
            if (Session["User"] == null)
            {
                //Used if the view is opened and a user is not loged in
                //-------------------------------------------------------------
                Session["loginck"] = "failed";
                return RedirectToAction("LogIn", "Verification");
            }
            else
            {
                //gets curent user loged in
                int currentUser = Convert.ToInt32(Session["User"].ToString());

                //linq to get all the information of a test a student has taken
                var TestCompleted = db.Results.Include(c => c.Test.Module).Include(c => c.User);

                //returns view with TestCompleted list
                return View(TestCompleted.ToList());
            }
        }

        public ActionResult TestMemo(int? id)
        {
            //Used if the view is opened and a user is not loged in
            if (Session["User"] == null)
            {
                //Used if the view is opened and a user is not loged in
                //-------------------------------------------------------------
                Session["loginck"] = "failed";
                return RedirectToAction("LogIn", "Verification");
            }
            else
            {
                //gets curent user loged in
                int currentUser = Convert.ToInt32(Session["User"].ToString());

                //lit that will be used to store the memo
                List<string> memo = new List<string>();

                //linQ to get the entire memo of a test
                var getMemo = from u in db.Users
                           join r in db.Results
                           on u.User_ID equals r.User_ID
                           join t in db.Tests
                           on r.Test_ID equals t.Test_ID
                           join uc in db.User_Course
                           on u.User_ID equals uc.User_ID
                           join c in db.Courses
                           on uc.Course_ID equals c.Course_ID
                           join cm in db.Course_Module
                           on c.Course_ID equals cm.Course_ID
                           join m in db.Modules
                           on cm.Module_ID equals m.Module_ID
                           where u.User_ID == currentUser
                           where t.Test_ID == id
                           where m.Module_ID == t.Module_ID
                           select new { t.Test_ID, t.TestTitle, t.TestDesc, t.TestTotal, c.CourseCode, m.ModuleCode, r.UserMark, r.UserAns };

                //declaring variables for caculating marks and to get certian data
                //-------------------------
                int i = 0;
                int mark = 0;
                //-------------------------

                foreach (var m in getMemo)
                {
                    string[] desc = m.TestDesc.Split(',');
                    //adds the test table details first
                    memo.Add(m.TestTitle + "&" + desc[0] + "&" + m.CourseCode + "&" + m.ModuleCode + "&" + m.UserMark + "&" + m.TestTotal);

                    int testID = Convert.ToInt32(id);

                    //foreach loop that will loop trough all the questions
                    foreach (var q in DisplayTest(testID))
                    {
                        //splits the user ansers to give marks and for display
                        string[] ans = m.UserAns.Split(' ');

                        //if statement to see if the user answer is the same as the question answer
                        if (q.QAns == ans[i])
                        {
                            //gives the mark if the user is correct
                            mark = Convert.ToInt32(q.QMark);
                        }
                        else
                        {
                            //gives the user 0 if the user did not get it correct
                            mark = 0;
                        }

                        //adds the question table details as well as user marks
                        memo.Add((i + 1) + "&" + q.Question + "&" + q.QA + "&" + q.QB + "&" + q.QC + "&" + q.QAns + "&" + ans[i] + "&" + q.QMark + "&" + mark);
                        //increments the variable i
                        i++;
                    }
                    break;
                }

                //return view
                return View(memo);
            }
        }
        //--------------------------------------------------------------------------

        public List<TestMemo> DisplayTest(int test)
        {
            List<TestMemo> questions = new List<TestMemo>();
            //foreach that will loop through all the test
            foreach (var t in db.Tests)
            {
                //if statement to see if test id equels test id the user chose
                if (t.Test_ID == test)
                {
                    //foreach to loop trough all the questions
                    foreach (var q in db.Questions)
                    {
                        //if statement to see if test id equels test id the user chose
                        if (q.Test_ID.Equals(t.Test_ID))
                        {
                            //adds the details to the list
                            questions.Add(new TestMemo(q.TestQuestion.ToString(), q.QA.ToString(), q.QB.ToString(), q.QC.ToString(), q.QAns.ToString(), q.QMark.ToString(), t.TestTitle.ToString(), t.TestDesc.ToString(), t.TestTotal.ToString()));
                        }
                    }
                }
            }
            //returns the list
            return questions;
        }
    }
}