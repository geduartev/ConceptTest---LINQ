using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Xml.Linq;
using LINQ.Classes;

namespace LINQ
{
    class Program
    {
        static void Main(string[] args)
        {
            // Sin LINQ Object
            WithoutLinqObject();
            
            // Con LINQ Object
            WithLinqObject();

            // Con LINQ Xml
            WithLinqXml();

            // Con LINQ DataSet
            //WithLinqDataSet();

            // Con LINQ Sql Classes
            WithLinqSqlClasses();

            // Con LINQ Sql Classes to Converto to XML
            WithLinqSqlClassesConvertToXml();
        }

        private static void WithLinqSqlClassesConvertToXml()
        {
            var db = new DataClassesConceptTestDataContext();
            var xml = new XElement("CustomerClasss",
                from x in db.Customers
                select new XElement("CustomerClass",
                    new XAttribute("CustomerId", x.CustomerId),
                    new XAttribute("CustomerName", x.CustomerName),
                    new XAttribute("City", x.City)));
            Console.WriteLine(xml);
            Console.ReadLine();
        }

        private static void WithLinqSqlClasses()
        {
            var db = new DataClassesConceptTestDataContext();
            var results = from c in db.Customers
                where c.City.StartsWith("M")
                group c by c.City
                into g
                select new Result()
                {
                    City = g.Key,
                    Count = g.Count()
                };
            db.Log = Console.Out;
            ObjectDumper.Write(results);
        }

        private static void WithLinqDataSet()
        {
            var results = from c in LoadCustomersSql()
                where c.City.StartsWith("M")
                group c by c.City
                into g
                select new Result
                {
                    City = g.Key,
                    Count = g.Count()
                };

            ObjectDumper.Write(results);
        }

        private static void WithLinqXml()
        {
            var results = from c in LoadCustomersXml()
                where c.City.StartsWith("M")
                group c by c.City
                into g
                select new Result
                {
                    City = g.Key,
                    Count = g.Count()
                };

            ObjectDumper.Write(results);
        }

        private static void WithLinqObject()
        {
            // Con LINQ me ahorro muchas líneas de código que las del método WithoutLinqObject
            var results = from c in LoadCustomers()
                where c.City.StartsWith("M")
                group c by c.City
                into g
                select new
                    Result // Result no hace parte del dominio del negocio. Si se retira la palabra Result sigue funcionando. Crea un nuevo tipo Anónimo.
                    {
                        City = g.Key,
                        Count = g.Count()
                    };

            ObjectDumper.Write(results);
        }

        private static void WithoutLinqObject()
        {
            var results = new List<Result>();

            // Mostrar lista de resultados sin LINQ
            foreach (var c in LoadCustomers())
            {
                if (c.City.StartsWith("M"))
                {
                    //Result res = results.Find(delegate(Result r)
                    //{
                    //    return c.City == r.City;
                    //});

                    // Esta línea reemplaza las anteriores por una sintáxis más sencilla.
                    var res = results.Find(cust => cust.City == c.City);

                    if (res == null)
                    {
                        //res = new Result();
                        //res.City = c.City;
                        //res.Count = 1;
                        //results.Add(res);

                        // Mediante Object Initializer reemplazamos las líneas anteriores.
                        results.Add(new Result() {City = c.City, Count = 1});
                    }
                    else
                    {
                        res.Count++;
                    }
                }
            }

            ObjectDumper.Write(results);
        }

        private static IEnumerable<CustomerClass> LoadCustomers()
        {
            #region Many Customers...

            //CustomerClass c = new CustomerClass
            //{
            //    CustomerId = "ANATR",
            //    ContactName = "Ana Trujillo",
            //    City = "Italia"
            //};
            //custs.Add(c);

            //c = new CustomerClass
            //{
            //    CustomerId = "BLONP",
            //    ContactName = "Frederique Citeaux",
            //    City = "Strasbourg"
            //};
            //custs.Add(c);

            //c = new CustomerClass
            //{
            //    CustomerId = "BONAP",
            //    ContactName = "Laurence Lebihan",
            //    City = "Mannheim"
            //};
            //custs.Add(c);

            // Las líneas anteriores las reemplazamos por Collection Initializers
            var custs = new List<CustomerClass>
            {
                new CustomerClass(){CustomerId = "GEDUVE", ContactName = "Gabriel arte", City = "Malaga" },
                new CustomerClass(){CustomerId = "LEVEGO", ContactName = "Luz Vega", City = "Bogota" },
                new CustomerClass(){CustomerId = "PPDUVE", ContactName = "Pedro Duarte", City = "Cucuta" },
                new CustomerClass(){CustomerId = "JAVEGO", ContactName = "Jorge Gomez", City = "Medellin" },
                new CustomerClass(){CustomerId = "VMMAVA", ContactName = "Viviana Martinez", City = "Pasto" }
            };

            #endregion

            return custs;
        }

        private static IEnumerable<CustomerClass> LoadCustomersXml()
        {
            var custs = from x in XElement.Load("XMLs/Customers.xml").Elements("CustomerClass")
                select new CustomerClass()
                {
                    City = x.Attribute("City").Value,
                    ContactName = x.Attribute("ContactName").Value
                };

            var xml = new XElement("Car");
            return custs;
        }

        private static DataSetCustomers.CustomersDataTable LoadCustomersSql()
        {
            //SqlDataReader myDataReader = null;
            //SqlConnection mySqlConnection =
            //    new SqlConnection("server=.\\SQLSERVER2008R2;Trusted_Connection=yes;database=ConceptTest");
            //SqlCommand mySqlCommand = new SqlCommand(
            //    "SELECT CustomerId, CustomerName, City FROM Customers WHERE City LIKE @City", mySqlConnection);
            //mySqlCommand.Parameters.Add(new SqlParameter("City", "M"));
            //mySqlConnection.Open();
            //myDataReader = mySqlCommand.ExecuteReader(CommandBehavior.CloseConnection);


            // Las anteriores líneas se pueden mejorar utilizando un DataSet
            var mySqlDataAdapter =
                new SqlDataAdapter(
                    "SELECT * FROM Customers",
                    @"Data Source=.\\SQLSERVER2008R2;Initial Catalog=ConceptTest;Integrated Security=True");
            
            // He agregado un item de tipo DataSet y he arrastrado al dataset la tabla Customers desde el Server Explorer.
            DataSetCustomers.CustomersDataTable table = new DataSetCustomers.CustomersDataTable();
            mySqlDataAdapter.Fill(table);
            return table;
        }
    }
}
