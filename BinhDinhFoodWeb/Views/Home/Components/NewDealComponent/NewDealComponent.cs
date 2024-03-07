﻿using BinhDinhFood.Models;
using BinhDinhFoodWeb.Intefaces;
using Microsoft.AspNetCore.Mvc;

namespace BinhDinhFoodWeb.Views.Home.Components.NewDealComponent
{
    public class NewDealComponent: ViewComponent
    {
        private readonly IProductRepository _repo;
        public NewDealComponent(IProductRepository repo)
        {
            _repo = repo;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var obj = await _repo.GetListAsync(
                filter: x=>x.ProductDateCreated.Month == DateTime.Now.Month);
            return View("NewDealComponent", obj);
        }
    }
}
