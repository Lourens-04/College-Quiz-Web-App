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
    public class LectureController : Controller
    {
        //Calling in the model of the database
        private SchoolApplicationAppEntities db = new SchoolApplicationAppEntities();
        //list that will store all the details of a test that will be used for editing or creating
        static List<string> testDetails = new List<string>();
        //list that will store all the questions that will be used for editing or creating
        static List<QuestionLayout> questions = new List<QuestionLayout>();
        //list that will store questions id that will be used in a methods 
        static List<int> editQuestionIDs = new List<int>();
        //list that used to store questions options answers
        static List<string> answerOptions = new List<string>();
        //int variable to store the current question
        static int currentQuestion = 0;

        // GET: Lecture
        public ActionResult Index()
        {
            return View();
        }

        //ActionResult for Student test Results 
        //--------------------------------------------------------------------------
        public ActionResult AllTestCreated()
        {
            //if statement to see if the seesion variable is equel to null
            if (Session["User"] == null)
            {
                //Used if the view is opened and a user is not loged in
                //-------------------------------------------------------------
                Session["loginck"] = "failed";
                return RedirectToAction("LogIn", "Verification");
            }
            else
            {
                //if statement to see that the questions list does not equel to null and editQuestionIDs list equels to null
                if (questions.Count != 0 && editQuestionIDs.Count == 0)
                {
                    //variable for test total
                    int saveTestTotal = 0;

                    //foreach to get all the marks for the test to get test total
                    foreach (var item in questions)
                    {
                        saveTestTotal = saveTestTotal + item.QMark;
                    }

                    //declaring varable to set a new test
                    var addSettest = db.Set<Test>();

                    //Adds the sets to the test in the model
                    addSettest.Add(new Test { TestTitle = testDetails[2], TestDesc = (testDetails[3]), Module_ID = Convert.ToInt32(testDetails[1]), TestTotal = saveTestTotal });

                    //saving changes made
                    db.SaveChanges();

                    //declaring varable to set a new question
                    var addSetques = db.Set<Question>();

                    //foreach to loop through to add all the questions to the questions table in the model
                    foreach (var q in questions)
                    {
                        //Adds the sets to the question in the model
                        addSetques.Add(new Question { TestQuestion = q.Question, QA = q.QA, QB = q.QB, QC = q.QC, QAns = q.QAns, QMark = q.QMark, Test_ID = Convert.ToInt32(testDetails[0])});
                    }

                    //saving changes made
                    db.SaveChanges();
                }

                //if to see if the editQuestionIDs does not equel to null
                if (editQuestionIDs.Count != 0)
                {
                    //variable for test total
                    int saveTestTotal = 0;

                    //foreach to get all the marks for the test to get test total
                    foreach (var item in questions)
                    {
                        saveTestTotal = saveTestTotal + item.QMark;
                    }

                    //varable to set the test id
                    int testID = Convert.ToInt32(testDetails[0]);
                    //getting the test that needs to be edit
                    //-------------------------------------------------------------
                    Test updateTest = db.Tests.Single(x => x.Test_ID == testID);
                    updateTest.TestTitle = testDetails[2];
                    updateTest.TestDesc = testDetails[3];
                    updateTest.Module_ID = Convert.ToInt32(testDetails[1]);
                    updateTest.TestTotal = saveTestTotal;
                    //-------------------------------------------------------------

                    //getting the questions that needs to be edit
                    var updateQuestions = db.Questions.Where(x => x.Test_ID == testID);

                    //declaring an i of type int that will be used to get the ids in the questionIDs list
                    int i = 0;

                    //changing the values in the database of the questions to the new values the lecture enterd
                    //---------------------------------------------------------------------
                    foreach (var updatedQuedtions in questions)
                    {
                        int id = editQuestionIDs[i];
                        Question questionsInDB = db.Questions.Single(x => x.Question_ID == id);
                        questionsInDB.TestQuestion = updatedQuedtions.Question;
                        questionsInDB.QA = updatedQuedtions.QA;
                        questionsInDB.QB = updatedQuedtions.QB;
                        questionsInDB.QC = updatedQuedtions.QC;
                        questionsInDB.QAns = updatedQuedtions.QAns;
                        questionsInDB.QMark = updatedQuedtions.QMark;
                        i++;
                    }
                    //----------------------------------------------------------------------------

                    //saving changes made
                    db.SaveChanges();
                }

                //Clear lists
                //------------------------------
                testDetails.Clear();
                questions.Clear();
                editQuestionIDs.Clear();
                answerOptions.Clear();
                //------------------------------

                //setting varbale back to 0
                currentQuestion = 0;

                //getting the current user loged in to get the correct data to display
                int currentUser = Convert.ToInt32(Session["User"]);
                List<string> testCreated = new List<string>();

                //LinQ statement to get all the test created by the user
                var qryTestCreated = from u in db.Users
                                  join lm in db.Lecture_Module
                                  on u.User_ID equals lm.User_ID
                                  join m in db.Modules
                                  on lm.Module_ID equals m.Module_ID
                                  join t in db.Tests
                                  on m.Module_ID equals t.Module_ID
                                  where u.User_ID == currentUser
                                  select new { t.TestTitle, t.TestDesc, t.Test_ID, t.TestTotal, m.ModuleCode };

                //foreach to put all the data into a list
                //-------------------------------------------------------------------
                foreach (var item in qryTestCreated)
                {
                    testCreated.Add(item.Test_ID + "&" + item.TestTitle + "&" + item.TestDesc + "&" + item.TestTotal + "&" + item.ModuleCode);
                }
                //-------------------------------------------------------------------

                //return the view with the list
                return View(testCreated.ToList());
            }
        }
        //--------------------------------------------------------------------------

        public ActionResult CreateTest()
        {
            //if statement to see if the seesion variable is equel to null
            if (Session["User"] == null)
            {
                //Used if the view is opened and a user is not loged in
                //-------------------------------------------------------------
                Session["loginck"] = "failed";
                return RedirectToAction("LogIn", "Verification");
            }
            else
            {
                //getting the current user loged in to get the correct data to display
                int currentUser = Convert.ToInt32(Session["User"]);

                //LinQ statement to get all the test created by the user
                var allLectureModules = from u in db.Users
                                        join lm in db.Lecture_Module
                                        on u.User_ID equals lm.User_ID
                                        join m in db.Modules
                                        on lm.Module_ID equals m.Module_ID
                                        where u.User_ID == currentUser
                                        select m;
                
                //Viewbag to store to get all the modules the user is linked to
                ViewBag.Module_ID = new SelectList(allLectureModules, "Module_ID", "ModuleCode");

                //return view
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateTest([Bind(Include = "Test_ID,Module_ID,TestTitle,TestDesc")] Test test, string Desc)
        {
            //if statement to see if the seesion variable is equel to null
            if (Session["User"] == null)
            {
                //Used if the view is opened and a user is not loged in
                //-------------------------------------------------------------
                Session["loginck"] = "failed";
                return RedirectToAction("LogIn", "Verification");
            }
            else
            {
                //checks if module state is valid
                if (ModelState.IsValid)
                {
                    //varable for test id
                    int testId = 0;

                    //foreach to get the new test id
                    //-------------------------------------------------------------
                    foreach (var item in db.Tests)
                    {
                        testId = (item.Test_ID + 1);
                    }
                    //-------------------------------------------------------------

                    //replacing any , in the description with ;
                    Desc.Replace(',', ';');

                    //clearing the list
                    testDetails.Clear();

                    //adding all the valuest to the testDetails list
                    //------------------------------------------------
                    testDetails.Add(testId.ToString());
                    testDetails.Add(test.Module_ID.ToString());
                    testDetails.Add(test.TestTitle.ToString());
                    testDetails.Add(Desc + ",publish");
                    //------------------------------------------------
                    //save chages
                    db.Entry(test).State = EntityState.Unchanged;
                    db.SaveChanges();
                    //Takes the user to enter in the questions for a test
                    return RedirectToAction("CreateTestQuestions");
                }

                //getting the current user loged in to get the correct data to display
                int currentUser = Convert.ToInt32(Session["User"]);

                //LinQ statement to get all the modules the user is linked to
                var allLectureModules = from u in db.Users
                                        join lm in db.Lecture_Module
                                        on u.User_ID equals lm.User_ID
                                        join m in db.Modules
                                        on lm.Module_ID equals m.Module_ID
                                        where u.User_ID == currentUser
                                        select m;

                //Viewbag to store to get all the modules the user is linked to
                ViewBag.Module_ID = new SelectList(allLectureModules, "Module_ID", "ModuleCode");

                //return view
                return View(test);
            }
        }

        public ActionResult CreateTestQuestions()
        {
            //if statement to see if the seesion variable is equel to null
            if (Session["User"] == null)
            {
                //Used if the view is opened and a user is not loged in
                //-------------------------------------------------------------
                Session["loginck"] = "failed";
                return RedirectToAction("LogIn", "Verification");
            }
            else
            {
                //adding options to a list
                //------------------------------
                answerOptions.Clear();
                answerOptions.Add("A");
                answerOptions.Add("B");
                answerOptions.Add("C");
                //------------------------------

                //Viewbag to store all the options for a correct answer
                ViewBag.QAns = new SelectList(answerOptions, "QAns");

                //return View
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateTestQuestions([Bind(Include = "Question_ID,Test_ID,TestQuestion,QA,QB,QC,QAns,QMark")] Question question)
        {
            //if statement to see if the seesion variable is equel to null
            if (Session["User"] == null)
            {
                //Used if the view is opened and a user is not loged in
                //-------------------------------------------------------------
                Session["loginck"] = "failed";
                return RedirectToAction("LogIn", "Verification");
            }
            else
            {
                //checks if module state is valid
                if (ModelState.IsValid)
                {
                    //add question to the list
                    questions.Add(new QuestionLayout(question.TestQuestion.ToString(), question.QA.ToString(), question.QB.ToString(), question.QC.ToString(), question.QAns.ToString(), question.QMark));

                    //save chages
                    db.Entry(question).State = EntityState.Unchanged;
                    db.SaveChanges();

                    //redirect user to the CreateTestQuestions view
                    return RedirectToAction("CreateTestQuestions");
                }

                //adding options to a list
                //------------------------------
                answerOptions.Clear();
                answerOptions.Add("A");
                answerOptions.Add("B");
                answerOptions.Add("C");
                //------------------------------

                //Viewbag to store all the options for a correct answer
                ViewBag.QAns = new SelectList(answerOptions, "QAns");

                //return View with question
                return View(question);
            }
        }

        public ActionResult EditTest(int? id)
        {
            //if statement to see if the seesion variable is equel to null
            if (Session["User"] == null)
            {
                //Used if the view is opened and a user is not loged in
                //-------------------------------------------------------------
                Session["loginck"] = "failed";
                return RedirectToAction("LogIn", "Verification");
            }
            else
            {
                //if to see if the id equels to null
                if (id == null)
                {
                    //returns bad request
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                //find the test the user wants to edit
                Test test = db.Tests.Find(id);

                //checks if there is no data
                if (test == null)
                {
                    //returns not found 
                    return HttpNotFound();
                }

                //getting the current user loged in to get the correct data to display
                int currentUser = Convert.ToInt32(Session["User"]);

                //LinQ statement to get all the modules the user is linked to
                var allLectureModules = from u in db.Users
                                        join lm in db.Lecture_Module
                                        on u.User_ID equals lm.User_ID
                                        join m in db.Modules
                                        on lm.Module_ID equals m.Module_ID
                                        where u.User_ID == currentUser
                                        select m;

                //Viewbag to store to get all the modules the user is linked to
                ViewBag.Module_ID = new SelectList(allLectureModules, "Module_ID", "ModuleCode", test.Module_ID);

                //return view with varabale test
                return View(test);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditTest([Bind(Include = "Test_ID,Module_ID,TestTitle,TestDesc,TestTotal")] Test test, string Desc)
        {
            //if statement to see if the seesion variable is equel to null
            if (Session["User"] == null)
            {
                //Used if the view is opened and a user is not loged in
                //-------------------------------------------------------------
                Session["loginck"] = "failed";
                return RedirectToAction("LogIn", "Verification");
            }
            else
            {
                //checks if module state is valid
                if (ModelState.IsValid)
                {
                    Desc.Replace(',', ';' );

                    //adding all the valuest to the testDetails list
                    //------------------------------------------------
                    testDetails.Clear();
                    testDetails.Add(test.Test_ID.ToString());
                    testDetails.Add(test.Module_ID.ToString());
                    testDetails.Add(test.TestTitle.ToString());
                    testDetails.Add(Desc + ",publish");
                    //------------------------------------------------
                    
                    //takes the user to edit questions
                    return RedirectToAction("EditTestQuestions", new { id = test.Test_ID });
                }

                //getting the current user loged in to get the correct data to display
                int currentUser = Convert.ToInt32(Session["User"]);

                //LinQ statement to get all the modules the user is linked to
                var allLectureModules = from u in db.Users
                                        join lm in db.Lecture_Module
                                        on u.User_ID equals lm.User_ID
                                        join m in db.Modules
                                        on lm.Module_ID equals m.Module_ID
                                        where u.User_ID == currentUser
                                        select m;

                ////Viewbag to store to get all the modules the user is linked to
                ViewBag.Module_ID = new SelectList(allLectureModules, "Module_ID", "ModuleCode", test.Module_ID);

                //return view with varabale test
                return View(test);
            }
        }

        public ActionResult EditTestQuestions(int? id)
        {
            //if statement to see if the seesion variable is equel to null
            if (Session["User"] == null)
            {
                //Used if the view is opened and a user is not loged in
                //-------------------------------------------------------------
                Session["loginck"] = "failed";
                return RedirectToAction("LogIn", "Verification");
            }
            else
            {
                //foreach to get question ids that needs to be edited
                //------------------------------------------------
                foreach (var item in db.Questions)
                {
                    if (item.Test_ID == id)
                    {
                        editQuestionIDs.Add(item.Question_ID);
                    }

                }
                //------------------------------------------------

                //checks if id is equel to null
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                //gets the question that will be displayed
                Question question = db.Questions.Find(editQuestionIDs[currentQuestion]);

                //checks that editQuestionIDs list does not equel to 1
                if (editQuestionIDs.Count() != 1)
                {
                    //plus the int value with 1
                    currentQuestion++;
                }

                //checks that there is a question
                if (question == null)
                {
                    return HttpNotFound();
                }

                //adding options to a list
                //------------------------------
                answerOptions.Clear();
                answerOptions.Add("A");
                answerOptions.Add("B");
                answerOptions.Add("C");
                //------------------------------

                //Viewbag to store all the options for a correct answer
                ViewBag.QAns = new SelectList(answerOptions, "QAns");

                //return view with question
                return View(question);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditTestQuestions([Bind(Include = "Question_ID,Test_ID,TestQuestion,QA,QB,QC,QAns,QMark")] Question question)
        {
            //if statement to see if the seesion variable is equel to null
            if (Session["User"] == null)
            {
                //Used if the view is opened and a user is not loged in
                //-------------------------------------------------------------
                Session["loginck"] = "failed";
                return RedirectToAction("LogIn", "Verification");
            }
            else
            {
                //checks if module state is valid
                if (ModelState.IsValid)
                {
                    //checks that the list total is grater or equel to the currentQuestion value
                    if (editQuestionIDs.Count >= (currentQuestion))
                    {
                        //add question to the list
                        questions.Add(new QuestionLayout(question.TestQuestion.ToString(), question.QA.ToString(), question.QB.ToString(), question.QC.ToString(), question.QAns.ToString(), question.QMark));

                        //checks that the list total is grater to the currentQuestion value
                        if (editQuestionIDs.Count > (currentQuestion))
                        {
                            //takes the user to the next question
                            return RedirectToAction("EditTestQuestions", new { id = currentQuestion });
                        }
                        else
                        {
                            //takes the user back to all test created 
                            return RedirectToAction("AllTestCreated");
                        }
                    }
                }

                //return view with question
                return View(question);
            }
        }
    }
}