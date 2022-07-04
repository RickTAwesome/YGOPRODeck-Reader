using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YGOPRODeck_Reader
{
    public struct Card
    {
        public string name;
        public int[] id;
    }

    class Program
    {
        public static string[] files = {
                "291078.ydk",
                "289821.ydk",
                "282562.ydk",
                "291169.ydk",
                "285508.ydk",
                "280499.ydk",
                "280420.ydk",
                "285018.ydk",
                "281184.ydk",
                "286001.ydk",
                "292288.ydk",
                "286357.ydk",
                "291094.ydk"};

        static void Main(string[] args)
        {
            Database database = Database.Load("card_database.txt");
            Console.WriteLine("Do you want to see the entire database? (Y/N)");
            Console.Write("Input: ");

            #region Confirm // Needs to check if the line can even be parsed.
            char confirm = char.Parse(Console.ReadLine());
            if (char.ToUpper(confirm) == 'Y')
            {
                Console.WriteLine();
                database.Write();
            }
            Console.WriteLine();
            #endregion

            LinkedList<int> list = Function.Count(files);

            Console.WriteLine($"Out of {files.Length} decks there were");
            Function.SortAndWrite();
            Console.WriteLine();

            Console.WriteLine("Unrecognized IDs:");
            foreach (var id in list)
            {
                Console.WriteLine(id);
            }
            //Function.WriteCount(1);

            Console.SetCursorPosition(0, 0);
            Console.ReadKey();
        }
    }

    public class Database
    {
        #region Linked List
        public class Node : IComparable<Node>
        {
            public Card carddata;
            public int count;
            public Node next;
            public Node prev;

            public Node()
            {

            }

            public Node(Card carddata, int count)
            {
                this.carddata = carddata;
                this.count = count;
                this.next = null;
                this.prev = null;
            }

            public int CompareTo(Node other)
            {
                return -count.CompareTo(other.count);
            }

            public override string ToString()
            {
                return $"{count}x - {carddata.name} ({(float)count / Program.files.Length})";
            }
        }

        public static Node head;
        public static Node tail;

        
        public static int Count
        {
            get
            {
                int anz = 0;

                for (Node current = head; current != null; current = current.next)
                {
                    anz++;
                }

                return anz;
            }
        }
        
        public Database()
        {
            head = null;
            tail = null;
        }
        #endregion

        #region Functions
        public static Database Load(string filename)
        {
            Database database = new Database();

            using (StreamReader reader = new StreamReader(filename))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();

                    if (line != "" && line[0] != '#')
                    {
                        string[] lines = line.Split('|');

                        Card card;
                        card.name = lines[0].Trim();
                        card.id = lines[1].Split(',').Select(int.Parse).ToArray();

                        database.AddLast(card, 0);
                    }
                }
            }

            return database;
        }

        public void AddLast(Card card, int count)
        {
            Node node = new Node(card, count);

            if (head == null)
            {
                head = tail = node;
                return;
            }

            node.prev = tail;
            tail.next = node;
            tail = node;
        }

        public void Write()
        {
            for (Node current = head; current != null; current = current.next)
            {
                Console.Write(current.carddata.id[0]);
                if (current.carddata.id[0].ToString().Length != 8)
                {
                    for (int i = 0; i < 8 - current.carddata.id[0].ToString().Length; i++)
                    {
                        Console.Write(' ');
                    }
                }
                Console.WriteLine($" - {current.carddata.name}");
            }
        }
        #endregion
    }

    public class Function
    {
        public static LinkedList<int> Count(string[] files)
        {
            LinkedList<int> list = new LinkedList<int>();

            for (int i1= 0; i1 < files.Length; i1++)
            {
                using (StreamReader reader = new StreamReader(files[i1]))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();

                        if (line != "" && line[0] != '#' && line[0] != '!')
                        {
                            for (Database.Node current = Database.head; current != null; current = current.next)
                            {
                                for (int i2 = 0; i2 < current.carddata.id.Length; i2++)
                                {
                                    if (current.carddata.id[i2] == int.Parse(line))
                                    {
                                        current.count++;
                                        goto label;
                                    }
                                    else if (current == Database.tail)
                                    {
                                        list.AddLast(int.Parse(line));
                                    }
                                }
                            }
                            label:;
                        }
                    }
                }
            }

            return list;
        }

        public static void WriteCount(int i)
        {
            for (Database.Node current = Database.head; current != null; current = current.next)
            {
                if (current.count > i)
                {
                    Console.WriteLine($"{current.count}x - {current.carddata.name}");
                }
            }
        }

        public static void SortAndWrite()
        {
            List<Database.Node> list = new List<Database.Node>();
            for (Database.Node current = Database.head; current != null; current = current.next)
            {
                list.Add(current);
            }
            list.Sort();

            foreach (var Node in list)
            {
                Console.WriteLine(Node);
            }
        }
    } // Essentially a pointless class, due to the fact you could move all the methods to the 'Database' class and save yourself the effort to make Nodes public and static, which basically also makes them pointless, but this looks cooler. And also makes me look stupider.
}
