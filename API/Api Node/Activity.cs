namespace ARX_PME_Connector
{
    /// <summary>
    /// Describe Activity.
    /// </summary>
	internal class Activity
	{
        public class Billing
        {
            public object amount { get; set; }
            public object bill { get; set; }
            public object billComment { get; set; }
            public string billingType { get; set; }
            public object estimate { get; set; }
            public object order { get; set; }
            public bool paid { get; set; }
            public string unitPrice { get; set; }
        }

        public class Break
        {
            public int hour { get; set; }
            public int minute { get; set; }
        }

        public class Category
        {
            public int key { get; set; }
            public string label { get; set; }
        }

        public class Customer
        {
            public int key { get; set; }
            public string label { get; set; }
            public string address { get; set; }
            public object building { get; set; }
            public string city { get; set; }
            public string company { get; set; }
            public object country { get; set; }
            public object doorCode { get; set; }
            public object email { get; set; }
            public object fax { get; set; }
            public object firstName { get; set; }
            public object floor { get; set; }
            public object lastName { get; set; }
            public object mobile { get; set; }
            public bool notValid { get; set; }
            public string number { get; set; }
            public object phone { get; set; }
            public object state { get; set; }
            public Style style { get; set; }
            public string title { get; set; }
            public string type { get; set; }
            public object webSite { get; set; }
            public object zip { get; set; }
        }

        public class Duration
        {
            public string unit { get; set; }
            public int day { get; set; }
            public Time time { get; set; }
        }

        public class Label
        {
            public int key { get; set; }
            public string label { get; set; }
            public string type { get; set; }
            public Style style { get; set; }
        }

        public class Place
        {
            public object address { get; set; }
            public object city { get; set; }
            public object doorCode { get; set; }
            public object floor { get; set; }
            public object zip { get; set; }
        }

        public class Project
        {
            public int key { get; set; }
            public string label { get; set; }
            public bool notValid { get; set; }
            public bool allDepartments { get; set; }
            public object departments { get; set; }
            public List<SubProject> subProjects { get; set; }
            public Style style { get; set; }
        }

        public class Resource
        {
            public int key { get; set; }
            public string label { get; set; }
            public bool fix { get; set; }
            public Schedule schedule { get; set; }
            public bool @readonly { get; set; }
        }

        public class Root
        {
            public int key { get; set; }
            public string label { get; set; }
            public Label _label { get; set; }
            public bool @readonly { get; set; }
            public object editor { get; set; }
            public string oneOrMoreResources { get; set; }
            public List<Resource> resources { get; set; }
            public bool allDay { get; set; }
            public DateTime start { get; set; }
            public DateTime end { get; set; }
            public string mode { get; set; }
            public Duration duration { get; set; }
            public object recurrence { get; set; }
            public string remark { get; set; }
            public bool isAssignment { get; set; }
            public Status status { get; set; }
            public bool isUnavailability { get; set; }
            public string description { get; set; }
            public int categoryIndex { get; set; }
            public Category category { get; set; }
            public List<Customer> customers { get; set; }
            public Project project { get; set; }
            public SubProject subProject { get; set; }
            public Break @break { get; set; }
            public Billing billing { get; set; }
            public Place place { get; set; }
            public string openMaps { get; set; }
            public string callCustomer { get; set; }
            public object tooltip { get; set; }
            public bool isBlocked { get; set; }
        }

        public class Schedule
        {
            public int key { get; set; }
            public string label { get; set; }
        }

        public class Status
        {
            public int key { get; set; }
            public string type { get; set; }
            public string label { get; set; }
            public bool isDefault { get; set; }
        }

        public class Style
        {
            public string backgroundColor { get; set; }
            public string color { get; set; }
        }

        public class SubProject
        {
            public int key { get; set; }
            public string label { get; set; }
        }

        public class SubProject2
        {
            public int key { get; set; }
            public string label { get; set; }
        }

        public class Time
        {
            public int hour { get; set; }
            public int minute { get; set; }
        }


    }
}
