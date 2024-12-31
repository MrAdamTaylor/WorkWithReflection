namespace ReflectionMonitoring.Demo
{
    [EmployeeMarkerAttribute]
    public class Employee : Person
    {
        public string Company { get; set; }
    }
}