﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVPZCard.Models;
using AVPZCard.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace AVPZCard.Data.Repository
{
    public class Repository : IRepository
    {
        private AppDbContext _ctx;

        public Repository(AppDbContext ctx)
        {
            _ctx = ctx;
        }
        public List<Post> GetAllPosts()
        {
            return _ctx.Posts.Include(o => o.Category).ToList();
        }

        public IndexViewModel GetAllPosts(int pageNumber,
            int category,
            string search)
        {

            int pageSize = 5;
            int skipAmount = pageSize * (pageNumber - 1);

            //AsNoTracking не будет отслеживать изменения найденых постов (но увеличивает скорость)
            //последи за сохранением
            var query = _ctx.Posts.Include(p => p.Category).AsNoTracking();


            if (category != 0)
            {
                query = query.Where(c => c.Category.Id_Category == category);
            }


            if (!String.IsNullOrEmpty(search))
                query = query.Where(x => EF.Functions.Like(x.Title, $"%{search}%")
                || EF.Functions.Like(x.Body, $"%{search}%")
                || EF.Functions.Like(x.Description, $"%{search}%"));

            int postCount = query.Count();
            var canNext = postCount > skipAmount + pageSize;

            return new IndexViewModel
            {
                PageNumber = pageNumber,
                CanNextPage = canNext,
                PageCount = (int)Math.Ceiling((double)postCount / pageSize),
                Category = _ctx.Categories.FirstOrDefault(o => o.Id_Category == category),
                Search = search,
                Posts = query
                    .Skip(skipAmount)
                    .Take(pageSize)
                    .ToList()
            };
        }

        public Post GetPost(int id)
        {
            return _ctx.Posts.Include(o => o.Category)
                    .FirstOrDefault(m => m.Id == id);
        }
        public void AddPost(Post post)
        {
            _ctx.Posts.Add(post);

        }


        public async void UpdateInf(AboutViewModel vm)
        {
            if (_ctx.Abouts.Count() == 0)
            {
                _ctx.Abouts.Add(new About { AboutMe = vm.About, Contacts = vm.Contacts });
                _ctx.SaveChanges();
            }
            else
            {
                var ab = _ctx.Abouts.First();
                if (vm.About != null && vm.About.Length > 10)
                {
                    ab.AboutMe = vm.About;
                }
                if (vm.Contacts != null && vm.Contacts.Length > 10)
                {
                    ab.Contacts = vm.Contacts;
                }
                 _ctx.Abouts.Update(ab);
                _ctx.SaveChanges();
            }
        }

        public void RemovePost(int id)
        {
            _ctx.Posts.Remove(GetPost(id));
        }
        public void UpdatePost(Post post)
        {
            _ctx.Posts.Update(post);
        }

        public async Task<bool> SaveChangesAsync()
        {
            if (await _ctx.SaveChangesAsync() != 0)
            {
                return true;
            }
            return false;
        }

        public IEnumerable<Category> DisplayCategories()
        {
            return _ctx.Categories.Where(o => o.Visible == true).AsNoTracking().ToList();
        }
    }
}
