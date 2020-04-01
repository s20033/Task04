using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Task04.DAL;
using Task04.Models;

namespace Task04.Controllers
{
    [ApiController]
    [Route("api/students")]
  
    public class StudentController : ControllerBase
    {
        private  IStudentsDb _dbService;
        private string ConnString= "Data Source=db-mssql;Initial Catalog = s20033; Integrated Security = True";
        //[3.1]
       

        public StudentController(IStudentsDb database)
        {
            this._dbService = database;
        }
        [HttpGet]
        public IActionResult GetStudents()
        {
            var result = new List<Student>();
            using (SqlConnection con = new SqlConnection(ConnString))
            using (SqlCommand command = new SqlCommand())
            { //[3.2]
                command.Connection = con;
                command.CommandText = "select s.FirstName, s.LastName, s.BirthDate, st.Name as Studies, e.Semester " +
                                            "from Student s " +
                                            "join Enrollment e on e.IdEnrollment = s.IdEnrollment " +
                                            "join Studies st on st.IdStudy = e.IdStudy; ";

                con.Open();
                SqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    var std = new Student
                    {
                        FirstName = dr["FirstName"].ToString(),
                        LastName = dr["LastName"].ToString(),
                        Studies = dr["Studies"].ToString(),
                        BirthDate = DateTime.Parse(dr["BirthDate"].ToString()),
                        Semester = int.Parse(dr["Semester"].ToString())
                    };
                     result.Add(std);
                }
                return Ok(result);
            }
            //[3.3]
        }
        [HttpGet("{indexNumber}")]
        public IActionResult GetStudent(string indexNumber)
        {
            using (SqlConnection con = new SqlConnection(ConnString))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = con;
                command.CommandText = "select * from student where indexNumber=@index";

                SqlParameter para1 = new SqlParameter();
                para1.ParameterName = "index";
                para1.Value = indexNumber;

                command.Parameters.Add(para1);
                command.Parameters.AddWithValue("index", indexNumber);

                con.Open();
                SqlDataReader dr = command.ExecuteReader();
                if (dr.Read())
                {
                    var st = new Student
                    {
                        FirstName = dr["FirstName"].ToString(),
                        LastName = dr["LastName"].ToString(),
                        Studies = dr["Studies"].ToString(),
                        BirthDate = DateTime.Parse(dr["BirthDate"].ToString()),
                        Semester = int.Parse(dr["Semester"].ToString())
                    };
                    return Ok(st);
                }

                return Ok();
            }
        }

        [HttpPost]
        public IActionResult CreateStudent([FromServices]IStudentsDb service, Student newStudent)
        {
            using (SqlConnection con = new SqlConnection(ConnString))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = con;
                command.CommandText = "insert into student(indexNumber, firstName, lastName) values (@par1, @par2, @par3)";

                command.Parameters.AddWithValue("par1", newStudent.IndexNumber);
                command.Parameters.AddWithValue("par2", newStudent.FirstName);
                command.Parameters.AddWithValue("par3", newStudent.LastName);

                con.Open();
                int affectedRows = command.ExecuteNonQuery();
            }

            return Ok();
        }

        [HttpGet("procedure")]
        public IActionResult GetStudents2()
        {
            using (SqlConnection con = new SqlConnection(ConnString))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = con;
                command.CommandText = "TestProc2";
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.AddWithValue("LastName", "Kowalski");

                var dr = command.ExecuteReader();
                //...
            }

            return Ok();
        }

        [HttpGet("procedure2")]
        public IActionResult GetStudents3()
        {
            using (SqlConnection con = new SqlConnection(ConnString))
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "insert ...";
                command.Connection = con;

                con.Open();

                SqlTransaction tran = con.BeginTransaction();
                try
                {
                    command.ExecuteNonQuery(); //insert

                    command.CommandText = "update sth...";
                    command.ExecuteNonQuery();
                    tran.Commit();
                }
                catch (Exception)
                {
                    tran.Rollback();
                }

            }

            return Ok();
        }
    }
}