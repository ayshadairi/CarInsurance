using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CarInsurance.Models;

namespace CarInsurance.Controllers
{
    public class InsurreeController : Controller
    {
        private InsuranceEntities db = new InsuranceEntities();

        // GET: Insurree
        public ActionResult Index()
        {
            return View(db.Insurances.ToList());
        }

        // GET: Insurree/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insurance insurance = db.Insurances.Find(id);
            if (insurance == null)
            {
                return HttpNotFound();
            }
            return View(insurance);
        }

        // GET: Insurree/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Insurree/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,DUI,SpeedingTickets,CoverageType,Quote")] Insurance insurance)
        {
            if (ModelState.IsValid)
            {
                decimal quote = 50; // start with the base of %50/month

                //Age calculation
                int age = DateTime.Now.Year - insurance.DateOfBirth.Year;
                if (insurance.DateOfBirth > DateTime.Now.AddYears(-age))
                    age--;

                if (age <= 18)
                    quote += 100;
                else if (age >= 19 && age <= 25)
                    quote += 50;
                else
                    quote += 25;

                //car year calculations
                if (insurance.CarYear < 2000)
                    quote += 25;
                else if (insurance.CarYear > 2015)
                    quote += 25;

                //Car make calculations
                if (insurance.CarMake.ToLower() == "Porsche")
                {
                    quote += 25;
                    if (insurance.CarMake.ToLower() == "911 carrera")
                        quote += 25;
                }

                //Speeding tickets calculations
                quote += insurance.SpeedingTickets * 10;

                // DUI calculation
                if (insurance.DUI)
                    quote += quote * 0.25m;

                //Coverage type calculation
                if (insurance.CoverageType.ToString() == "Full")
                {
                    quote += quote * 0.5m;
                }

                insurance.Quote = quote;

                db.Insurances.Add(insurance);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(insurance);
        }

        public ActionResult Admin()
        {
            var quote = db.Insurances.ToList();
            return View(quote);
        }

        // GET: Insurree/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insurance insurance = db.Insurances.Find(id);
            if (insurance == null)
            {
                return HttpNotFound();
            }
            return View(insurance);
        }

        // POST: Insurree/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,DUI,SpeedingTickets,CoverageType,Quote")] Insurance insurance)
        {
            if (ModelState.IsValid)
            {
                db.Entry(insurance).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(insurance);
        }

        // GET: Insurree/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insurance insurance = db.Insurances.Find(id);
            if (insurance == null)
            {
                return HttpNotFound();
            }
            return View(insurance);
        }

        // POST: Insurree/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Insurance insurance = db.Insurances.Find(id);
            db.Insurances.Remove(insurance);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
