using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ToDoAPI.DATA.EF;//Added for access to EF layer
using ToDoAPI.API.Models;//Added for access to Data Transfer Objects
using System.Web.Http.Cors;//Added to access and modify cross origin resource sharing

namespace ToDoAPI.API.Controllers
{
    public class CategoriesController : ApiController
    {
        //In the code below we are giving permission to specific apps to consume the data this application will serve up. Browsers have a built-in mechanism that will restrict CORS HTTP requests and the code below informs the browser that it is okay for the apps listed in the origins param to access the app's data. Headers specify the laguage the data will be serialized to/deserialize from and methods allow Get(Read), Post(Create), Put(Edit), Delete.

        ToDoTaDoneEntities db = new ToDoTaDoneEntities();

        //READ functionality
        //api/CategoryAPI
        public IHttpActionResult GetCategories()
        {
            //Create a list to house resources
            //If this doesn't display (intellisense), consider installing entity framework on the API layer.
            List<CategoryViewModel> cats = db.Categories.Select(c => new CategoryViewModel()

            {
                //This section of code is translating the database CategoriesItems objects to the Data Transfer objects. This is called abstraction as we are adding a layer which consuming apps will access instead of accessing the domain models directly.
                CategoryId = c.CategoryID,
                CategoryName = c.CategoryName,
            }).ToList<CategoryViewModel>();

            //Check on the results and if there are no results we will send back to the consuming app a 404
            if (cats.Count == 0)
            {
                return NotFound();//404 error
            }

            return Ok(cats);
        }//end GetCategory()

        //api/CategoryAPI/id
        //Details
        public IHttpActionResult GetCategory(int id)
        {
            //Create a new ResourceViewModel object and assign it the value of the appropriate resource from the database
            CategoryViewModel cat = db.Categories.Where(c => c.CategoryID == id).Select(c => new CategoryViewModel()
            {
                //copy our assignments from above - Get Resources
                CategoryId = c.CategoryID,
                CategoryName = c.CategoryName,
            }).FirstOrDefault();
            //Check that there is a resource and return the resource to user with an OK (200)
            if (cat == null)
            {
                return NotFound(); //<<<< NotFound(404)
            }
            return Ok(cat);
        }//end GetCategory() / id

        //api/CategoryAPI (HttpPost)
        public IHttpActionResult PostCategory(CategoryViewModel Category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");
            }

            db.Categories.Add(new Category()
            {
                CategoryID = Category.CategoryId,
                CategoryName = Category.CategoryName,
            });

            db.SaveChanges();
            return Ok();
        }//end PostCategory() -- like insert


        //api/CategoryAPI (HttpPut)
        public IHttpActionResult PutCategory(CategoryViewModel cat)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid Data");//scopeless if...this is one of the few good examples of a scopeless if, with just one ine of code in the scope of this if

            //get the resource from the db that we want to edit
            Category existingCategory = db.Categories.Where(c => c.CategoryID == cat.CategoryId).FirstOrDefault();

            //If the resource isn't null, then we will reassign its values from the consuming application's request
            if (existingCategory != null)
            {
                existingCategory.CategoryID= cat.CategoryId;
                existingCategory.CategoryName = cat.CategoryName;
                db.SaveChanges();
                return Ok();
            }
            else
            {
                return NotFound();
            }

        }//end PutCategory() - like edit or update

        //api/CategoryAPI/id (httpDelete)
        public IHttpActionResult DeleteCategory(int id)
        {
            Category cat = db.Categories.Where(c => c.CategoryID == id).FirstOrDefault();

            if (cat != null)
            {
                db.Categories.Remove(cat);
                db.SaveChanges();
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }//end DeleteCategory()

        //We use the Dispose() below to dispose of any connections to the db after we are done with them. Best practice to handle performance - dispose of the instance of the controller and db when done with it.
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();//disposes the db connection
            }
            base.Dispose(disposing);//disposes the instance of the controller
        }


    }//end class
}//end namespace