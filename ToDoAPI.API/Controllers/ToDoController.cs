using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ToDoAPI.DATA.EF;//Added for access to EF layer
using ToDoAPI.API.Models;//Added for access to Data Transfer Objects
using System.Web.Http.Cors;//Added to access and modify cross origin resource sharing

//Cross Origin Resource Sharing - This is a mechanism that browsers use to ensure that any resources shared bewtween websites will be permissable by both parties. By default, CORS is blocked by the browser. In order to allow CORS with this API, we need to:
//1. Install a NuGet Package - Microsoft.AspNet.WebApi.Cors
//2. Navigate to the App_Start/WebApiConfig.cs and add a line of code to allow CORS in the app
//3. Add the above using statement 
//4. Add the metadata to the class below (ResourcesController)

namespace ToDoAPI.API.Controllers
{
    public class ToDoController : ApiController
    {

        //In the code below we are giving permission to specific apps to consume the data this application will serve up. Browsers have a built-in mechanism that will restrict CORS HTTP requests and the code below informs the browser that it is okay for the apps listed in the origins param to access the app's data. Headers specify the laguage the data will be serialized to/deserialize from and methods allow Get(Read), Post(Create), Put(Edit), Delete.
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public class ToDoAPIController : ApiController
        {
            ToDoTaDoneEntities db = new ToDoTaDoneEntities();

            //READ functionality
            //api/ToDoAPI
            public IHttpActionResult GetToDoItems()
            {
                //Create a list to house resources
                //If this doesn't display (intellisense), consider installing entity framework on the API layer.
                List<ToDoViewModel> todo = db.ToDoItems.Include("Category").Select(t => new ToDoViewModel()

                {
                    //This section of code is translating the database ToDoItems objects to the Data Transfer objects. This is called abstraction as we are adding a layer which consuming apps will access instead of accessing the domain models directly.
                    Todoid = t.Todoid,
                    Action = t.Action,
                    Done = t.Done,
                    Category = new CategoryViewModel()
                    {
                        CategoryId = t.Category.CategoryID,
                        Name = t.Category.Name,
                        Description = t.Category.Description
                    }
                }).ToList<ToDoViewModel>();

                //Check on the results and if there are no results we will send back to the consuming app a 404
                if (todo.Count == 0)
                {
                    return NotFound();//404 error
                }

                return Ok(todo);
            }//end Get

            //api/ToDoAPI/id
            //Details
            public IHttpActionResult GetToDoItems(int id)
            {
                //Create a new ResourceViewModel object and assign it the value of the appropriate resource from the database
                ToDoViewModel items = db.ToDoItems.Include("Category").Where(t => t.CategoryId == id).Select(t => new ToDoViewModel()
                {
                    //copy our assignments from above - Get Resources
                    Todoid = t.Todoid,
                    Action = t.Action,
                    Done = t.Done,
                    CategoryId = t.Category.CategoryID,
                    Category = new CategoryViewModel()
                    {
                        CategoryId = t.Category.CategoryID,
                        Name = t.Category.Name,
                        Description = t.Category.Description
                    }
                }).FirstOrDefault();
                //Check that there is a resource and return the resource to user with an OK (200)
                if (items == null)
                {
                    return NotFound(); //<<<< NotFound(404)
                }
                return Ok(items);
            }//end GetToDo

            //api/ToDoAPI (HttpPost)
            public IHttpActionResult PostToDo(ToDoViewModel todo)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid Data");
                }

                ToDoItem newItems = new ToDoItem()
                {
                    Action = todo.Action,
                    Done = todo.Done,
                    CategoryId = todo.CategoryId
                };

                db.ToDoItems.Add(newItems);
                db.SaveChanges();
                return Ok(newItems);
            }//end PostToDo -- like insert


            //api/ToDoAPI (HttpPut)
            public IHttpActionResult PutToDoItem(ToDoViewModel items)
            {
                if (!ModelState.IsValid)
                    return BadRequest("Invalid Data");//scopeless if...this is one of the few good examples of a scopeless if, with just one ine of code in the scope of this if

                //get the resource from the db that we want to edit
                ToDoItem existingItem = db.ToDoItems.Where(t => t.Todoid == items.Todoid).FirstOrDefault();

                //If the resource isn't null, then we will reassign its values from the consuming application's request
                if (existingItem != null)
                {
                    existingItem.Todoid = items.Todoid;
                    existingItem.Action = items.Action;
                    existingItem.Done = items.Done;
                    existingItem.CategoryId = items.CategoryId;
                    db.SaveChanges();
                    return Ok();
                }
                else
                {
                    return NotFound();
                }

            }//end PutResource - like edit or update

            //api/ToDoAPI/id (httpDelete)
            public IHttpActionResult DeleteResource(int id)
            {
                ToDoItem todo = db.ToDoItems.Where(t => t.Todoid == id).FirstOrDefault();

                if (todo != null)
                {
                    db.ToDoItems.Remove(todo);
                    db.SaveChanges();
                    return Ok();
                }
                else
                {
                    return NotFound();
                }
            }//end DeleteResource

            //We use the Dispose() below to dispose of any connections to the db after we are done with them. Best practice to handle performance - dispose of the instance of the controller and db when done with it.
            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    db.Dispose();//disposes the db connection
                }
                base.Dispose(disposing);//disposes the instance of the controller
            }

        }
    }//end class
}//end namespace
