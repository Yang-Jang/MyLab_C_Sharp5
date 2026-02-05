using Microsoft.AspNetCore.Mvc;
using Bai1.Models;

namespace Bai1.Controller
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private static List<Student> students = new List<Student>
        {
            new Student { Id = 1, Name = "Giang", Age = 20, Email = "giang@gmail.com" },
            new Student { Id = 2, Name = "Bob", Age = 22, Email = "bob@gmail.com" },
            new Student { Id = 3, Name = "Charlie", Age = 21, Email = "charlie@gmail.com" }
        };

        [HttpGet]
        public IActionResult GetAllStudents()
        {
            return Ok(students);
        }

        [HttpGet("{id}")]
        public IActionResult GetStudentById(int id)
        {
            var student = students.FirstOrDefault(s => s.Id == id);
            if (student == null)
            {
                return NotFound($"Không tìm thấy sinh viên có ID = {id}");
            }
            return Ok(student);
        }

        [HttpPost]
        public IActionResult CreateStudents(Student student)
        {
            if(students.Count > 0)
                student.Id = students.Max(s => s.Id) + 1;
            else
                student.Id = 1;

            students.Add(student);
            
            return CreatedAtAction(nameof(GetStudentById), new { id = student.Id }, student);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateStudent(int id, Student updatedStudent)
        {
            var existingStudent = students.FirstOrDefault(s => s.Id == id);
            if (existingStudent == null)
            {
                return NotFound();
            }
            students.Remove(existingStudent);
            students.Add(updatedStudent);
            return Ok(updatedStudent);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(int id)
        {
            var student = students.FirstOrDefault(s => s.Id == id);
            if (student == null)
            {
                return NotFound();
            }
            students.Remove(student);
            return Ok(student);
        }
    }
}
