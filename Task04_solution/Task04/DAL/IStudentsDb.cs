using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task04.Models;

namespace Task04.DAL
{
    public interface IStudentsDb

    {
        IEnumerable<Student> GetStudents();
    }
}
