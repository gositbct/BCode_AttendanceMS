using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace MauiApp1.Models
{
    public class Admin
    {
        [PrimaryKey]
        public string Username { get; set; }

       /* eto yung nag hahhandle pag first time login basically pag wala pang naka 
        * set na password sa database di pa required mag input ng password*/
        public string? Password { get; set; }
    }
}
