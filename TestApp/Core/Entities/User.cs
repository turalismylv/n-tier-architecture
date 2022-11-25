﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class User :IdentityUser
    {
        public string FullName { get; set; }
        public int  Age { get; set; }
    }
}
