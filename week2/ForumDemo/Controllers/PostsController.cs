using System;
using System.Collections.Generic;
using System.Linq;
using ForumDemo.Models;
using Microsoft.AspNetCore.Mvc;

namespace ForumDemo.Controllers
{
    public class PostsController : Controller
    {
        private ForumContext db;
        public PostsController(ForumContext context)
        {
            db = context;
        }

        [HttpGet("/posts")]
        public IActionResult All()
        {
            // no .Where because we want all of them
            List<Post> allPosts = db.Posts.ToList();
            return View("All", allPosts);
        }

        [HttpGet("/posts/new")]
        public IActionResult New()
        {
            return View("New");
        }

        [HttpPost("/posts/create")]
        /* 
          <form 
            asp-controller="Posts"
            asp-action="Create" method="POST">
         */
        public IActionResult Create(Post newPost)
        {
            if (ModelState.IsValid == false)
            {
                // send back to the page with the form so error messages are displayed
                return View("New");
            }

            // ModelState IS valid
            db.Posts.Add(newPost);
            // db does NOT update until you run this, after SaveChanges, the newPost now has it's ID from the DB
            db.SaveChanges();
            return RedirectToAction("All");
        }

        // {postId} is a URL parameter, it MUST match the asp-route-paramName attribute on the anchor/form tag
        // where paramName is whatever you named it in the html
        [HttpGet("/posts/{postId}")]
        public IActionResult Details(int postId)
        {
            Post selectedPost = db.Posts.FirstOrDefault(post => post.PostId == postId);

            if (selectedPost == null)
            {
                return RedirectToAction("All");
            }

            return View("Details", selectedPost);
        }

        [HttpPost("/posts/{postId}/delete")]
        public IActionResult Delete(int postId)
        {
            Post selectedPost = db.Posts.FirstOrDefault(post => post.PostId == postId);

            if (selectedPost != null)
            {
                db.Posts.Remove(selectedPost);
                db.SaveChanges();
            }
            return RedirectToAction("All");
        }

        [HttpGet("/posts/{postId}/edit")]
        public IActionResult Edit(int postId)
        {
            Post selectedPost = db.Posts.FirstOrDefault(post => post.PostId == postId);

            if (selectedPost == null)
            {
                return RedirectToAction("All");
            }
            return View("Edit", selectedPost);
        }

        [HttpPost("/posts/{postId}/update")]
        public IActionResult Update(Post editedPost, int postId)
        {
            if (ModelState.IsValid == false)
            {
                // the id assignment line is happening automatically because our id param name matches the primary key name
                // if not named the same need to do it manually or use a hidden input with the id in it b/c editedPost is a newly constructed object from the form data
                // editedPost.PostId = idParamName;

                // send back to the page with the form so error messages are displayed
                return View("Edit", editedPost);
            }

            // ModelState IS valid
            Post selectedPost = db.Posts.FirstOrDefault(post => post.PostId == postId);

            if (selectedPost == null)
            {
                return RedirectToAction("All");
            }

            // selectedPost is not null
            selectedPost.Topic = editedPost.Topic;
            selectedPost.Body = editedPost.Body;
            selectedPost.ImgUrl = editedPost.ImgUrl;
            selectedPost.UpdatedAt = DateTime.Now;

            db.Posts.Update(selectedPost);
            db.SaveChanges();

            return RedirectToAction("Details", new { postId = postId });
        }
    }
}