using DBProg_A3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DBProg_A3.Controllers
{
    public class StatesController : Controller
    {
        // GET: States
        /// <summary>
        ///     The view to return a list of all states. Table columns are searchable and sortable (descending/ascending).
        ///     Search: Once user enters the item to be searched in the textbox, it will search for that item.
        ///     Sort: Once user clicks a table column, column is sorted.
        /// </summary>
        /// <param name="id">The Search Term</param>
        /// <param name="sortBy">Integer 0 = StateCode, 1 = StateName</param>
        /// <param name="isDesc">The Sort Descending Boolean</param>
        /// <returns>AllStates View</returns>
        public ActionResult AllStates(string id, int sortBy = 0, bool isDesc = false)
        {
            BooksEntities context = new BooksEntities();
            List<State> states;

            switch (sortBy)
            {
                case 1:
                    {
                        if (isDesc)
                            states = context.States.OrderByDescending(s => s.StateName).ToList();
                        else
                            states = context.States.OrderBy(s => s.StateName).ToList();
                        break;
                    }
                case 0:
                default:
                    {
                        if (isDesc)
                            states = context.States.OrderByDescending(s => s.StateCode).ToList();
                        else
                            states = context.States.OrderBy(s => s.StateCode).ToList();
                        break;
                    }
            }

            //id is used as searchTerm param
            //if id is not null or not whiteSpace, trim it
            if (!string.IsNullOrWhiteSpace(id))
            {
                id = id.Trim().ToLower();
                
                // Where is a LINQ. This is to filter search.
                states = states.Where(s =>
                                      s.StateCode.ToLower().Contains(id) ||
                                      s.StateName.ToLower().Contains(id)
                                      ).ToList();
            }

            states = states.Where(s => s.IsDeleted == false).ToList();
            return View(states);
        }

        // GET: State
        /// <summary>
        ///     View to allow the user to upsert a state by retrieving the state first
        /// </summary>
        /// <param name="id">The Search Term</param>
        /// <returns>State View</returns>        

        [HttpGet]
        public ActionResult UpsertState(string id)
        {
            BooksEntities context = new BooksEntities();
            State state = context.States.Where(s => s.StateCode == id).FirstOrDefault();

            if (state == null)
            {
                state = new State();
            }

            return View(state);
        }

        // POST: State
        /// <summary>
        ///     View to allow the user to post a new state
        /// </summary>
        /// <param name="newState">New state to be added</param>
        /// <returns>Redirection View to All States</returns>

        [HttpPost]
        public ActionResult UpsertState(State newState)
        {
            BooksEntities context = new BooksEntities();
            try
            {
                if (context.States.Where(s => s.StateCode == newState.StateCode).Count() > 0)
                {
                    //State already exists
                    var stateToSave = context.States.Where(s =>
                                        s.StateCode == newState.StateCode
                                        ).FirstOrDefault();

                    stateToSave.StateName = newState.StateName;
                }
                else
                {
                    context.States.Add(newState);
                }

                context.SaveChanges();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

            return RedirectToAction("AllStates");
        }

        // DELETE: State
        /// <summary>
        ///      View to allow the user to delete a state.
        ///      State is deleted on the front-end but the record is saved on the back-end (IsDeleted = true).
        /// </summary>
        /// <param name="id">The Search Term</param>
        /// <returns>JSON with three fields: Success Status, Id, and States View</returns>

        [HttpGet]
        public ActionResult Delete(string id)
        {
            // Delete state with id from the database
            BooksEntities context = new BooksEntities();
            
            try
            {
                State state = context.States.Where(s => s.StateCode == id).FirstOrDefault();               
                state.IsDeleted = true;
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Success = false,
                    Id = id,
                    Message = ex.Message
                });
            }
            
            return Json(new
            {
                Success = true,
                Id = id,
                returnUrl = "/States/AllStates"
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
