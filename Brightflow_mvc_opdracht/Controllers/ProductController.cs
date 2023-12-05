using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Brightflow_mvc_opdracht.Models;

namespace Brightflow_mvc_opdracht.Controllers
{
    public class ProductController : Controller
    {
        private DBEntities db = new DBEntities();

        public ActionResult Products()
        {
            return View(db.Product.ToList());
        }

        // GET: Product
        public ActionResult Index(string SearchString)
        {
            List<Product> Products = new List<Product>(); 

            if (String.IsNullOrEmpty(SearchString)) 
                 Products = (from P in db.Product // als searchstring leeg is pak dan alles
                             select P).ToList();
            else {
                Products = (from P in db.Product 
                    where P.Product1.Contains(SearchString) select P).ToList();
                if (Products.Count == 0)
                    ModelState.AddModelError("","Er zijn geen producten gevonden met de naam: " + SearchString); // als er niks in products zit na het zoeken krijgt de user een error
            }

            return View(Products);
        }

        // GET: Product/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Product product = db.Product.Find(id);
            if (product == null)
                return HttpNotFound();

            return View(product);
        }

        // GET: Product/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductId,Product1,ImagePath,Price,Details,Stock")] Product product)
        {
            if (ModelState.IsValid) {

                if(product.Stock == null)
                    product.Stock = 0;
                if(product.ImagePath == null)
                    product.ImagePath = ""; // als er geen image is dan wordt deze in de database gezet. ik kon er ook een stock image in zetten maar in de test db is niet veel ruimte dus ik heb de character list laag gezet.
                

                db.Product.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(product);
        }

        // GET: Product/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            Product product = db.Product.Find(id);
            if (product == null)
                return HttpNotFound();

            return View(product);
        }

        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductId,Product1,ImagePath,Price,Details,Stock")] Product product)
        {
            if (ModelState.IsValid)
            {
                if (product.ImagePath == null)
                    product.ImagePath = "";

                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(product);
        }

        // GET: Product/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            Product product = db.Product.Find(id);
            if (product == null)
                return HttpNotFound();

            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Product.Find(id);
            db.Product.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}
