// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
//
//Copyright (C) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Linq;
using SampleSupport;
using Task.Data;

// Version Mad01

namespace SampleQueries
{
	[Title("LINQ Module")]
	[Prefix("Linq")]
	public class LinqSamples : SampleHarness
	{

		private DataSource dataSource = new DataSource();

		[Category("Restriction Operators")]
		[Title("Where - Task 1")]
		[Description("This sample uses the where clause to find all elements of an array with a value less than 5.")]
		public void Linq1()
		{
			int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };

			var lowNums =
				from num in numbers
				where num < 5
				select num;

			Console.WriteLine("Numbers < 5:");
			foreach (var x in lowNums)
			{
				Console.WriteLine(x);
			}
		}

		[Category("Restriction Operators")]
		[Title("Where - Task 2")]
		[Description("This sample return all presented in market products")]

		public void Linq2()
		{
			var products =
				from p in dataSource.Products
				where p.UnitsInStock > 0
				select p;

			foreach (var p in products)
			{
				ObjectDumper.Write(p);
			}
		}

        [Category("Restriction Operators")]
        [Title("Where - Task 3")]
        [Description("This sample return return all customers from London")]
        public void Linq3()
        {
            var customers =
                from c in dataSource.Customers
                where c.City == "London"
                select c;

            foreach (var c in customers)
            {
                ObjectDumper.Write(c);
            }
        }

        [Category("Restriction Operators")]
        [Title("Homework Task 1")]
        [Description("This sample return all customers where orders sum is greater than 5000.")]
        public void Linq5()
        {
            var customers =
                from c in dataSource.Customers
                where c.Orders.Sum(t => t.Total) > 5000
                select new
                {
                    Id = c.CustomerID,
                    Sum = c.Orders.Sum(t => t.Total)
                };

            foreach (var c in customers)
            {
                ObjectDumper.Write(c);
            }
        }

        [Category("Restriction Operators")]
        [Title("Homework Task 2")]
        [Description("This sample return all customers and suppliers from the same city and country.")]
        public void Linq7()
        {
            var customers =
                from s in dataSource.Suppliers
                from c in dataSource.Customers
                where s.City == c.City && s.Country == c.Country
                select new
                {
                    c.CustomerID, s.Country, s.City
                };

            foreach (var c in customers)
            {
                ObjectDumper.Write(c);
            }
        }

        [Category("Restriction Operators")]
        [Title("Homework Task 3")]
        [Description("This sample return all customers who has order which is greater than 5000.")]
        public void Linq6()
        {
            var customers =
                from c in dataSource.Customers
                where c.Orders.Length != 0 && c.Orders.Max(p => p.Total) > 5000
                select new
                {
                    Id = c.CustomerID,
                    OrderPrice = c.Orders.Max(p => p.Total)
                };

            foreach (var c in customers)
            {
                ObjectDumper.Write(c);
            }
        }

        [Category("Restriction Operators")]
        [Title("Homework Task 4")]
        [Description("This sample return all customers with the first order date.")]
        public void Linq8()
        {
            var customers =
                from c in dataSource.Customers
                where c.Orders.Length != 0
                select new
                {
                    Id = c.CustomerID,
                    OrderDate = c.Orders.Min(d => d.OrderDate)
                };

            foreach (var c in customers)
            {
                ObjectDumper.Write(c);
            }
        }

        [Category("Restriction Operators")]
        [Title("Homework Task 5")]
        [Description("This sample return all customers with the first order date sorted by year, month, orders summ and name.")]
        public void Linq9()
        {
            var customers = 
                from c in dataSource.Customers
                where c.Orders.Length != 0
                orderby c.Orders.Min(d => d.OrderDate.Year),
                c.Orders.Min(d => d.OrderDate.Month),
                c.Orders.Sum(s => s.Total) descending,
                c.CompanyName
                select new
                {
                    Id = c.CustomerID,
                    CompanyName = c.CompanyName,
                    OrderDate = c.Orders.Min(d => d.OrderDate),
                    Total = c.Orders.Sum(s => s.Total)
                };

            foreach (var c in customers)
            {
                ObjectDumper.Write(c);
            }
        }

        [Category("Restriction Operators")]
        [Title("Homework Task 6")]
        [Description("This sample return all customers with non-numerical postl code, empty region or blank phone number.")]
        public void Linq10()
        {
            var customers = 
                from c in dataSource.Customers
                where (c.PostalCode == null || Regex.IsMatch(c.PostalCode, "[A-Za-z]")
                    || c.Region == null
                    || !c.Phone.StartsWith("("))
                select new
                {
                    c.CompanyName,
                    c.PostalCode,
                    c.Region,
                    c.Phone
                };

            foreach (var c in customers)
            {
                ObjectDumper.Write(c);
            }
        }

        [Category("Restriction Operators")]
        [Title("Homework Task 7")]
        [Description("This sample return all products grouped by category and availability on warehouse and sorted by price.")]
        public void Linq11()
        {
            var products = 
                from p in dataSource.Products
                group p by p.Category into categoryGrp
                from presenceGrp in
                (
                    from p in categoryGrp
                    orderby p.UnitsInStock, p.UnitPrice
                    group p by p.UnitsInStock
                )
                group presenceGrp by categoryGrp.Key;
                

            foreach (var outerGroup in products)
            {
                foreach (var innerGroup in outerGroup)
                {
                    foreach (var innerGroupElement in innerGroup)
                    {
                        ObjectDumper.Write(innerGroupElement);
                    }
                }
            }
        }

        [Category("Restriction Operators")]
        [Title("Homework Task 8")]
        [Description("This sample return all products grouped by price.")]
        public void Linq12()
        {
            var products = 
                from p in dataSource.Products
                let price = 
                (
                    p.UnitPrice < 40 ? "Cheap" :
                    p.UnitPrice >= 40 && p.UnitPrice < 80 ? "Average" :
                    "Expensive"
                )
                orderby price
                select new
                {
                    p.ProductID,
                    p.Category,
                    p.UnitPrice,
                    price
                };

            foreach (var p in products)
            {
                ObjectDumper.Write(p);
            }
        }

        [Category("Restriction Operators")]
        [Title("Homework Task 9")]
        [Description("This sample return average income and average customers intensity by city.")]
        public void Linq13()
        {
            var customers =
                from c in dataSource.Customers
                group c by c.City into cityGrp
                select new
                {
                    cityGrp.Key,
                    AverageIncome = cityGrp.Sum(o => o.Orders.Sum(p => p.Total)) / cityGrp.Sum(o => o.Orders.Count()),
                    AverageIntensity = cityGrp.Sum(o => o.Orders.Count()) / cityGrp.Count()
                };

            foreach (var c in customers)
            {
                ObjectDumper.Write(c);
            }
        }

        [Category("Restriction Operators")]
        [Title("Homework Task 10")]
        [Description("This sample return average annual customers intensity by month.")]
        public void Linq14()
        {
            var customers = from c in dataSource.Customers
                            select new
                            {
                                c.CompanyName,
                                MonthActivity = from o in c.Orders
                                                orderby o.OrderDate.Month
                                                group o by o.OrderDate.Month into monthGrp
                                                select new
                                                {
                                                    Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(monthGrp.Key),
                                                    OrdersAmoutn = monthGrp.Count()
                                                }
                            };
            

            foreach (var customer in customers)
            {
                ObjectDumper.Write(customer.CompanyName);
                foreach (var activity in customer.MonthActivity)
                {
                    ObjectDumper.Write(activity);
                }
            }
        }

        [Category("Restriction Operators")]
        [Title("Homework Task 11")]
        [Description("This sample return average annual customers intensity by year.")]
        public void Linq15()
        {
            var customers = from c in dataSource.Customers
                            select new
                            {
                                c.CompanyName,
                                YearActivity = from o in c.Orders
                                                orderby o.OrderDate.Year
                                                group o by o.OrderDate.Year into yearGrp
                                                select new
                                                {
                                                    Year = yearGrp.Key,
                                                    OrdersAmoutn = yearGrp.Count()
                                                }
                            };


            foreach (var customer in customers)
            {
                ObjectDumper.Write(customer.CompanyName);
                foreach (var activity in customer.YearActivity)
                {
                    ObjectDumper.Write(activity);
                }
            }
        }

        [Category("Restriction Operators")]
        [Title("Homework Task 12")]
        [Description("This sample return average annual customers intensity by year and month.")]
        public void Linq16()
        {
            var customers = from c in dataSource.Customers
                            select new
                            {
                                c.CompanyName,
                                YearMonthActivity = from o in c.Orders
                                               orderby o.OrderDate.Year, o.OrderDate.Month
                                               group o by
                                               new
                                               {
                                                   o.OrderDate.Year,
                                                   o.OrderDate.Month
                                               }
                                                into yearGrp
                                               select new
                                               {
                                                   Year = yearGrp.Key.Year,
                                                   Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(yearGrp.Key.Month),
                                                   OrdersAmoutn = yearGrp.Count()
                                               }
                            };


            foreach (var customer in customers)
            {
                ObjectDumper.Write(customer.CompanyName);
                foreach (var activity in customer.YearMonthActivity)
                {
                    ObjectDumper.Write(activity);
                }
            }
        }
    }
}
