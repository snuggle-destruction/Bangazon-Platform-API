﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BangazonAPI.Controllers
{
    public class AssignedComputersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}