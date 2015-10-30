using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Daily_197_Intermediate_CS
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<char, Intersection> intersections = "ABCDEFGHIJKLMNOP".ToDictionary(c => c, c => new Intersection(c.ToString()));
            StreetFactory factory = new StreetFactory(intersections);
            List<Street> streets = new List<Street>();
            streets.Add(factory.CreateStreet("South Acorn Drive", 'A', 'B', 5, 10, 5, 10));
            streets.Add(factory.CreateStreet("Acorn Drive", 'B', 'C', 15, 5, 15, 5));
            streets.Add(factory.CreateStreet("North Acorn Drive", 'C', 'D', 7, 10, 15, 7));
            streets.Add(factory.CreateStreet("South Almond Way", 'H', 'G', 10, 10, 10, 10));
            streets.Add(factory.CreateStreet("Almond Way", 'G', 'F', 15, 10, 15, 20));
            streets.Add(factory.CreateStreet("North Almond Way", 'F', 'E', 5, 6, 5, 6));
            streets.Add(factory.CreateStreet("South Peanut Lane", 'I', 'J', 8, 9, 10, 11));
            streets.Add(factory.CreateStreet("Peanut Lane", 'J', 'K', 11, 10, 9, 8));
            streets.Add(factory.CreateStreet("North Peanut Lane", 'K', 'L', 7, 5, 7, 5));
            streets.Add(factory.CreateStreet("South Walnut", 'P', 'O', 6, 5, 6, 5));
            streets.Add(factory.CreateStreet("Walnut", 'O', 'N', 10, 8, 10, 8));
            streets.Add(factory.CreateStreet("North Walnut", 'N', 'M', 9, 6, 9, 6));
            streets.Add(factory.CreateStreet("West Elm Street", 'D', 'E', 10, 8, 12, 7));
            streets.Add(factory.CreateStreet("Elm Street", 'E', 'L', 12, 11, 12, 8));
            streets.Add(factory.CreateStreet("East Elm Street", 'L', 'M', 5, 4, 5, 4));
            streets.Add(factory.CreateStreet("West Central Avenue", 'C', 'F', 9, 8, 9, 8));
            streets.Add(factory.CreateStreet("Central Avenue", 'F', 'K', 5, 4, 5, 4));
            streets.Add(factory.CreateStreet("East Central Avenue", 'K', 'N', 9, 9, 9, 9));
            streets.Add(factory.CreateStreet("West Pine Road", 'B', 'G', 7, 6, 7, 6));
            streets.Add(factory.CreateStreet("Pine Road", 'G', 'J', 9, 8, 9, 8));
            streets.Add(factory.CreateStreet("East Pine Road", 'J', 'O', 6, 5, 6, 5));
            streets.Add(factory.CreateStreet("West Oak Expressway", 'A', 'H', 9, 8, 7, 7));
            streets.Add(factory.CreateStreet("Oak Expressway", 'H', 'I', 10, 10, 10, 10));
            streets.Add(factory.CreateStreet("East Oak Expressway", 'I', 'P', 8, 7, 8, 7));

            string[] testRoutes = { "A M 0800", "A M 1200", "A M 1800", "A M 2200", "P D 0800", "P D 1200", "P D 1800", "P D 2200" };
            foreach (string testRoute in testRoutes)
                FindShortestRoute(intersections, streets, testRoute);
        }

        private static void FindShortestRoute(IDictionary<char, Intersection> intersections, IEnumerable<Street> streets, string requirements)
        {
            Regex pattern = new Regex(@"^(?<from>[A-Z]{1}) (?<to>[A-Z]{1}) (?<time>\d{4})$");
            Match match = pattern.Match(requirements);
            if (!match.Success)
            {
                Console.Write("Bad requirements: {0}", requirements);
                return;
            }

            int timeIndex;
            int time = int.Parse(match.Groups["time"].Value);
            if (time >= 600 && time <= 1000)
                timeIndex = 0;
            else if (time >= 1000 && time <= 1500)
                timeIndex = 1;
            else if (time >= 1500 && time <= 1900)
                timeIndex = 2;
            else
                timeIndex = 3;

            DijkstraSearch search = new DijkstraSearch();

            foreach (Street street in streets)
                search.AddConnection(street.Intersection1, street.Intersection2, street.Times[timeIndex]);

            Intersection from = intersections[match.Groups["from"].Value[0]];
            Intersection to = intersections[match.Groups["to"].Value[0]];

            DijkstraPathSearchResult result = search.FindShortestPath(from, to);
            Console.WriteLine("Path from {0} to {1} at {2}:", from, to, time);
            IDijkstraSearchNode[] path = result.Path.ToArray();
            for (int i = 0; i < path.Length - 1; ++i)
            {
                Street street = streets.First(s => (s.Intersection1 == path[i] || s.Intersection2 == path[i]) && (s.Intersection1 == path[i + 1] || s.Intersection2 == path[i + 1]));
                Console.WriteLine("  ({0,2} minutes) {1}", street.Times[timeIndex], street.Name);
            }
            Console.WriteLine("Total time: {0} minutes", result.TotalDistance);
        }
    }

    class StreetFactory
    {
        private readonly IDictionary<char, Intersection> _intersections;

        public StreetFactory(IDictionary<char, Intersection> intersections)
        {
            _intersections = intersections;
        }

        public Street CreateStreet(string name, char intersection1, char intersection2, int time1, int time2, int time3, int time4)
        {
            return new Street(name, _intersections[intersection1], _intersections[intersection2], new[] { time1, time2, time3, time4 });
        }
    }

    class Intersection : IDijkstraSearchNode
    {
        public string Name { get; private set; }

        public Intersection(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name ?? "";
        }
    }

    class Street
    {
        public Intersection Intersection1 { get; private set; }
        public Intersection Intersection2 { get; private set; }
        public string Name { get; private set; }
        public int[] Times { get; private set; }

        public Street(string name, Intersection intersection1, Intersection intersection2, int[] times)
        {
            Name = name;
            Intersection1 = intersection1;
            Intersection2 = intersection2;
            Times = (int[])times.Clone();
        }

        public override string ToString()
        {
            return Name ?? "";
        }
    }
}
