using System.Data;

namespace InterviewConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            DataTable dtEmployees = DatabaseHelper.GetQueryResult("SELECT * FROM Employee");
        }
    }
}
