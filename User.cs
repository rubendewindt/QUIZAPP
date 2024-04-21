using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QUIZAPP
{
    internal class User
    {

        [Key]
        public int UserId { get; set; }
        public int Score { get; set; }
        public string Name { get; set; } = null!;
        public string category { get; set; } = null!;

    }
}
